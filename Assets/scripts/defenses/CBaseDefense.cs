using UnityEngine;
using System.Collections;

public class CBaseDefense : MonoBehaviour
{

	protected enum targetPref {nearest,closeToAngle,strongest,mostAdvanced,leastAdvanced,mostAggressive}

	// Use this for initialization
	protected virtual void Start()
	{
		// blubb
		m_hitPoints = ( def_baseHintpoints>0.0f ? def_baseHintpoints : 1.0f );
	}

	protected virtual void Awake()
	{
	}

	protected virtual void OnDestroy()
	{

	}

	public virtual void takeDamage( GameObject instigator , Vector3 impact , float amount )
	{
		m_hitPoints -= 1.0f;
		if(m_hitPoints<=0.0f)
		{
			// spawn a boom.
			if( ! System.Object.ReferenceEquals(def_boom,null) )
				Instantiate(def_boom,transform.position,new Quaternion());
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		GameObject other_go = other.gameObject;
		CBullet shot = other_go.GetComponent<CBullet>();
		if(!System.Object.ReferenceEquals(shot, null))
		{
			// hit by a bullet.
			Destroy(other_go);	// ..... add boom.
			takeDamage( other_go , other_go.GetComponent<Rigidbody2D>().velocity , shot.def_damage );
		}
	}

	protected GameObject chooseTarget(float minRange,float maxRange,float angle,targetPref tPref)
	{
		// ..... todo: ignores params.
		// ..... todo: optimize - use a grid or built-in mechanism.
		float best_q;
		float max2 = maxRange*maxRange;
		float min2 = minRange*minRange;
		best_q = -1.0e9f;
		GameObject hit=null;
		foreach(GameObject it in GameObject.FindGameObjectsWithTag("enemy"))
		{
			Vector2 dif = it.transform.position-transform.position;
			float tmp;
			tmp = dif.sqrMagnitude;
			if( tmp > max2 )continue;		// too far.
			if( tmp < min2 )continue;		// too close.
			float q;
			switch(tPref)
			{
			case targetPref.closeToAngle:
				q = Mathf.Atan2(dif.y,dif.x)-angle;
				if( q>Mathf.PI )q-=2.0f*Mathf.PI;
				if( q<-Mathf.PI )q+=2.0f*Mathf.PI;
				q = -q*q;
				break;
			default:
				q = -dif.sqrMagnitude;
				break;
			}
			if( System.Object.ReferenceEquals(hit,null) || q>best_q )
			{
				hit = it;
				best_q = q;
			}
		}
		return hit;
	}

	public float def_baseHintpoints=1.0f;
	public GameObject def_boom;		// to show when explode.

	public float def_energyCost=10.0f;

	public bool showRoom=false;

	protected float m_hitPoints;
}

