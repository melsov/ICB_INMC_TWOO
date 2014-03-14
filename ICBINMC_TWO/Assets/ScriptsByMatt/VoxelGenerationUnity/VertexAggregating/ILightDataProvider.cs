using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface ILightDataProvider 
{
	Coord chunkCoord {
		get;
	}
	
	Color32 lightDataForQuad(Quad quad, Direction dir, int normalOffset);
	Vector4 lightDataForQuadFloat(Quad quad, Direction dir, int normalOffset);
}

public enum CoordSurfaceStatus {
	BELOW_SURFACE_TRANSLUCENT, BELOW_SURFACE_SEMI_TRANSLUCENT, BELOW_SURFACE_SOLID, ABOVE_SURFACE 
}

public class LightDataProvider : ILightDataProvider
{
	public DebugLinesMonoBAssistant debugLinesAssistant; // = ChunkManager.debugLinesAssistant;
	
	private Chunk m_chunk;
	private NoisePatch m_noisePatch;
	
	public const int NUM_LIGHT_LEVELS = 8;
	public const int BITS_PER_LIGHT_VALUE = 3;
	private const float LIGHT_VALUE_CONVERTER = (NUM_LIGHT_LEVELS - 1)/Window.LIGHT_LEVEL_MAX;
	
	public Coord chunkCoord {
		get{
			return this.m_chunk.chunkCoord;	
		}
	}
	
	public Vector4 lightDataForQuadFloat(Quad quad, Direction dir, int normalOffset)
	{
		
//		if (debugLinesAssistant == null)
//			debugLinesAssistant = ChunkManager.debugLinesAssistant; //BUG ON SEP THREAD // CompareBaseObjectsInternal can only be called from the main thread
		
		if (m_noisePatch == null)
			m_noisePatch = m_chunk.m_noisePatch;
		
		
//		AssertUtil.Assert(m_noisePatch != null, "what? noisepatch still null?");
		
		Coord chunkRelCoOfOrigin = chunkRelativeCoordFrom(quad, dir, normalOffset) + DirectionUtil.NudgeCoordForDirection(dir);

		// check the nine blocks right in front.
		// for those that are not solid
		// check the windows.
		
		CoordSurfaceStatus surfaceStatus;

		int[] rows = new int[4];

		for (int i = 0; i < quad.dimensions.s; ++i)
		{
			for (int j = 0; j < quad.dimensions.t; ++j)
			{
				Coord quadOffset = DirectionUtil.CoordForPTwoAndNormalDirectionWithFaceAggregatorRules(new PTwo(i,j), dir);
				
				Coord thePatchRelCo = CoordUtil.PatchRelativeChunkCoordForChunkCoord(this.chunkCoord) * (int) ChunkManager.CHUNKLENGTH + chunkRelCoOfOrigin + quadOffset;
				surfaceStatus = m_noisePatch.patchRelativeCoordIsAboveSurface(thePatchRelCo);

				int lightValue = NUM_LIGHT_LEVELS - 1;
				if (surfaceStatus != CoordSurfaceStatus.ABOVE_SURFACE)
				{
					lightValue = (int) (m_noisePatch.lightValueAtPatchRelativeCoord(thePatchRelCo, dir) * LIGHT_VALUE_CONVERTER);
				} 
				
				//SET THE VALUES IN THE WAY THAT THE SHADER EXPECTS THEM
				//EXAMPLE: IF QUAD.ORIGIN.S = 7, I = 0, 
				//THE SHADER WILL TAKE THE VALUE FROM COMPONENT WITH 'INDEX' 3 (7 % 4 => 3)
				rows[(quad.origin.s + i) % FaceSet.MAX_DIMENSIONS.s] |= colorValueFloatWith((quad.origin.t + j) 
					% FaceSet.MAX_DIMENSIONS.t, lightValue);
			}
		}

		return new Vector4((float)rows[0],(float)rows[1],(float)rows[2],(float)rows[3]);
	}
	
