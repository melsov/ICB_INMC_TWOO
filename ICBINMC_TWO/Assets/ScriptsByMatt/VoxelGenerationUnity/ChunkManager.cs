#define TEST_LIBNOISENET
#define FAST_BLOCK_REPLACE
#define NEIGH_CHUNKS
#define NO_ASYNC_CHUNK
//#define LIMITED_WORLD



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


/*
 *MAIN CLASS FOR ICBINMC
 *To anyone looking at this code
 *there's a lot of messiness right now 
 *some variables are essential and others
 *should be purged, for example.
 *
 *CHUNK MANAGER checks for chunks that should be loaded
 *loads new chunks
 *destroys far away chunks 
 *runs a dictionary of noisePatches that
 *get more noise for new chunks...
 *
 */

/*
*BUGS:
*sometimes, (esp. when moving quickly) entire chunks (or other large sections) will be missing from the world. This seemed to happen
* more frequently when we startred making chunks on a separate thread.

*sometimes, chunks won't be able to know the blocks adjacent to them,right on their borders in neighbor chunks—probably because the noise for this chunk has
* yet to be generated. Result is a wall of missing chunks—which at this writing are just patched with a face: i.e. if the block at the edge is null
	*assume that it is air and add a face.
*/
using System.ComponentModel;
using System.Threading;
using System.Collections.Specialized;
using System.Runtime.InteropServices;


public class ChunkManager : MonoBehaviour 
{

//	public Chunk prefabChunk;
	public Transform prefabMeshHolder;
	
	public DebugLinesMonoBAssistant debugLines;
	public static DebugLinesMonoBAssistant debugLinesAssistant;


	private ChunkMap chunkMap;

	public NoiseHandler noiseHandler;

	//TODO: make the rest const
	public const uint CHUNKLENGTH = 16;
	public const int CHUNKHEIGHT = 128;
	public const int WORLD_HEIGHT_CHUNKS = 1;
	private const int WORLD_HEIGHT_BLOCKS = (int) (WORLD_HEIGHT_CHUNKS * CHUNKHEIGHT);

	public Transform playerCameraTransform; 
	private AudioSource audioSourceCamera; 

	private List<Chunk> activeChunks;
	private List<Chunk> createTheseChunks;
	private List<Coord> createTheseVeryCloseAndInFrontChunks;
	private List<Chunk> checkTheseAsyncChunksDoneCalculating;
	private List<Chunk> createTheseFurtherAwayChunks;
	private List<Chunk> destroyTheseChunks;
	
	private List<Coord> bugCoordsThatWaitedForAdjacentNPatches = new List<Coord>();
	
	private List<Coord> rebuildChunkChunkCoordList = new List<Coord>();
//	private List<Coord> hasEverBeenDestroyedDebugList = new List<Coord>();
	private Quad m_destroyNoisePatchesOutsideOfRealm = new Quad(new PTwo(-9999,-9999), new PTwo(9999999, 9999999));

	private CoRange m_wantActiveRealm;
	private CoRange m_veryCloseAndInFrontRealm;
	private CoRange m_dontDestroyRealm;

	private	List<NoisePatch> setupThesePatches;
	private List<NoisePatch> checkDoneForThesePatches = new List<NoisePatch>();
	private List<NoiseCoord> bugNoiseCoordsThatDidntExist  = new List<NoiseCoord>();

	private Coord lastPlayerBlockCoord;
	private Coord lastPlayerChunkCoord;
	private NoiseCoord lastPlayerNoiseCoord;


	private Coord m_testBreakMakeBlockWorldPos;

	public Coord spawnPlayerAtCoord = new Coord (6, 0, 6);

//	private FrustumChecker frustumChecker;
	private bool gotAFarAwayChunkTest;
	private Ray farawayRayTest;
	private Vector3 farAwayPos;
	private bool wasNullAndNeededToRenderTest;
	private bool wasInactiveAndNotNullTest;

	public BlockCollection blocks;

	private bool firstNoisePatchDone = false;

	public Transform terrainTestPlane;
//	#if TERRAIN_TEST
	private Texture2D terrainTex;
//	#endif
	
//	TODO: make a break block/ make block test cube. it teleports to any location where breaking blocks is requested
	
	public LibNoiseNetHandler m_libnoiseNetHandler;
	
	private NoiseCoord currentTargetedForCreationNoiseCo;
	
	public Transform cursorBlock;

	public ChunkManager()
	{
		noiseHandler = new NoiseHandler ();

//		WORLD_HEIGHT_BLOCKS = (int) (WORLD_HEIGHT_CHUNKS * CHUNKHEIGHT);

		blocks = new BlockCollection (); // (NoisePatch.PATCHDIMENSIONSCHUNKS * CHUNKLENGTH));

		activeChunks = new List<Chunk> ();
		createTheseVeryCloseAndInFrontChunks = new List<Coord> ();
		checkTheseAsyncChunksDoneCalculating = new List<Chunk> ();
		createTheseChunks = new List<Chunk> ();
		destroyTheseChunks = new List<Chunk> ();

		setupThesePatches = new List<NoisePatch> ();

	}
	
	#region ask a chunkmanager
	
	public Block blockAtChunkCoordOffset(Coord cc, Coord offset)
	{
		Coord index = new Coord ((int)(cc.x * CHUNKLENGTH + offset.x),
									(int) (cc.y * CHUNKHEIGHT + offset.y),
									(int) (cc.z * CHUNKLENGTH + offset.z));

		if (index.y < 0 || index.y >= WORLD_HEIGHT_BLOCKS) {  
			b.bug ("index. y out of range: (negative or greater than world height) " + index.toString ());
			return null;
		}
		
		if (!blocks.noisePatchExistsAtWorldCoord(index) )
		{
			b.bug ("(Block at chCo Offset)trying to get a block from ch coord: " + cc.toString() + "\n  offset: " + offset.toString() +" for which we don't have a noise patch coord at woco: " + index.toString() );
			return null;
		}
		

		return blocks [index]; 
	}
	
	public BlockType blockTypeAtChunkCoordOffset(Coord cc, Coord offset)
	{
		Coord index = new Coord ((int)(cc.x * CHUNKLENGTH + offset.x),
									(int) (cc.y * CHUNKHEIGHT + offset.y),
									(int) (cc.z * CHUNKLENGTH + offset.z));

		if (index.y < 0 || index.y >= WORLD_HEIGHT_BLOCKS) {  
			bug ("index. y out of range: (negative or greater than world height) " + index.toString ());
			return BlockType.TheNullType;
		}
		
//		if (!blocks.noisePatches.ContainsKey (new NoiseCoord (cc / NoisePatch.CHUNKDIMENSION) ))
		if (!blocks.noisePatchExistsAtWorldCoord(index) )
		{
			bug ("(Block at: )trying to get a block from ch coord: " + cc.toString() + " offset: " + offset.toString() +" for which we don't have a noise patch coord at woco: " + index.toString() );
			return BlockType.TheNullType;
		}
		
		NoisePatch np = blocks.noisePatchAtWorldCoord(index);
		return np.blockTypeAtChunkCoordOffset(cc,offset);
	}
	
	public List<Range1D> rangesAtWorldCoord(Coord woco)
	{
		if (woco.y < 0 || woco.y >= WORLD_HEIGHT_BLOCKS) {  
			bug ("index. y out of range: (negative or greater than world height) " + woco.toString ());
			return null;
		}	
		
		if (!blocks.noisePatchExistsAtWorldCoord(woco) )
		{
//			bug ("(Rs at woco): trying to get a block from woco coord: " + woco.toString() + "\n for which we don't have a noise patch coord at woco: ");
			return null;
		}
		
		NoisePatch np = blocks.noisePatchAtWorldCoord(woco);
		return np.rangesAtWorldCoord(woco);
	}
	
	public List<Range1D> heightsListAtChunkCoordOffset(Coord cc, Coord offset)
	{
		Coord index = new Coord ((int)(cc.x * CHUNKLENGTH + offset.x),
									(int) (cc.y * CHUNKHEIGHT + offset.y),
									(int) (cc.z * CHUNKLENGTH + offset.z));

		if (index.y < 0 || index.y > WORLD_HEIGHT_BLOCKS - 1) {  
			bug ("index. y out of range: (negative or greater than world height) " + index.toString ());
			throw new Exception("index out of range.");
			return null;
		} 
		
//		if (!blocks.noisePatches.ContainsKey (new NoiseCoord (cc / NoisePatch.CHUNKDIMENSION) ))
		if (!blocks.noisePatchExistsAtWorldCoord(index) )
		{
//			string prob =String.Format( "(get HeighList) trying to get a block from ch coord: {%} \n offset: %s for which we don't have a noise patch coord at woco: %s" , cc.toString(), offset.toString(), index.toString()) ;
//			b.bug(prob);
			bug ("(get HeighList----) trying to get a block from ch coord: " + cc.toString() + "\n offset: " + offset.toString() +" " +
				"for which we don't have a noise patch coord at woco: " + index.toString() );
			
			//DBUG
			NoiseCoord ncoNoE = blocks.noiseCoordContainingWorldCoord(index);
			if (!bugNoiseCoordsThatDidntExist.Contains(ncoNoE))
				bugNoiseCoordsThatDidntExist.Add(ncoNoE);
			
//			throw new Exception("yack");
			return null;
		}

		
		return blocks.heightsListAtWorldCoord(index); //   np.heightsListAtChunkCoordOffset(cc, offset);
	}
	
	public CoordSurfaceStatus coordIsAboveSurface(NoiseCoord nco, Coord offset) // Coord noiseCoordOffset, NoiseCoord nCo)
	{
		Coord woconoco = CoordUtil.WorldCoordFromNoiseCoord(nco);
		Coord index = woconoco + offset;
		return coordIsAboveSurface(index);
	}
	
