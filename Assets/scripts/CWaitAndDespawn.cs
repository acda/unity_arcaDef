using UnityEngine;
using System.Collections;

public class CWaitAndDespawn : MonoBehaviour
{
	void Start()
	{
		StartCoroutine(waitAndDestroy(waitTime));
	}

	IEnumerator waitAndDestroy(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		Destroy(gameObject);
	}

	public float waitTime;
}
