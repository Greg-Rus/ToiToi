﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public MeshRenderer MyMeshRenderer;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private int[] _triangles;
    private Vector2[] uv;

    public GameObject[] Obstacles;
    public GameObject Moment;
    public Material Material;
    public float AccumulatedRotation;

    // Start is called before the first frame update
    void Awake()
    {
        MyMeshRenderer.material = new Material(Material);
        CurveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        _curveSegmentCount =
            Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);

        MyMeshFilter.name = "Pipe";
        _mesh = new Mesh();
        MyMeshFilter.mesh = _mesh;
        SetVertices();
        SetTriangles();
        SetUV();

        SpawnObstacles();
        SpawnMoments();
    }

    private void SpawnMoments()
    {
        ParentPickupAlongPipe(
            Instantiate(Moment).transform,
            0.25f,
            Random.Range(0, PipeRadius));

        ParentPickupAlongPipe(
            Instantiate(Moment).transform,
            0.75f,
            Random.Range(0, PipeRadius));
    }

    private void SpawnObstacles()
    {
        ParentObstacleAlongPipe(Instantiate(Obstacles[Random.Range(0,Obstacles.Length)]).transform, 0f);
        ParentObstacleAlongPipe(Instantiate(Obstacles[Random.Range(0, Obstacles.Length)]).transform, 0.5f);
        //ParentObstacleAlongPipe(Instantiate(Obstacles[Random.Range(0, Obstacles.Length)]).transform, 0.75f);
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

    private void ParentPickupAlongPipe(Transform pickup, float pipePercentage, float distanceFromCenter)
    {
        pickup.transform.SetParent(transform);
        pickup.transform.rotation = transform.rotation;
        pickup.transform.Rotate(Vector3.back * pipePercentage * CurveAngle);
        pickup.transform.position = transform.position;
        pickup.transform.position += pickup.transform.up * CurveRadius;
        pickup.transform.position -= pickup.transform.up * distanceFromCenter;
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
        int index = 0;
        int curveIndex = 0;
        float step = 1f / PipeSegmentCount;
        float curveStep = 1f / _curveSegmentCount;
        for (int i = 0; i < _vertices.Length; i += 4)
        {

            var offsetClose = index * step;

            var offsetFar = offsetClose + step;
            if (index == PipeSegmentCount - 1)
            {
                offsetFar = 1f;
            }

            var curveClose = curveIndex * curveStep;
            var curveFar = curveClose + curveStep;

            uv[i] = new Vector2(offsetClose, curveClose);
            uv[i + 1] = new Vector2(offsetFar, curveClose);
            uv[i + 2] = new Vector2(offsetClose, curveFar);
            uv[i + 3] = new Vector2(offsetFar, curveFar);


            index++;
            if (index == PipeSegmentCount)
            {
                index = 0;
                curveIndex++;
            }

        }
        _mesh.uv = uv;

    }

    private void SetVertices()
    {
        _vertices = new Vector3[PipeSegmentCount * _curveSegmentCount * 4];
        _normals = new Vector3[PipeSegmentCount * _curveSegmentCount * 4];
        float uStep = RingDistance / _curveSegmentCount;
        CurveAngle = uStep * _curveSegmentCount * (360f / (2f * Mathf.PI));
        CreateFirstQuadRing(uStep);

        int iDelta = PipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= _curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }

        _mesh.vertices = _vertices;
        _mesh.normals = _normals;
    }

    private void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / PipeSegmentCount;
        int ringOffset = PipeSegmentCount * 4;
        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= PipeSegmentCount; v++, i += 4)
        {
            _vertices[i] = _vertices[i - ringOffset + 2];
            _normals[i] = _normals[i - ringOffset + 2];

            _vertices[i + 1] = _vertices[i - ringOffset + 3];
            _normals[i + 1] = _normals[i - ringOffset + 3];

            _vertices[i + 2] = vertex;
            _normals[i + 2] = _vertices[i + 2].GetNormalToCurvePoint(GetPointOnCurve(u));

            _vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
            _normals[i + 3] = _vertices[i + 3].GetNormalToCurvePoint(GetPointOnCurve(u));
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
            _normals[i] = _vertices[i].GetNormalToCurvePoint(GetPointOnCurve(0));

            _vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
            _normals[i + 1] = _vertices[i + 1].GetNormalToCurvePoint(GetPointOnCurve(0));

            _vertices[i + 2] = vertexB;
            _normals[i + 2] = _vertices[i + 2].GetNormalToCurvePoint(GetPointOnCurve(u));

            _vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
            _normals[i + 3] = _vertices[i + 3].GetNormalToCurvePoint(GetPointOnCurve(u));
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
        var relativeRotation = Random.Range(0, PipeSegmentCount /2) * 360f / PipeSegmentCount;
        AccumulatedRotation = pipe.AccumulatedRotation + relativeRotation;
        if (AccumulatedRotation >= 360f)
        {
            AccumulatedRotation = AccumulatedRotation - 360;
        }

        float normalizedRotationOffset = AccumulatedRotation / 180f;

        
        MyMeshRenderer.material.SetFloat("Vector1_487CCA14", normalizedRotationOffset);
        
        transform.SetParent(pipe.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.CurveAngle);
        transform.Translate(0f, pipe.CurveRadius, 0f);
        transform.Rotate(relativeRotation, 0f, 0f);
        transform.Translate(0f, -CurveRadius, 0f);
        transform.SetParent(pipe.transform.parent);
    }


}

public static class VertexHelper
{
    public static Vector3 GetNormalToCurvePoint(this Vector3 vertex, Vector3 curvePoint)
    {
        return (curvePoint - vertex).normalized;
    }
}
