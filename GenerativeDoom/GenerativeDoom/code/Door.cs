using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeGen
{
    public class Door
    {
        public enum Door_Type { LOCKED_RED, LOCKED_BLUE, LOCKED_YELLOW };//, UNLOCKED };
        public Door_Type currentDoorType;// = Door_Type.UNLOCKED;
        public bool isLocked = false;
        public Node keyNode { get; set; }//where the key is located
        public Node node1 { get; set; }
        public Node node2 { get; set; }
        public bool isInstanciated { get; set; }
    }
}
