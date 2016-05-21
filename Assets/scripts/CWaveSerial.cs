using UnityEngine;
using System;
using System.Collections;

public class CWaveSerial : CWaveflight
{

	class SegData
	{
	public float len;
	public Vector2 dirvec;
	public float dir;
	public float pos0;
	}

	public CWaveSerial ()
	{
		m_pathdata = new System.Collections.Generic.List<SegData>();
	}

	protected override void Awake()
	{
		base.Awake();
//		m_startTime = def_startTime;
		m_pos = 0.0f;
		process_pathdata();
		numberEnemies = def_number;
	}

	private void process_pathdata()
	{
	  float p;
		p=0.0f;
		if(System.Object.ReferenceEquals(def_path,null))
		{
			def_path = new System.Collections.Generic.List<Vector2>();
			def_path.Add(new Vector2(-1.0f,0.0f));
			def_path.Add(new Vector2(1.0f,0.0f));
		}

		if (def_path.Count <= 2 || !def_round)
		{
			// just copy
			path = def_path;
		}
		else
		{
			// interpolate
			int subs = 10;
			int i;
			float sf = 1.0f / (float)subs;
			Vector2 P;
			path = new System.Collections.Generic.List<Vector2>();
			path.Add(def_path[0]);
			for (int j = 1; j < 10; j++)
			{
				P.x = interpolate_r_xx(def_path[0].x,def_path[1].x,def_path[2].x,sf*j);
				P.y = interpolate_r_xx(def_path[0].y,def_path[1].y,def_path[2].y,sf*j);
				path.Add(P);
			}
			for (i = 1; i + 2 < def_path.Count; i++)
			{
				path.Add(def_path[i]);
				for (int j = 1; j < 10; j++)
				{
					P.x = interpolate_lr_xx(def_path[i-1].x,def_path[i].x,def_path[i+1].x,def_path[i+2].x,sf*j);
					P.y = interpolate_lr_xx(def_path[i-1].y,def_path[i].y,def_path[i+1].y,def_path[i+2].y,sf*j);
					path.Add(P);
				}
			}
			i=def_path.Count-2;
			path.Add(def_path[i]);
			for (int j = 1; j < 10; j++)
			{
				P.x = interpolate_l_xx(def_path[i-1].x,def_path[i].x,def_path[i+1].x,sf*j);
				P.y = interpolate_l_xx(def_path[i-1].y,def_path[i].y,def_path[i+1].y,sf*j);
				path.Add(P);
			}
			i=def_path.Count-1;
			path.Add(def_path[i]);
		}

		for(int i=0;i+1<path.Count;i++)
		{
			SegData d= new SegData();
			Vector2 a = path[i];
			Vector2 b = path[i+1] - a;
			d.pos0 = p;
			d.len = b.magnitude;
			b.Normalize();
			d.dirvec = b;
			d.dir = (float)Math.Atan2(b.y,b.x);
			p += d.len;
			m_pathdata.Add(d);
		}
	}

	protected override void update_positions()
	{
		base.update_positions();
		m_pos += Time.deltaTime*def_speed;
		//Debug.Log("update_pos  pos=" + m_pos.ToString()+" memlen = "+m_members.Count.ToString()+" pathlen="+m_pathdata.Count.ToString());
		for(int i=0;i<numberEnemies;i++)
		{
			float pos = m_pos-(def_speed*def_timespacing)*i;
			if(pos<0.0)continue;
			CWaveflight.AttachPoint at = m_members[i];
			Vector2 xy;
			while(true)
			{
				bool beyond;
				float direc;
				if(at.phase>=m_pathdata.Count)
				{
					at.pointState = AttachPoint.PointState.left;
					break;
				}
				SegData d = m_pathdata[at.phase];
				xy = segpos(at.phase,pos-d.pos0,out direc,out beyond);
				if(beyond)
				{
					at.phase++;continue;
				}
				//if(i==1)
				//	Debug.Log(" xy = " + xy.ToString());
				at.loc = xy;
				at.rot = direc;
				at.pointState = AttachPoint.PointState.inField;
				break;
			}
		}
	}

	Vector2 segpos(int segIdx,float pos,out float direc,out bool beyond)
	{
		SegData d = m_pathdata[segIdx];
		beyond = (pos>d.len);
		direc = d.dir;
		return path[segIdx] + d.dirvec*pos;
/*
		Vector2 res;
		float t = pos / d.len;
		if (segIdx >= 1 && segIdx + 2 < path.Count)
		{
			// middle
			res.x = interpolate_lr_xx(path[segIdx-1].x,path[segIdx].x,path[segIdx+1].x,path[segIdx+2].x,t) ;
			res.y = interpolate_lr_xx(path[segIdx-1].y,path[segIdx].y,path[segIdx+1].y,path[segIdx+2].y,t) ;
		}
		else if (segIdx >= 1)
		{
			// right
			res.x = interpolate_l_xx(path[segIdx-1].x,path[segIdx].x,path[segIdx+1].x,t) ;
			res.y = interpolate_l_xx(path[segIdx-1].y,path[segIdx].y,path[segIdx+1].y,t) ;
		}
		else
		{
			// left
			res.x = interpolate_r_xx(path[segIdx].x,path[segIdx+1].x,path[segIdx+2].x,t) ;
			res.y = interpolate_r_xx(path[segIdx].y,path[segIdx+1].y,path[segIdx+2].y,t) ;
		}
		return res;
*/
	}

	public int def_number = 10;
	public float def_timespacing = 0.2f;
	public float def_speed = 2.0f/3.0f;
	public bool def_round = false;
	public System.Collections.Generic.List<Vector2> def_path;
	protected System.Collections.Generic.List<Vector2> path;

	System.Collections.Generic.List<SegData> m_pathdata;

	private float m_pos;
}


