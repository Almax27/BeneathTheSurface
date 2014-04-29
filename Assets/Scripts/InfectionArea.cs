using UnityEngine;
using System.Collections.Generic;

public class InfectionArea : MonoBehaviour {

    #region public variables

    public Character owner;
    public float lifeTime = 1;
    public int infection = 0;

	public Renderer renderer = null;

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

		transform.localScale = new Vector3(0,0,0);
		Color color = renderer.material.color;
		color.a = 0;
		renderer.material.color = color;
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

        scale = tick / lifeTime;
        transform.localScale = new Vector3(scale,1,scale);
		Color color = renderer.material.color;
		color.a = 1 - scale;
		renderer.material.color = color;
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
