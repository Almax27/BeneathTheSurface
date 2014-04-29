using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	static public Music instance = null;

	// Use this for initialization
	void Awake () 
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(this);
	}
}
