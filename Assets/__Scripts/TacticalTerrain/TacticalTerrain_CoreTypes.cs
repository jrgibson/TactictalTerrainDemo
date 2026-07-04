namespace TacticalTerrain
{
    /// <summary>
    /// Counter-clockwise starting at the south-west corner.
    ///
    ///   3 ----- 2
    ///   |       |
    ///   |       |
    ///   0 ----- 1
    /// </summary>
    public enum Corner {
        SW = 0, SE = 1, NE = 2, NW = 3
    }

    public enum FaceOrientation {
        Up, // top surface (+Y)
        Down, // underside (rarely used)
        North, // +Z
        South, // -Z
        East, // +X
        West // -X
    }

    public enum QuadDiagonal {
        SW_NE, SE_NW
    }

    public enum QuadSeamMode {
        FixedSW_NE, 
        FixedSE_NW, 
        MinimizeCrease,
        FavorConvex, 
        FavorConcave
    }

    public class TriangleWindingTable
    {
        // 6 faces * 2 diagonals * 6 indices
        private static readonly int[] DATA =
        {
            // =========================
            // UP
            // =========================

            // SW_NE
            0, 1, 2,  0, 2, 3,

            // SE_NW
            0, 1, 3,  1, 2, 3,

            // =========================
            // DOWN
            // =========================

            // SW_NE
            0, 2, 1,  0, 3, 2,

            // SE_NW
            0, 3, 1,  1, 3, 2,

            // =========================
            // NORTH (+Z)
            // =========================

            // SW_NE
            0, 2, 1,  1, 2, 3,

            // SE_NW
            0, 1, 2,  1, 3, 2,

            // =========================
            // SOUTH (-Z)
            // =========================

            // SW_NE
            0, 1, 2,  1, 3, 2,

            // SE_NW
            0, 2, 1,  1, 2, 3,

            // =========================
            // EAST (+X)
            // =========================

            // SW_NE
            0, 1, 2,  1, 3, 2,

            // SE_NW
            0, 2, 1,  1, 2, 3,

            // =========================
            // WEST (-X)
            // =========================

            // SW_NE
            0, 2, 1,  1, 2, 3,

            // SE_NW
            0, 1, 2,  1, 3, 2
        };

        private const int FACE_STRIDE = 12;   // 2 diagonals × 6 ints
        private const int DIAG_STRIDE = 6;     // 6 ints per diagonal

        /// <summary>
        /// Allows an instance of TriangleWindingTable to be indexed with [,,]
        /// </summary>
        /// <param name="face"></param>
        /// <param name="diagonal"></param>
        /// <param name="vertIndex"></param>
        public int this[FaceOrientation face, QuadDiagonal diagonal, int vertIndex]
            => Get(face, diagonal, vertIndex);

        /// <summary>
        /// A static Get method that bypasses the need for a TriangleWindingTable instance.
        /// </summary>
        /// <param name="face"></param>
        /// <param name="diagonal"></param>
        /// <param name="vertIndex"></param>
        /// <returns></returns>
        static public int Get( FaceOrientation face, QuadDiagonal diagonal, int vertIndex ) {
            int index =
                ((int)face * FACE_STRIDE) +
                ((int)diagonal * DIAG_STRIDE) +
                vertIndex;

            if ( index < 0 || index >= DATA.Length ) {
#if UNITY_EDITOR
                throw new System.IndexOutOfRangeException(
                    $"TriangleWindingTable[ {face}, {diagonal}, {vertIndex} ]" +
                    $" generated an index={index}, which is out of range!"
                );
#else
                return 0; // This will fail silently if not in editor - JGB 2026-07-04
#endif
            }
            
            return DATA[index];
        }
    }
}



