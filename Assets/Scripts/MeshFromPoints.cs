using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct Quad
{
    public Vector2 position;
    public Vector2 uv;
    public float tUnit;
}

[RequireComponent(typeof(MeshFilter))]
public class MeshFromPoints : MonoBehaviour
{
    public Mesh Mesh { get { return _mesh; } }

    private Mesh _mesh;
    private List<int> _indicies;
    private List<Vector2> _uv;
    private List<Vector3> _vertices;
    private MeshCollider[] _colliders;

    private void Awake()
    {
        _mesh = new Mesh();

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>()) 
            meshFilter.mesh = _mesh;

        _colliders = GetComponentsInChildren<MeshCollider>();
        foreach (var meshCollider in _colliders)
            meshCollider.sharedMesh = _mesh;

        _indicies = new List<int>();
        _vertices = new List<Vector3>();
        _uv = new List<Vector2>();
    }

       public void Create(Quad[] quads)
    {
        _indicies.Clear();
        _vertices.Clear();
        _uv.Clear();

        var z = transform.position.z;

        quads.OrderBy(q => q.position.x).ThenBy(q => q.position.y);

        var tUnit = 1 / 32f;

        var index = 0;
        for(var i = 0; i < quads.Length; i++)
        {
            var p = quads[i].position;

            _vertices.Add(new Vector3(p.x, p.y, z));
            _vertices.Add(new Vector3(p.x + 1, p.y, z));
            _vertices.Add(new Vector3(p.x, p.y + 1, z));
            _vertices.Add(new Vector3(p.x + 1, p.y + 1, z));

            _indicies.Add(index * 4 + 0);
            _indicies.Add(index * 4 + 2);
            _indicies.Add(index * 4 + 1);

            _indicies.Add(index * 4 + 2);
            _indicies.Add(index * 4 + 3);
            _indicies.Add(index * 4 + 1);

            var uv = quads[i].uv;

            _uv.Add(new Vector2 (uv.x, uv.y));
            _uv.Add(new Vector2 (uv.x + tUnit, uv.y));
            _uv.Add(new Vector2 (uv.x, uv.y + tUnit));
            _uv.Add(new Vector2 (uv.x + tUnit, uv.y + tUnit));

            index++;
        }

        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _indicies.ToArray();
        _mesh.uv = _uv.ToArray();
        _mesh.Optimize();   
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        foreach (var meshCollider in _colliders)
            meshCollider.sharedMesh = _mesh;
    }

    public void Create(Vector2[] points)
    {
        _indicies.Clear();
        _vertices.Clear();
        _uv.Clear();

        var z = transform.position.z;

        points.OrderBy(p => p.x).ThenBy(p => p.y);

        var tUnit = 1 / 32f;

        var index = 0;
        for(var i = 0; i < points.Length; i++)
        {
            var p = points[i];

            _vertices.Add(new Vector3(p.x, p.y, z));
            _vertices.Add(new Vector3(p.x + 1, p.y, z));
            _vertices.Add(new Vector3(p.x, p.y + 1, z));
            _vertices.Add(new Vector3(p.x + 1, p.y + 1, z));

            _indicies.Add(index * 4 + 0);
            _indicies.Add(index * 4 + 2);
            _indicies.Add(index * 4 + 1);

            _indicies.Add(index * 4 + 2);
            _indicies.Add(index * 4 + 3);
            _indicies.Add(index * 4 + 1);


            _uv.Add(new Vector2 (0, 0));
            _uv.Add(new Vector2 (tUnit, 0));
            _uv.Add(new Vector2 (0, tUnit));
            _uv.Add(new Vector2 (tUnit, tUnit));


            // _uv.Add(new Vector2 (tUnit * tStone.x, tUnit * tStone.y + tUnit));
            // _uv.Add(new Vector2 (tUnit * tStone.x + tUnit, tUnit * tStone.y + tUnit));
            // _uv.Add(new Vector2 (tUnit * tStone.x + tUnit, tUnit * tStone.y));
            // _uv.Add(new Vector2 (tUnit * tStone.x, tUnit * tStone.y));

            index++;
        }

        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _indicies.ToArray();
        _mesh.uv = _uv.ToArray();
        _mesh.Optimize();   
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        foreach (var meshCollider in _colliders)
            meshCollider.sharedMesh = _mesh;
    }
}
