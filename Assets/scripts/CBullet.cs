using UnityEngine;
using System.Collections;

public class CBullet : MonoBehaviour
{

	void Awake()
	{
	}

	void Start ()
	{
		// what is this??
		float rot = 2.0f*Mathf.Asin(transform.rotation.z*0.9999f);
		Vector2 v;
		v.x = def_speed*Mathf.Cos(rot);
		v.y = def_speed*Mathf.Sin(rot);
		GetComponent<Rigidbody2D>().velocity = v;
	}


	public float def_speed=1.0f;
	public float def_damage=1.0f;
	public uint hitmask = 65536;
}