	public CoordSurfaceStatus coordIsAboveSurface(Coord cc, Coord offset) // Coord noiseCoordOffset, NoiseCoord nCo)
	{	
		Coord index = new Coord ((int)(cc.x * CHUNKLENGTH + offset.x),
									(int) (cc.y * CHUNKHEIGHT + offset.y),
									(int) (cc.z * CHUNKLENGTH + offset.z));
		
		return coordIsAboveSurface(index);
	}
		
	public CoordSurfaceStatus coordIsAboveSurface(Coord index) // Coord noiseCoordOffset, NoiseCoord nCo)
	{
		if (index.y > WORLD_HEIGHT_BLOCKS - 1) {  
			return CoordSurfaceStatus.ABOVE_SURFACE;
		} 
		if (index.y < 0) {  
			throw new Exception("why below??" + index.toString());
			return CoordSurfaceStatus.BELOW_SURFACE_SOLID;
		}

		if (!blocks.noisePatchExistsAtWorldCoord(index) )
		{
			throw new Exception("noisePatch doesn't exist...");
			return CoordSurfaceStatus.ABOVE_SURFACE;
		}

		return blocks.worldCoordIsAboveSurface(index); //   np.heightsListAtChunkCoordOffset(cc, offset);
	}
	
	public float ligtValueAtPatchRelativeCoordNoiseCoord(Coord patchRelCo, NoiseCoord nco, Direction dir)
	{
		Coord woco = CoordUtil.WorldCoordFromNoiseCoord(nco) + patchRelCo;
		if (woco.y > WORLD_HEIGHT_BLOCKS - 1) {  
			return Window.LIGHT_LEVEL_MAX;
		} 
		if (woco.y < 0) {  
//			throw new Exception("why below??" + woco.toString());
			return 4f;
		}

		if (!blocks.noisePatchExistsAtWorldCoord(woco) )
		{
//			throw new Exception("noisePatch doesn't exist...");
			return 6f;
		}
		
		return blocks.ligtValueAtWorldCoord(woco, dir);
		
	}
	
//	
//	public CoordSurfaceStatus coordIsAboveSurface(Coord noiseCoordOffset, NoiseCoord nCo)
//	{
//		Coord index = worldCoordForNoiseCoord(nCo) + noiseCoordOffset;
//
//		if (index.y < 0 || index.y > WORLD_HEIGHT_BLOCKS - 1) {  
//			throw new Exception("index out of range.");
//			return CoordSurfaceStatus.ABOVE_SURFACE;
//		} 
//
//		if (!blocks.noisePatchExistsAtWorldCoord(index) )
//		{
//			throw new Exception("noisePatch doesn't exist...");
//			return CoordSurfaceStatus.ABOVE_SURFACE;
//		}
//
//		return blocks.worldCoordIsAboveSurface(index); //   np.heightsListAtChunkCoordOffset(cc, offset);
//	}
	
	#endregion
	
	#region make chunks
	
	void makeChunksFromOnMainThread(ChunkCoord start, ChunkCoord range)
	{
		bug ("MKING CHS ON MAIN THREAD START");
		// first give chunks their blocks
		int i = start.x;
		for (; i < start.x + range.x; ++i) {

			int j = (int) start.y;
			for (; j < start.y + range.y; ++j) {

				int k = start.z;
				for (; k < start.z + range.z; ++k) {

					makeChunksFromOnMainThreadAtCoord (new Coord (i, j, k)); 
				}
			}
		} 
	}

	void makeChunksFromOnMainThreadAtCoord(Coord coord)
	{
		if (!chunkMap.coIsOnMap (coord)) {
			bug ("got co not on map at coord: " + coord.toString ());
//			throw new System.ArgumentException ("looking for a chunk that was not on the map..." + coord.toString());
			return;
		}

		Chunk ch = chunkMap.chunkAt (coord);
		makeChunkOnMainThread (ch);

	}
	
	void makeChunksFromOnSepThreadAtCoord(Coord coord)
	{
		if (!chunkMap.coIsOnMap (coord)) {
			bug ("got co not on map at coord: " + coord.toString ());
//			throw new System.ArgumentException ("looking for a chunk that was not on the map..." + coord.toString());
			return;
		}

		Chunk ch = chunkMap.chunkAt (coord);
		makeChunkOnSepThread (ch);

	}
	
	void makeChunkOnSepThread(Chunk ch)
	{
		makeChunk(ch, true);
	}
		
	void makeChunkOnMainThread(Chunk ch)
	{
		makeChunk(ch, false);
	}
	
	void makeChunk(Chunk ch, bool wantSepThread)
	{
		if (ch == null )
			throw new System.ArgumentException("Ch should not be null at this point", "ch");
		
		if (checkTheseAsyncChunksDoneCalculating.Contains(ch))
		{
			throw new Exception("trying to make a chunk that is already on check async list??");
			return;
		}
		
		if (ch.meshHoldingGameObject == null ) {
			giveChunkItsMeshObject (ch, ch.chunkCoord);
		}
		
		if (wantSepThread) {
//			ch.Start (); 
			if (!ch.hasStarted) {
				ch.Start();
				checkTheseAsyncChunksDoneCalculating.Add (ch);
			}
			return;
		}
		
		ch.makeMesh (); // ???

		ch.applyMesh ();
		activeChunks.Add (ch);

		ch.isActive = true;
	}


	Chunk makeNewChunkButNotItsMesh(Coord coord)
	{
		Chunk c = new Chunk ();// (Chunk) Instantiate (prefabChunk, pos, transform.rotation);

//		c.m_chunkManager = this;

		c.chunkCoord = coord;

		// give chunk a noise patch
		// maybe would want the noise patch to conform to an interface instead of exposing the whole noise patch to the chunk
		// just a thought...
		NoiseCoord ncoo = noiseCoordContainingChunkCoord (coord);

		if (!blocks.noisePatches.ContainsKey(ncoo))
		{
			return null;
		}
//		if (!blocks.noisePatchAtCoordIsReady(ncoo))
//		{
//			throw new System.ArgumentException ("yo we don't have a noise patch for this chunk?? " + ncoo.toString());
//			bug ("um... we don't have a noise patch for this chunk. " + ncoo.toString ());
//			c.isActive = false;
//			return null;
//		}
		

		NoisePatch np = blocks.noisePatches [ncoo];
		c.m_noisePatch = np;

		return c;
	}

	void giveChunkItsMeshObject(Chunk c, Coord coord)
	{
		Vector3 pos = new Vector3 (CHUNKLENGTH * coord.x, CHUNKHEIGHT * coord.y, CHUNKLENGTH * coord.z) * Chunk.VERTEXSCALE;

		//change: the mesh is NOT of type Chunk (just a game object)
		Transform gObjTrans = (Transform) Instantiate(prefabMeshHolder, pos, transform.rotation);
		GameObject gObj = gObjTrans.gameObject;

		//c.transform.parent = transform;
		gObj.transform.parent = transform;

		c.meshHoldingGameObject = gObj; //the chunk gets a reference to the gameObject that it will work with (convenient)
	}
	
	#endregion


	public System.Collections.IEnumerable chunksTouchingBlockCoord(Coord blockWorldCoord)
	{
		Coord chunkCoord = CoordUtil.ChunkCoordContainingBlockCoord (blockWorldCoord);
		Coord chRelCo = CoordUtil.ChunkRelativeCoordForWorldCoord (blockWorldCoord);

		foreach (Coord dirCo in directionCoordsForRelativeEdgeCoords(chRelCo, blockWorldCoord)) {
			Chunk adjacentChunk = chunkMap.chunkAt (chunkCoord + dirCo);

			if (adjacentChunk != null)
				yield return adjacentChunk;

		}
	}

	public System.Collections.IEnumerable directionCoordsForRelativeEdgeCoords(Coord blkRelativeCoord, Coord worldCoordOfBlockInQuestion)
	{
		bug ("block rel coord: " + blkRelativeCoord.toString () + " world co of changed block: " + worldCoordOfBlockInQuestion.toString ());
		Coord negPosOne = Coord.coordOne (); // worldCoordOfBlockInQuestion.negNegOnePosPosOne ();
		if (blkRelativeCoord.x == 0)
			yield return new Coord (-1, 0, 0) * negPosOne;
		if (blkRelativeCoord.x == CHUNKLENGTH - 1)
			yield return new Coord (1, 0, 0) * negPosOne;

		//DON'T BOTHER WHILE THE WORLD IS ONLY ONE CHUNK HIGH. (CHUNKS ARE TALL)
//		if (blkRelativeCoord.y == 0)
//			yield return new Coord (0, -1, 0) * negPosOne;
//		if (blkRelativeCoord.y == CHUNKLENGTH - 1)
//			yield return new Coord (0, 1, 0) * negPosOne;

		if (blkRelativeCoord.z == 0)
			yield return new Coord (0, 0, -1) * negPosOne;
		if (blkRelativeCoord.z  == CHUNKLENGTH - 1)
			yield return new Coord (0, 0, 1) * negPosOne;

	}

	void bug(string str) {
		UnityEngine.Debug.Log (str);
	}
	
	#region degub chunk managing
	
	public void assertNoChunksActiveInNoiseCoord(NoiseCoord nco)
	{
		List<Coord> chunkCoordsInNoisePatch = CoordUtil.WorldRelativeChunkCoordsWithinNoiseCoord(nco);
		
		foreach(Coord chCo in chunkCoordsInNoisePatch)
		{
			Chunk chunk = chunkMap.chunkAt(chCo);
			AssertUtil.Assert( chunk == null || !chunk.isActive , "this chunk shouldn't be ready yet??" + chCo.toString());	
		}
	}
	
