using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using System.Threading;
using CodeImp.DoomBuilder.Editing;
using System.IO;
using MazeGen;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;

namespace GenerativeDoom
{
    public partial class GDForm : Form
    {

        public const int TYPE_PLAYER_START = 1;

        private IList<DrawnVertex> points;

        List<Control> _controls = new List<Control>();

        public GDForm()
        {
            InitializeComponent();
            points = new List<DrawnVertex>();
        }

        // We're going to use this to show the form
        public void ShowWindow(Form owner)
        {
            // Position this window in the left-top corner of owner
            this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90);

            // Show it
            base.Show(owner);
        }

        // Form is closing event
        private void GDForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // When the user is closing the window we want to cancel this, because it
            // would also unload (dispose) the form. We only want to hide the window
            // so that it can be re-used next time when this editing mode is activated.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Just cancel the editing mode. This will automatically call
                // OnCancel() which will switch to the previous mode and in turn
                // calls OnDisengage() which hides this window.
                General.Editing.CancelMode();
                e.Cancel = true;
            }
        }

        private void GDForm_Load(object sender, EventArgs e)
        {
            _controls.Add(upDownWidth);
            _controls.Add(upDownHeight);
            _controls.Add(upDownX);
            _controls.Add(upDownY);
            _controls.Add(upDownRoom);
            _controls.Add(upDownRoom);
            _controls.Add(btnGenerate);

            /*
            upDownWidth.Value = 4;
            upDownHeight.Value = 2;
            upDownX.Value = -1;
            upDownY.Value = -1;
            upDownRoom.Value = -1;
            */

            readJsonRoom("Data/jsonConfig.json");

            options = new PasteOptions();
            copyPaste = General.Map.CopyPaste;
        }

        void ToggleButtonState(bool isEnabled)
        {
            _controls.ForEach(item => item.Enabled = isEnabled);
        }

        private void VisualizeMaze(Maze maze, int selectMethod = -1)
        {
            int interval = 50;//ms refresh picture box //Convert.ToInt32(sntInterval.Text);
            Size cellSz = new Size(10, 10);

            btnDoom.Enabled = false;
            ToggleButtonState(false);

            List<Bitmap> progress = new List<Bitmap>();

            //pbProgress.Visible = true;

            maze.ProgressChanged += (int done, int total) =>
            {
                progress.Add(maze.Visualize(cellSz));
            };
            new Thread(delegate ()
            {
                using (maze)
                {
                    maze.Generate();
                }

            })
            { IsBackground = true }.Start();

            maze.Completed += () =>
            {
                new Thread(delegate ()
                {
                    for (int i = 0; i < progress.Count; i++)
                    {
                        picVisual.Invoke((MethodInvoker)delegate ()
                        {
                            picVisual.Image = progress[i];

                        });
                        Thread.Sleep(interval);
                    }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        _nodes = maze.Nodes;
                        ToggleButtonState(true);
                        btnDoom.Enabled = _nodes.Count > 0;
                        //btnThings.Enabled = _nodes.Count > 0;
                    });
                })
                { IsBackground = true }.Start();
            };
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            General.Editing.CancelMode();
        }

        private Thing addThing(Vector2D pos, String category, float proba = 0.5f)
        {
            Thing t = addThing(pos);

            //t.Action = 137;
            //t.Args

            if (t != null)
            {

                IList<ThingCategory> cats = General.Map.Data.ThingCategories;
                Random r = new Random();

                bool found = false;
                foreach (ThingTypeInfo ti in General.Map.Data.ThingTypes)
                {
                    if (ti.Category.Name == category)
                    {
                        t.Type = ti.Index;
                        Console.WriteLine("Add thing cat " + category + " for thing at pos " + pos);
                        found = true;
                        if (r.NextDouble() > proba)
                            break;
                    }

                }
                if (!found)
                {
                    Console.WriteLine("###### Could not find category " + category + " for thing at pos " + pos);
                } else
                    t.Rotate(0);
            } else
            {
                Console.WriteLine("###### Could not add thing for cat " + category + " at pos " + pos);
            }

            return t;
        }

        private Thing addThing(Vector2D pos)
        {
            if (pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
                pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
            {
                Console.WriteLine("Error Generaetive Doom: Failed to insert thing: outside of map boundaries.");
                return null;
            }

            // Create thing
            Thing t = General.Map.Map.CreateThing();
            if (t != null)
            {
                General.Settings.ApplyDefaultThingSettings(t);

                t.Move(pos);

                t.UpdateConfiguration();

                // Update things filter so that it includes this thing
                General.Map.ThingsFilter.Update();

                // Snap to map format accuracy
                t.SnapToAccuracy();
            }

            return t;
        }

        private void correctMissingTex()
        {

            String defaulttexture = "-";
            if (General.Map.Data.TextureNames.Count > 1)
                defaulttexture = General.Map.Data.TextureNames[1];

            // Go for all the sidedefs
            foreach (Sidedef sd in General.Map.Map.Sidedefs)
            {
                // Check upper texture. Also make sure not to return a false
                // positive if the sector on the other side has the ceiling
                // set to be sky
                if (sd.HighRequired() && sd.HighTexture[0] == '-')
                {
                    if (sd.Other != null && sd.Other.Sector.CeilTexture != General.Map.Config.SkyFlatName)
                    {
                        sd.SetTextureHigh(General.Settings.DefaultCeilingTexture);
                    }
                }

                // Check middle texture
                if (sd.MiddleRequired() && sd.MiddleTexture[0] == '-')
                {
                    sd.SetTextureMid(General.Settings.DefaultTexture);
                }

                // Check lower texture. Also make sure not to return a false
                // positive if the sector on the other side has the floor
                // set to be sky
                if (sd.LowRequired() && sd.LowTexture[0] == '-')
                {
                    if (sd.Other != null && sd.Other.Sector.FloorTexture != General.Map.Config.SkyFlatName)
                    {
                        sd.SetTextureLow(General.Settings.DefaultFloorTexture);
                    }
                }

            }
        }

        //Prefab insert
        PasteOptions options;
        CopyPasteManager copyPaste;
        //General random
        Random rand = new Random();//seed ici
        //Maze nodes
        List<List<Node>> _nodes = new List<List<Node>>();
        //Json config rooms
        List<Room> rooms = new List<Room>();
        //Const sizes
        const int sizeDoor = 32;
        const int sizeRoom = 512;
        const int doorLength = 128;
        //Proba
        double probaLoop = 0.2;
        double probaBoss = 0.8;
        double endKeyProba = 0.8;
        //Diff
        double difficulty = 0.3;
        //
        List<Door.Door_Type> keyUsed;
        List<int> powerUpLeft;
        List<int> weaponLeft;
        //
        Hashtable distances;
        List<Thing> listThingsInstanciated;
        //Things ID
        int[] powerUpId = { 8, 2022, 2024, 2023, 2013, 83 };
        int[] weaponId = { 2006, 2002, 2005, 2004, 2003, 2001, 82 };
        int[] monsterId = { 3004, 9, 3001, 3002, 3006, 58, 3005, 71, 65, 67, 64, 66, 68, 69, 3003 };// (16, 7) => bosses };//Left to right : more efficient
        int[][] ammoId = new int[][] {//Left to right : more efficient
            new int[]{ 2008, 2049 },//shotung shell and box of shell
            new int[]{ 2047, 17 },//cell charge and cell charge pack
            new int[]{ 2010, 2046 },//rocket and box of rocket
            new int[]{ 2007, 2048 }//ammo clip and box of ammo
        };
        int[] healthId = { 2014, 2011, 2012 };//Left to right : more efficient
        int[] armorId = { 2018, 2019, 2015 };//Left to right : more efficient





        private double normal_distribution(double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }

        private void generateLevel()
        {
            probaLoop = (double) upDownLoop.Value;
            probaBoss = (double) upDownBoss.Value;
            difficulty = (double) upDownDiff.Value;
            endKeyProba = (double) upDownKeyEnd.Value;

            int maxNormalKey = (int) upDownKey.Value;//3


            listThingsInstanciated = new List<Thing>();
            keyUsed = new List<Door.Door_Type>();
            
            powerUpLeft = new List<int>(powerUpId);
            weaponLeft = new List<int>(weaponId);

            Node theStart = getStart();
            theStart.currentRoomType = Node.Room_Type.START;

            List<Node> endNodes = findEndNodes();
            System.Diagnostics.Debug.WriteLine("end nodes: " + endNodes.Count);

            Dijkstra dij = new Dijkstra(_nodes, theStart);
            distances = dij.DijkstraSolving();

            //End positioning
            Node theEnd;
            List<Node> possibilities = endNodes;
            int placementScenarioEnd = rand.Next(0, 3);
            switch (placementScenarioEnd)
            {
                case 0://close
                    possibilities = getClosestNodesFromStart(distances, endNodes);
                    break;
                case 1://far
                    possibilities = getFarthestNodesFromStart(distances, endNodes);
                    break;
                case 2://everything
                    possibilities = endNodes;
                    break;
            }

            theEnd = possibilities[rand.Next(0, possibilities.Count)];
            theEnd.currentRoomType = Node.Room_Type.END;

            endNodes.Remove(theEnd);//used

            Debug.WriteLine("distance end from start: " + distances[theEnd]);

            initDoors();//create door link between nodes
            createRooms();//chose a room for every nodes

            

            //Key for end room
            if (endNodes.Count > 0 && checkProbability(endKeyProba))
            {
                Debug.WriteLine("key end");
                Door chosenDoor = setKeyToSingleDoor(theEnd);
                possibilities = getFarthestNodesFromStart(distances, endNodes);
                Node placeKeyNode = possibilities[rand.Next(0, possibilities.Count)];
                endNodes.Remove(placeKeyNode);//used
                placeKey(chosenDoor, placeKeyNode);
            }
            //check thing left si peut poser clef et donc porte 

            List<Door> doorUnlocked = getMixedUnlockedDoors();
            while (keyUsed.Count < maxNormalKey)//3 keys max possible
            {
                if (doorUnlocked.Count == 0)
                    break;

                while (true)
                {
                    if (doorUnlocked.Count == 0)
                        break;

                    Door startDoorSelected = doorUnlocked[0];

                    startDoorSelected.currentDoorType = getAKeyTypeLeft();
                    startDoorSelected.isLocked = true;

                    List<Node> res = getAccessibleRooms(theStart, startDoorSelected.node1, startDoorSelected);
                    if (res.Count == 0)
                    {
                        res = getAccessibleRooms(theStart, startDoorSelected.node2, startDoorSelected);
                    }
                    if (res.Count == 0)
                    {
                        //select new door;
                        doorUnlocked.Remove(startDoorSelected);
                        startDoorSelected.isLocked = false;

                        continue;
                    }

                    while (true)
                    {
                        if (res.Count == 0)
                        {
                            doorUnlocked.Remove(startDoorSelected);
                            startDoorSelected.isLocked = false;
                            break;
                        }

                        Node toPlaceKey = res[rand.Next(0, res.Count)];
                        if (toPlaceKey.thingPositionsLeft.Count > 0)
                        {
                            keyUsed.Add(startDoorSelected.currentDoorType);
                            Debug.WriteLine("door key chosen locked: " + startDoorSelected.currentDoorType.ToString());
                            placeKey(startDoorSelected, toPlaceKey);
                            //Update
                            doorUnlocked = getMixedUnlockedDoors();

                            break;
                        }
                        else
                        {
                            res.Remove(toPlaceKey);
                            continue;
                        }
                    }

                    break;
                }

            }



            checkIfCanLoop();
            distances = dij.DijkstraSolving();


            /*
            while (endNodes.Count > 0 && keyUsed.Count < 3)//3 keys possible
            {
                Node selected = endNodes[rand.Next(0, endNodes.Count)];
                endNodes.Remove(selected);//used

                Door startDoorSelected = getFirstNonNullDoor(selected);
                startDoorSelected.currentDoorType = getAKeyTypeLeft();
                keyUsed.Add(startDoorSelected.currentDoorType);

                startDoorSelected.isLocked = true;
                //Node temp = selected;
                //if(startDoorSelected.node1 != selected)
                //{
                //    temp = startDoorSelected.node1;
                //}
                //else
                //{
               //     temp = startDoorSelected.node2;
                //}
                
                List<Node> res = getAccessibleRooms(theStart, selected, startDoorSelected, startDoorSelected);
                //List<Node> farAway = getFarthestNodesFromStart(distances, res);
               
                //res.Sort(delegate (Node c1, Node c2) { return ((int) distances[c1]).CompareTo((int)distances[c2]); });
                //foreach(Node n in res)
               // {
                //    if(res.)
                //}

                //check if thing left enough


                Node toPlaceKey = res[rand.Next(0, res.Count)];

                placeKey(startDoorSelected, toPlaceKey);

                Debug.WriteLine("ACCESSIBLE ROOMS " + res.Count);
            }
            */
            Debug.WriteLine("end nodes: " + endNodes.Count);


            instanciateRoomsAndDoors();
            placeThings(distances);

            checkBoss();

            //SendKeys.SendWait("{ENTER}");

            /*
            List<Thing> things = General.Map.Map.Things.Cast<Thing>().ToList();
            foreach(Thing t in things)
            {
                if(t.Type == 0)
                {

                }
            }
            */
        }
        
        private void checkBoss()
        {
            if (!checkProbability(probaBoss))
                return;

            Debug.WriteLine("boss incoming !");

            foreach (Linedef ld in General.Map.Map.Linedefs)
            {
                if (ld.Action == 52)
                {
                    ld.Action = 39;
                }
            }

            List<Room> currentRoomsSelected = new List<Room>();
            foreach (Room r in rooms)
            {
                if (r.type == "BOSS")
                    currentRoomsSelected.Add(r);
            }
            Room bossRoom = currentRoomsSelected[rand.Next(0, currentRoomsSelected.Count)];

            int maxheight = 0;
            int maxwidth = 0;
            int height = 0;
            Node current;
            int[] widths = new int[_nodes[0].Count];

            
            int minWidthX = _nodes.Count;
            int minWidthY = 0;
            int minHeightX = 0;
            int minHeightY = _nodes[0].Count;

            int maxWidthX = 0;
            int maxWidthY = 0;
            int maxHeightX = 0;
            int maxHeightY = 0;

            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    current = _nodes[x][y];
                    if (current.selectedRoom != null)
                    {
                        widths[y] += current.selectedRoom.width;
                        height += current.selectedRoom.height;

                        if(x < minWidthX)
                        {
                            minWidthX = x;
                            minWidthY = y;
                        }
                        if (y < minHeightY)
                        {
                            minHeightX = x;
                            minHeightY = y;
                        }

                        if (x > maxWidthX)
                        {
                            maxWidthX = x;
                            maxWidthY = y;
                        }
                        if (y > maxHeightY)
                        {
                            maxHeightX = x;
                            maxHeightY = y;
                        }
                    }
                }
                
                if (height > maxheight)
                {
                    maxheight = height;
                }
                height = 0;
            }
            maxwidth = widths.Max();

            Debug.WriteLine("maxwidth: " + maxwidth + " maxheight: " + maxheight);

            Node minNodeWidth = _nodes[minWidthX][minWidthY];
            Node minNodeHeight = _nodes[minHeightX][minHeightY];

            Debug.WriteLine("minWidthX: " + minWidthX + " minWidthY: " + minWidthY);
            Debug.WriteLine("minHeightX: " + minHeightX + " minHeightY: " + minHeightY);
            Debug.WriteLine("maxWidthX: " + maxWidthX + " maxWidthY: " + maxWidthY);
            Debug.WriteLine("maxHeightX: " + maxHeightX + " minHeightY: " + maxHeightY);

            //pos room, to the right of the maze
            int xPos = (minWidthX * (sizeRoom + sizeDoor)) + (maxWidthX - minWidthX + 2) * (sizeRoom + sizeDoor);
            int yPos = ((_nodes[0].Count - maxHeightY) * (sizeRoom + sizeDoor)) + ((maxHeightY - minHeightY + 1) * (sizeRoom + sizeDoor)) / 2 - bossRoom.height/2;

            using (FileStream stream = File.Open(@"Data/" + bossRoom.file + ".dbprefab", FileMode.Open))
            {
                RectangleF rect = new RectangleF(xPos, yPos, bossRoom.width, bossRoom.height);
                ((ClassicMode)General.Editing.Mode).CenterOnArea(rect, 0f);
                copyPaste.InsertPrefabStream(stream, options);
                forceSnapRefreshMap();
            }

            Vector2D bossPos = new Vector2D(xPos + bossRoom.things[0].x, yPos + bossRoom.things[0].y);//need one position for room boss
            Thing t = addThing(bossPos);
            int idBoss = 16;
            if (rand.Next(0, 2) == 0)
                idBoss = 7;
            t.Type = idBoss;
            t.Rotate(rand.Next(0, 360));
            t.Action = 75;//action end game, not working with this version

            //t.SetFlag("BOSS", true);
            
        }

        private void checkIfCanLoop()
        {
            Node current;
            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    current = _nodes[x][y];
                    if (current.selectedRoom != null)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (!current.GetWall(i) && current[i] != null)
                            {
                                if (current.currentRoomType == Node.Room_Type.NORMAL
                                    && current[i].currentRoomType == Node.Room_Type.NORMAL
                                    && getAccessibleRooms(current[i], current, null).Count > 0
                                    && current.selectedRoom.getSideDoor(i) != null
                                    && current[i].selectedRoom.getSideDoor((i + 2) % 4) != null
                                    && checkProbability(probaLoop))
                                {
                                    current.UnWall(i);
                                    switch (i)//can be turned into function (see initDoors)
                                    {
                                        case 0://up
                                            if (current.DoorUp == null)
                                            {
                                                current.DoorUp = new Door();
                                            }
                                            current.DoorUp.node1 = current;
                                            current.DoorUp.node2 = current.Up;
                                            current.Up.DoorDown = current.DoorUp;
                                            break;
                                        case 3://left
                                            if (current.DoorLeft == null)
                                            {
                                                current.DoorLeft = new Door();
                                            }
                                            current.DoorLeft.node1 = current;
                                            current.DoorLeft.node2 = current.Left;
                                            current.Left.DoorRight = current.DoorLeft;
                                            break;
                                        case 2://down
                                            if (current.DoorDown == null)
                                            {
                                                current.DoorDown = new Door();
                                            }
                                            current.DoorDown.node1 = current;
                                            current.DoorDown.node2 = current.Down;
                                            current.Down.DoorUp = current.DoorDown;
                                            break;
                                        case 1://right
                                            if (current.DoorRight == null)
                                            {
                                                current.DoorRight = new Door();
                                            }
                                            current.DoorRight.node1 = current;
                                            current.DoorRight.node2 = current.Right;
                                            current.Right.DoorLeft = current.DoorRight;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private int getMaxDistance(Hashtable distance)
        {
            int distMax = 0;
            foreach (DictionaryEntry entry in distance)
            {
                if ((int)entry.Value != int.MaxValue && (int)entry.Value > distMax)//ignore maxvalue => inaccessible node
                    distMax = (int)entry.Value;
            }
            return distMax;
        }

        //Using normal distribution
        private void placeThings(Hashtable distances)
        {
            difficulty = Clamp(difficulty, 0.1, 1.0);

            int distMax = getMaxDistance(distances);

            if (distMax == 0)
                return;

            Node current;

            double mean = 0.0;
            double stDev = 0.0;
            int numberThings = 0;
            int numberMonsters = 0;
            int numberGoods = 0;
            double distRatioDifficulty = 0.0;
            double distDifficulty = 0.0;
            double distDifficultyInverse = 0.0;
            double spawnWeaponOrPowerUp = 0.0;
            int weaponOrPowerUp = 0;
            int dist = 0;
            int idMonster = 0;
            int idGood = 0;
            int indexMonster = 0;
            int indexGood = 0;
            int ammoOrHealth = 0;
            int healthOrArmor = 0;
            int randomAmmo = 0;

            Debug.WriteLine("\ndistMax: " + distMax+ "\n");

            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    current = _nodes[x][y];
                    if(current.selectedRoom != null)
                    {
                        dist = (int) distances[current];

                        if (dist == 0)//Start
                            continue;

                        distDifficultyInverse = (double) ((double)distMax / dist);
                        distDifficulty = (double) ((double)dist / distMax);
                        distRatioDifficulty = (double) ((double)distDifficulty * difficulty);

                        Debug.WriteLine("distRatioDifficulty: " + distRatioDifficulty + " - distDifficulty: " + distDifficulty + " - distDifficultyInverse: " + distDifficultyInverse);

                        //Choose number of things to instanciate
                        mean = current.thingPositionsLeft.Count * distDifficulty;
                        stDev = (1 / distDifficulty) * distRatioDifficulty;// 1 + distMax - dist; // 0.1 //1 - distDifficultyInverse
                        numberThings = (int) Clamp(Math.Ceiling(Math.Abs(normal_distribution(mean, stDev))), 0, current.thingPositionsLeft.Count);

                        Debug.WriteLine("Node x:"+ current.Pos.X + "y:" + current.Pos.Y + " - distance: " + dist + " - number thing: " + numberThings + " - things left: " + current.thingPositionsLeft.Count);

                        if (numberThings == 0)
                            continue;

                        //Choose if spawn weapon or powerup
                        mean = dist;
                        stDev = dist;
                        spawnWeaponOrPowerUp = Math.Abs(normal_distribution(mean, stDev));
                        if(spawnWeaponOrPowerUp > distMax)
                        {
                            Debug.WriteLine("YESSSSSSSSSSSSSSSSSSSSSSS YESSSSSSSSSSSSSSSSS");
                            weaponOrPowerUp = rand.Next(0, 2);
                            switch(weaponOrPowerUp)
                            {
                                case 0:
                                    if (weaponLeft.Count > 0)
                                    {
                                        Debug.WriteLine("weapon");
                                        int w = weaponLeft[rand.Next(0, weaponLeft.Count)];
                                        weaponLeft.Remove(w);

                                        instanciateThing(w, ThingType.Thing_Type.WEAPON, current, getRandomThingPosition(current));
                                    }
                                    break;
                                case 1:
                                    if (powerUpLeft.Count > 0)
                                    {
                                        Debug.WriteLine("powerUp");
                                        int p = powerUpLeft[rand.Next(0, powerUpLeft.Count)];
                                        powerUpLeft.Remove(p);

                                        instanciateThing(p, ThingType.Thing_Type.POWER, current, getRandomThingPosition(current));
                                    }
                                    break;
                            }

                            numberThings--;

                            if (numberThings == 0)
                                continue;
                        }
                        
                        //Choose number bad things (aka monsters)
                        mean = (double) numberThings * distDifficulty;
                        stDev = distDifficultyInverse;
                        numberMonsters = (int) Clamp(Math.Ceiling(Math.Abs(normal_distribution(mean, stDev))), 0, numberThings);

                        //Choose number good things (aka ammo or health/armor)
                        mean = (double) numberMonsters * distDifficulty;
                        stDev = (double) (distDifficulty) * distMax;
                        numberGoods = (int) Clamp(Math.Abs(normal_distribution(mean, stDev)), 0, numberMonsters);//interesting: nb goods <= nb monsters ?

                        //Share numberThings between bad and good
                        int totalWant = numberMonsters + numberGoods;

                        double temp1 = (double)numberThings * (double)((double)numberMonsters / totalWant);
                        double temp2 = (double)numberThings * (double)((double)numberGoods / totalWant) - 0.1;//so numberMonsters + numberGoods never > numberThings

                        numberMonsters = (int)Math.Round(temp1);
                        numberGoods = (int)Math.Round(temp2);

                        if(numberMonsters + numberGoods > numberThings)
                        {
                            Debug.WriteLine("numberMonsters: " + numberMonsters + " - numberGoods: " + numberGoods);
                            Debug.WriteLine("temp1: " + temp1 + " - temp2: " + temp2);
                            Debug.WriteLine("ERROR COUNT HERE");
                            return;
                        }

                        //Instanciate monsters
                        for (int i = 0; i < numberMonsters; i++)
                        {
                            //ID monster selection
                            mean = (double) (monsterId.Length - 1) * distRatioDifficulty; //improvement: have the max things left depending on chosen rooms to have good ratio difficulty between room with lot of things and small ones
                            stDev = distDifficulty; // (double) distMax - ((double)(distDifficulty) * distMax);

                            Debug.WriteLine("meanMonsters: " + mean + " - distRatioDifficulty: " + distRatioDifficulty);

                            indexMonster = (int) Clamp(Math.Abs(normal_distribution(mean, stDev)), 0, monsterId.Length - 1);
                            idMonster = monsterId[indexMonster];

                            instanciateThing(idMonster, ThingType.Thing_Type.MONSTER, current, getRandomThingPosition(current));
                        }

                        //Instanciate goods
                        for (int i = 0; i < numberGoods; i++)
                        {
                            ammoOrHealth = rand.Next(0, 2);
                            switch (ammoOrHealth)
                            {
                                case 0:
                                    randomAmmo = rand.Next(0, ammoId.Length);//select random type of ammo to spawn

                                    //Do not spawn rocket or cell if weapon not here
                                    if(randomAmmo == 1 && (weaponLeft.Contains(2004) && weaponLeft.Contains(2006)))
                                    {
                                        randomAmmo = 0;
                                    }
                                    if(randomAmmo == 2 && weaponLeft.Contains(2003))
                                    {
                                        randomAmmo = 3;
                                    }

                                    //ID ammo selection (same length for all)
                                    mean = (double) (ammoId[randomAmmo].Length - 1) * distRatioDifficulty;
                                    stDev = 0.2;

                                    indexGood = (int)Clamp(Math.Abs(normal_distribution(mean, stDev)), 0, ammoId[randomAmmo].Length - 1);
                                    idGood = ammoId[randomAmmo][indexGood];

                                    instanciateThing(idGood, ThingType.Thing_Type.AMMO, current, getRandomThingPosition(current));
                                    break;
                                case 1:
                                    //ID health or armor selection (same length for health and armor)
                                    mean = (double) (healthId.Length - 1) * distRatioDifficulty;
                                    stDev = 0.2;

                                    indexGood = (int)Clamp(Math.Abs(normal_distribution(mean, stDev)), 0, healthId.Length - 1);

                                    healthOrArmor = rand.Next(0, 2);
                                    switch (healthOrArmor)
                                    {
                                        case 0:
                                            idGood = healthId[indexGood];
                                            instanciateThing(idGood, ThingType.Thing_Type.HEALTH, current, getRandomThingPosition(current));
                                            break;
                                        case 1:
                                            idGood = armorId[indexGood];
                                            instanciateThing(idGood, ThingType.Thing_Type.ARMOR, current, getRandomThingPosition(current));
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private Position getRandomThingPosition(Node node)
        {
            List<Position> thingsPos = node.thingPositionsLeft;
            return thingsPos[rand.Next(0, thingsPos.Count)];
        }

        public double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void instanciateThing(int idThing, ThingType.Thing_Type thingType, Node node, Position pos)
        {
            Vector2D relpos = getRelativePosition(node, pos);
            Thing t = addThing(relpos);
            t.Type = idThing;
            t.Rotate(rand.Next(0, 360));

            node.thingPositionsLeft.Remove(pos);
            node.thingsInstanciated.Add(new ThingType(pos, thingType, t));

            listThingsInstanciated.Add(t);
        }

        public void Shuffle(List<Door> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                Door value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private List<Door> getMixedUnlockedDoors()//list door plutot => check node1 et node2 si end pour pas reposer dessus !!
        {
            Node current;
            List<Door> unlockedDoors = new List<Door>();
            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    current = _nodes[x][y];
                    if(current.DoorDown != null && !current.DoorDown.isLocked && !unlockedDoors.Contains(current.DoorDown))
                    {
                        unlockedDoors.Add(current.DoorDown);
                    }
                    if (current.DoorLeft != null && !current.DoorLeft.isLocked && !unlockedDoors.Contains(current.DoorLeft))
                    {
                        unlockedDoors.Add(current.DoorLeft);
                    }
                    if (current.DoorRight != null && !current.DoorRight.isLocked && !unlockedDoors.Contains(current.DoorRight))
                    {
                        unlockedDoors.Add(current.DoorRight);
                    }
                    if (current.DoorUp != null && !current.DoorUp.isLocked && !unlockedDoors.Contains(current.DoorUp))
                    {
                        unlockedDoors.Add(current.DoorUp);
                    }
                }
            }
            Shuffle(unlockedDoors);
            return unlockedDoors;
        }

        //Attention: must have 1 key left to use
        private Door.Door_Type getAKeyTypeLeft()
        {
            List<Door.Door_Type> poss = new List<Door.Door_Type>();
            Door.Door_Type temp;
            Array values = Enum.GetValues(typeof(Door.Door_Type));
            for(int i = 0; i < values.Length; i++) {
                temp = (Door.Door_Type)values.GetValue(i);
                if (!keyUsed.Contains(temp))
                    poss.Add(temp);
            }
            return poss[rand.Next(poss.Count)];
        }

        private Door getFirstNonNullDoor(Node node)
        {
            Door onlyDoor = null;
            if (node.DoorDown != null)
                onlyDoor = node.DoorDown;
            else if (node.DoorLeft != null)
                onlyDoor = node.DoorLeft;
            else if (node.DoorRight != null)
                onlyDoor = node.DoorRight;
            else if (node.DoorUp != null)
                onlyDoor = node.DoorUp;
            return onlyDoor;
        }

        private Door setKeyToSingleDoor(Node node)
        {
            Door onlyDoor = getFirstNonNullDoor(node);

            //if (onlyDoor == null)
            //  return 0;//error

            //Array values = Enum.GetValues(typeof(Door.Door_Type));
            //Door.Door_Type chosen = (Door.Door_Type)values.GetValue(rand.Next(values.Length));

            Door.Door_Type chosen = getAKeyTypeLeft();

            keyUsed.Add(chosen);
            onlyDoor.currentDoorType = chosen;
            onlyDoor.isLocked = true;

            return onlyDoor;
        }

        
        //Get accessible rooms from a door (accessible => from start)
        private List<Node> getAccessibleRooms(Node target, Node startingPoint, Door initialWantLock)//, Door currentWanLock)
        {
            Node current = startingPoint;
            List<Node> visitedNodes = new List<Node>();
            List<Node> toDoNodes = new List<Node>();

            toDoNodes.Add(startingPoint);
            
            /*
            if (startingPoint.DoorDown != null)
                toDoNodes.Add(startingPoint.Down);
            if (startingPoint.DoorLeft != null)
                toDoNodes.Add(startingPoint.Left);
            if (startingPoint.DoorRight != null)
                toDoNodes.Add(startingPoint.Right);
            if (startingPoint.DoorUp != null)
                toDoNodes.Add(startingPoint.Up);
                */

            List<Node> cantAccess = new List<Node>();
            


            bool targetAccessible = false;

            bool possible = false;

            while(toDoNodes.Count > 0)
            {
                possible = false;
                current = toDoNodes[0];

                if (current == target)
                {
                    targetAccessible = true;
                }

                toDoNodes.Remove(current);

                if (current.GetWall(0)) {
                    if( ((!current.DoorUp.isLocked)))// || getAccessibleRooms(current.DoorUp.keyNode, current.Up, initialWantLock, current.DoorUp).Count > 0))
                    {// && current.DoorUp != currentWanLock
                        possible = true;
                        if (!toDoNodes.Contains(current.Up) && !visitedNodes.Contains(current.Up))
                        {
                            
                            toDoNodes.Add(current.Up);
                        }
                            
                    }
                }
                if (current.GetWall(1))
                {
                    if (((!current.DoorRight.isLocked)))// || getAccessibleRooms(current.DoorRight.keyNode, current.Right, initialWantLock, current.DoorRight).Count > 0))
                    {//&& current.DoorRight != currentWanLock
                        possible = true;
                        if (!toDoNodes.Contains(current.Right) && !visitedNodes.Contains(current.Right))
                        {
                            
                            toDoNodes.Add(current.Right);
                        }
                            
                    }
                }
                if (current.GetWall(2))
                {
                    if (((!current.DoorDown.isLocked)))// || getAccessibleRooms(current.DoorDown.keyNode, current.Down, initialWantLock, current.DoorDown).Count > 0))
                    {//&& current.DoorDown != currentWanLock
                        possible = true;
                        if (!toDoNodes.Contains(current.Down) && !visitedNodes.Contains(current.Down))
                        {
                            
                            toDoNodes.Add(current.Down);
                        }
                           
                    }
                }
                if (current.GetWall(3))
                {
                    if (((!current.DoorLeft.isLocked)))// || getAccessibleRooms(current.DoorLeft.keyNode, current.Left, initialWantLock, current.DoorLeft).Count > 0))
                    {//&& current.DoorLeft != currentWanLock 
                        possible = true;
                        if (!toDoNodes.Contains(current.Left) && !visitedNodes.Contains(current.Left))
                        {
                            
                            toDoNodes.Add(current.Left);
                        }
                           
                    }
                }

                if(!visitedNodes.Contains(current))
                    visitedNodes.Add(current);

                if(!possible && !cantAccess.Contains(current))
                {
                    cantAccess.Add(current);
                }
            }

            if(!targetAccessible)
            {
                visitedNodes.Clear();
            }

            foreach(Node n in cantAccess)
            {
                visitedNodes.Remove(n);
            }

            return visitedNodes;
        }
       
        //Get the type of key from the door, place it inside the node
        private void placeKey(Door door, Node node)
        {
            List<Position> thingsPos = node.thingPositionsLeft;
            Position chosen = thingsPos[rand.Next(0, thingsPos.Count)];
            thingsPos.Remove(chosen);

            door.keyNode = node;//linked

            int thingId = 0;
            switch (door.currentDoorType)
            {
                case Door.Door_Type.LOCKED_BLUE:
                    thingId = 5;
                    break;
                case Door.Door_Type.LOCKED_YELLOW:
                    thingId = 6;
                    break;
                case Door.Door_Type.LOCKED_RED:
                    thingId = 13;
                    break;
            }
            Vector2D pos = getRelativePosition(node, chosen);
            Thing t = addThing(pos);
            t.Type = thingId;
            node.thingsInstanciated.Add(new ThingType(chosen, ThingType.Thing_Type.KEY, t));
        }

        private Vector2D getRelativePosition(Node node, Position thingsPos)
        {
            int x = ((sizeRoom + sizeDoor) * node.Pos.X) + thingsPos.x;
            int y = ((sizeRoom + sizeDoor) * ((int)upDownHeight.Value - node.Pos.Y)) + thingsPos.y;
            return new Vector2D(x,y);
        }

        private bool checkProbability(double proba)
        {
            if (rand.NextDouble() < proba)
                return true;
            return false;
        }

        private List<Node> getClosestNodesFromStart(Hashtable distances, List<Node> poss)
        {
            List<Node> nodes = new List<Node>();
            int distanceMin = int.MaxValue;
            foreach (Node n in poss)
            {
                if((int) distances[n] < distanceMin)
                {
                    nodes.Clear();
                    distanceMin = (int) distances[n];
                    nodes.Add(n);
                }
                else if((int) distances[n] == distanceMin)
                {
                    nodes.Add(n);
                }
            }
            return nodes;
        }

        private List<Node> getFarthestNodesFromStart(Hashtable distances, List<Node> poss)
        {
            List<Node> nodes = new List<Node>();
            int distanceMax = 1;
            foreach (Node n in poss)
            {
                if ((int)distances[n] > distanceMax)
                {
                    nodes.Clear();
                    distanceMax = (int)distances[n];
                    nodes.Add(n);
                }
                else if ((int)distances[n] == distanceMax)
                {
                    nodes.Add(n);
                }
            }
            return nodes;
        }

        /*
        private void filterNodes()
        {
            List<List<Node>> temp = new List<List<Node>>();
            Node current;
            for (int x = 0; x < _nodes.Count; x++)
            {
                temp.Add(new List<Node>());
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    current = _nodes[x][y];
                    if (current.isStart || (current.parentInfo.Count > 0 && !current.isFrontier))//create only linked nodes
                    {
                        temp[x].Add(current);
                    }
                }
            }
            _nodes = temp;
        }
        */

        private void instanciateRoomsAndDoors()
        {
            Node current;
            Room currentRoom;
            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    current = _nodes[x][y];
                    if (current.isStart || (current.parentInfo.Count > 0 && !current.isFrontier))
                    {
                        currentRoom = current.selectedRoom;
                        instanciatePrefab(current, currentRoom, x, y);
                        instanciateDoors(current, currentRoom, x, y);
                    }
                }
            }
        }

        private void createRooms()
        {
            Node current;
            Room currentRoom;
            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    current = _nodes[x][y];
                    if (current.isStart || (current.parentInfo.Count > 0 && !current.isFrontier))//create only linked nodes
                    {
                        switch (current.currentRoomType)
                        {
                            case Node.Room_Type.START:
                                currentRoom = getRandomRoom("START", current);
                                break;
                            case Node.Room_Type.END:
                                currentRoom = getRandomRoom("END", current);
                                break;
                            case Node.Room_Type.NORMAL:
                                currentRoom = getRandomRoom("NORMAL", current);
                                break;
                            default:
                                currentRoom = getRandomRoom("NORMAL", current);
                                break;
                        }
                    
                        current.selectedRoom = currentRoom;
                        current.thingPositionsLeft = new List<Position>(currentRoom.things);//copy => to not touch the json data
                    }
                }
            }
        }

        private Room getRandomRoom(string roomType, Node node)
        {
            //Dabord chercher room avec nb portes pareil
            //sinon prendre ceux avec les 4 possibiltiés => plus d'originalité

            //check door anchor compatibility here ?

            List<Room> currentRoomsSelected = new List<Room>();

            int anchorCheck = 0;
            //int anchorCount = 0;

            foreach (Room r in rooms) {
                if(r.type == roomType)
                {
                    anchorCheck = 0;
                    //anchorCount = 0;

                    if (node.GetWall(0) && r.getSideDoor(0) == null)
                        continue;
                    if (node.GetWall(1) && r.getSideDoor(1) == null)
                        continue;
                    if (node.GetWall(2) && r.getSideDoor(2) == null)
                        continue;
                    if (node.GetWall(3) && r.getSideDoor(3) == null)
                        continue;

                    //Check anchor for doors

                    if ((node.Up == null || node.Up.selectedRoom == null || r.getSideDoor(0) == null || node.Up.selectedRoom.getSideDoor(2) == null)//!node.GetWall(0) || 
                        || (
                        r.getSideDoor(0).anchor.x == node.Up.selectedRoom.getSideDoor(2).anchor.x
                        && r.getSideDoor(0).anchor.y - sizeRoom == node.Up.selectedRoom.getSideDoor(2).anchor.y))
                    {
                        anchorCheck++;
                    }
                    if ((node.Left == null || node.Left.selectedRoom == null || r.getSideDoor(3) == null || node.Left.selectedRoom.getSideDoor(1) == null)//!node.GetWall(3) || 
                        || (
                        r.getSideDoor(3).anchor.y == node.Left.selectedRoom.getSideDoor(1).anchor.y
                        && r.getSideDoor(3).anchor.x + sizeRoom == node.Left.selectedRoom.getSideDoor(1).anchor.x))
                    {
                        anchorCheck++;
                    }
                    if ((node.Down == null || node.Down.selectedRoom == null || r.getSideDoor(2) == null || node.Down.selectedRoom.getSideDoor(0) == null)//!node.GetWall(2) || 
                        || (
                        r.getSideDoor(2).anchor.x == node.Down.selectedRoom.getSideDoor(0).anchor.x
                        && r.getSideDoor(2).anchor.y + sizeRoom == node.Down.selectedRoom.getSideDoor(0).anchor.y))
                    {
                        anchorCheck++;
                    }
                    if ((node.Right == null || node.Right.selectedRoom == null || r.getSideDoor(1) == null || node.Right.selectedRoom.getSideDoor(3) == null)//!node.GetWall(1) || 
                        || (
                        r.getSideDoor(1).anchor.y == node.Right.selectedRoom.getSideDoor(3).anchor.y
                        && r.getSideDoor(1).anchor.x - sizeRoom == node.Right.selectedRoom.getSideDoor(3).anchor.x))
                    {
                        anchorCheck++;
                    }
                    

                    if (anchorCheck == 4)
                        currentRoomsSelected.Add(r);
                    else
                    {
                        Debug.WriteLine("current node: " + node.Pos.X + " " + node.Pos.Y + " " + roomType.ToString() +" " + r.file + " anchorCheck: " + anchorCheck);
                    }
                }
            }


            //if error, missing a prefab for all compatibility
            return currentRoomsSelected[rand.Next(0, currentRoomsSelected.Count)];
        }
        
        //Dictionary<string, Door> generatedDoors;

        private void initDoors()
        {
            Node node;
            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    node = _nodes[x][y];

                    for (int c = 0; c < 4; c++)
                    {
                        if (node.GetWall(c))//door
                        {
                            switch (c)
                            {
                                case 0://up
                                    if (node.DoorUp == null)
                                    {
                                        node.DoorUp = new Door();
                                        node.DoorUp.node1 = node;
                                        node.DoorUp.node2 = node.Up;
                                        node.Up.DoorDown = node.DoorUp;
                                    }
                                    break;
                                case 3://left
                                    if (node.DoorLeft == null)
                                    {
                                        node.DoorLeft = new Door();
                                        node.DoorLeft.node1 = node;
                                        node.DoorLeft.node2 = node.Left;
                                        node.Left.DoorRight = node.DoorLeft;
                                    }
                                    break;
                                case 2://down
                                    if (node.DoorDown == null)
                                    {
                                        node.DoorDown = new Door();
                                        node.DoorDown.node1 = node;
                                        node.DoorDown.node2 = node.Down;
                                        node.Down.DoorUp = node.DoorDown;
                                    }
                                    break;
                                case 1://right
                                    if (node.DoorRight == null)
                                    {
                                        node.DoorRight = new Door();
                                        node.DoorRight.node1 = node;
                                        node.DoorRight.node2 = node.Right;
                                        node.Right.DoorLeft = node.DoorRight;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
    
        private void instanciateDoors(Node node, Room room, int i, int j)
        {
            //if (!node.isStart)
              //  return;

            System.Diagnostics.Debug.WriteLine("door start");

            string prefabName = "doorLeftRight";
            int sizeX = 0, sizeY = 0, x = 0, y = 0;

            DoorAnchor doorAnchor;
            int doorAnchorOffsetX = 0;
            int doorAnchorOffsetY = 0;
            Door currentDoor = null;

            for (int c = 0; c < 4; c++)
            {
                if(node.GetWall(c))//door
                {
                    if (c == 0 || c == 2)//up down
                    {
                        prefabName = "doorUpDown";
                        sizeX = doorLength;
                        sizeY = sizeDoor;
                    }
                    else///left right
                    {
                        prefabName = "doorLeftRight";
                        sizeX = sizeDoor;
                        sizeY = doorLength;
                    }

                    doorAnchor = room.getSideDoor(c);

                    doorAnchorOffsetX = doorAnchor.anchor.x;
                    doorAnchorOffsetY = doorAnchor.anchor.y;

                    //Up =  Right =  Down =  Left = , wall side in doom representation
                    switch (c) {
                        case 0://up
                            currentDoor = node.DoorUp;
                            if (!currentDoor.isInstanciated)
                                currentDoor.isInstanciated = true;
                            else
                                continue;

                            x = ((sizeRoom + sizeDoor) * node.Pos.X) + doorAnchorOffsetX;
                            y = ((sizeRoom + sizeDoor) * ((int)upDownHeight.Value - node.Pos.Y)) + doorAnchorOffsetY;//sizeRoom

                            if(node.isStart)
                                System.Diagnostics.Debug.WriteLine("____________door up"  + doorAnchorOffsetX + " "  + doorAnchorOffsetY);
                            break;
                        case 3://left
                            currentDoor = node.DoorLeft;
                            if (!currentDoor.isInstanciated)
                                currentDoor.isInstanciated = true;
                            else
                                continue;

                            x = ((sizeRoom + sizeDoor) * node.Pos.X) - sizeDoor + doorAnchorOffsetX;
                            y = ((sizeRoom + sizeDoor) * ((int)upDownHeight.Value - node.Pos.Y)) + doorAnchorOffsetY;

                            if (node.isStart)
                                System.Diagnostics.Debug.WriteLine("____________door left");
                            break;
                        case 2://down
                            currentDoor = node.DoorDown;
                            if (!currentDoor.isInstanciated)
                                currentDoor.isInstanciated = true;
                            else
                                continue;

                            x = ((sizeRoom + sizeDoor) * node.Pos.X) + doorAnchorOffsetX;
                            y = ((sizeRoom + sizeDoor) * ((int)upDownHeight.Value - node.Pos.Y)) - sizeDoor + doorAnchorOffsetY;

                            if (node.isStart)
                                System.Diagnostics.Debug.WriteLine("____________door down");
                            break;
                        case 1://right
                            currentDoor = node.DoorRight;
                            if (!currentDoor.isInstanciated)
                                currentDoor.isInstanciated = true;
                            else
                                continue;

                            x = ((sizeRoom + sizeDoor) * node.Pos.X) + doorAnchorOffsetX;
                            y = ((sizeRoom + sizeDoor) * ((int)upDownHeight.Value - node.Pos.Y)) + doorAnchorOffsetY;

                            if (node.isStart)
                                System.Diagnostics.Debug.WriteLine("____________door right");
                            break;
                    }

                    if(currentDoor.isLocked)
                    {
                        switch(currentDoor.currentDoorType)
                        {
                            case Door.Door_Type.LOCKED_BLUE:
                                prefabName += "LockBlue";
                                break;
                            case Door.Door_Type.LOCKED_YELLOW:
                                prefabName += "LockYellow";
                                break;
                            case Door.Door_Type.LOCKED_RED:
                                prefabName += "LockRed";
                                break;
                        }
                    }

                    using (FileStream stream = File.Open(@"Data/door/" + prefabName + ".dbprefab", FileMode.Open))
                    {
                        RectangleF rect = new RectangleF(x, y, sizeX, sizeY);

                        ((ClassicMode)General.Editing.Mode).CenterOnArea(rect, 0f);

                        copyPaste.InsertPrefabStream(stream, options);

                        forceSnapRefreshMap();
                    }
                }
            }
        }

        private void forceSnapRefreshMap()
        {
            General.Map.Map.StitchGeometry();

            // Snap to map format accuracy
            General.Map.Map.SnapAllToAccuracy();

            // Clear selection
            General.Map.Map.ClearAllSelected();

            // Update cached values
            General.Map.Map.Update();

            // Update the used textures
            General.Map.Data.UpdateUsedTextures();

            // Map is changed
            General.Map.IsChanged = true;

            General.Interface.RedrawDisplay();

            //Ajoute, on enleve la marque sur les nouveaux secteurs
            General.Map.Map.ClearMarkedSectors(false);
        }

        private void instanciatePrefab(Node node, Room room, int i, int j)
        {
            using (FileStream stream = File.Open(@"Data/" + room.file+".dbprefab", FileMode.Open))
            {
                int x = ((sizeRoom + sizeDoor) * (i)) - room.origin.x;// node.Pos.X;
                int y = ((sizeRoom + sizeDoor) * ((int)upDownHeight.Value - j)) - room.origin.y;// node.Pos.Y; //changing y because doom builder (0,0) is left bottom
                //int y = ((sizeRoom + sizeDoor) * j);

                RectangleF rect = new RectangleF(x, y, room.width, room.height);//sizeRoom, sizeRoom

                ((ClassicMode)General.Editing.Mode).CenterOnArea(rect, 0f);

                copyPaste.InsertPrefabStream(stream, options);

                forceSnapRefreshMap();
            }
        }

        private void readJsonRoom(string path)
        {
            System.Diagnostics.Debug.WriteLine("Loading json config");
            System.Diagnostics.Debug.WriteLine(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
            rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(@path));
        }

        private Node getStart()
        {
            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    if (_nodes[x][y].isStart)
                    {
                        return _nodes[x][y];
                    }
                }
            }
            return null;
        }

        private List<Node> findEndNodes()
        {
            int i = 0;
            Node current;
            List<Node> ends = new List<Node>();
            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    current = _nodes[x][y];
                    for (int c = 0; c < 4; c++)
                    {
                        if (current.GetWall(c))
                        {
                            i++;
                        }
                    }
                    if (i == 1 && current.currentRoomType == Node.Room_Type.NORMAL)
                    {
                        ends.Add(current);
                    }
                    i = 0;
                }
            }
            return ends;
        }
        
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //makeOnePath(true);
            if((int)upDownX.Value >= (int)upDownWidth.Value)
            {
                upDownX.Value = upDownWidth.Value - 1;
            }
            if ((int)upDownY.Value >= (int)upDownHeight.Value)
            {
                upDownY.Value = upDownHeight.Value - 1;
            }

            VisualizeMaze(new MazePrim((int)upDownWidth.Value, (int)upDownHeight.Value, (int)upDownRoom.Value, new Position((int)upDownX.Value, (int)upDownY.Value)));

            
        }

        private void btnDoom_Click(object sender, EventArgs e)
        {
            btnDoom.Enabled = false;
            if(_nodes.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Generation incoming");
                //clear map
                General.Map.Map.Dispose();
                General.Map.Map = new MapSet();
                try
                {
                    generateLevel();
                    correctMissingTex();
                }
                catch(ArgumentOutOfRangeException ex)
                {
                    Debug.WriteLine("ArgumentOutOfRangeException, have you check every room can be interlinked?");
                    labelError.Text = "ArgumentOutOfRangeException, have you check every room can be interlinked?";
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("{0} Exception caught.", ex.ToString());
                    labelError.Text = ex.ToString() + " Exception caught.";
                }
                
            }

            //Reset all, can doomify same level afterwards
            for (int x = 0; x < _nodes.Count; x++)
            {
                for (int y = 0; y < _nodes[x].Count; y++)
                {
                    _nodes[x][y].Reset();
                }
            }

            btnDoom.Enabled = true;
        }

        /* Not working, cant find how to remove thing
        private void btnThings_Click(object sender, EventArgs e)
        {
            
            foreach (Thing t in listThingsInstanciated)
            {
                t.Marked = true;
            }
            General.Map.Map.ClearMarkedThings(true);

            

            listThingsInstanciated.Clear();
            Debug.WriteLine("reset things");
            rand = new Random();
            //placeThings(distances);
            
        }
        */
    }
}
