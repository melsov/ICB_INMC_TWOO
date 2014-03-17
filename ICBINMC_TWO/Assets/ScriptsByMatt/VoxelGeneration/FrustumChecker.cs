using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class FrustumChecker 
{
	private Transform cameraTransform;
	private int CHUNKLENGTH;

	private int lastScreenPointIndex = 0;
	private int quadrantFlipper = 0;
	

	private const float SCREENDIVISIONS = 12.0f;

	private Vector3 lastGeneralCheckingPos;

	private Ray debugRay;

//	private List<Chunk> visibleChunks;

	public FrustumChecker(Transform camTransform, int chLEN)
	{
		this.cameraTransform = camTransform;
		this.CHUNKLENGTH = chLEN;

		lastGeneralCheckingPos = Vector3.zero;
	}

//	public IEnumerator addToVisibleChunksList(List<Chunk> activeChunks) 
//	{
//		// a continual and potentially dense process to do once every or every few frames
//	}

	public void resetIndex() {
		lastScreenPointIndex = 0;
	}

	public Ray nextRaycastMiss()
	{
		// find a raycast that misses

//		Vector3 playerMovedBy = cameraTransform.position - lastGeneralCheckingPos;
//		if (playerMovedBy.magnitude > 6.0f)
//		{
			resetIndex (); //crude?
			lastGeneralCheckingPos = cameraTransform.position;
//		}

		RaycastHit hit;
		Ray ray; // = null;
		int attempts = 0;
		bool hitAMesh = false;
		do {
			ray = Camera.main.ScreenPointToRay (nextScreenPoint ());
			hitAMesh = Physics.Raycast(ray, out hit);
			++attempts;
		} while(hitAMesh && attempts < 10);

		debugRay = ray;

		if (!hitAMesh)
			return new Ray (Vector3.zero, Vector3.up);

		return ray;
	}

	public void drawLastRay() {
//		if (debugRay != null)
//			Debug.DrawLine(debugRay.origin, debugRay.GetPoint(CHUNKLENGTH * 10.0f), Color.cyan);
	}
//
//	float playerToChunkBorderDistancePlusSmallFudgeAtDirection(Vector3 direction) 
//	{
//		Coord chunkPosCoord = ((new Coord (cameraTransform.position)) / CHUNKLENGTH) * CHUNKLENGTH;
//
//		//assuming dir is already in world terms (not cam relative)
//		//coord members == 0 if dir is neg in the axis 'in question', CHUNKLEN if dir is pos 
//		// e.g. if dir = (x = -.3, y = .2, z = .2) the Coord will = (0, CHLEN, CHLEN)
//		// use to get the 'chunk faces' towards which dir is pointing
//		Coord unitChunkCoInPosDirections = new Coord (CHUNKLENGTH) * booleanCoordFromDirection(direction);
//
//		Coord chunkBorderPositionCoord = chunkPosCoord + unitChunkCoInPosDirections;
//
//		Vector3 difFromBorder = chunkBorderPositionCoord.Vector3 () - cameraTransform.position;
//		difFromBorder = Vector3.Scale (difFromBorder, direction);
//		return difFromBorder.magnitude * 1.001; // THE FUDGE
//	}
//
//	Coord booleanCoordFromDirection(Vector3 dir) {
//		return new Coord (dir + Vector3.one);
//	}

	public int screenIterationCount() {
		return 1 + lastScreenPointIndex / (int)(SCREENDIVISIONS * SCREENDIVISIONS);
	}

	void bug (string str) {
		UnityEngine.Debug.Log (str);
	}

	Vector3 nextScreenPoint() {
		int lastScreenIndexRemainder = lastScreenPointIndex % (int)(SCREENDIVISIONS * SCREENDIVISIONS);
		int widthNudges = (lastScreenIndexRemainder % (int)SCREENDIVISIONS);
		int heightNudges = (int)( lastScreenIndexRemainder / SCREENDIVISIONS);

		Vector2 nudge = new Vector2 (Screen.width / SCREENDIVISIONS * (float) widthNudges * .5f, Screen.height / SCREENDIVISIONS * (float) heightNudges * .5f);
//		nudge = nudge * .5;

		if (quadrantFlipper == 0 || quadrantFlipper == 2)
		{
			nudge.x *= -1.0f;
		}
		if (quadrantFlipper == 1 || quadrantFlipper == 2)
		{
			nudge.y *= -1.0f;
		}

		nudge.x += Screen.width * .5f;
		nudge.y += Screen.height * .5f;

		if (quadrantFlipper == 0) {
			lastScreenPointIndex = lastScreenPointIndex + 1; // % (int)(SCREENDIVISIONS * SCREENDIVISIONS);
		}

		quadrantFlipper = (quadrantFlipper + 1) % 4;

		//test
//		return Input.mousePosition;
		return new Vector3 (nudge.x, nudge.y, 0);
	}
}
