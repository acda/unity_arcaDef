using UnityEngine;
using System.Collections;

public class CRandomShooter : MonoBehaviour
{

	void Start ()
	{
		float ang = def_shoot_angle;
		if(ang>180.0f)ang-=360.0f;
		if(ang<-180.0f)ang+=360.0f;
		ang = 0.5f * (ang*Mathf.PI/180.0f);
		m_shotdir = new Quaternion(0.0f,0.0f,Mathf.Sin(ang),Mathf.Cos(ang));
		if( def_shootrate < 0.001f )
			def_shootrate = 0.001f;
		StartCoroutine(wait_shoot_wait());
	}

	IEnumerator wait_shoot_wait()
	{
		yield return new WaitForSeconds(def_init_noshoot_time);
		while(true)
		{
			float rval = UnityEngine.Random.value;
			rval = rval*0.98f + 0.01f;	// exclude values too close to 0 and to 1.
			rval = -Mathf.Log(rval)/def_shootrate;
			yield return new WaitForSeconds(rval);
			fire();
		}
	}

	private void fire()
	{
		// todo: flash, shotsound, etc.
		Instantiate( def_shot , transform.position , m_shotdir );
	}

	public float def_init_noshoot_time = 1.0f;
	public float def_shootrate = 0.1f;	// random markov.
	public GameObject def_shot;
	public float def_shoot_angle = -90.0f;

	private Quaternion m_shotdir;

}
