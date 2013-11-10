using UnityEngine;
using System.Collections;

public class BlockPointer : MonoBehaviour {

	public BlockHitter hitHandler; // to do write POVMANAGER (sep class from chunk manager)

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{


//		RaycastHit hit;
//		Vector3 fwd = transform.TransformDirection(Vector3.forward);
//		if (Physics.Raycast(transform.position, fwd , out hit, 100))
//		{
//			float distanceToGround = hit.distance;
//
//			bug ("world point: " + hit.point + " tri index: " + hit.triangleIndex);
//
//
//		}
		Vector3 midScreen = new Vector3 (Screen.width * .5f, Screen.height * .5f, 0f);
		RaycastHit hit;

		if (Input.GetMouseButtonDown (1)) { // right clicks are just clicks (for placing blocks) therefore, use GetMouseButtonDOWN (not GetMouseButton)

			if (!Physics.Raycast(camera.ScreenPointToRay(midScreen), out hit))
				return;

			hitHandler.handleRightButtonHit (hit);

			return;
		}


//		RaycastHit hit;
//		Vector3 midScreen = new Vector3 (Screen.width * .5f, Screen.height * .5f, 0f);
//		if (!Physics.Raycast(camera.ScreenPointToRay(midScreen), out hit))
//			return;
//
//		if (Input.GetMouseButton (0)) { //GetMouseButton(0) is true for the whole time that the left mouse button is down (so useful for punching blocks)
//			
//			hitHandler.handleLeftButtonHit (hit);
//			
//		}
//

//		bug ("world point: " + hit.point + " tri index: " + hit.triangleIndex);
			
////// Draw lines in the three d views - don't need right now
//		MeshCollider meshCollider = hit.collider as MeshCollider;
//		if (meshCollider == null || meshCollider.sharedMesh == null)
//			return;
//
//		Mesh mesh = meshCollider.sharedMesh;
//		Vector3[] vertices = mesh.vertices;
//		int[] triangles = mesh.triangles;
//		Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
//		Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
//		Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
//		Transform hitTransform = hit.collider.transform;
//		p0 = hitTransform.TransformPoint(p0);
//		p1 = hitTransform.TransformPoint(p1);
//		p2 = hitTransform.TransformPoint(p2);
//		Debug.DrawLine(p0, p1);
//		Debug.DrawLine(p1, p2);
//		Debug.DrawLine(p2, p0);



		// TODO: draw some lines around the block (later draw a graphic or an animation maybe)...
//	http://docs.unity3d.com/Documentation/ScriptReference/GL.LINES.html

//		if (Input.GetMouseButtonDown (0)) {
			//			Debug.Log ("Pressed left click.");
			//
			//			Coord dPoint = testDestroyCoord;
			//
			//			for (int i = 6; i < 14; ++ i) {
			//				for (int j = 14; j > 4; -- j) {
			//					dPoint.x = i;
			//					dPoint.y = j;
			//					destroyBlockAt (dPoint);
			//					destroyBlockAt (dPoint + new Coord (0, 0, -1));
			//				}
			//
			//			}
			//
			//			destroyBlockAt (testDestroyCoord);
			//			testDestroyCoord = testDestroyCoord + new Coord (0, 0, -1);


			//		}

			//		if (Input.GetMouseButtonDown(1))
			//			Debug.Log("Pressed right click.");
			//
			//		if (Input.GetMouseButtonDown(2))
			//			Debug.Log("Pressed middle click.");
	}

	void bug (string str){
		Debug.Log (str);
	}
}
