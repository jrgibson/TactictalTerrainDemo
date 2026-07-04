using System;

namespace TacticalTerrain
{
    [Serializable]
    public struct CellHeights
    {
        public float SW;
        public float SE;
        public float NE;
        public float NW;

        public CellHeights(
            float sw,
            float se,
            float ne,
            float nw)
        {
            SW = sw;
            SE = se;
            NE = ne;
            NW = nw;
        }

        public float this[Corner c]
        {
            get
            {
                return c switch
                {
                    Corner.SW => SW,
                    Corner.SE => SE,
                    Corner.NE => NE,
                    Corner.NW => NW,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            set
            {
                switch (c)
                {
                case Corner.SW: SW = value; break;
                case Corner.SE: SE = value; break;
                case Corner.NE: NE = value; break;
                case Corner.NW: NW = value; break;
                default:        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public float this[int i]
        {
            get => this[(Corner)i];
            set => this[(Corner)i] = value;
        }
    }
}
