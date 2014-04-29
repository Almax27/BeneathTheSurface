using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour 
{
	#region types

	public enum State
	{
		SNEEZING,
		DANCING,
		DRINKING,
		IDLING,
		DEAD
	}

	#endregion

	#region public variables

    public Animator charAnimator = null;

	public State state = State.IDLING;

    public int infection = 0;

    public Attributes attributes;

    public InfectionArea infectionAreaPrefab;
	public GameObject deathExplosionPrefab;

	#endregion

    #region protected variables

    float aiDelay = 0;
    public bool isMoving = false;

    protected Vector3 desiredPosition;
    protected Vector3 currentVelocity;

	protected Vector3 lastPosition;

    BoxCollider currentArea = null;

	protected float tick = 0;

    #endregion

	#region monobehaviour methods

	// Use this for initialization
	void Start () 
    {
        attributes.Randomise();

        aiDelay = 0;
        DecideNextState();
        transform.position = desiredPosition;
		lastPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		tick += Time.deltaTime;

        switch (state)
        {
	    case State.SNEEZING:
	    {
			if(tick < 1.0f)
			{
				this.rigidbody.velocity = Vector3.zero;
				return;
			}
	        break;
	    }
	    case State.DANCING:
	    {
	        if(!isMoving)
	        {
	            ApplyModifiers(GameManager.instance.dancingModifiers);
	        }
	        break;
	    }
	    case State.DRINKING:
	    {
	        if(!isMoving)
	        {
	            ApplyModifiers(GameManager.instance.drinkingModifiers);
	        }
	        break;
	    }
	    case State.IDLING:
	    {
	        if(!isMoving)
	        {
	            ApplyModifiers(GameManager.instance.idleModifiers);
	        }
	        break;
	    }
	    case State.DEAD:
	    {
			tick += Time.deltaTime;
			if(tick > 0.2f)
			{

			}
	        break;
	    }
        }

        if (!isMoving)
        {
            ApplyModifiers(GameManager.instance.constantModifiers);
        }

        if (!isMoving)
        {
            aiDelay -= Time.deltaTime;
        }
        DecideNextState();

		//handle movement
		this.rigidbody.mass = isMoving ? 1.0f : 0.3f;
		Vector3 dir = desiredPosition - transform.position;
		float maxDeltaPos = Mathf.Min(dir.magnitude / Time.deltaTime, GameManager.instance.characterMoveSpeed);
		this.rigidbody.velocity = dir.normalized * maxDeltaPos;
		
		if (maxDeltaPos > 0.5f)
		{
			Quaternion desiredRot = Quaternion.LookRotation(this.rigidbody.velocity.normalized);
			this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, desiredRot, 360*Time.deltaTime);
		}
		
		if(isMoving && dir.magnitude < 0.5f)
		{
			isMoving = false;
			charAnimator.SetBool("isMoving", false);
			charAnimator.SetBool("isDancing", state == State.DANCING);
			charAnimator.SetBool("isDrinking", state == State.DRINKING);
			charAnimator.SetBool("isIdle", state == State.IDLING);
		}
		
		lastPosition = transform.position;
	}
	
	#endregion

    #region public methods

    public void Cough()
    {
		//if(state != State.SNEEZING)
		{
	        float coughInfection = infection;
	        Infect(-1);

	        GameObject gobj = GameObject.Instantiate(infectionAreaPrefab.gameObject) as GameObject;
	        InfectionArea infectionArea = gobj.GetComponent<InfectionArea>();
	        infectionArea.lifeTime = 0.5f;
	        infectionArea.infection = 1;
	        infectionArea.owner = this;
	        gobj.transform.position = this.transform.position + new Vector3(0,0.5f,0);

			charAnimator.SetTrigger("triggerSneeze");

			SetState(State.SNEEZING);
		}
    }

    public void Infect(int infectionModifier)
    {
		if(state != State.DEAD)
		{
	        //quick hack to catch null exception
	        Renderer ourRenderer = GetComponentInChildren<Renderer>();
	        if (ourRenderer)
	        {
	            infection += infectionModifier;

	            Debug.Log("Infecting " + this.name + " (+" + infectionModifier + ") = " + infection);
	            if (infection == 0)
	            {
	                ourRenderer.material.color = new Color(1, 1, 1);
	            } else if (infection == 1)
	            {
	                ourRenderer.material.color = new Color(0, 1, 0);
	            } else if (infection == 2)
	            {
	                ourRenderer.material.color = new Color(1, 0, 0);
	            } else if (infection >= 3)
	            {
	                //explode!
	                Cough();
					GameObject gobj = GameObject.Instantiate(deathExplosionPrefab) as GameObject;
					gobj.transform.position = this.transform.position;
					Destroy(gameObject);
				}
	        }
		}
    }

    public void DecideNextState()
    {
		if(aiDelay > 0)
		{
			return;
		}

		if (attributes.hydration < 0.2f && GameManager.instance.characters.Count > 10)
		{
			SetState(State.DRINKING);
        } else if ((attributes.energy + attributes.confidence) * 0.5f > 0.5f || infection >= 2 || GameManager.instance.characters.Count < 20)
        {
            SetState(State.DANCING);
        } else
        {
            SetState(State.IDLING);
        }

    }

    void MoveToArea(BoxCollider[] _areas)
    {
        currentArea = _areas[Random.Range(0, _areas.Length)];
        MoveInArea(currentArea);
    }

    void MoveInArea(BoxCollider _area)
    {
        float xPos = Random.Range(currentArea.bounds.min.x, currentArea.bounds.max.x);
        float zPos = Random.Range(currentArea.bounds.min.z, currentArea.bounds.max.z);
        
        desiredPosition = new Vector3(xPos, 0, zPos);

		charAnimator.SetBool("isMoving", true);
		isMoving = true;
    }

    void ApplyModifiers(Attributes _mod)
    {
        attributes.hydration = Mathf.Clamp01(attributes.hydration + (_mod.hydration * Time.deltaTime));
        attributes.fun = Mathf.Clamp01(attributes.fun + (_mod.fun * Time.deltaTime));
        attributes.energy = Mathf.Clamp01(attributes.energy + (_mod.energy * Time.deltaTime));
        attributes.confidence = Mathf.Clamp01(attributes.confidence + (_mod.confidence * Time.deltaTime));
    }

	void SetState(State _newState)
	{
        aiDelay = Random.Range(0.5f, 5f);
		tick = 0;

        if (currentArea && state == _newState)
        {
            MoveInArea(currentArea);
        } else
        {
            switch (_newState)
            {
	        case State.SNEEZING:
	        {

	            break;
	        }
	        case State.DANCING:
	        {
	            MoveToArea(GameManager.instance.danceAreas);
	            break;
	        }
	        case State.DRINKING:
	        {
	            MoveToArea(GameManager.instance.drinkAreas);
	            break;
	        }
	        case State.IDLING:
	        {
	            if (attributes.confidence < 0.2f)
	            {
	                MoveToArea(GameManager.instance.unsocialAreas);
	            } else
	            {
	                MoveToArea(GameManager.instance.socialAreas);
	            }
	            break;
	        }
	        case State.DEAD:
	        {
				charAnimator.SetTrigger("triggerDeath");
	            break;
	        }
            }
        }

		state = _newState;
	}

    #endregion
}
