using UnityEngine;
using System.Collections;

public class CBaseEnemy : MonoBehaviour
{

	void Start ()
	{
	}

	void Awake()
	{
		UnityEngine.Assertions.Assert.IsTrue( this.CompareTag("enemy") );
	}

	void OnDestroy()
	{
		if (!System.Object.ReferenceEquals(m_formation, null))
		{
			// unregister from formation ..... clean? count? callback?
			CHitable hit = GetComponent<CHitable>();
			if( !System.Object.ReferenceEquals(hit,null) && hit.m_hitPoints<=0.0f )
				m_formation.wave.shipKilledCallback(this.gameObject);
			m_formation.killed = true;
			m_formation.obj = null;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (!System.Object.ReferenceEquals(m_formation, null))
		{
			// we're formation bound.
			if (m_formation_freeflight)
				flyFree();
			else
			{
				// bound to anchor-point
				float r = m_formation.rot*0.5f;
				transform.position = new Vector3(m_formation.loc.x,m_formation.loc.y,0.0f);
				transform.rotation = new Quaternion(0.0f,0.0f,Mathf.Sin(r),Mathf.Cos(r));
			}
		}else{
			// not on formation
			flyFree();
		}
	}

	protected virtual void flyFree()
	{
		Vector3 p = transform.position;
		p.y -= 0.5f*Time.deltaTime ;
		transform.position = p;
	}

	internal CWaveflight.AttachPoint m_formation;		// formation object if present.
	internal bool m_formation_freeflight;				// true if deviating (attack-flight)

}

