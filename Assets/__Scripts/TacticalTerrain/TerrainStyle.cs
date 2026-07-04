using System;

namespace TacticalTerrain
{
    [Serializable]
    public class TerrainStyle
    {
        public float CellSize = 1f;

        public float Inset = 0.05f;

        public QuadSeamMode SeamMode =
            QuadSeamMode.MinimizeCrease;

        public bool GenerateWalls = true;

        public bool GenerateBottom = false;
    }
}
