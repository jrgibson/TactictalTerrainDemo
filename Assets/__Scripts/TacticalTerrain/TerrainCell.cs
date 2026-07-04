using System;

namespace TacticalTerrain
{
    [Serializable]
    public struct TerrainCell
    {
        public CellHeights Heights;

        /// <summary>
        /// User-defined material/type.
        /// Grass, lava, poison, etc.
        /// </summary>
        public byte SurfaceID;

        public float this[Corner c]
        {
            get => Heights[c];
            set => Heights[c] = value;
        }

        public float this[int i]
        {
            get => Heights[i];
            set => Heights[i] = value;
        }

        public float SW {
            get => Heights[Corner.SW];
            set => Heights[Corner.SW] = value;
        }
        public float SE {
            get => Heights[Corner.SE];
            set => Heights[Corner.SE] = value;
        }
        public float NE {
            get => Heights[Corner.NE];
            set => Heights[Corner.NE] = value;
        }
        public float NW {
            get => Heights[Corner.NW];
            set => Heights[Corner.NW] = value;
        }

    }
}
