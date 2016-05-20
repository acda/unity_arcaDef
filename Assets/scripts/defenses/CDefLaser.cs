using UnityEngine;
using System.Collections;

public class CDefLaser : CBaseDefTurret
{

	protected override void Awake()
	{
		base.Awake();
		GameObject beam = transform.Find("beam").gameObject;	// if null-pointer here, gun misses it's 'beam'.
		m_lineRend = (LineRenderer)beam.GetComponent<LineRenderer>();
		UnityEngine.Assertions.Assert.IsNotNull(m_lineRend);
		m_lineRend.useWorldSpace = true;
		m_lineRend.enabled = false;
	}

	protected override void Start()
	{
		base.Start();
	}

	protected virtual void Update ()
	{
		m_shot_cooldown -= Time.deltaTime;
		if(m_shot_cooldown<=0.0f)
		{
			m_shot_cooldown = 1.0f/def_rateOfFire;
			// check for target
			Vector2 di = new Vector2(Mathf.Cos(m_turDirec),Mathf.Sin(m_turDirec));
			Vector2 p0 = new Vector2(m_turret.transform.position.x,m_turret.transform.position.y);
			RaycastHit2D hit;
			hit = UnityEngine.Physics2D.Raycast( p0 , di , def_range , 0x1800 );
			if( hit.collider != null )
			{
				// have a hit.
				Transform walk = hit.collider.gameObject.transform;
				while(!System.Object.ReferenceEquals(walk.parent,null))
					walk = walk.parent;
				m_target = walk.gameObject;
				fire_laser( p0 + di*hit.distance );
			}
		}
	}

	protected virtual void fire_laser( Vector2 hitHere )
	{
		if( m_target == null )return;
		Vector3 p;
		p = transform.position;
		p.z = -0.1f;
		m_lineRend.SetPosition(0,p);
		p.x = hitHere.x;
		p.y = hitHere.y;
		p.z = -0.1f;
		m_lineRend.SetPosition(1,p);
		m_lineRend.enabled = true;
		CHitable hit = m_target.GetComponent<CHitable>();
		if( ! System.Object.ReferenceEquals(hit,null) )
		{
			Vector3 v = m_target.transform.position - transform.position;
			v.Normalize();
			hit.takeDamage(gameObject,v,1.0f);
		}
		StartCoroutine(waitAndDestroy(0.15f));
	}

	IEnumerator waitAndDestroy(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		m_lineRend.enabled = false;
	}

	private LineRenderer m_lineRend;

}

