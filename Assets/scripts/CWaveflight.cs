using UnityEngine;
using System;


public class CWaveflight : CWave
{

	internal class AttachPoint
	{
		public AttachPoint(float x0,float y0,int idx,CWaveflight wave){loc.x=x0;loc.y=y0;index=idx;killed=false;this.wave=wave;obj=null;state=PointState.preEnter;phase=0;exdat=null;}

	public enum PointState : ushort {preEnter,inField,left}

	public Vector2 loc;
	public float rot;
	public int index;
	public CWaveflight wave;
	public GameObject obj;
	public bool killed;
	public PointState state;
	// for use by special formation
	public int phase;
	public System.Object exdat;
	}

	public CWaveflight ()
	{
		m_number=0;
		m_members = new System.Collections.Generic.List<AttachPoint>();
		this.numberEnemies=1;
	}

	protected override void Awake()
	{
		base.Awake();
		m_startTime = def_startTime;
		m_spawnCount=0;
		for(int i=0;i<m_number;i++)
		{
			AttachPoint at = m_members[i];
			if( at.obj != null )
				Destroy(at.obj);
			at.obj=null;
			at.state = AttachPoint.PointState.preEnter;
		}
	}

	void OnDestroy()
	{
		for(int i=0;i<m_number;i++)
		{
			AttachPoint at = m_members[i];
			if( at.obj != null )
				Destroy(at.obj);
			at.obj=null;
		}
	}

	void Update()
	{
		if (m_startTime > 0.0f)
		{
			m_startTime -= Time.deltaTime;
			return;
		}
		update_positions();
		for(int i=0;i<m_number;i++)
		{
			AttachPoint at = m_members[i];
			if(System.Object.ReferenceEquals(at.obj,null))
			{
				// is not there. spawn if it gets in and is not yet killed
				if( at.state==AttachPoint.PointState.inField && !at.killed )
				{
					//Debug.Log("spawn " + i.ToString ());
					CBaseEnemy be;
					at.obj = (GameObject)Instantiate(def_shipType0,new Vector3(at.loc.x,at.loc.y,transform.position.z),new Quaternion());
					be = at.obj.GetComponent<CBaseEnemy>();
					be.m_formation = at;		// if having null-exception here, then the def does not contain Component 'CBaseEnemy'. It should.
					m_spawnCount ++;
				}
			}else{
				// object exists. Check if leaving
				if( at.obj == null )	// overloaded operator check if destroyed.
				{
					at.killed = true;
					at.obj = null;
				}else if( at.state==AttachPoint.PointState.left )
				{
					Destroy(at.obj);
					at.obj=null;
				}
			}
		}
	}

	public override float spawn_progress(out int totalEnemies,out int spawnedEnemies)
	{
		totalEnemies = numberEnemies;
		spawnedEnemies = m_spawnCount;
		return ( totalEnemies>0 ? spawnedEnemies/(float)totalEnemies : 1.0f );
	}

	protected virtual void update_positions()
	{}

	internal override void shipKilledCallback(GameObject ship)
	{
		base.shipKilledCallback(ship);
		AttachPoint at = ship.GetComponent<CBaseEnemy>().m_formation;
		at.obj = null;
		at.killed = true;

	}

	internal System.Collections.Generic.List<AttachPoint> m_members;

	protected int numberEnemies
	{
	set{
		while(m_number<value){m_members.Add(new AttachPoint(0.0f,0.0f,m_number++,this));}
		while(m_number>value){m_members.RemoveAt(--m_number);}
	}
	get{return m_number;}
	}

	public GameObject def_shipType0;
	public float def_startTime;

	private int m_number;
	protected int m_spawnCount;
	protected float m_startTime;

	// spline interpolate
	// f(x) := a*x^3 + b*x^2 + m0*x + x0
	// f'(x) := 3*a*x^2 + 2*b*x + m0
	// params: x0, x1, m0, m1
	// eq:
	//   f(1) = a + b + m0 + x0 := x1
	//   f'(1) = 3*a + 2*b + m0 := m1
	//   from first:   b :=  x1-x0-m0-a
	//   into second:  3*a + 2*x1-2*x0-2*m0-2*a + m0 := m1
	//                   a  := m1 - 2*x1 + 2*x0 + m0
	//   into first:     b := x1-x0-m0- m1 + 2*x1 - 2*x0 - m0
	//                   b := 3*(x1-x0) - 2*m0 - m1

	static public float interpolate_lr_xm(float x0,float x1,float m0,float m1,float t)
	{
		float a, b;
		if (t < 0.0f)t = 0.0f;
		if (t > 1.0f)t = 1.0f;
		a = m1 - 2.0f * x1 + 2.0f * x0 + m0;
		b = 3.0f * (x1 - x0) - 2.0f * m0 - m1;
		return ((a * t + b) * t + m0) * t + x0;
	}

	static public float interpolate_lr_xx(float xm1,float x0,float x1,float x2,float t)
	{
		return interpolate_lr_xm(x0,x1,0.5f*(x1-xm1),0.5f*(x2-x0),t);
	}

	// spline interpolate
	// f(x) := a*x^2 + m0*x + x0
	// params: x0, x1, m0
	// eq:
	//   f(1) = a + m0 + x0 := x1
	//                   a  := x1 - x0 - m0

	static public float interpolate_l_xm(float x0,float x1,float m0,float t)
	{
		float a;
		if (t < 0.0f)t = 0.0f;
		if (t > 1.0f)t = 1.0f;
		a = x1-x0-m0;
		return (a * t + m0) * t + x0;
	}

	static public float interpolate_l_xx(float xm1,float x0,float x1,float t)
	{
		return interpolate_l_xm(x0,x1,0.5f*(x1-xm1),t);
	}


	// spline interpolate
	// f(x) := a*x^2 + b*x + x0
	// f'(x) := 2*a*x + b
	// params: x0, x1, m1
	// eq:
	//   f(1) = a + b + x0 := x1
	//   f'(1) = 2*a + b := m1
	//   from first:   b :=  x1-x0-a
	//   into second:  2*a + b := m1
	//                   a := m1 +x0-x1
	//   into first:     b := x1-x0- m1-x0+x1
	//                   b := 2*(x1-x0)-m1

	static public float interpolate_r_xm(float x0,float x1,float m1,float t)
	{
		float a,b;
		if (t < 0.0f)t = 0.0f;
		if (t > 1.0f)t = 1.0f;
		a = m1 + x0 - x1;
		b = 2.0f * (x1 - x0) - m1;
		return (a * t + b) * t + x0;
	}

	static public float interpolate_r_xx(float x0,float x1,float x2,float t)
	{
		return interpolate_r_xm(x0,x1,0.5f*(x2-x0),t);
	}

}