	private int colorValueFloatWith(int shiftByS, int shaderLightValue) 
	{
//		return (int) (shaderLightValue * Mathf.Pow(NUM_LIGHT_LEVELS, shiftByS));
		return (int) ( shaderLightValue << (shiftByS * BITS_PER_LIGHT_VALUE));
	}	
	
	private static Vector4 maxLightVec4() {
		float maxVal = (float)(8 * 8 * 8 * 8 - 1);
		return new Vector4(maxVal,maxVal,maxVal,maxVal);
	}
	
	public Color32 lightDataForQuad(Quad quad, Direction dir, int normalOffset)
	{
//		if (debugLinesAssistant == null)
//			debugLinesAssistant = ChunkManager.debugLinesAssistant;
		
		if (m_noisePatch == null)
			m_noisePatch = m_chunk.m_noisePatch;
		
		if (m_noisePatch == null)
			throw new Exception("what? noisepatch still null?");
		
		Coord chunkRelCoOfOrigin = chunkRelativeCoordFrom(quad, dir, normalOffset);
		if (dir != Direction.xpos && dir != Direction.zpos)
			chunkRelCoOfOrigin += DirectionUtil.NudgeCoordForDirection(dir);
		
		// check the nine blocks right in front.
		// for those that are not solid
		// check the windows.
		
//		Coord noisePatchRelCo = CoordUtil.PatchRelativeCoordWithChunkCoordOffset(chunkRelCoOfOrigin, new Coord(0));
		
		int[] rows = new int[4];
		
//		for (int i = 0; i < quad.dimensions.s; ++i)
//		{
//			for (int j = 0; j < quad.dimensions.t; ++j)
//			{
//				Coord quadOffset = DirectionUtil.CoordForPTwoAndNormalDirectionWithFaceAggregatorRules(new PTwo(i,j), dir);
//				int  lightValue = (int) ( m_noisePatch.lightValueAtPatchRelativeCoord(noisePatchRelCo + quadOffset) * LIGHT_VALUE_CONVERTER);
//				
//			}
//		}
		
		
//		Coord chunkWoco = CoordUtil.WorldCoordForChunkCoord(this.chunkCoord);
		
		CoordSurfaceStatus surfaceStatus = m_noisePatch.coordIsAboveSurface(this.chunkCoord, chunkRelCoOfOrigin);
//		if (surfaceStatus == CoordSurfaceStatus.ABOVE_SURFACE)
		if (surfaceStatus == CoordSurfaceStatus.ABOVE_SURFACE)
		{
			return new Color32(255,255,255,255); 
			//for now...origin above == all above
		}
		
		//for now....
		return new Color32(0,0,0,0);
		// ********************************* //
		
//		int[] rows = new int[4];
		
		for (int i = 0; i < quad.dimensions.s; ++i)
		{
			for (int j = 0; j < quad.dimensions.t; ++j)
			{
				Coord quadOffset = DirectionUtil.CoordForPTwoAndNormalDirectionWithFaceAggregatorRules(new PTwo(i,j), dir);
				byte lightVal = lightValueAt(this.chunkCoord, chunkRelCoOfOrigin + quadOffset, dir);
				rows[i] |= colorValueWith(j, lightVal);
			}
		}
		
		return new Color32((byte)rows[0],(byte)rows[1],(byte)rows[2],(byte)rows[3]);
	}

	
	private int colorValueWith(int shiftByS, byte lightValue) {
		return(int) (lightValue << (shiftByS * 2));	
	}
	
	private byte lightValueAt(Coord chunkCo, Coord offsetStart, Direction dir)
	{
		// get a bunch of angles for the correct hemisphere
		// keep a list of coords that already resulted
		int result = 0;
		Vector3[] sampler = HemiSphereData.NorthernHemispherePoints30;
		int sampleCount = sampler.Length;
		Vector3 v;
		foreach(Vector3 vec in sampler)
		{
			v = LightDataProvider.ConvertFromYPosToDirection(vec, dir);
			CoordSurfaceStatus surfaceStatus = lightValueAt(chunkCo, offsetStart, v);
			if (surfaceStatus == CoordSurfaceStatus.ABOVE_SURFACE)
				result++; // dead simple! // really want to word with the angles in question...
			if (result > 2 * sampleCount/3)
				return 3;
		}
		
		if (result > sampleCount/3)
			return 2;
		
		if (result > sampleCount/4)
			return 1;
		return 0;
		
		return (byte) (result/(2 * sampleCount/3));
	}
	