	#endregion
	
	public void updateAllActiveChunksInNoiseCoord(NoiseCoord nco)
	{
		List<Coord> chunkCoordsInNoisePatch = CoordUtil.WorldRelativeChunkCoordsWithinNoiseCoord(nco);
		
		foreach(Coord chCo in chunkCoordsInNoisePatch)
		{
			Chunk chunk = chunkMap.chunkAt(chCo);
			
			if (chunk != null && !createTheseVeryCloseAndInFrontChunks.Contains(chCo))
			{
				createTheseVeryCloseAndInFrontChunks.Insert(0,chCo);
			}
//			AssertUtil.Assert( chunk == null || !chunk.isActive , "this chunk shouldn't be ready yet??" + chCo.toString());	
		}
	}
	
	#region rebuild chunks for NoisePatch structures
	
	public void rebuildChunksAtNoiseCoordPatchRelativeChunkCoords(NoiseCoord nco, List<Coord> patchRelChunkCoords)
	{
		return ; //TEST //TEST ******
		
		// TODO: add to a list of chunks to rebuild
		
		Coord patchWoco = worldCoordForNoiseCoord(nco);
		Coord patchWorldChunkCo = CoordUtil.ChunkCoordContainingBlockCoord(patchWoco);
		foreach(Coord pRelChCo in patchRelChunkCoords)
		{
			Coord chunkCoord = pRelChCo + patchWorldChunkCo;
			
			Chunk chunk = chunkMap.chunkAt(chunkCoord);
			if (chunk != null)
			{
				if (chunk.hasStarted) {
					b.bug("chunk start already");
					chunk.Abort(); // really???(we don't know what this will do...admittedly.
				}
				
				//TRY VIOLENCE
//				chunkMap.destroyChunkAt(chunkCoord);
				
				
				chunk.resetCalculatedAlready();
				
				//END T Violence
				
			
				if (!createTheseVeryCloseAndInFrontChunks.Contains(chunkCoord))
					createTheseVeryCloseAndInFrontChunks.Add(chunkCoord);
			}
			
//			rebuildChunkChunkCoordList.Add(chunkCoord); //WANT?

		}
	}
	
	#endregion

	#region finding coords and chunks

	Coord playerPosCoord() {
		return new Coord (playerCameraTransform.position);
	}

	private bool playerOccupiesCoord(Coord cc) {
		//BUG: doesn't work if block surface is right in player's face!!
		//works too well for blocks in front
		Vector3 cocenter = Coord.GeomCenterOfBLock (cc);
		if (Vector3.Distance (playerCameraTransform.position, cocenter) < 1f)
			return true;

		if (Vector3.Distance (playerCameraTransform.position - Vector3.up, cocenter) < 1f)
			return true; 

		Coord ppos = playerPosCoord ();
		if (Coord.Equals (ppos, cc))
			return true;

		return false;

//
//		Coord ppos = playerPosCoord ();
//		if (Coord.Equals (ppos, cc))
//			return true;
//		ppos.y -= 1;
//		if (Coord.Equals (ppos, cc))
//			return true;
//		return false;
	}

	#endregion
	
	public void OnGUI() 
	{
		
//		int y = 0;
//		foreach ( string i in System.Enum.GetNames(typeof(NoiseType)) ) {
//			if (GUI.Button(new Rect(0,y,100,20), i) ) {
//				noiseHandler.noise = (NoiseType) Enum.Parse(typeof(NoiseType), i);
//
//				bug ("new noise: " + i);
////				buildMap ();
//				noiseHandler.Generate ();
////				rebuildMapFrom (new ChunkCoord (0, 2, 0), new ChunkCoord (2, 2, 1));
//
//				ChunkCoord start = ChunkCoord.chunkCoordZero (); // new ChunkCoord (2, 2, 2);
//				ChunkCoord range = new ChunkCoord (8, 4, 4);
//				populateBlocksArray (start, range, false );
//				makeChunksFromOnMainThread (start, range); 
//			}
//			y+=20;
//		}
		
//		#if TERRAIN_TEST
		if (TestRunner.DontRunDoTerrainTestInstead)
		{
			float imScale = 1f;
			GUI.Box(new Rect ( 4, Screen.height - terrainTex.height * imScale , terrainTex.width * imScale, terrainTex.height * imScale), terrainTex );
		}
//		#endif
		
		// DRAW A '+' IN THE MIDDLE
		float boxSize = 20;
		GUI.Box (new Rect (Screen.width * .5f - boxSize/2.0f, Screen.height * .5f - boxSize/2.0f, boxSize, boxSize), "+");
		
		//legend for debug lines
//		GUI.Box (new Rect ( 40, Screen.height - 40, Screen.width - 40, 60), "Chunk lists: Destroy: red, CheckASync: blue, VCloseAInfront: cyan" );
		
		return; // !!!!!
		
		GUI.Box (new Rect (Screen.width - 170, Screen.height - 320, 150, 40), "chckASync Cnt:" + checkTheseAsyncChunksDoneCalculating.Count );
		GUI.Box (new Rect (Screen.width - 170, Screen.height - 240, 150, 40), "desThsChs Cnt:" + destroyTheseChunks.Count );
		GUI.Box (new Rect (Screen.width - 170, Screen.height - 280, 150, 40), "verCls Cnt:" + createTheseVeryCloseAndInFrontChunks.Count );
		
		Coord playerCoord = new Coord (playerCameraTransform.position);
		Coord chChoord = CoordUtil.ChunkCoordContainingBlockCoord (playerCoord);
		NoiseCoord npatchCo = noiseCoordContainingChunkCoord (chChoord);

		GUI.Box (new Rect (Screen.width - 170, Screen.height - 40, 150, 40), "plyr co: \n" + playerCoord.toString() );
		GUI.Box (new Rect (Screen.width - 170, Screen.height - 80, 150, 40), "plyr chunk co: \n" + chChoord.toString() );
		GUI.Box (new Rect (Screen.width - 170, Screen.height - 120, 150, 40), "" + npatchCo.toString() );
		GUI.Box (new Rect (Screen.width - 170, Screen.height - 160, 150, 40), "actv chs:" + activeChunks.Count + "chldn:" + transform.childCount );
		GUI.Box (new Rect (Screen.width - 170, Screen.height - 200, 150, 40), "creThsChs Cnt:" + createTheseChunks.Count );

		if (GUI.Button (new Rect (Screen.width - 170, Screen.height - 320, 150, 40), "SAVE" ))
		{
			blocks.saveNoisePatchesToPlayerPrefs ();
		}

//		GUI.Box (new Rect (Screen.width - 170, Screen.height - 320, 150, 40), "FPS:" + Time.deltaTime );

//		Coord corner = new Coord (Screen.width - 170, 1, Screen.height - 200);
//		Coord box = new Coord (10, 1, 10);
//		float margin = 2;
//		foreach(Chunk chch in activeChunks)
//		{
//			Coord boxPos = corner - (chch.chunkCoord * box);
//			GUI.Box (new Rect(boxPos.z, boxPos.x, box.x, box.z ), ".");
//		}

	}

	#region handle block hits
	
	private void moveCursorBlockToPosition(Vector3 wopo)
	{
		cursorBlock.renderer.enabled = true;
		cursorBlock.position = wopo;			
	}
	
	private void hideCursorBlock() {
		cursorBlock.renderer.enabled = false;
	}
	
	public void handleBreakBlockInProgress(RaycastHit hit)
	{
		Vector3 hitWoPo = worldPositionOfBlock(hit);
		moveCursorBlockToPosition(hitOrPlaceBlockCoordFromWorldPos(hitWoPo).toVector3());
	}
	
	public void handleBreakBlockAborted() {
		hideCursorBlock();	
	}

	public void handleBreakBlockAt(RaycastHit hit)
	{
		hideCursorBlock();

		Vector3 blockWorldPos = worldPositionOfBlock (hit);

		Coord altBlockCoord =  hitOrPlaceBlockCoordFromWorldPos (blockWorldPos);// new Coord (blockWorldPos);
		moveCursorBlockToPosition(altBlockCoord.toVector3());

		if (altBlockCoord.y == 0) //BEDROCK
		{
			bug ("hit bedrock");
			return;
		}

//		if (blockCoord.equalTo (altBlockCoord)) {
//			bug ("block coords from two methods are equal");
////			destroyBlockAt (blockCoord );
//		} else {
//			bug ("block coords not equal: blockCoord: " + blockCoord.toString () + " alt b coord: " + altBlockCoord.toString ());
////			destroyBlockAt (altBlockCoord);
//		}

		// TODO: somewhere in here, (pos. using the tris, draw an animation or just some lines, around the block)

		m_testBreakMakeBlockWorldPos = altBlockCoord;
		destroyBlockAt (altBlockCoord);
		//end of triangle approach

//		destroyBlockAt (blockCoord );
	}
	
	
	public void destroyBlockAt(Coord blockCoord) 
	{
		Block destroyMe = blocks [blockCoord]; 

		if (destroyMe.type == BlockType.Air) {
			bug ("block was air already! " + blockCoord.toString());
			return;
		}

		destroyMe.type = BlockType.Air;

		Chunk ch = chunkMap.chunkContainingCoord (blockCoord);

		if (ch == null) {
			bug ("null chunk, nothing to update");
			return;
		}

		blocks [blockCoord] = destroyMe; // get NoisePatch to update lists.
		
		
#if FAST_BLOCK_REPLACE
		Coord chunkRelCo = CoordUtil.ChunkRelativeCoordForWorldCoord(blockCoord);
		ch.editBlockAtCoord(chunkRelCo, BlockType.Air);
#else
		updateChunk (ch);
#endif

		// also update any chunks touching the destroyed block
		foreach (Chunk adjCh in chunksTouchingBlockCoord(blockCoord) )
			updateChunk(adjCh);
	}

