using UnityEngine;
using System.Collections;

public class CMaster : MonoBehaviour
{
	public CMaster()
	{
		m_players = new CPlayerState[4];	// ..... todo static 4 maximum?
		m_players[0] = new CPlayerState(this);
	}

	void Awake()
	{
		// search items
		m_textfield_energy   = GameObject.Find("TextField_Energy");
		m_textfield_announce = GameObject.Find("TextField_Announce");
		UnityEngine.Assertions.Assert.IsNotNull(m_textfield_energy);
		UnityEngine.Assertions.Assert.IsNotNull(m_textfield_announce);

		m_textfield_announce.SetActive(false);
	}

	// Use this for initialization
	void Start ()
	{
		m_currentWave = 0;
		StartCoroutine(wave_runner());
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void userClicked(float x,float y)
	{
		m_players[0].mouseClick(x,y);
	}

	IEnumerator wave_runner()
	{
		Vector3 p;
		GameObject ship;
		int shipOut;
		UnityEngine.UI.Text tx = m_textfield_announce.GetComponent<UnityEngine.UI.Text>();

		ship = null; shipOut = 1000;

		while (m_currentWave<def_stage_waves.Length)
		{
			yield return new WaitForSeconds(0.5f);
			tx.text = "Wave "+(m_currentWave+1).ToString();
			m_textfield_announce.SetActive(true);
			yield return new WaitForSeconds(1.0f);
			m_textfield_announce.SetActive(false);
			yield return new WaitForSeconds(1.0f);

			p = new Vector3( -0.5f , -0.9f , 0.0f );
			Instantiate( def_cannon_types[1] , p , new Quaternion(0.0f,0.0f,0.707107f,0.707107f) );
			yield return new WaitForSeconds(0.25f);
			p = new Vector3( 0.5f , -0.9f , 0.0f );
			Instantiate( def_cannon_types[0] , p , new Quaternion(0.0f,0.0f,0.707107f,0.707107f) );

			GameObject wav;
			wav = (GameObject)Instantiate( def_stage_waves[m_currentWave] , new Vector3() , new Quaternion() );
			while (wav != null)
			{
				yield return new WaitForSeconds(0.25f);
				if (ship == null)
				{
					if(shipOut++>7)
					{
						p = new Vector3(0.0f, -0.9f, 0.0f);
						ship = (GameObject)Instantiate(def_playerShip, p, new Quaternion(0.0f, 0.0f, 0.707107f, 0.707107f));
					}
				}else{
					shipOut = 0;
				}
			}
			m_currentWave++;
		}
	}

	// defs set in inspector
	public GameObject[] def_stage_waves;
	public GameObject[] def_cannon_types;
	public GameObject def_playerShip;

	// item handles
	private GameObject m_textfield_energy;
	private GameObject m_textfield_announce;
	private GameObject[] m_playerShip;
	private CPlayerState[] m_players;

	private int m_currentWave;
}
