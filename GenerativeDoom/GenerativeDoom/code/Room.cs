using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenerativeDoom
{
    public class Room
    {
        public string file { get; set; }
        public string type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Position origin { get; set; }
        public List<DoorAnchor> doors { get; set; }
        public List<Position> things { get; set; }


        public DoorAnchor getSideDoor(int index)
        {
            foreach(DoorAnchor d in doors)
            {
                if (d.side == index)
                    return d;
            }
            return null;
        }
    }
}