	public void handlePlaceBlockAt(RaycastHit hit)
	{
		Vector3 blockWorldPos = worldPositionOfBlock (hit, true);
		
		Coord placingCoord = hitOrPlaceBlockCoordFromWorldPos (blockWorldPos); //  new Coord (blockWorldPos);

//		moveCursorBlockToPosition(placingCoord.toVector3());
		
		if (playerOccupiesCoord(placingCoord)){
			bug ("player occupies coord");
			return;
		}

		m_testBreakMakeBlockWorldPos = placingCoord;

		Block b = blocks [placingCoord]; 

		if (b == null) {
			bug ("null block");
			return;
		}

		if (b.type != BlockType.Air) {
			bug ("a solid block is already located here: " + placingCoord);
			return;
		}

		b.type = currentHeldBlockType ();
		
//		bug ("placing block at coord: " + placingCoord.toString());
		
		Chunk ch = chunkMap.chunkContainingCoord (placingCoord);
//		ch.noNeedToRenderFlag = false;

		blocks [placingCoord] = b; // get saved blocks to update.
		
#if FAST_BLOCK_REPLACE
		Coord chunkRelCo = CoordUtil.ChunkRelativeCoordForWorldCoord(placingCoord);
		ch.editBlockAtCoord(chunkRelCo, b.type);
#else
		updateChunk (ch);
#endif

	}

	BlockType currentHeldBlockType () {
		return BlockType.Grass;
	}

	//deal with negative V3 coordinates
	// still brings the funk...
	Coord hitOrPlaceBlockCoordFromWorldPos( Vector3 wopos)
	{
		Vector3 adjustNegMembers = new Vector3 (0f, 0f, 0f);
		adjustNegMembers.x = wopos.x < 0 ? -1f : 0f;
		adjustNegMembers.y = wopos.y < 0 ? -1f : 0f;
		adjustNegMembers.z = wopos.z < 0 ? -1f : 0f;

		return new Coord (wopos + adjustNegMembers);
	}

	Vector3 worldPositionOfBlock(RaycastHit hit)
	{
		return worldPositionOfBlock (hit, false);
	}

	Vector3 worldPositionOfBlock(RaycastHit hit, bool placingBlock)
	{
		// we should get a Vec3 
		// that describes the relative position of the triangle to the camera: "relPos"
		// then we can figure out which face we want...by
		// knowing that if they are facing in a (say) a negative direction,
		// we should 'cheat' the vec3 value slightly negative
		// if we ever start having wedge shaped blocks,
		// this won't be an issue, because the vec3 value will be more solidly in the middle?

		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null)
			return new Vector3(9999999999999.0f, 0,0);

//		bug ("what was the triangle index: " + hit.triangleIndex);

