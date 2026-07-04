using System;
using UnityEngine;

namespace TacticalTerrain
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class TerrainGrid_Demo_MB : MonoBehaviour
    {
        public int width = 16;
        public int height = 16;

        public TerrainStyle style = new TerrainStyle();

        public float initialHeight = 0f;

        private TerrainCell[,] cells;

        private Mesh mesh;
        private MeshCollider meshCollider;

        private TerrainMeshBuilder builder;

        // =========================================================
        // INIT
        // =========================================================

        void Awake()
        {
            mesh = new Mesh();
            mesh.name = "Procedural Terrain";

            GetComponent<MeshFilter>().sharedMesh = mesh;

            meshCollider = GetComponent<MeshCollider>();

            builder = new TerrainMeshBuilder();

            CreateCells();
            RebuildMesh();
        }

        private void CreateCells()
        {
            cells = new TerrainCell[width, height];

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    cells[x, z] = new TerrainCell
                    {
                        SurfaceID = 0,
                        Heights = new CellHeights(
                            initialHeight,
                            initialHeight,
                            initialHeight,
                            initialHeight)
                    };
                }
            }
        }

        // =========================================================
        // UPDATE INPUT (SIMPLE DEMO INTERACTION)
        // =========================================================

        void Update()
        {
            // TODO: This demo uses the old UnityEngine.Input class instead of InputSystem.
            if (Input.GetMouseButtonDown(0))
            {
                ModifyCell(+1f);
            }

            if (Input.GetMouseButtonDown(1))
            {
                ModifyCell(-1f);
            }
        }

        // =========================================================
        // TERRAIN EDITING (SPELL SIMULATION)
        // =========================================================

        private void ModifyCell(float delta) {
            if (!TryGetCellUnderMouse(out int x, out int z, out bool isCorner))
                return;

            if ( !isCorner ) {
                TerrainCell cell = cells[x, z];

                // Raise/lower ALL corners equally (simple spell)
                cell.SW = Mathf.Max( cell.SW + delta, 0 );
                cell.SE = Mathf.Max( cell.SE + delta, 0 );
                cell.NE = Mathf.Max( cell.NE + delta, 0 );
                cell.NW = Mathf.Max( cell.NW + delta, 0 );

                cells[x, z] = cell;
            } else {
                // This is a corner, so raise all the surrounding cell verts
                if ( x > 0 && z > 0 ) cells[x - 1, z - 1].NE = Mathf.Max( 0, cells[x - 1, z - 1].NE + delta );
                if ( x < width && z > 0 ) cells[x, z - 1].NW = Mathf.Max( 0, cells[x, z - 1].NW + delta );
                if ( x < width && z < height ) cells[x, z].SW = Mathf.Max( 0, cells[x, z].SW + delta );
                if ( x > 0 && z < height ) cells[x - 1, z].SE = Mathf.Max( 0, cells[x - 1, z].SE + delta );
            }

            RebuildMesh();
        }

        // =========================================================
        // RAYCAST TO GRID
        // =========================================================
        private const float CORNER_RADIUS = 0.2f; 
        private bool TryGetCellUnderMouse( out int x, out int z, out bool isCorner ) {
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            x = z = 0;
            isCorner = false;

            if ( Physics.Raycast( ray, out RaycastHit hitInfo ) ) {
                if ( hitInfo.collider != meshCollider ) {
                    return false;
                }
            }
            
            
            
            Vector3 hitPoint = hitInfo.point;
            x = Mathf.FloorToInt( hitPoint.x / style.CellSize );
            z = Mathf.FloorToInt( hitPoint.z / style.CellSize );
            if ( x < 0 || z < 0 || x >= width || z >= height ) {
                return false;
            }
            
            // Determine isCorner and adjust
            float xFrac = hitPoint.x - x;
            float zFrac = hitPoint.z - z;
            if ( xFrac > 0.5f ) xFrac--;
            if ( zFrac > 0.5f ) zFrac--;
            if ( new Vector2( xFrac, zFrac ).magnitude <= CORNER_RADIUS ) {
                isCorner = true;
                if ( xFrac < 0 ) x++;
                if ( zFrac < 0 ) z++;
            }

            return true;
        }

        // =========================================================
        // MESH REBUILD
        // =========================================================

        private void RebuildMesh()
        {
            mesh = builder.Build(cells, style, mesh);

            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }

        /// <summary>
        /// This is for Debugging the verts on the mesh (due to winding issues).<para/>
        /// It can probably be removed.<para/>
        /// - JGB 2026-07-04
        /// </summary>
        private void OnDrawGizmos() {
            builder?.OnDrawGizmos();
        }
    }
}