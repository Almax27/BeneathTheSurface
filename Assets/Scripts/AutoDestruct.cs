using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {

	public float delay = 0;
	protected float tick = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		tick += Time.deltaTime;
		if(tick > delay)
		{
			Destroy(gameObject);
		}
	}
}
