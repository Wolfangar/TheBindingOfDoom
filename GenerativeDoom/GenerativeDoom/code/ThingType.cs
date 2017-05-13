using CodeImp.DoomBuilder.Map;
using GenerativeDoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeGen
{
    public class ThingType
    {
        public Position pos;
        public enum Thing_Type { MONSTER, KEY, AMMO, HEALTH, ARMOR, POWER, WEAPON };//, UNLOCKED };
        public Thing_Type currentThingType;
        public Thing reference;

        public ThingType(Position pos, Thing_Type type, Thing reference)
        {
            this.pos = pos;
            this.currentThingType = type;
            this.reference = reference;
        }
    }
}