		Mesh mesh = meshCollider.sharedMesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
		Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
		Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];

		Vector3 worldAvg = hit.point; // TEST**** hitTransform.TransformPoint(avg);
		Vector3 relPos =  worldAvg - playerCameraTransform.position; // Vector3.Distance (worldAvg, playerCameraTransform.position); //

		// for now assume the world is always all cubes
		float x_same = (p0.x == p1.x && p1.x == p2.x) ? 1 : 0;
		float y_same = (p0.y == p1.y && p1.y == p2.y) ? 1 : 0;
		float z_same = (p0.z == p1.z && p1.z == p2.z) ? 1 : 0;

		Vector3 cheaterV = new Vector3 (x_same, y_same, z_same);

		relPos = Vector3.Scale (relPos, cheaterV);
		relPos = relPos.normalized * 0.1f;

		// put the point 'just inside' (by .1) of the block we want...
		// or if placing a block, put just outside
		return worldAvg + (relPos * (placingBlock ? -1.0f : 1.0f) ); 
	}

	#endregion

	Block blockAt(Coord cc) {
		return blocks [cc.x, cc.y, cc.z];
	}

	void buildMap()
	{
		CoRange some_range = new CoRange (Coord.coordZero (), new Coord (3, 4, 4));
		buildMapAtRange (some_range);
	}

	void buildMapAtRange(CoRange mapChunkRange)
	{
		ChunkCoord start = new ChunkCoord (mapChunkRange.start);
		ChunkCoord range = new ChunkCoord (mapChunkRange.range);

		//must update map before 'manually' adding chunks
		updateCreateTheseChunksList ( mapChunkRange);
		makeChunksFromOnMainThread (start, range);
	}


	void updateChunk(Chunk ch)
	{
		ch.clearMeshLists ();
		ch.makeMesh ();
		ch.applyMesh ();
	}


	#region get player realms and update lists

	Coord playerLocatedAtChunkCoord()
	{
		Coord pCoo =  new Coord (playerCameraTransform.position);
		return CoordUtil.ChunkCoordContainingBlockCoord (pCoo);
	}

	CoRange nearbyChunkRange(Coord playerChunkCoord)
	{
//		Coord halfR = new Coord (2, 2, 1);
//		Coord halfR = new Coord (1, 1, 1);
		Coord halfR = new Coord (1, 1, 1);
		return playerChunkRange (playerChunkCoord, halfR);
	}

	CoRange nearbyChunkRangeInitial(Coord playerChunkCoord)
	{
		//		Coord halfR = new Coord (2, 2, 1);
		Coord halfR = new Coord (1, 1, 1);
		//		Coord halfR = new Coord (3, 1, 3);
		return playerChunkRange (playerChunkCoord, halfR);
	}

	CoRange nearbyChunkRangeInitialForNoisePatch(NoisePatch noisePatch)
	{
		return noisePatch.coRangeChunks ();
	}

	CoRange getDontDestroyRealm() 
	{
		//TEST
//		return getVeryCloseAndInFrontRange();
		//END TEST
		
		Coord halfR = new Coord (5, 4, 5); // larger than wActiveRealm
		return playerChunkRange (playerLocatedAtChunkCoord (), halfR);
	}
	
	Quad getDestroyNoisePatchesOutsideOfRealm(CoRange dontDestroyChunksRealm)
	{
		NoiseCoord startCoord = noiseCoordContainingChunkCoord(dontDestroyChunksRealm.start);
		NoiseCoord rangeCoord = noiseCoordContainingChunkCoord(dontDestroyChunksRealm.range);
		PTwo start = new PTwo(startCoord.x, startCoord.z);
		PTwo range = new PTwo(rangeCoord.x, rangeCoord.z);
		return new Quad(start, range);
	}

	CoRange playerChunkRange(Coord playerChunkCoord, Coord halfRange)
	{
//		int halfLength = 1;
//		Coord halfRealmLength = new Coord (halfLength);  

		return clipRangeHeightToWorldHeightDimsChunks (new CoRange (playerChunkCoord - halfRange, (halfRange * 2) + 1) );
	}

	CoRange clipRangeHeightToWorldHeightDimsChunks(CoRange the_range) {
		the_range.start.y = the_range.start.y < 0 ? 0 : the_range.start.y;
		the_range.range.y = the_range.outerLimit ().y > (int)WORLD_HEIGHT_CHUNKS ? (int)(WORLD_HEIGHT_CHUNKS - the_range.start.y) : the_range.range.y;
		return the_range;
	}

	CoRange getVeryCloseAndInFrontRange()
	{
		Coord plCoord = playerLocatedAtChunkCoord ();

		Coord halfR = new Coord (1,0,1);

		CoRange retRange = playerChunkRange (plCoord, halfR);

		Coord addToStart = new Coord (0);
		Coord addToRange = new Coord (0);

		return retRange;
	}
	
	//YA chunk identifier:
	// get the chunks in a list all of a row or all of a corner (to the mid-line each way), given a direction and a radius.
	private static List<Coord> setOfCoordsInDirection(int PizzaAngle, int distance)
	{
		if (distance < 0) throw new Exception("neg distance doesn't make sense");
		List<Coord> resultList = new List<Coord>();
		Coord offSetCoord = blockyRadar(PizzaAngle) * distance;
		
		resultList.Add(offSetCoord);
		
		Coord nOne = new Coord(0, 0, 1);
		Coord nTwo = new Coord(0, 0, -1);
		
		if (PizzaAngle % 2 == 1) // corner angle
		{
			if (PizzaAngle == 1)
				nOne = new Coord(-1, 0, 0);
			else if (PizzaAngle == 3)
				nOne = new Coord(0, 0, -1);
			else if (PizzaAngle == 5)
				nOne = new Coord(1, 0, 0);
			else 
				nOne = new Coord(0, 0, 1);
			
			nTwo = Coord.FlipXZ(nOne);
			
			if (PizzaAngle == 3 || PizzaAngle == 7)
				nTwo *= -1;
		} else {
			if (PizzaAngle == 2 || PizzaAngle == 6)
			{
				nOne = Coord.FlipXZ(nOne);
				nTwo = Coord.FlipXZ(nTwo);
			}
		}
		
		for (int i = 1; i <= distance ; ++i)
		{
			resultList.Add(offSetCoord + (nOne * i));
			resultList.Add(offSetCoord + (nTwo * i));
		}
		
		return resultList;
	}
	
	private int playerCameraAngleToPizzaAngle() 
	{
		int unityangle = (int)((playerCameraTransform.eulerAngles.y + 22.0f) / 45);
		int angle = unityangle;
		angle = angle == 8 ? 0 : angle;
		
		// Unity angles start at z pos and go clock-wise (pizza starts at x pox and go CCW)
		angle = 8 - angle; 
		angle += 2;
		angle = angle % 8;
		
		return angle;
	}
	
	private static int incrementPizzaAngleCCW(int startAngle, int increment) {
		startAngle += increment;
		startAngle = startAngle % 8;
		startAngle += 8;
		return startAngle % 8;
	}
	
	private void addToCreateTheseChunksWithDistanceFromPlayer(int distance, int pizzaAngle )
	{
		Coord playerChCo = playerLocatedAtChunkCoord();
		
		
		foreach(Coord offset in setOfCoordsInDirection(pizzaAngle, distance))
		{
			Coord coord_ = playerChCo + offset;
//			bug("the coord to add with pizza was: " + coord_.toString() );
			prepareCandidateChunkAtCoord(playerChCo + offset);
		}
	}
	

	// TODO: an overhaul for this
	// decide what we want!
	IEnumerator updateChunkLists()
	{
		while (true) 
		{
			//simplistic
//			if (!Coord.Equals (lastPlayerBlockCoord, new Coord (playerCameraTransform.position))) 
			if (true)
			{
				lastPlayerBlockCoord = new Coord (playerCameraTransform.position);

				// very close and in front range
				m_veryCloseAndInFrontRealm = getVeryCloseAndInFrontRange ();
//				bug (" v close and in f realm: " + m_veryCloseAndInFrontRealm.toString ());

				CoRange wantActiveRealm = m_veryCloseAndInFrontRealm;

				Coord playerChunkCoo = playerLocatedAtChunkCoord ();

				//want?
				if (!playerChunkCoo.equalTo (lastPlayerChunkCoord)) 
				{
					lastPlayerChunkCoord = playerChunkCoo;

					//want?
//					wantActiveRealm = nearbyChunkRange (playerChunkCoo);
//					m_wantActiveRealm = wantActiveRealm; 

					m_dontDestroyRealm = getDontDestroyRealm ();
					m_destroyNoisePatchesOutsideOfRealm = getDestroyNoisePatchesOutsideOfRealm(m_dontDestroyRealm);

					foreach (Chunk chunk in activeChunks) 
					{
						if (chunk == null)
							continue;

						if (!chunk.chunkCoord.isInsideOfRange (m_dontDestroyRealm)) 
						{
							if (!destroyTheseChunks.Contains (chunk)) 
							{
								chunk.isActive = false;
								destroyTheseChunks.Add (chunk);
							}
						}
					}
				}

//				
				// pizza chunks
				int cameraPizzaAngle = playerCameraAngleToPizzaAngle();
				addToCreateTheseChunksWithDistanceFromPlayer(1, cameraPizzaAngle);
				addToCreateTheseChunksWithDistanceFromPlayer(2, cameraPizzaAngle);
				yield return new WaitForSeconds (.88f);
				addToCreateTheseChunksWithDistanceFromPlayer(3, cameraPizzaAngle);
				yield return new WaitForSeconds (.88f);
				addToCreateTheseChunksWithDistanceFromPlayer(4, cameraPizzaAngle);
				
				updateCreateTheseChunksList (wantActiveRealm);
			}
			yield return new WaitForSeconds (.88f);
		}

	}

	public void updateCreateTheseChunksList(CoRange _activeCoRange) 
	{
		Coord start = _activeCoRange.start;
		Coord range = _activeCoRange.range;
		int i = start.x;
		for (; i < start.x + range.x; ++i) 
		{
			int j = (int) start.y;
			for (; j < start.y + range.y; ++j) 
			{
				int k = start.z;
				for (; k < start.z + range.z; ++k) 
				{
					Coord chco = new Coord (i, j, k);
					
					prepareCandidateChunkAtCoord(chco);
				}
			}
		}
	}
	
	private void prepareCandidateChunkAtCoord(Coord chco)
	{
		Chunk chh = chunkMap.chunkAtOrNullIfUnready (chco); 
		
		if(checkTheseAsyncChunksDoneCalculating.Contains(chh))
			return; // be paranoid?
		
		if (chh == null || !chh.isActive) 
		{
#if NEIGH_CHUNKS
			//TEST WANT!!
			NoiseCoord nco = noiseCoordContainingChunkCoord(chco);	
			
			NoisePatch npatch = blocks.noisePatchAtNoiseCoord(nco);
			if (npatch == null || !npatch.patchesAdjacentToChunkCoordAreReady(chco)) {
				if (!bugCoordsThatWaitedForAdjacentNPatches.Contains(chco))
					bugCoordsThatWaitedForAdjacentNPatches.Add(chco);
				return;
			}
#endif
			// chunks don't get destroyed when their meshes do (when, for example, the player moves away from them)
			// therefore we might have an existing chunk that (hopefully) is just "!isActive."
			// or we may have never have encountered (made) this chunk at all...
			// in any case, don't make its accompanying mesh just yet (it might be all air, or all solid and surrounded for example)

			if (chh == null) 
			{
				chh = makeNewChunkButNotItsMesh (chco);

				if (chh == null)
					return;

				chunkMap.addChunkAt (chh, chco);
			} 

			destroyTheseChunks.Remove (chh);

			chh.isActive = true;
			
			if (!createTheseVeryCloseAndInFrontChunks.Contains(chh.chunkCoord))
				createTheseVeryCloseAndInFrontChunks.Add (chh.chunkCoord);

		}
	}

	#endregion

	#region create and destroy chunk enumerators


	private Chunk getAReadyChunkFromList(List<Chunk> chunkList) {
		
		foreach (Chunk ch in chunkList)
		{
			
			NoiseCoord ncoCh = noiseCoordContainingChunkCoord(ch.chunkCoord);
			
			if (blocks.noisePatchAtCoordIsReady(ncoCh)) {
				return ch;
			} 
		}
		return null;
	}
	
	private Chunk getAChunkToDestroyFromList(List<Chunk> destroyChunkList) {
		foreach(Chunk chunk in destroyChunkList)
		{
			if (!chunk.chunkCoord.isInsideOfRange (m_dontDestroyRealm))	
			{
				return chunk;	
			}
		}
		return null;
	}
	
	private Chunk getAReadyChunkCoordFromList(List<Coord> chunkCoordList) {
		if (chunkCoordList.Count == 0) 
			return null;
		
		for (int i = 0; i < chunkCoordList.Count; ++i)
		{
			Coord chco = chunkCoordList[i];

			NoiseCoord ncoCh = noiseCoordContainingChunkCoord(chco);
			
			if (blocks.noisePatchAtCoordIsReady(ncoCh)) {
				Chunk ch = chunkMap.chunkAt(chco);
				if (ch != null)
					return ch;
			}
		}
		return null;
	}

	private Coord getChunkCoordWithinDontDestroyRealmFromList(List<Coord> chunkCoordList) {
		
		for (int i = 0; i < chunkCoordList.Count; ++i)
		{
			Coord chco = chunkCoordList[i];
//			if (chco.isInsideOfRange(m_dontDestroyRealm)) //TEST //much better without!
				return chco;

		}
		return Coord.TheErsatzNullCoord();
	}
	
	private Coord getChunkCoordWithinVeryCloseRealmFromList(List<Coord> chunkCoordList) {
		
		for (int i = 0; i < chunkCoordList.Count; ++i)
		{
			Coord chco = chunkCoordList[i];
			if (chco.isInsideOfRange(m_veryCloseAndInFrontRealm))
				return chco;

		}
		return Coord.TheErsatzNullCoord();
	}
	
	private void cullVeryCloseList(List<Coord> chunkCoordList) {
		
		for (int i = 0; i < chunkCoordList.Count; ++i)
		{
			Coord chco = chunkCoordList[i];
			if (!chco.isInsideOfRange(m_veryCloseAndInFrontRealm))
			{
				chunkCoordList.RemoveAt(i);
				i--;
			}
		}
	}

	IEnumerator createAndDestroyChunksFromLists()
	{
		while (true)
		{
			if (shouldBeCreatingChunksNow ()) 
			{
				Chunk chunk = null; // = createTheseVeryCloseAndInFrontChunks [0];

				if (createTheseVeryCloseAndInFrontChunks.Count > 0)
				{
					
					Coord chco = getChunkCoordWithinDontDestroyRealmFromList(createTheseVeryCloseAndInFrontChunks);
					if (!chco.equalTo(Coord.TheErsatzNullCoord()))
					{
						chunk = chunkMap.chunkAt(chco);
						if (chunk == null)
							throw new Exception("surprising chunk was in v close list but not on chunk map!");
					}
					
					if (chunk != null)
					{
						
#if NO_ASYNC_CHUNK
						makeChunksFromOnMainThreadAtCoord(chunk.chunkCoord);
#else
						makeChunksFromOnSepThreadAtCoord(chunk.chunkCoord);
#endif
						createTheseVeryCloseAndInFrontChunks.Remove(chunk.chunkCoord); // isn't that better??
					} else {
						cullVeryCloseList(createTheseVeryCloseAndInFrontChunks);
					}
				}
				else if (!TestRunner.RunGameOnlyNoisPatchesWithinWorldLimits)
				{
					if (shouldBeDestroyingChunksNow ()) 
					{
						chunk = getAChunkToDestroyFromList(destroyTheseChunks);
						
						if (chunk != null) 
						{
							if (!createTheseVeryCloseAndInFrontChunks.Contains(chunk.chunkCoord)) 
							{
								chunkMap.destroyChunkAt (chunk.chunkCoord);
								activeChunks.Remove(chunk);
								
							}
						} 
						//TODO: use removeAt by passing the index to the getAChunkToDestroy, etc..
						// probably faster, right?
						destroyTheseChunks.Remove(chunk);
					}
				}

			}
			yield return new WaitForSeconds(.2f);
		}
	}

	
	private Chunk getADoneCalculatingChunkWithChunkList(List<Chunk> chunkList)
	{
		foreach(Chunk chunk in chunkList) {
			if (chunk != null)
				if (chunk.Update())
					return chunk;
		}
		return null;
	}

	IEnumerator checkAsyncChunksList()
	{

		while (true)
		{
			if (true)
			{
				if (checkTheseAsyncChunksDoneCalculating.Count > 0) 
				{
					Chunk chunk = getADoneCalculatingChunkWithChunkList(checkTheseAsyncChunksDoneCalculating); // checkTheseAsyncChunksDoneCalculating[0];

					if (chunk != null)
					{
//						if (!chunk.noNeedToRenderFlag) {
							//async...
							activeChunks.Add (chunk);
							yield return StartCoroutine(chunk.applyMeshToGameObjectCoro ());
							chunk.isActive = true;
//						}
//						else {
//							throw new Exception("Surprising. chunk had no need to render flag true??");
//						}
						
						checkTheseAsyncChunksDoneCalculating.Remove(chunk);
					}

				}
			}
			yield return new WaitForSeconds(.1f);
		}
	}

	bool shouldBeCreatingChunksNow() {
		return true;
	}

	bool shouldBeDestroyingChunksNow() {
		return true;
	}
	
	#endregion

	void setNoiseHandlerResolution() 
	{
		int resXZ = (int)(NoisePatch.CHUNKDIMENSION * CHUNKLENGTH);
		noiseHandler.resolutionX = resXZ;
		noiseHandler.resolutionZ = resXZ;
	}

	//test use
	CoRange corangeFromNoiseCoord(NoiseCoord nco)
	{
		Coord start = new Coord(nco.x, 0, nco.z) * (float) NoisePatch.CHUNKDIMENSION;
		Coord range = new Coord (NoisePatch.CHUNKDIMENSION);
		return new CoRange (start, range);
	}

	#region funcs for noise patches
	// instead of blocks array, now there's a noise patch 
	// in the beginning, make noise for and blocks for one noise patch (hopefully the one where we're putting the player
	// then make noise for the surrounding noise patches
	// good if we can detect which noise patch the player is closest to
	// make blocks for that one first? etc.

	IEnumerator updateSetupPatchesListI()
	{
		while (true) 
		{
			NoiseCoord currentNoiseCo = noiseCoordContainingWorldCoord (new Coord (playerCameraTransform.position));
			
			NoiseCoord nco = noiseCoordClosestToPlayerThatHasNotStartedSetup(2);
			
			if (!NoiseCoord.Equal(nco, NoiseCoord.TheErsatzNullNoiseCoord()) ) //  !NoiseCoord.Equal (currentNoiseCo, lastPlayerNoiseCoord)) 
			{
				currentTargetedForCreationNoiseCo = nco;

				NoisePatch np = makeNoisePatchIfNotExistsAtNoiseCoordAndGet(nco);//  blocks.noisePatches [nco];
				
				if (!setupThesePatches.Contains(np))
				{
					setupThesePatches.Add (np);
				}
#if NEIGH_CHUNKS
				//NEIGHBORS
				List<NoiseCoord> neighborsToStart = np.neighborCoordsWhoHaveNotStartedSetup();
				foreach(NoiseCoord neico in neighborsToStart) 
				{
					NoisePatch neipatch = makeNoisePatchIfNotExistsAtNoiseCoordAndGet(neico);
					if (!setupThesePatches.Contains(neipatch))
						setupThesePatches.Add(neipatch);
				}
				//END NEIGHBORS
#endif
				

			}
			yield return new WaitForSeconds (.1f); //null; // 
		}
	}


	
	IEnumerator setupPatchesFromPatchesList()
	{
		//yield return new WaitForSeconds (0.1f);
		while (true) 
		{
			if (setupThesePatches.Count > 0) 
			{	
				NoisePatch np = setupThesePatches [0];

				if (np != null) 
				{
					if (!np.hasStarted) {
						np.populateBlocksAsync (); // async does seem to smooth out game play...
					}

				} else {
					throw new Exception("block from set up these patches was null");
				}
				
				setupThesePatches.RemoveAt (0);
				checkDoneForThesePatches.Add(np);
			}
			yield return new WaitForSeconds (.82f); //silly
		}
	}
	
	IEnumerator checkAsyncPatchesDone()
	{

		while (true)
		{
			if (checkDoneForThesePatches.Count > 0) 
			{
				NoisePatch npatch = checkDoneForThesePatches[0];

				if (npatch != null)
				{
					if (npatch.Update()) {
						if (checkDoneForThesePatches.Count > 0)
							checkDoneForThesePatches.RemoveAt (0); //bizarrely arg is out of range?
					} 
				}

			}
			
			yield return new WaitForSeconds(.1f);
		}
	}
	
	IEnumerator destroyFarAwayNoisePatches()
	{
		while(true)
		{
			List<NoiseCoord> furthestNCos = blocks.furthestNoiseCoords();
			
			foreach(NoiseCoord nco in furthestNCos) 
			{
				if (!m_destroyNoisePatchesOutsideOfRealm.contains(PTwo.PTwoXZFromNoiseCoord(nco)))
				{
					blocks.destroyPatchAt(nco);
				}
			}
			
			yield return new WaitForSeconds(1f);
		}
	}

	void makeNewAndSetupPatchAtNoiseCoordMainThread(NoiseCoord ncoord)
	{
		NoisePatch np = makeNoisePatchIfNotExistsAtNoiseCoordAndGet (ncoord); 

		np.populateBlocksFromNoiseMainThread();

	}

