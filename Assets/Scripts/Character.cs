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

	#endregion

    #region protected variables

    float aiDelay = 0;
    public bool isMoving = false;

    protected Vector3 desiredPosition;
    protected Vector3 currentVelocity;

    BoxCollider currentArea = null;

    #endregion

	#region monobehaviour methods

	// Use this for initialization
	void Start () 
    {
        attributes.Randomise();

        aiDelay = 0;
        DecideNextState();
        transform.position = desiredPosition;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 dir = desiredPosition - transform.position;
        float maxDeltaPos = Mathf.Min(dir.magnitude / Time.deltaTime, GameManager.instance.characterMoveSpeed);
        this.rigidbody.velocity = dir.normalized * maxDeltaPos;
        if (this.rigidbody.velocity.sqrMagnitude > 0.5f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(this.rigidbody.velocity.normalized);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, desiredRot, 360*Time.deltaTime);
        }

        isMoving = dir.magnitude > 2f;
        charAnimator.SetBool("isMoving", isMoving);

        switch (state)
        {
            case State.SNEEZING:
            {
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

        /*
        //if(!isMoving)
        {
            float minDistSq = float.MaxValue;
            Character closestCharacter = null;
            for(int i = 0; i < GameManager.instance.characters.Count; i++)
            {
                Character character = GameManager.instance.characters[i];
                if(character != this)
                {
                    float distSq = (character.transform.position - this.transform.position).sqrMagnitude;
                    if(distSq < minDistSq)
                    {
                        minDistSq = distSq;
                        closestCharacter = character;
                    }
                    float dist = dir.magnitude;
                    if(dist < GameManager.instance.minCharacterDist)
                    {
                        //desiredPosition = transform.position + dir.normalized * GameManager.instance.minCharacterDist;
                        this.transform.position = dir.normalized * GameManager.instance.minCharacterDist;
                    }
                    else if(dist > GameManager.instance.maxCharacterDist)
                    {
                        //desiredPosition = transform.position + dir.normalized * GameManager.instance.maxCharacterDist;
                    }
                }
            }
        }*/
	}

	#endregion

    #region public methods

    public void Cough()
    {
        float coughInfection = infection;
        Infect(-infection);

        GameObject gobj = GameObject.Instantiate(infectionAreaPrefab.gameObject) as GameObject;
        InfectionArea infectionArea = gobj.GetComponent<InfectionArea>();
        infectionArea.lifeTime = 0.5f;
        infectionArea.infection = 1;
        infectionArea.owner = this;
        gobj.transform.position = this.transform.position + new Vector3(0,0.5f,0);


    }

    public void Infect(int infectionModifier)
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
                ourRenderer.material.color = new Color(1, 1, 0);
            } else if (infection == 2)
            {
                ourRenderer.material.color = new Color(1, 0, 0);
            } else if (infection >= 3)
            {
                //explode!
                Cough();
                gameObject.SetActive(false);
            }
        }
    }

    public void DecideNextState()
    {
        if (attributes.hydration < 0.2f)
        {
            SetState(State.DRINKING);
        } else if ((attributes.energy + attributes.confidence) * 0.5f > 0.5f)
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
        if (aiDelay > 0)
        {
			return;
        }

        aiDelay = Random.Range(0.5f, 5f);

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
                    break;
                }
            }
        }

		state = _newState;
	}

    #endregion
}
