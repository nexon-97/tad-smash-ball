using UnityEngine;
using System;
using System.Collections.Generic;

public class PlatformMesh : MonoBehaviour
{
    [System.Serializable]
    public class HoleDef
	{
        public int offset;
        public int size;
	}

    public int segments = 64;
    public float height = 0.2f;
    public float size = 5.0f;
    public HoleDef[] holes;

    public struct HitResult
	{
        public bool collisionHit;
        public bool death;
    }

    private void RegenerateMesh()
	{
        Mesh mesh = new Mesh
        {
            name = "Procedural Mesh"
        };

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> tangents = new List<Vector4>();
        List<int> triangles = new List<int>();

        // Add central vertices
        vertices.Add(new Vector3(0, height, 0));
        vertices.Add(new Vector3(0, 0, 0));
        normals.Add(Vector3.up);
        normals.Add(Vector3.down);
        tangents.Add(new Vector4(1f, 0f, 0f, -1f));
        tangents.Add(new Vector4(1f, 0f, 0f, -1f));

        bool[] segmentsMask = new bool[segments];
        Array.Fill(segmentsMask, true);
        foreach (HoleDef hole in holes)
        {
            for (int i = 0; i < hole.size; i++)
            {
                segmentsMask[hole.offset + i] = false;
            }
        }

        // Add radial segments
        float step = 2.0f * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(i * step) * size;
            float y = Mathf.Sin(i * step) * size;

            vertices.Add(new Vector3(x, height, y));
            normals.Add(Vector3.up);
            tangents.Add(new Vector4(1f, 0f, 0f, -1f));

            vertices.Add(new Vector3(x, 0, y));
            normals.Add(Vector3.down);
            tangents.Add(new Vector4(1f, 0f, 0f, -1f));

            if (segmentsMask[i])
            {
                triangles.Add(2 + (i * 2 + 2) % (segments * 2));
                triangles.Add(2 + i * 2);
                triangles.Add(0);

                triangles.Add(1);
                triangles.Add(2 + i * 2 + 1);
                triangles.Add(2 + (i * 2 + 3) % (segments * 2));
            }
        }

        // Add radial segments for sides
        int faceOffset = vertices.Count;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(i * step) * size;
            float y = Mathf.Sin(i * step) * size;
            Vector3 normal = new Vector3(x, 0, y);

            vertices.Add(new Vector3(x, height, y));
            normals.Add(normal);
            tangents.Add(new Vector4(1f, 0f, 0f, -1f));

            vertices.Add(new Vector3(x, 0, y));
            normals.Add(normal);
            tangents.Add(new Vector4(1f, 0f, 0f, -1f));

            if (segmentsMask[i])
            {
                triangles.Add(faceOffset + i * 2);
                triangles.Add(faceOffset + (i * 2 + 2) % (segments * 2));
                triangles.Add(faceOffset + (i * 2 + 1) % (segments * 2));
                triangles.Add(faceOffset + (i * 2 + 1) % (segments * 2));
                triangles.Add(faceOffset + (i * 2 + 2) % (segments * 2));
                triangles.Add(faceOffset + (i * 2 + 3) % (segments * 2));
            }
        }

        // Fix holes
        foreach (HoleDef hole in holes)
        {
            faceOffset = vertices.Count;
            float x1 = Mathf.Cos(hole.offset * step) * size;
            float y1 = Mathf.Sin(hole.offset * step) * size;
            float x2 = Mathf.Cos((hole.offset + hole.size) * step) * size;
            float y2 = Mathf.Sin((hole.offset + hole.size) * step) * size;

            vertices.Add(new Vector3(0, height, 0));
            vertices.Add(new Vector3(0, 0, 0));
            vertices.Add(new Vector3(x1, 0, y1));
            vertices.Add(new Vector3(x1, height, y1));
            vertices.Add(new Vector3(0, height, 0));
            vertices.Add(new Vector3(0, 0, 0));
            vertices.Add(new Vector3(x2, 0, y2));
            vertices.Add(new Vector3(x2, height, y2));

            Vector3 normal1 = Quaternion.AngleAxis(-90, Vector3.up) * new Vector3(x1, 0, y1);
            Vector3 normal2 = Quaternion.AngleAxis(+90, Vector3.up) * new Vector3(x2, 0, y2);

            for (int j = 0; j < 4; j++)
            {
                normals.Add(normal1);
            }

            for (int j = 0; j < 4; j++)
            {
                normals.Add(normal2);
            }

            for (int j = 0; j < 8; j++)
            {

                tangents.Add(new Vector4(1f, 0f, 0f, -1f));
            }

            triangles.Add(faceOffset);
            triangles.Add(faceOffset + 1);
            triangles.Add(faceOffset + 2);
            triangles.Add(faceOffset + 2);
            triangles.Add(faceOffset + 3);
            triangles.Add(faceOffset);

            faceOffset += 4;
            triangles.Add(faceOffset + 2);
            triangles.Add(faceOffset + 1);
            triangles.Add(faceOffset);
            triangles.Add(faceOffset);
            triangles.Add(faceOffset + 3);
            triangles.Add(faceOffset + 2);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.tangents = tangents.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    [ExecuteInEditMode]
    void OnEnable()
    {
        RegenerateMesh();
    }

    void Start()
	{
        RegenerateMesh();
    }

    public HitResult CheckHit()
	{
        float rotationRad = Mathf.Repeat(Mathf.Deg2Rad * transform.rotation.eulerAngles.y - Mathf.PI * 0.5f, Mathf.PI * 2);
        float step = 2.0f * Mathf.PI / segments;

        bool holeFound = false;
        foreach (HoleDef hole in holes)
        {
            float startFactor = hole.offset * step;
            float endFactor = (hole.offset + hole.size) * step;
            if (rotationRad >= startFactor && rotationRad <= endFactor)
			{
                holeFound = true;
                break;
            }
        }

        return new HitResult
        {
            collisionHit = !holeFound,
            death = false,
		};
	}
}