//	void makeNewAndSetupPatchAtNoiseCoord(NoiseCoord ncoord)
//	{
//		makeNoisePatchIfNotExistsAtNoiseCoord (ncoord); 
//
//		populateBlocksForPatchAtCoord (ncoord, false);
//	}

//	void makeNewPatchesSurroundingNoiseCoord(NoiseCoord ncoord) 
//	{
//		foreach (NoiseCoord nco in noiseCoordsSurroundingNoiseCoord(ncoord))
//		{
//			makeNoisePatchIfNotExistsAtNoiseCoord (nco);
//		}
//	}
	
	NoiseCoord noiseCoordClosestToPlayerThatDoesNotExist(int RadiusLimit)
	{
		return noiseCoordClosestToPlayer(RadiusLimit, false);
	}
	
	NoiseCoord noiseCoordClosestToPlayerThatHasNotStartedSetup(int RadiusLimit)
	{
		return noiseCoordClosestToPlayer(RadiusLimit, true);
	}
	
	NoiseCoord noiseCoordClosestToPlayer(int RadiusLimit, bool wantNotSetUp)
	{
		Coord playerCoord = playerPosCoord();
		NoiseCoord playerNCo = noiseCoordContainingWorldCoord(playerCoord);
				
		Coord currentNCoWoco = worldCoordForNoiseCoord(playerNCo);
		
		Coord relCo = playerCoord - currentNCoWoco;
		int firstPizzaAngle = closestPizzaAngleForRelativeCoord(relCo, new Coord ((int)(NoisePatch.CHUNKDIMENSION * CHUNKLENGTH)) );
		int pizzaAngle = firstPizzaAngle;
		
		int radius = 1;
		
		while(true)
		{
			NoiseCoord nextNoiseCo = playerNCo + new NoiseCoord(blockyRadar(pizzaAngle) * radius);
			if (!blocks.noisePatchExistsAtNoiseCoord(nextNoiseCo))
				return nextNoiseCo;
			else if (wantNotSetUp) {
//				NoisePatch np = blocks.noisePatches[nextNoiseCo];
//				if (!np.hasStarted)
				if (!blocks.noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(nextNoiseCo))
					return nextNoiseCo;
			}
			
			nextNoiseCo = playerNCo + new NoiseCoord(blockyRadar(pizzaAngle + 1) * radius);
			if (!blocks.noisePatchExistsAtNoiseCoord(nextNoiseCo))
				return nextNoiseCo;
			else if (wantNotSetUp) {
//				NoisePatch np = blocks.noisePatches[nextNoiseCo];
//				if (!np.hasStarted)
				if (!blocks.noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(nextNoiseCo))
					return nextNoiseCo;
			}
			
			nextNoiseCo = playerNCo + new NoiseCoord(blockyRadar(pizzaAngle - 1) * radius);
			if (!blocks.noisePatchExistsAtNoiseCoord(nextNoiseCo))
				return nextNoiseCo;
			else if (wantNotSetUp) {
//				NoisePatch np = blocks.noisePatches[nextNoiseCo];
//				if (!np.hasStarted)
				if (!blocks.noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(nextNoiseCo))
					return nextNoiseCo;
			}
			
			nextNoiseCo = playerNCo + new NoiseCoord(blockyRadar(pizzaAngle + 2) * radius);
			if (!blocks.noisePatchExistsAtNoiseCoord(nextNoiseCo))
				return nextNoiseCo;
			else if (wantNotSetUp) {
//				NoisePatch np = blocks.noisePatches[nextNoiseCo];
//				if (!np.hasStarted)
				if (!blocks.noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(nextNoiseCo))
					return nextNoiseCo;
			}
			
			nextNoiseCo = playerNCo + new NoiseCoord(blockyRadar(pizzaAngle - 2) * radius);
			if (!blocks.noisePatchExistsAtNoiseCoord(nextNoiseCo))
				return nextNoiseCo;
			else if (wantNotSetUp) {
//				NoisePatch np = blocks.noisePatches[nextNoiseCo];
//				if (!np.hasStarted)
				if (!blocks.noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(nextNoiseCo))
					return nextNoiseCo;
			}
			
			nextNoiseCo = playerNCo + new NoiseCoord(blockyRadar(pizzaAngle + 3) * radius);
			if (!blocks.noisePatchExistsAtNoiseCoord(nextNoiseCo))
				return nextNoiseCo;
			else if (wantNotSetUp) {
//				NoisePatch np = blocks.noisePatches[nextNoiseCo];
//				if (!np.hasStarted)
				if (!blocks.noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(nextNoiseCo))
					return nextNoiseCo;
			}
			
			nextNoiseCo = playerNCo + new NoiseCoord(blockyRadar(pizzaAngle - 3) * radius);
			if (!blocks.noisePatchExistsAtNoiseCoord(nextNoiseCo))
				return nextNoiseCo;
			else if (wantNotSetUp) {
//				NoisePatch np = blocks.noisePatches[nextNoiseCo];
//				if (!np.hasStarted)
				if (!blocks.noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(nextNoiseCo))
					return nextNoiseCo;
			}

			nextNoiseCo = playerNCo + new NoiseCoord(blockyRadar(pizzaAngle + 4) * radius);
			if (!blocks.noisePatchExistsAtNoiseCoord(nextNoiseCo))
				return nextNoiseCo;
			else if (wantNotSetUp) {
//				NoisePatch np = blocks.noisePatches[nextNoiseCo];
//				if (!np.hasStarted)
				if (!blocks.noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(nextNoiseCo))
					return nextNoiseCo;
			}

			radius++;
			
			if (radius > RadiusLimit)
				return NoiseCoord.TheErsatzNullNoiseCoord();
		}

	}
	
	//only returns 0, 2, 4 or 6 (0 == 3 oclock, 2 == 12 o'clock, 4 == 9 o'clock, 6 == 6 o'clock)
	int closestPizzaAngleForRelativeCoord(Coord relCoord, Coord zoneDimension) {
		
		if (relCoord.x > relCoord.z) { //lower right half triangle
			
			// this point is also inside the lower left half triangle so must be 6 o'clock
			if (zoneDimension.x - relCoord.x > relCoord.z) {
				return 6;
			} 
			
			return 0; // must be in the right side triange 
		} 
		
		if (zoneDimension.x - relCoord.x > relCoord.z) {
			return 4;
		}
		return 2; 
	}
	
	static Coord blockyRadar(int pizzaSliceCount) {
		// the pos x axis bisects the slice at sliceCount = 0
		
		pizzaSliceCount = pizzaSliceCount % 8;
		
		if (pizzaSliceCount < 0) {
			pizzaSliceCount = 8 - pizzaSliceCount;
		}
		
		int x = 0; int z = 0;
		
		if (pizzaSliceCount == 7 || pizzaSliceCount <= 1) {
			x = 1;	
		} else if (pizzaSliceCount < 6 && pizzaSliceCount > 2) {
			x = -1;
		}
		
		if (pizzaSliceCount > 0 && pizzaSliceCount < 4) {
			z = 1;
		} else if (pizzaSliceCount > 4) {
			z = -1;
		}
		
		return new Coord(x, 0, z);
	}

	//MORE EFFICIENT TO MAKE AN ARRAY OF NUDGE COORDS? (TODO:)
