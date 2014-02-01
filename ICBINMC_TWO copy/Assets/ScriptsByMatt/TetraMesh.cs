using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetraMesh : MonoBehaviour {

//	public Vector3[] newVertices;
//	public Vector2[] newUV;
//	public int[] newTriangles;
//
//
//	void Start() {
//		Mesh mesh = new Mesh();
//		GetComponent<MeshFilter>().mesh = mesh;
////		mesh.vertices = newVertices;
////		mesh.uv = newUV;
////		mesh.triangles = newTriangles;
//
//		// morton nobel's code:
//
////		MeshFilter meshFilter = GetComponent();
////		if (meshFilter==null){
////			Debug.LogError("MeshFilter not found!");
////			return;
////		}
//
//		Vector3 p0 = new Vector3(0,0,0);
//		Vector3 p1 = new Vector3(1,0,0);
//		Vector3 p2 = new Vector3(0.5f,0,Mathf.Sqrt(0.75f));
//		Vector3 p3 = new Vector3(0.5f,Mathf.Sqrt(0.75f),Mathf.Sqrt(0.75f)/3);
//
//		//CUBE verts
//		Vector3 c0 = new Vector3 (0, 0, 0);
//		Vector3 c1 = new Vector3 (1, 0, 0);
//		Vector3 c2 = new Vector3 (1, 0, 1); 
//		Vector3 c3 = new Vector3 (0, 0, 1);
//		Vector3 c4 = new Vector3 (0, 1, 0);
//		Vector3 c5 = new Vector3 (1, 1, 0);
//		Vector3 c6 = new Vector3 (1, 1, 1);
//		Vector3 c7 = new Vector3 (0, 1, 1);
//
////		Mesh mesh = meshFilter.sharedMesh;
//		mesh.Clear();
//
//		//vertices for cube in clockwise order
////		mesh.vertices = new Vector3[]{
////			c0,c1,c2,
////			c0,c2,c3, // bottom face
////			c0,c4,c5,
////			c0,c5,c1,
////
////		};
////		mesh.vertices = new Vector3[]{
////			p0,p1,p2,
////			p0,p2,p3,
////			p2,p1,p3,
////			p0,p3,p1
////		};
////		mesh.triangles = new int[]{
////			0,1,2,
////			3,4,5,
////			6,7,8,
////			9,10,11
////		};
//
//		// shared vertices
//		mesh.vertices = new Vector3[]{
//			c0,c1,c2,
//			c3,c4,c5, // bottom face
//			c6,c7
//		};
//
//		//shared verts way?
//		mesh.triangles = new int[]{
//			0,1,2,
//			0,2,3,
//			2,6,3,
//			6,3,7
//		};
//
////		mesh.triangles = new int[]{
////			0,1,2,
////			3,4,5,
////			6,7,8,
////			9,10,11
////		};
//
////		Vector2 uv3a = new Vector2(0,0);
////		Vector2 uv1  = new Vector2(0.5f,0);
////		Vector2 uv0  = new Vector2(0.25f,Mathf.Sqrt(0.75f)/2);
////		Vector2 uv2  = new Vector2(0.75f,Mathf.Sqrt(0.75f)/2);
////		Vector2 uv3b = new Vector2(0.5f,Mathf.Sqrt(0.75f));
////		Vector2 uv3c = new Vector2(1,0);
////
////		mesh.uv = new Vector2[]{
////			uv0,uv1,uv2,
////			uv0,uv2,uv3b,
////			uv0,uv1,uv3a,
////			uv1,uv2,uv3c
////		};
//
//		float th = 0.333f;
//		float qu = 0.25f;
//
//		Vector2 uv0 = new Vector2 (th, 0);
//		Vector2 uv1 = new Vector2 (th * 2, 0);
//		Vector2 uv2 = new Vector2 (th * 2, qu);
//		Vector2 uv3 = new Vector2 (th , qu );
//
//		Vector2 uv6 = new Vector2 (th * 2, qu * 2);
//		Vector2 uv7 = new Vector2 (th , qu * 2 );
//
//		mesh.uv = new Vector2[] { uv0, uv1, uv2, uv3, uv1, uv3, uv6, uv7 };
//
////		mesh.uv = uvsForCubeSides (1); // ((uint)mesh.triangles.Length/6);
//
//		mesh.RecalculateNormals();
//		mesh.RecalculateBounds();
//		mesh.Optimize();
//
//
//
//	}
//
//	Vector2[] uvsForCubeSides(uint num_sides) // to do: a more sophisticated version that knows what 'up' is
//	{
//		List<Vector2> uvcoords = new List<Vector2>();
//		Vector2 uv0 = new Vector2 (0, 0);
//		Vector2 uv1 = new Vector2 (0, 1);
//		Vector2 uv2 = new Vector2 (1, 1);
//		Vector2 uv3 = new Vector2 (1, 0);
//
//		for (int i = 0 ; i < num_sides ; ++i)
//		{
//			uvcoords.Add (uv0);
//			uvcoords.Add (uv1);
//			uvcoords.Add (uv2);
//
//			uvcoords.Add (uv0);
//			uvcoords.Add (uv2);
//			uvcoords.Add (uv3);
//		}
//
//		return uvcoords.ToArray ();
//	}
//
//	
//	// Update is called once per frame
//	void Update() {
////		Mesh mesh = GetComponent<MeshFilter>().mesh;
////		Vector3[] vertices = mesh.vertices;
////		Vector3[] normals = mesh.normals;
////		int i = 0;
////		while (i < vertices.Length) {
////			vertices[i] += normals[i] * Mathf.Sin(Time.time);
////			i++;
////		}
//
//		transform.Rotate (Vector3.right, 30.0f * Time.deltaTime);
//		transform.Rotate (Vector3.up, 20.0f * Time.deltaTime);
//
////		mesh.vertices = vertices;
//	}
}
