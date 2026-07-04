using UnityEngine;
using System.Collections.Generic;

internal sealed class MeshData {
    public readonly List<Vector3> Vertices  = new();
    public readonly List<int>     Triangles = new();
    public readonly List<Vector2> UV        = new();
    public readonly List<Color32> Colors    = new();

    public void Clear() {
        Vertices.Clear();
        Triangles.Clear();
        UV.Clear();
        Colors.Clear();
    }

    public Mesh ToMesh( Mesh mesh ) {
        mesh.Clear();

        mesh.SetVertices( Vertices );
        mesh.SetTriangles( Triangles, 0 );
        mesh.SetUVs( 0, UV );
        mesh.SetColors( Colors );

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
