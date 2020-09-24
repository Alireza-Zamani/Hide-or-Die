using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOFView : MonoBehaviour
{
	Mesh mesh;
	[SerializeField] private float fov = 90f;
	[SerializeField] private int rayCount = 2;
	[SerializeField] private float viewDistance = 10f;
	[SerializeField] private LayerMask fovLayerMask = new LayerMask();
	private Vector3 origin = Vector3.zero;

	private void Start()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
	}

	private void Update()
	{
		BakeMesh();

	}

	private void BakeMesh()
	{
		float angle = 0f;
		
		float angleIncrease = fov / rayCount;

		//Mesh Fields
		Vector3[] vertices = new Vector3[rayCount + 2];
		Vector2[] uv = new Vector2[vertices.Length];
		int[] triangles = new int[rayCount * 3];


		vertices[0] = origin;

		//Indexes
		int verticesIndex = 1;
		int trianglesIndex = 0;

		//Bake The Mesh
		for (int i = 0; i <= rayCount; i++)
		{
			Vector3 vertic = Vector3.zero;
			RaycastHit2D hit = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance , fovLayerMask);
			if (hit.collider == null)
			{
				//No hit
				vertic = origin + GetVectorFromAngle(angle) * viewDistance;
			}
			else
			{
				//Hited a object
				vertic = hit.point;
			}

			vertices[verticesIndex] = vertic;

			if (i > 0)
			{
				triangles[trianglesIndex] = 0;
				triangles[trianglesIndex + 1] = verticesIndex - 1;
				triangles[trianglesIndex + 2] = verticesIndex;

				trianglesIndex += 3;
			}

			verticesIndex++;
			angle -= angleIncrease;
		}

		//Set the mesh
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
	}

	public void SetTheOrigin(Vector3 origin)
	{
		this.origin = origin;
	}

	private Vector3 GetVectorFromAngle(float angle)
	{
		float angleRad = angle * (Mathf.PI / 180f);
		return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

	private float GetAngleFromVectorFloat(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if(n < 0)
		{
			n += 360;
		}
		return n;
	}

}