//	System.Collections.IEnumerable noiseCoordsSurroundingNoiseCoord(NoiseCoord ncoord)
//	{
//		//sloppy? (return this coord?)
//		yield return ncoord;
//		
//		int nudge = 1;
//		int znudge = 1;
//		yield return new NoiseCoord (ncoord.x + nudge, ncoord.z);
//		yield return new NoiseCoord (ncoord.x + nudge, ncoord.z + znudge);
//		yield return new NoiseCoord (ncoord.x - nudge, ncoord.z);
//		yield return new NoiseCoord (ncoord.x + nudge, ncoord.z - znudge);
//		yield return new NoiseCoord (ncoord.x, ncoord.z + znudge);
//		yield return new NoiseCoord (ncoord.x - nudge, ncoord.z + znudge);
//		yield return new NoiseCoord (ncoord.x, ncoord.z - znudge);
//		yield return new NoiseCoord (ncoord.x - nudge, ncoord.z - znudge);
//
////		#if NOT_NOW
//
//		nudge = 2;
//		znudge = 2;
//		yield return new NoiseCoord (ncoord.x + nudge, ncoord.z);
//		yield return new NoiseCoord (ncoord.x + nudge, ncoord.z + znudge);
//		yield return new NoiseCoord (ncoord.x - nudge, ncoord.z);
//		yield return new NoiseCoord (ncoord.x + nudge, ncoord.z - znudge);
//		yield return new NoiseCoord (ncoord.x, ncoord.z + znudge);
//		yield return new NoiseCoord (ncoord.x - nudge, ncoord.z + znudge);
//		yield return new NoiseCoord (ncoord.x, ncoord.z - znudge);
//		yield return new NoiseCoord (ncoord.x - nudge, ncoord.z - znudge);
//
////		#endif
//	}

	NoisePatch makeNoisePatchIfNotExistsAtNoiseCoordAndGet(NoiseCoord ncoord) 
	{
		if (blocks.noisePatchExistsAtNoiseCoord(ncoord))
			return blocks.noisePatches [ncoord];

		NoisePatch npatch = new NoisePatch (ncoord, this);
		blocks.addNoisePatchAt (ncoord, npatch);

		return npatch;
	}


	void populateBlocksForPatchAtCoord(NoiseCoord ncoord, bool mainThread) {
		if (!blocks.noisePatchExistsAtNoiseCoord(ncoord))
		{
			throw new System.ArgumentException("no noise patch at this coord", "ncoord");
		}

		NoisePatch np = blocks.noisePatches [ncoord];

		if (mainThread)
			np.populateBlocksFromNoiseMainThread();
		else
			np.Start();
	}

	NoiseCoord noiseCoordContainingWorldCoord(Coord woco) {
		return blocks.noiseCoordContainingWorldCoord(woco);
	}
	
	NoiseCoord noiseCoordContainingPlayer() {
		return noiseCoordContainingWorldCoord(new Coord(playerCameraTransform.position) );	
	}
	
	static Coord worldCoordForNoiseCoord(NoiseCoord nco) {
		int blocksPerPatch = (int) (NoisePatch.CHUNKDIMENSION * CHUNKLENGTH);
		return new Coord(nco * blocksPerPatch);
	}

	NoiseCoord noiseCoordContainingChunkCoord(Coord chcoord)
	{
		Coord adjustChCoord = chcoord.booleanNegative () * NoisePatch.CHUNKDIMENSION;

		// -4 -> -3 -> -7 -> -1
		return new NoiseCoord ((chcoord + chcoord.booleanNegative() - adjustChCoord) / NoisePatch.CHUNKDIMENSION);
	}


	// delegate-ish for noise patch
	public void noisePatchFinishedSetup (NoisePatch npatch) 
	{
		bug ("heard from noise patch?");

		if (setupThesePatches.Contains(npatch))
		{
			bug ("removing patch at: " + npatch.coord.toString ());
			setupThesePatches.Remove (npatch);
		}

		if (!firstNoisePatchDone ) // && setupThesePatches.Count == 0)
		{

			firstNoisePatchDone = true;
			bug ("heard back from first noisepatch and exhausted set up patches");
			finishStartSetup ();
			placePlayerAtSpawnPoint ();
		}

		// old just one noise patch
//		bug ("heard from noise patch?");
//		if (!firstNoisePatchDone)
//		{
//
//			firstNoisePatchDone = true;
//			bug ("heard back from first noisepatch");
//			finishStartSetup ();
//			placePlayerAtSpawnPoint ();
//		}
	}

	#endregion
	
	void movePlayerToXZofCoordAtSurface (Coord moveToCo)
	{
		moveToCo = highestSurfaceBlockCoordAt (moveToCo);
		moveToCo.y += 5;
		playerCameraTransform.parent.transform.position = moveToCo.toVector3 ();
	}

	// Use this for initialization
	void Start () 
	{
		debugLinesAssistant = (DebugLinesMonoBAssistant) debugLines;
		
		if (TestRunner.DontRunGame() )
			return;

		m_libnoiseNetHandler = new LibNoiseNetHandler();

		firstNoisePatchDone = false;

		chunkMap = new ChunkMap (); //new Coord (WORLD_XLENGTH_CHUNKS, WORLD_HEIGHT_CHUNKS, WORLD_ZLENGTH_CHUNKS));

		blocks.getSavedNoisePatches (); // from player prefs, if any...
		
		// REFRESH SAVED NOISEPATCHES
		foreach(KeyValuePair<NoiseCoord, NoisePatch> npatch in blocks.noisePatches)
		{
			NoisePatch np = (NoisePatch)npatch.Value;
			np.coord = npatch.Key;
			np.updateChunkManager (this);
			
			// TODO: if noisepatch is within the 'init' zone, make it regen its blocks
			// or are we already doing this?? (but are saved np's being 'lazy' somehow?)
		}

		lastPlayerNoiseCoord = new NoiseCoord (100000000, -100000003);
		setNoiseHandlerResolution ();
		noiseHandler.initNoiseMap ();
//		frustumChecker = new FrustumChecker (playerCameraTransform, (int) CHUNKLENGTH);

		audioSourceCamera = playerCameraTransform.parent.GetComponent<AudioSource> ();

		NoiseCoord initialNoiseCoord = new NoiseCoord (0, 0);
		makeNewAndSetupPatchAtNoiseCoordMainThread (initialNoiseCoord);
		
		if (TestRunner.RunGameOnlyNoisPatchesWithinWorldLimits)
		{
			int xstart, zstart, xend,zend;
			xstart = TestRunner.WorldLimits.start.x;
			xend = TestRunner.WorldLimits.outerLimit().x;
			zstart = TestRunner.WorldLimits.start.z;
			zend = TestRunner.WorldLimits.outerLimit().z;
			
			for(int x = xstart; x < xend ; ++x)
			{
				for(int z = zstart; z < zend ; ++z) 
				{
					NoiseCoord nco = new NoiseCoord(x,z);
					makeNewAndSetupPatchAtNoiseCoordMainThread(nco);
				}
			}
		}
		else if(!TestRunner.RunGameOnlyOneNoisePatch)
		{
			//NEIGHBORS
#if NEIGH_CHUNKS
			
//			NoisePatch initialPatch = blocks.noisePatchAtNoiseCoord(initialNoiseCoord);
			foreach(NeighborDirection ndir in NeighborDirectionUtils.neighborDirections())
			{
				NoiseCoord neiCo = initialNoiseCoord + NeighborDirectionUtils.nudgeCoordFromNeighborDirection(ndir);
				makeNewAndSetupPatchAtNoiseCoordMainThread(neiCo);
			}
			
#else	
			int times = 0;
			while (times < 9) //test?
			{
				NoiseCoord nco = noiseCoordClosestToPlayerThatHasNotStartedSetup(2);
				
				makeNewAndSetupPatchAtNoiseCoordMainThread(nco);
				times++;
			}
			times = 0;
#endif
		}
//		foreach (NoiseCoord ncoord in noiseCoordsSurroundingNoiseCoord(initialNoiseCoord)) 
//		{
////			if (!blocks.noisePatches.ContainsKey (ncoord))
//			makeNewAndSetupPatchAtNoiseCoordMainThread (ncoord);
//			if (times > 1)
//				break; //test???
//			times++; //test
//		}

		if (TestRunner.DontRunDoTerrainTestInstead)
		{
/*
			//extra noise cos 
			for (int k = -6 ; k < 7 ; k++) 
			{
				NoiseCoord nnco = new NoiseCoord(0,k);	
	//			if (!blocks.noisePatches.ContainsKey (nnco))
					makeNewAndSetupPatchAtNoiseCoordMainThread (nnco);
			}
			
			
			terrainTex = blocks.textureForTerrainAtXEqualsZero();
			terrainTex.Apply();
	
			bug("terrain tex height: " + terrainTex.height + " width: "  + terrainTex.width);
			terrainTex.filterMode = FilterMode.Point;
			terrainTex.anisoLevel = 1;
			terrainTestPlane.renderer.material.mainTexture = terrainTex;
			File.WriteAllBytes(Application.dataPath + "/../TerrainSlice.png", terrainTex.EncodeToPNG() );
	
			return;
*/
		}


		// ** MAKE SURROUNDING PATCHES AS WELL **// AND WAIT UNTIL ALL ARE DONE...
//		updateSetupPatchesList ();
//		foreach(NoisePatch np in setupThesePatches) {
//			bug ("gen np in start");
//			if (np != null) 
//			{
//				if (!np.generatedNoiseAlready) {
//					bug ("about to gen noise at: " + np.coord.x + " y: " + np.coord.z);
//					np.generateNoiseAndPopulateBlocksAsync ();
//				}
////				setupThesePatches.RemoveAt (0);
//			} else {
//				bug ("np was null");
//			}
//		}
		// **** END NEW STUFF ****

//		wait until its done (like it was on a main thread :)
//		while (!firstNoisePatch.Update()) {
//			bug ("noise patch one still not done");
//		}

		finishStartSetup ();

	}

	private void finishStartSetup() {
		Coord spawnPAtChunkCoord  = CoordUtil.ChunkCoordContainingBlockCoord (spawnPlayerAtCoord);

		//		CoRange nearbyCoRa = corangeFromNoiseCoord (initalNoiseCoord);
		//		bug ("noise patch based co range: " + nearbyCoRa.toString ());
		// want**
		CoRange nearbyCoRa = nearbyChunkRangeInitialForNoisePatch (blocks.noisePatches [new NoiseCoord (0, 0)]); //  nearbyChunkRangeInitial (spawnPAtChunkCoord);

		buildMapAtRange (nearbyCoRa);
		
		if (TestRunner.RunGameOnlyNoisPatchesWithinWorldLimits)
		{
			int xstart, zstart, xend,zend;
			xstart = TestRunner.WorldLimits.start.x;
			xend = TestRunner.WorldLimits.outerLimit().x;
			zstart = TestRunner.WorldLimits.start.z;
			zend = TestRunner.WorldLimits.outerLimit().z;
			
			for(int x = xstart; x < xend ; ++x)
			{
				for(int z = zstart; z < zend ; ++z) 
				{
					NoiseCoord nco = new NoiseCoord(x,z);
					Coord start = new Coord(x * NoisePatch.CHUNKDIMENSION, 0, z * NoisePatch.CHUNKDIMENSION);
					Coord range = new Coord(NoisePatch.CHUNKDIMENSION, 1, NoisePatch.CHUNKDIMENSION);
					CoRange noisePatchCoRange = new CoRange(start, range); //   nearbyChunkRangeInitialForNoisePatch(blocks.noisePatches[nco]);
					buildMapAtRange(noisePatchCoRange);
				}
			}
		}
		
		placePlayerAtSpawnPoint ();


		if (!TestRunner.RunGameOnlyOneNoisePatch) 
		{
			StartCoroutine (createAndDestroyChunksFromLists ());
			
			if (!TestRunner.RunGameOnlyNoisPatchesWithinWorldLimits)
			{
				StartCoroutine (setupPatchesFromPatchesList ());
				StartCoroutine (updateSetupPatchesListI ());
				StartCoroutine (checkAsyncPatchesDone ());
				
//				StartCoroutine(destroyFarAwayNoisePatches()); //crash city! TODO: investigate
				
				StartCoroutine (updateChunkLists ());
				StartCoroutine (checkAsyncChunksList ());
			}
			
//				StartCoroutine(updatePlayerPositionInShader());
		}

		

	}
	
	#region shader update
	private IEnumerator updatePlayerPositionInShader() 
	{
		while(true)
		{
			prefabMeshHolder.renderer.sharedMaterial.SetVector("_PlayerLoc", playerCameraTransform.position);	
			yield return new WaitForSeconds(.1f);
		}
	}
	#endregion

	private void placePlayerAtSpawnPoint() {
		movePlayerToXZofCoordAtSurface (spawnPlayerAtCoord);
	}


	// Update is called once per frame
	void Update () 
	{
		if (TestRunner.DontRunGame())
			return;
	
//		#if TEST_LIBNOISENET
//		return;
//		#endif
		//**want
		
//		DebugLinesUtil.drawDebugCubesForAllCreatedNoisePatches(currentTargetedForCreationNoiseCo, blocks.noisePatches);
//		DebugLinesUtil.drawDebugCubesForNoisePatchesList(setupThesePatches, Color.yellow, 6);
//		DebugLinesUtil.drawDebugCubesForNoisePatchesList(checkDoneForThesePatches, Color.magenta, 2);
//		DebugLinesUtil.drawDebugCubesForNoiseCoordList(bugNoiseCoordsThatDidntExist, Color.red, 1);
//		drawDebugLinesForNoisePatch(new NoiseCoord(0,0));
		
//		DebugLinesUtil.drawDebugCubesForAllUncreatedChunks(createTheseVeryCloseAndInFrontChunks);
//		DebugLinesUtil.drawDebugCubesForChunkCoordList(bugCoordsThatWaitedForAdjacentNPatches, new Color(.1f,.7f,.4f,1f), new Coord(-2,0,2));
//		DebugLinesUtil.drawDebugCubesForChunksOnDestroyList(destroyTheseChunks);
//		DebugLinesUtil.drawDebugCubesForChunksOnCheckASyncList(checkTheseAsyncChunksDoneCalculating);
//		DebugLinesUtil.drawDebugCubesForChunkList(activeChunks, new Color(.9f,.4f,.4f,1.0f), new Coord(1,0,4));
//		drawDebugCubesForEverBeenDestroyedList();


//		drawDebugLinesForChunkRange (m_wantActiveRealm);

//		drawDebugLinesForChunkRange (m_veryCloseAndInFrontRealm);

//		frustumChecker.drawLastRay ();
//		drawDebugRay ();

//		drawDebugLinesForBlockAtWorldCoord (m_testBreakMakeBlockWorldPos);

	}

