using UnityEngine;
using System;


public class CWaveGroup : CWave
{

	protected override void Awake()
	{
		base.Awake();
		m_list = new pair[waves.Count];
		for (int i = 0; i < waves.Count; i++)
		{
			UnityEngine.Assertions.Assert.IsNotNull(waves[i]);		// waves must be defined.
			UnityEngine.Assertions.Assert.IsNotNull(waves[i].GetComponent<CWave>());	// waves must have CWave behavior
			m_list[i].wave = waves[i];
			m_list[i].start = (i<waves_starttime.Count?waves_starttime[i]:0.0f);
		}
		System.Array.Sort(m_list);
		m_next=0;
		m_time=0.0f;
	}

	void Update()
	{
		m_time += Time.deltaTime;
		while( m_next<m_list.Length && m_time>=m_list[m_next].start )
		{
			Debug.Log("spawn sub-wave "+m_next.ToString());
			Instantiate(m_list[m_next++].wave,new Vector3(),new Quaternion());
		}
	}

	public override float spawn_progress(out int totalEnemies,out int spawnedEnemies)
	{
		int t,s;
		totalEnemies = 0;
		spawnedEnemies = 0;
		for(int i=0;i<m_list.Length;i++)
		{
			m_list[i].wave.GetComponent<CWave>().spawn_progress(out t,out s);
			totalEnemies += t;
			spawnedEnemies += s;
		}
		return ( totalEnemies>0 ? spawnedEnemies/(float)totalEnemies : 1.0f );
	}

	public System.Collections.Generic.List<GameObject> waves;
	public System.Collections.Generic.List<float> waves_starttime;

	private int m_next;
	private float m_time;

	struct pair : IComparable
	{
		public GameObject wave;
		public float start;

		public int CompareTo(System.Object o)
		{
			if (start < ((pair)o).start)
				return -1;
			if (start > ((pair)o).start)
				return 1;
			return 0;
		}
	}

	private pair[] m_list;
}
