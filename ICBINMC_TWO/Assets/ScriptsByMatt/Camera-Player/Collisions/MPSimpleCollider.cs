using UnityEngine;
using System.Collections;

public interface iBlockProvider
{
	Block blockAt(int x, int y, int z);
}

public class MPSimpleCollider : MonoBehaviour 
{
	
	private iBlockProvider m_blockProvider;
	
	public void setBlockProvider(iBlockProvider blockProvider) {
		m_blockProvider = blockProvider;
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		AssertUtil.Assert(m_blockProvider != null, "need a blockprovider MPSimpleCollider");
	}
	
	private void updateTransform()
	{
		
	}
}
