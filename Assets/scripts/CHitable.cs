#define with2D
//#define with3D

using UnityEngine;
using System.Collections;


public class CHitable : MonoBehaviour
{

	public delegate void KilledCallback(GameObject thisOne,GameObject instigator);

	public event KilledCallback kill_event;

	// Use this for initialization
	void Start ()
	{
		m_hitPoints = def_baseHitpoints;
		if (m_hitPoints <= 0.0f)
			m_hitPoints = 1.0e-6f;
	}
	
	public void takeDamage( GameObject instigator , Vector3 impact , float amount )
	{
		if (m_hitPoints <= 0.0f)
			return;
		m_hitPoints -= amount;
		if (m_hitPoints <= 0.0f)
		{
			// spawn a boom.
			if (!System.Object.ReferenceEquals(def_boomAnim, null))
				Instantiate(def_boomAnim, transform.position, new Quaternion());
			Transform walk = transform;
			while (!System.Object.ReferenceEquals(walk.parent, null))
			{
				walk = walk.parent;
			}
			if (kill_event != null)
				kill_event.Invoke(gameObject, instigator);
			kill_event = null;	// this probably doesn't work. long thread on stackoverflow on this.
			Destroy(walk.gameObject);
		}
	}

	private void OnTouchOther(GameObject other)
	{
		CBullet shot = other.GetComponent<CBullet>();
		if ((shot!=null) && 0 != ((1u<<gameObject.layer) & shot.hitmask))
		{
			Vector3 speed;
			// hit by a bullet.
#if with3D
			Rigidbody r3 = other.GetComponent<Rigidbody>();
			if (r3 != null)
			{
				Debug.Log(r3.ToString());
				speed = r3.velocity;
			}else
#endif
			{
#if with2D
				Rigidbody2D r2 = other.GetComponent<Rigidbody2D>();
				if (r2 != null)
					speed = r2.velocity;
				else
#endif
					speed = new Vector3();
			}
			takeDamage(other, speed, shot.def_damage);
			Destroy(other);  // ..... add boom.
		}
	}

#if with2D
	void OnTriggerEnter2D(Collider2D other)
	{
		OnTouchOther(other.gameObject);
	}
#endif

#if with3D
	void OnTriggerEnter(Collider other)
	{
		OnTouchOther(other.gameObject);
	}

	void OnCollisionEnter(Collision info)
	{
		OnTouchOther(info.collider.gameObject);
	}
#endif

	public float def_baseHitpoints = 1.0f;
	public GameObject def_boomAnim;

	internal float m_hitPoints;
}
