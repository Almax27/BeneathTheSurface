using UnityEngine;
using System.Collections.Generic;

public class DiscoLightTileManager : MonoBehaviour 
{
    #region public variables

    public enum Pattern
    {
        RandomPulse
    }

    Pattern pattern = Pattern.RandomPulse;

    public Color[] colours;

    public List<Renderer> tiles;

    #endregion

    #region protected variables

    protected float tick = 0;

    #endregion

	// Use this for initialization
	void Start () 
    {
        tick = float.MaxValue;
	}
	
	// Update is called once per frame
	void Update () 
    {
        tick += Time.deltaTime;

        switch (pattern)
        {
            case Pattern.RandomPulse:
            {
                if(tick > 0.5f)
                {
                    tick = 0;
                    for(int i = 0; i < tiles.Count; i++)
                    {
                        tiles[i].material.color = colours[Random.Range(0,colours.Length)];
                    }
                }
                break;
            }
        }
	}
}
