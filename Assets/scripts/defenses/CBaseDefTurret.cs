using UnityEngine;
using System.Collections;

public class CBaseDefTurret : CBaseDefense
{

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		m_target=null;
		m_turDirec = 2.0f*Mathf.Asin(m_turret.transform.rotation.z*0.9999f);
		m_shot_cooldown = 0.0f;
	}

	protected override void Awake()
	{
		base.Awake();
		m_turret = transform.Find("turret").gameObject;	// must have a 'turret' child object.
	}

	// Update is called once per frame
	protected virtual void Update ()
	{
		m_shot_cooldown -= Time.deltaTime;
		if( m_target != null )	// if valid target and not destroyed
		{
			// check if target moved out-of-range
			if( ( m_target.transform.position - m_turret.transform.position ).sqrMagnitude >= def_range*def_range )
				m_target = null;
		}
		if( m_target == null )	// if not assigned or (overloaded) if destroyed
		{
			m_target = chooseTarget( 0.0f , def_range , m_turDirec , targetPref.closeToAngle );
		}
		if(!( m_target == null))	// if valid target and not destroyed
		{
			if( aimAt(m_target) && (m_shot_cooldown<=0.0f) )
			{
				m_shot_cooldown = 1.0f/def_rateOfFire;
				fire();
			}

		}

	}

	protected virtual bool aimAt(GameObject target)
	{
		Vector2 T = target.transform.position - m_turret.transform.position;
		Rigidbody2D r2d = target.GetComponent<Rigidbody2D>();
		if(!System.Object.ReferenceEquals(r2d,null))
		{
//			float bulSpd = 1.0f;
			// ..... todo: lead target. Need uniform way to get bullet-speed.
//			T += def_leadTarget * T.magnitude/def_bullet.
		}
		float todir = Mathf.Atan2(T.y,T.x);
		todir -= m_turDirec;
		if( todir<-Mathf.PI )todir+=2.0f*Mathf.PI;
		if( todir>Mathf.PI )todir-=2.0f*Mathf.PI;
		float dstep = Time.deltaTime*def_turnspeed_radsPerSec;
		bool result = false;
		if( todir<=dstep && todir>=-dstep )
		{
			// exact
			m_turDirec += todir;
			result = true;
		}else{
			// turn
			if( todir<0.0f )
				m_turDirec -= dstep;
			else
				m_turDirec += dstep;
		}
		if( m_turDirec < -Mathf.PI )m_turDirec += 2.0f*Mathf.PI;
		if( m_turDirec > Mathf.PI )m_turDirec -= 2.0f*Mathf.PI;
		m_turret.transform.rotation = new Quaternion(0.0f,0.0f,Mathf.Sin(m_turDirec*0.5f),Mathf.Cos(m_turDirec*0.5f));
		return result;
	}

	protected virtual void fire()
	{
		Instantiate( def_bullet , m_turret.transform.position , m_turret.transform.rotation );
		// todo: shot sound, muzzle flash, ...
	}

	public float def_range=2.0f;
	public float def_rateOfFire=2.0f;
	public float def_turnspeed_radsPerSec=0.314f;
	public float def_leadTarget=0.75f;
	public GameObject def_bullet;

	protected GameObject m_target;
	protected GameObject m_turret;
	protected float m_turDirec;
	protected float m_shot_cooldown;
}

