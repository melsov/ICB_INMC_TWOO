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


//courtesy of: http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html
[Serializable]
public class ThreadedJob : ISerializable
{
	private bool m_IsDone = false;
	private object m_Handle = new object();
	private System.Threading.Thread m_Thread = null;

	public ThreadedJob () 
	{

	}

	public bool IsDone
	{
		get
		{
			bool tmp;
			lock (m_Handle)
			{
				tmp = m_IsDone;
			}
			return tmp;
		}
		set
		{
			lock (m_Handle)
			{
				m_IsDone = value;
			}
		}
	}

	public virtual void Start()
	{
		m_Thread = new System.Threading.Thread(Run);
		m_Thread.Start();
	}
	public virtual void Abort()
	{
		m_Thread.Abort();
	}

	protected virtual void ThreadFunction() { }

	protected virtual void OnFinished() { }

	public virtual bool Update()	
	{
		if (IsDone)
		{
			OnFinished();
			return true;
		}
		return false;
	}
	private void Run()
	{
		ThreadFunction();
		IsDone = true;
	}

	#region Serializable interface

	protected virtual void doGetObject (SerializationInfo info, StreamingContext context) { }

	// awkardly, we must implement iSerializable, to AVOID doing any serialization in ThreadedJob—or so it seems.
	// 'doGetObject()' gets called on NoisePatch which actually does the serializing
	void ISerializable.GetObjectData(SerializationInfo oInfo, StreamingContext oContext)
	{
		this.doGetObject (oInfo, oContext);
	}

	// The special constructor is used to deserialize values. 
	public ThreadedJob(SerializationInfo info, StreamingContext context)
	{

	}
	#endregion
}