//	void OnApplicationQuit() {
//		blocks.saveNoisePatchesToPlayerPrefs ();
//		bug ("saved hopefully");
//		PlayerPrefs.Save();
//	}

	IEnumerator sillyYieldFunc()
	{
		int i = 0;
		for (; i < 1000000; ++i)
		{
			bug ("frame count: " + Time.frameCount + "i was: " + i);
			yield return i * 2;
		}

	}
	
	public Coord highestSurfaceBlockCoordAt(Coord c) 
	{
//		NoisePatch npAtCoord = blocks.
		Block retBlock = null;
		int yy = WORLD_HEIGHT_BLOCKS - 1;
//		c = c.makeIndexSafe (m_mapDims_blocks);
		do {
			retBlock = blocks [c.x, yy--, c.z];
			if (yy == 0)
				break;
		} while (retBlock.type == BlockType.Air);

		bug("place player at coord with yy: " + yy);
		return new Coord (c.x, yy, c.z);
	}

	// TODO: hightsurface by getting the patch and running through it


}

[Serializable]
public class SavableBlock : Block
{
	public Coord coord;

	public SavableBlock (BlockType _type , Coord _coord) {
		this.type = _type;
		this.coord = _coord;
	}
}


public enum BlockType
{
	TheNullType = -1, Grass, Path, TreeTrunk, TreeLeaves, BedRock, Air, Stone, Stucco, Sand, Dirt, ParapetStucco, LightBulb
}

public static class B 
{
	public static void brk() 
	{
		UnityEngine.Debug.Break();
	}
}
