using UnityEngine;
using System.Collections;

public class LightLevelTable 
{

// a double ? (?) could potentially store light levels for a chunk: (of 16 x 16 x 256 dims ) x say 8 light levels x 6 faces.
// i.e. 16 x 16 (1 byte) * height 256 (another byte) * 8 (3/8 of a byte) * 6 (also 3/8 of a byte)
	
// in the shader, the verts could do a look up based on their position to get their light level (don't need world pos of course....)
// we should test this idea...
	
//	for simplicity, throw out all faces except y pos/neg... therefore, don't need to factor in faces
//	for simplicity, only the direct neighbors are lit at all

// TEST: lets say that there are light blocks every 16,17,18 x/y/z

// set the int: lights = 0; //nothing lit 
// coord = 16, 17, 18
//	z is least significant
//	the times 8 for light options expands the z axis ( * NUM_LIGHT_LEVELS (8))
//	lights = 7 would mean that 0,0,0 was lit fully (and nothing else is lit at all)
//	so to set the nth 'light map block' to MAX (7);
//	int set_to_seven = (16 * (CHLEN * CHHEIGHT) + 17 * (CHLEN) + 18 ) * NUM_LIGHT_LEVELS + 7;
//	
//	 now that we do the math even a double for a whole chunk may not be feasible: (it's pow(8) not * 8 :)
//	but in any case...there can be a single int or somthing for each (and any) 
//	face aggregator (i.e. each plane) ... and that number gets packed along with each vertex on that plane.
//	 then the vertex does a look up and kaboom? (actually in this case) the lookup will have to happen in the
//	fragment shader--right? (which will know its position and have the special number etc.)
}
