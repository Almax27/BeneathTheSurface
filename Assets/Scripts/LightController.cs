using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {

    public float pulseDelay = 0.5f;
    protected float tick = 0;
    protected bool pulse = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        tick += Time.deltaTime;

        if (tick > pulseDelay)
        {
            tick = 0;
            light.intensity = pulse ? 3 : 1;
            pulse = !pulse;
        }

	}
}
