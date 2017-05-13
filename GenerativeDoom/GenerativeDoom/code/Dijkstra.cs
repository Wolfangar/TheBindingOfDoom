using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MazeGen
{
    public class Dijkstra
    {
        List<List<Node>> grid;
        Node start;

        public Dijkstra(List<List<Node>> nodes, Node start)
        {
            this.grid = nodes;
            this.start = start;
        }

        private Node findSmallestDistance(Hashtable dist, List<Node> Q)
        {
            int distance = int.MaxValue;
            Node node = null;

            foreach (Node n in Q)
            {
                if ((int)dist[n] < distance)
                {
                    distance = (int)dist[n];
                    node = n;
                }
            }
            return node;
        }

        private Node[] findNeighbor(Node n)
        {
            List<Node> neighbor = new List<Node>();
            if (n.GetWall(0))
                neighbor.Add(n.Up);
            if (n.GetWall(1))
                neighbor.Add(n.Right);
            if (n.GetWall(2))
                neighbor.Add(n.Down);
            if (n.GetWall(3))
                neighbor.Add(n.Left);

            return neighbor.ToArray<Node>();
        }

        public Hashtable DijkstraSolving()
        {
            //hashtable to store the sum (or distance)
            Hashtable dist = new Hashtable();
            //List to store all the nodes
            List<Node> Q = new List<Node>();

            for (int x = 0; x < grid.Count; x++)
            {
                for (int y = 0; y < grid[x].Count; y++)
                {
                    Node n = grid[x][y];
                    //set node as key and value to infinity (or int max)
                    //because all sum (or distance) is infinity
                    dist[n] = int.MaxValue;
                    //add node to list
                    Q.Add(n);
                }
            }

            //set the first sum to the first value in the grid
            //this is where we set the start
            dist[start] = 0;

            while (Q.Count > 0)
            {
                //find the node with the smallest sum (or shortest distance)
                Node u = findSmallestDistance(dist, Q);

                //Check if there is only non-connected nodes left
                if (u == null)
                    break;

                //remove it from the list Q
                Q.Remove(u);

                //if the smallest sum (or distance) is infinity
                //then no need to continue
                if ((int)dist[u] == int.MaxValue)
                    break;

                //locate the neighbors in the grid
                Node[] neighbors = findNeighbor(u);
                foreach (Node v in neighbors)
                {
                    //add the neighbor's value to the sum (or distance) of current node. 
                    int alt = (int)dist[u] + 1;//value is always the same; + v.value;
                    //if the value is smaller, replace that value
                    if (alt < (int)dist[v])
                    {
                        dist[v] = alt;
                    }
                }
            }
            
            return dist;
        }
    }
}