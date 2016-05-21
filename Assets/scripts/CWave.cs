using UnityEngine;
using System;


public class CWave : MonoBehaviour
{

	public CWave()
	{
	}

	protected virtual void Awake()
	{
		m_done = false;
	}

	protected virtual void Update()
	{
		if (m_done)
		{
			Destroy(gameObject);
		}
	}

	public virtual float spawn_progress(out int totalEnemies,out int spawnedEnemies)
	{
		totalEnemies = spawnedEnemies = 0;
		return 1.0f;
	}

	internal bool m_done;
}