	private CoordSurfaceStatus lightValueAt(Coord chunkCo, Coord offsetStart, Vector3 nudgeV)
	{
		CoordLine testLine = new CoordLine();
		//radius is chunkLength
		for (int i = 1; i < Chunk.CHUNKLENGTH; ++i)
		{
			Coord nudgeC = LightDataProvider.BlockCoordFromVectorBlockRadius(nudgeV, i);
			Coord totalOffset =  offsetStart + nudgeC;
			
			//TEST
			if (i == 1)
				testLine.start = CoordUtil.WorldCoordForChunkCoord(chunkCo) +  totalOffset;
			
			//END TEST
			
			// TODO: if already a result at this coord return that result.
			CoordSurfaceStatus surfaceStatus = m_noisePatch.coordIsAboveSurface(chunkCo, totalOffset );
			
			if (surfaceStatus == CoordSurfaceStatus.ABOVE_SURFACE || surfaceStatus == CoordSurfaceStatus.BELOW_SURFACE_SOLID)
			{
//				if (surfaceStatus == CoordSurfaceStatus.BELOW_SURFACE_SOLID) //TEST
//				{
//					testLine.end = CoordUtil.WorldCoordForChunkCoord(chunkCo) +  totalOffset;
//					debugLinesAssistant.addCoordLine(testLine.start, testLine.end); //TEST
//				}
				return surfaceStatus;
			}
		}
		
		return CoordSurfaceStatus.BELOW_SURFACE_TRANSLUCENT;
	}
	
	private List<Vector3> pointsOnHemishpereInDirection(Direction dir)
	{
		return null;
		List<Vector3> points = new List<Vector3>();
		
		// hemisphere picking
		
		
		Axis axis = DirectionUtil.AxisForDirection(dir);		
		
	}
	
	private static Coord BlockCoordFromVectorBlockRadius(Vector3 v, int blockRadius)
	{
		return new Coord((int)(v.x * blockRadius + .5f), (int) (v.y * blockRadius + .5f), (int)(v.z * blockRadius + .5f)) ;	
	}
	
	/*
	 * Assumes vec starts in 'standard' y pos is 'up' space 
	 */
	private static Vector3 ConvertFromYPosToDirection(Vector3 vec, Direction dir)
	{
		Axis axis = DirectionUtil.AxisForDirection(dir);
		vec = convertFromYAxisToAxis(vec, axis);
		if (!DirectionUtil.IsPosDirection(dir))
			vec.y *= -1.0f;
		return vec;
	}
	
	private static Vector3 convertFromYAxisToAxis(Vector3 vec, Axis axis)
	{
		if (axis == Axis.X)
			return new UnityEngine.Vector3(vec.y, vec.x, vec.z);
		if (axis == Axis.Z)
			return new UnityEngine.Vector3(vec.x, vec.z, vec.y);
		return vec;
	}
	
	private CoordSurfaceStatus getSurfaceStatusAt(Coord chunkCo, Coord offsetStart, float theta)
	{
		return CoordSurfaceStatus.ABOVE_SURFACE;	
	}
	
	public LightDataProvider(Chunk _m_chunk)
	{
		m_chunk = _m_chunk;
		m_noisePatch = _m_chunk.m_noisePatch;
	}
	
	private Coord chunkRelativeCoordFrom(Quad quad, Direction dir, int normalOffset)
	{
		return DirectionUtil.CoordForPTwoNormalOffsetNormalDirectionWithFaceAggregatorRules(quad.origin, normalOffset, dir);
	}
	
	
}

