using UnityEngine;
using System.Collections;

public class BlockHitter : MonoBehaviour {

	public ChunkManager blockDelegate;



	private float punchStartTime; 
	private Coord hitBlockCoord;
	private Coord alreadyHandledBlockCoord;



	private float blockBreakTimeSeconds;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void handleLeftButtonHit(RaycastHit hit)
	{
		Coord worldHitCoord = new Coord (hit.point);

		if (!hitBlockCoord.equalTo(worldHitCoord) )
		{
			hitBlockCoord = worldHitCoord;
			punchStartTime = Time.fixedTime;
			blockBreakTimeSeconds = getBlockBreakTimeSeconds(hitBlockCoord);
			return;
		}

		if (Time.fixedTime - punchStartTime > blockBreakTimeSeconds)
		{
			if (hitBlockCoord.equalTo (alreadyHandledBlockCoord)) {
//				bug (" already handled: " + alreadyHandledBlockCoord.toString () + " hitBlock: " + hitBlockCoord.toString ());
				return;
			}

//			bug ("sending hit to ch manager block is: " + hitBlockCoord.toString() );
//			blockDelegate.handleBreakBlockAt(hitBlockCoord);
			blockDelegate.handleBreakBlockAt(hit );

			alreadyHandledBlockCoord = hitBlockCoord;
		}
	}

	public void handleRightButtonHit(RaycastHit hit)
	{
		blockDelegate.handlePlaceBlockAt (hit);
	}



	private float getBlockBreakTimeSeconds(Coord b_coord)
	{
		return 0.3f; // for now
	}

	void bug(string str) {
		Debug.Log(str);
	}
}
