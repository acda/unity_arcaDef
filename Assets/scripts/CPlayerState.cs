using UnityEngine;
using System.Collections;

public class CPlayerState
{
	public CPlayerState(CMaster master)
	{
		m_master = master;
		max_turret_levels = new int[10];	// ..... todo static 10 maximum?
	}


	public void mouseClick(float x,float y)
	{
		Vector3 p = new Vector3(x,y,0.0f);
//		Debug.Log("mouse"+p.ToString());
		GameObject.Instantiate( m_master.def_cannon_types[0] , p , new Quaternion() );
		// todo: ..... cloud and place-sound and effect...
	}

	public GameObject ship;
	public float energy;
	public int[] max_turret_levels;

	private CMaster m_master;
}
