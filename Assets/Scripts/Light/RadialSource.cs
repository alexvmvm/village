using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RadialSource : MonoBehaviour
{
    public LayerMask LayerMask;
    public float Angle;
    public float ZPosition;
    public int SortingOrder;
    [Range(0f, 360f)]
    public float Spread;
    public float Radius;
    public float EdgeDistanceThreshold;
    [Range(1, 10)]
    public int EdgeResolveIterations;

    [Range(4, 40)]
    public int Segments;

    [Range(0.001f, 1f)]
    public float UpdateRate;

    [Header("Flicker")]
    public bool FlickerEnabled;
    [Range(0.1f, 1f)]
    public float FlickerRange;
    [Range(0, 100)]
    public float FlickerSpeed;

    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;

    private List<BoundaryPoint> _hits;
    private Mesh _mesh;

    private float _time;

    struct BoundaryPoint
    {
        public float angle;
        public bool hit;
        public Vector3 point;
        public Vector3 normal;
        public Color debug;
    }

    struct EdgeInfo
    {
        public BoundaryPoint pointA;
        public BoundaryPoint pointB;
    }

    void Start()
    {
        _hits = new List<BoundaryPoint>();
        _mesh = new Mesh();
        _mesh.name = transform.name;

        MeshFilter.mesh = _mesh;
		MeshRenderer.material.SetFloat("_Radius", Radius);

        if(Application.isPlaying)
            StartCoroutine(UpdateSource());

        var position = transform.position;
        position.z = ZPosition;
        transform.position = position;

        MeshRenderer.sortingOrder = SortingOrder;
    }

    BoundaryPoint FindBoundaryPoint(float angle)
    {
        var bp = new BoundaryPoint { angle = angle, hit = false };
        var angleVector = (Vector3)GetVectorAtAngle(angle);
        var hit = Physics2D.Raycast(transform.position, angleVector, Radius, LayerMask);
        if(hit.collider != null)
        {
            bp.hit = true;
            bp.point = hit.point;
            bp.point.z = transform.position.z;
            bp.normal = hit.normal;
        }
        else
        {
            bp.point = transform.position + angleVector * Radius;
            bp.normal = angleVector;
        }

        return bp;
    }

    void UpdatePoints()
    {
        if (_hits == null)
            _hits = new List<BoundaryPoint>();
        else
            _hits.Clear();

        var angleIncrement = (360f / Segments);
        var lastPoint = new BoundaryPoint();
        for (var i = 0; i <= Segments; i++)
        {
            var angle = Angle + i * angleIncrement;
            var bp = FindBoundaryPoint(angle);
            bp.debug = Color.red;

            if (i > 0)
            {
                if((bp.hit || lastPoint.hit) && (bp.normal != lastPoint.normal || Vector2.Distance(bp.point, lastPoint.point) > EdgeDistanceThreshold))
                {
                    var prevAngle = angle - angleIncrement;
                    for(var k = 1; k < EdgeResolveIterations; k++)
                    {
                        var nbp = FindBoundaryPoint(prevAngle + k * (angleIncrement/EdgeResolveIterations));
                        nbp.debug = Color.cyan;
                        _hits.Add(nbp);
                    }
                }

            }

            _hits.Add(bp);
            lastPoint = bp;
        }


        var hits = _hits.OrderBy(x => x.angle).ToArray();
        var vertexCount = _hits.Count + 1;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector2.zero;
        for (var i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(hits[i].point);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }

    IEnumerator UpdateSource()
    {
        for (;;)
        {
            UpdatePoints();
            yield return new WaitForSeconds(UpdateRate);
        }
    }

    EdgeInfo FindEdge(BoundaryPoint bp1, BoundaryPoint bp2)
    {
        var min = bp1;
        var max = bp2;

        for(var i = 0; i < EdgeResolveIterations; i++)
        {
            var angle = min.angle + (max.angle - min.angle) / 2;
            var bp = FindBoundaryPoint(angle);

            if (bp.hit == min.hit)
                min = bp;
            else
                max = bp;
        }

        min.debug = Color.white;
        max.debug = Color.white;

        return new EdgeInfo
        {
            pointA = min,
            pointB = max
        };
    }

    Vector2 GetVectorAtAngle(float angle)
    { 
        return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    void Update()
    {
        if(!Application.isPlaying)
            UpdatePoints();    

        if(FlickerEnabled)
        {
            _time += Time.deltaTime;
            var perlin = Mathf.PerlinNoise(_time * FlickerSpeed, .1f);
            var range = Radius - Radius * FlickerRange * perlin;
            MeshRenderer.material.SetFloat("_Radius", range);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_hits != null)
        {
            var points = _hits.OrderBy(x => x.angle).ToList();
            foreach (var p in points)
            {
                Gizmos.color = p.debug;
                Gizmos.DrawLine(transform.position, p.point);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(p.point, p.point + p.normal);
            }

            for (var i = 0; i < points.Count; i++)
            {
                Gizmos.DrawLine(points[i].point, points[(i + 1) % points.Count].point);
            }
        }
    }
}
