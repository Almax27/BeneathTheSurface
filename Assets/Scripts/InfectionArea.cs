using UnityEngine;
using System.Collections.Generic;

public class InfectionArea : MonoBehaviour {

    #region public variables

    public Character owner;
    public float lifeTime = 1;
    public int infection = 0;

    #endregion

    #region protected variables

    protected float tick = 0;
    protected float scale = 1;
    protected float scaleVel = 0;

    protected List<Character> charactersInfected = new List<Character>();

    #endregion

    #region monobehaviour methods

	// Use this for initialization
	void Start () 
    {
        charactersInfected.Clear();
	}
	
	// Update is called once per frame
	void Update () 
    {
        tick += Time.deltaTime;
        if (tick > lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        scale = (lifeTime-tick) / lifeTime;
        transform.localScale = new Vector3(scale,1,scale);
	}

    void OnTriggerStay(Collider _collider)
    {
        if (owner != null)
        {
            Character character = _collider.GetComponent<Character>();

            if (character && character != owner && !charactersInfected.Contains(character))
            {
                character.Infect(infection);
                charactersInfected.Add(character);
            }
        }
    }
        
    #endregion
}
