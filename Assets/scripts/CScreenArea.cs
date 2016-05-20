using UnityEngine;
using System.Collections;

public class CScreenArea : MonoBehaviour
{

	void Awake()
	{
		sm_instance = this;
		m_master = GameObject.Find("Master").GetComponent<CMaster>();
		UnityEngine.Assertions.Assert.IsNotNull(m_master);
		if(def_gridSizeX<4)def_gridSizeX=4;
		if(def_gridSizeY<4)def_gridSizeY=4;
		if(def_gridSizeX>256)def_gridSizeX=256;
		if(def_gridSizeY>256)def_gridSizeY=256;
		m_grid = new GridCell[def_gridSizeX,def_gridSizeY];
	}

	// Use this for initialization
	void Start ()
	{
	
	}

	void OnTriggerExit2D(Collider2D other)
	{
		Destroy(other.gameObject);
	}

	void OnMouseDown()
	{
		Vector3 p = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
		m_master.userClicked(p.x,p.y);
	}

	// want to find this quickly. todo: ..... nice this way?

	public float def_x0 = -1.77777f;
	public float def_x1 = 1.77777f;
	public float def_y0 = -1.0f;
	public float def_y1 = 1.0f;
	public int def_gridSizeX = 16;
	public int def_gridSizeY = 9;

	public CScreenArea instance {get{return sm_instance;}}

	class GridCell
	{
		int dummy;
	}

	GridCell[,] m_grid;

	static private CScreenArea sm_instance;

	private CMaster m_master;

}
