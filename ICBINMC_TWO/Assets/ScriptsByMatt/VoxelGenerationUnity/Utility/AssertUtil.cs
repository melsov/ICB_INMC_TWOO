using UnityEngine;
using System.Collections;
using System;

public static class AssertUtil
{
	public static void Assert(bool assertion, string complaint)
	{
		if (!assertion)
			throw new Exception(complaint);
	}
}
