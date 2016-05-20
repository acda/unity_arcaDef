using UnityEngine;
using System;


public class CWave : MonoBehaviour
{

	public CWave()
	{
	}

	protected virtual void Awake()
	{
	}

	protected virtual void onWaveFinished()
	{
		Debug.Log("Wave finished  "+GetInstanceID().ToString());
		Destroy(this);
	}

	internal virtual void shipKilledCallback(GameObject ship)
	{
//		Debug.Log("   ship killed  "+ship.GetInstanceID().ToString());
	}

	public virtual float spawn_progress(out int totalEnemies,out int spawnedEnemies)
	{
		totalEnemies = spawnedEnemies = 0;
		return 1.0f;
	}

}

