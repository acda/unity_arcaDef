using UnityEngine;
using System.Collections;

public class CPlayerShip : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		m_pressed0 = false;
		m_pressed1 = false;
		m_pressed2 = false;
	}

	// Update is called once per frame
	void Update ()
	{
		float ix,iy;
		Vector3 p = transform.position;
		ix = Input.GetAxis("Horizontal");
		iy = Input.GetAxis("Vertical");
		p.x += ix * def_moveSpeed*Time.deltaTime;
		p.y += iy * def_moveSpeed*Time.deltaTime;
		transform.position = p;
		if(Input.GetAxis ("Fire1") > 0.5f)
		{
			if(!m_pressed0)
			{
				onButton0();
			}
			m_pressed0 = true;
		}else
			m_pressed0 = false;
		if(Input.GetAxis ("Fire2") > 0.5f)
		{
			if(!m_pressed1)
			{
				onButton1();
			}
			m_pressed1 = true;
		}else
			m_pressed1 = false;
		if(Input.GetAxis ("Fire3") > 0.5f)
		{
			if(!m_pressed2)
			{
				onButton2();
			}
			m_pressed2 = true;
		}else
			m_pressed2 = false;
	}

	private void onButton0()
	{
		Vector3 p=transform.position;
		p.x -= 0.05f;
		Instantiate(def_bullet0,p,transform.rotation);
		p.x += 0.1f;
		Instantiate(def_bullet0,p,transform.rotation);
	}

	private void onButton1()
	{
	}

	private void onButton2()
	{
	}

	public float def_moveSpeed = 0.75f;
	public UnityEngine.GameObject def_bullet0;

	private bool m_pressed0;
	private bool m_pressed1;
	private bool m_pressed2;
}
