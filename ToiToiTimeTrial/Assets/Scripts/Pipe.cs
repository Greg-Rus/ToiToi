using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float CurveRadius;
    public float PipeRadius;
    public int PipeSegmentCount;
    private int _curveSegmentCount;
    public float RingDistance;
    public float CurveAngle;
    public float minCurveRadius, maxCurveRadius;
    public int minCurveSegmentCount, maxCurveSegmentCount;

    public MeshFilter MyMeshFilter;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;
    private Vector2[] uv;

    public GameObject[] Obstacles;

    // Start is called before the first frame update
    void Awake()
    {
        CurveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        _curveSegmentCount =
            Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);

        MyMeshFilter.name = "Pipe";
        _mesh = new Mesh();
        MyMeshFilter.mesh = _mesh;
        SetVertices();
        SetTriangles();
        SetUV();
        _mesh.RecalculateNormals();

        SpawnObstacles();
    }

    private void SpawnObstacles()
    {
        ParentObstacleAlongPipe(Instantiate(Obstacles[Random.Range(0,Obstacles.Length)]).transform, 0.25f);
        ParentObstacleAlongPipe(Instantiate(Obstacles[Random.Range(0, Obstacles.Length)]).transform, 0.5f);
        ParentObstacleAlongPipe(Instantiate(Obstacles[Random.Range(0, Obstacles.Length)]).transform, 0.75f);
    }

    private void ParentObstacleAlongPipe(Transform obstacle, float pipePercentage)
    {
        obstacle.transform.SetParent(transform);
        obstacle.transform.rotation = transform.rotation;
        obstacle.transform.Rotate(Vector3.back * pipePercentage * CurveAngle);
        obstacle.transform.position = transform.position;
        obstacle.transform.position += obstacle.transform.up * CurveRadius;
        obstacle.transform.Rotate(Vector3.right * Random.Range(0f,360f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetPointOnTorus(float angleAlongCurve, float angleAlongPipe)
    {
        Vector3 p;
        float r = (CurveRadius + PipeRadius * Mathf.Cos(angleAlongPipe));
        p.x = r * Mathf.Sin(angleAlongCurve);
        p.y = r * Mathf.Cos(angleAlongCurve);
        p.z = PipeRadius * Mathf.Sin(angleAlongPipe);
        return p;
    }

    public Vector3 GetPointOnCurve(float angleAlongCurve)
    {
        Vector3 p;
        float r = (CurveRadius);
        p.x = r * Mathf.Sin(angleAlongCurve);
        p.y = r * Mathf.Cos(angleAlongCurve);
        p.z = 0;
        return p;
    }

    private void SetUV()
    {
        uv = new Vector2[_vertices.Length];
        for (int i = 0; i < _vertices.Length; i += 4)
        {
            uv[i] = Vector2.zero;
            uv[i + 1] = Vector2.right;
            uv[i + 2] = Vector2.up;
            uv[i + 3] = Vector2.one;
        }
        _mesh.uv = uv;

    }

    //private void SetUV()
    //{
    //    uv = new Vector2[_vertices.Length];
    //    float offset = 0f;
    //    float step = 1 / (float)PipeSegmentCount;
    //    for (int i = 0; i < _vertices.Length; i += 4)
    //    {
    //        uv[i] = new Vector2(offset, 0);
    //        uv[i + 1] = new Vector2(offset+step, 0);
    //        uv[i + 2] = new Vector2(offset, 1);
    //        uv[i + 3] = new Vector2(offset + step, 1);
    //        offset += step;
    //        if (offset > 1) offset = 0f;
    //    }
    //    _mesh.uv = uv;
    //}

    private void SetVertices()
    {
        _vertices = new Vector3[PipeSegmentCount * _curveSegmentCount * 4];
        float uStep = RingDistance / _curveSegmentCount;
        CurveAngle = uStep * _curveSegmentCount * (360f / (2f * Mathf.PI));
        CreateFirstQuadRing(uStep);

        int iDelta = PipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= _curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }

        _mesh.vertices = _vertices;
    }

    private void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / PipeSegmentCount;
        int ringOffset = PipeSegmentCount * 4;

        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= PipeSegmentCount; v++, i += 4)
        {
            _vertices[i] = _vertices[i - ringOffset + 2];
            _vertices[i + 1] = _vertices[i - ringOffset + 3];
            _vertices[i + 2] = vertex;
            _vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }

    private void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / PipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f);
        Vector3 vertexB = GetPointOnTorus(u, 0f);
        for (int v = 1, i = 0; v <= PipeSegmentCount; v++, i += 4)
        {
            _vertices[i] = vertexA;
            _vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
            _vertices[i + 2] = vertexB;
            _vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }

    private void SetTriangles()
    {
        _triangles = new int[PipeSegmentCount * _curveSegmentCount * 6];
        for (int t = 0, i = 0; t < _triangles.Length; t += 6, i += 4)
        {
            _triangles[t] = i;
            _triangles[t + 1] = _triangles[t + 4] = i + 2;
            _triangles[t + 2] = _triangles[t + 3] = i + 1;
            _triangles[t + 5] = i + 3;
        }
        _mesh.triangles = _triangles;
    }

    public void AlignWith(Pipe pipe)
    {
        float relativeRotation = Random.Range(0, _curveSegmentCount) * 360f / PipeSegmentCount;

        transform.SetParent(pipe.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.CurveAngle);
        transform.Translate(0f, pipe.CurveRadius, 0f);
        transform.Rotate(relativeRotation, 0f, 0f);
        transform.Translate(0f, -CurveRadius, 0f);
        transform.SetParent(pipe.transform.parent);
    }
}
