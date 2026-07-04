// #define DEBUG_MESH_VERTS

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TacticalTerrain
{
    public class TerrainMeshBuilder {
        private const Corner SW = Corner.SW;
        private const Corner SE = Corner.SE;
        private const Corner NE = Corner.NE;
        private const Corner NW = Corner.NW;
        
        private const float  BOTTOM_HEIGHT = -1;

        #if DEBUG_MESH_VERTS
        private Vector3[] debugPoints = null;
        #endif
        
        // Reused buffers (important for spell edits)
        private readonly MeshData meshData = new MeshData();

        // Cached style per build
        private TerrainStyle _style;

        // TriangleWindingTable
        private static readonly TriangleWindingTable TriTable = new TriangleWindingTable();
        // =========================================================
        // PUBLIC ENTRY POINT
        // =========================================================

        public Mesh Build(TerrainCell[,] cells, TerrainStyle style, Mesh targetMesh)
        {
            _style = style;

            meshData.Clear();

            int width = cells.GetLength(0);
            int height = cells.GetLength(1);

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    AddCell(cells, x, z);
                }
            }

            return meshData.ToMesh(targetMesh);
        }

        // =========================================================
        // CELL
        // =========================================================

        private void AddCell(TerrainCell[,] cells, int x, int z)
        {
            TerrainCell cell = cells[x, z];
            
            float cellSize = _style.CellSize;
            float topInset = _style.Inset;

            Vector3 origin = new Vector3( x * cellSize, 0f, z * cellSize );
            
            // Calculate the top points
            Vector3 sw = origin + new Vector3(topInset, cell.SW, topInset);
            Vector3 se = origin + new Vector3(cellSize - topInset, cell.SE, topInset);
            Vector3 ne = origin + new Vector3(cellSize - topInset, cell.NE, cellSize - topInset);
            Vector3 nw = origin + new Vector3(topInset, cell.NW, cellSize - topInset);

            // Add the Top
            QuadDiagonal seam = ChooseTopSeam(cell);
            AddQuad(sw, se, ne, nw, default, seam, cell.SurfaceID);
            
#if DEBUG_MESH_VERTS
            // If this is Cell [0,0], output Debug points
            if ( x == 0 && z == 0 ) {
                debugPoints = new[] { sw, se, ne, nw };
            }
#endif            
            
            // TODO: Replace BOTTOM_HEIGHT with something that adjusts based on adjacent ver heights - JGB 2026-07-04
            Vector3 swB = origin + new Vector3(0, BOTTOM_HEIGHT, 0);
            Vector3 seB = origin + new Vector3(cellSize, BOTTOM_HEIGHT, 0);
            Vector3 neB = origin + new Vector3(cellSize, BOTTOM_HEIGHT, cellSize);
            Vector3 nwB = origin + new Vector3(0, BOTTOM_HEIGHT, cellSize);
            
            // Add the Walls
            AddQuad( swB, seB, se, sw );
            AddQuad( seB, neB, ne, se );
            AddQuad( neB, nwB, nw, ne );
            AddQuad( nwB, swB, sw, nw );
            
        }

        
        // =========================================================
        // QUAD EMISSION
        // =========================================================

        /// <summary>
        /// Converts four verts into two triangles that are added to the mesh. 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="color"></param>
        /// <param name="diagonal"></param>
        /// <param name="surfaceId">TODO: Add a use for surfaceID</param>
        private void AddQuad(
            Vector3 v0,
            Vector3 v1,
            Vector3 v2,
            Vector3 v3,
            Color color=default,
            QuadDiagonal diagonal=QuadDiagonal.SE_NW,
            byte surfaceId=0) {
            
            if ( color == default ) color = Color.gray;
            
            int start = meshData.Vertices.Count;

            meshData.Vertices.Add(v0);
            meshData.Vertices.Add(v1);
            meshData.Vertices.Add(v2);
            meshData.Vertices.Add(v3);

            meshData.UV.Add(new Vector2(0, 0));
            meshData.UV.Add(new Vector2(1, 0));
            meshData.UV.Add(new Vector2(1, 1));
            meshData.UV.Add(new Vector2(0, 1));

            meshData.Colors.Add(color);
            meshData.Colors.Add(color);
            meshData.Colors.Add(color);
            meshData.Colors.Add(color);

            if (diagonal == QuadDiagonal.SW_NE)
            {
                meshData.Triangles.Add(start + 0);
                meshData.Triangles.Add(start + 2);
                meshData.Triangles.Add(start + 1);

                meshData.Triangles.Add(start + 0);
                meshData.Triangles.Add(start + 3);
                meshData.Triangles.Add(start + 2);
            }
            else // SE_NW
            {
                meshData.Triangles.Add(start + 0);
                meshData.Triangles.Add(start + 3);
                meshData.Triangles.Add(start + 1);

                meshData.Triangles.Add(start + 1);
                meshData.Triangles.Add(start + 3);
                meshData.Triangles.Add(start + 2);
            }
        }

        // =========================================================
        // SEAM SELECTION
        // =========================================================

        private QuadDiagonal ChooseTopSeam(TerrainCell cell)
        {
            switch (_style.SeamMode)
            {
                case QuadSeamMode.FixedSW_NE:
                    return QuadDiagonal.SW_NE;

                case QuadSeamMode.FixedSE_NW:
                    return QuadDiagonal.SE_NW;

                case QuadSeamMode.MinimizeCrease:
                default:
                {
                    float d02 = Mathf.Abs(cell.SW - cell.NE);
                    float d13 = Mathf.Abs(cell.SE - cell.NW);

                    return d02 < d13
                        ? QuadDiagonal.SW_NE
                        : QuadDiagonal.SE_NW;
                }
            }
        }

        
        public void OnDrawGizmos() {
#if UNITY_EDITOR
#if DEBUG_MESH_VERTS
            // NOTE: This only works correctly if the Transform of the GameObject is [0,0,0],[0,0,0],[1,1,1] - JGB 2026-07-04
            if ( debugPoints == null ) return;
            Handles.Label(debugPoints[0], "SW");
            Handles.Label(debugPoints[1], "SE");
            Handles.Label(debugPoints[2], "NE");
            Handles.Label(debugPoints[3], "NW");
#endif      
#endif
        }
        
    }
}