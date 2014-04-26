using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    #region static

    public static GameManager instance;

    #endregion

    #region public variables

    public Character characterPrefab;
    public int numberOfCharacters;

    public List<Character> characters = new List<Character>();

    public float characterMoveSpeed = 5;
    public float minCharacterDist = 1;
    public float maxCharacterDist = 10;
    public float baseAttraction = 1;
    public float baseRepulsion = 1;

    public BoxCollider[] drinkAreas;
    public BoxCollider[] danceAreas;
    public BoxCollider[] socialAreas;
    public BoxCollider[] unsocialAreas;

    public Attributes constantModifiers;
    public Attributes dancingModifiers;
    public Attributes drinkingModifiers;
    public Attributes idleModifiers;

    #endregion

    #region protected vairables

    public GameObject characterContainer;

    #endregion

    #region monobehaviour methods

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        } else
        {
            instance = this;
        }
    }

	// Use this for initialization
	void Start () 
    {
        characterContainer = new GameObject("Characters");
	}
	
	// Update is called once per frame
	void Update () 
    {
        //build characters
        while (characters.Count < numberOfCharacters)
        {
            GameObject gobj = GameObject.Instantiate(characterPrefab.gameObject) as GameObject;
            gobj.transform.parent = characterContainer.transform;
            Character character = gobj.GetComponent<Character>();
            characters.Insert(0, character);
            character.name = "Character " + (characters.Count-1);

            //on creation of the first character
            if(characters.Count == 1)
            {
                character.Infect(1);
            }
        }
        //destroy characters
        while (characters.Count > numberOfCharacters)
        {
            GameObject.Destroy(characters[0].gameObject);
            characters.RemoveAt(0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryCough(Input.mousePosition);
        }

        int infected = 0;
        for (int i = 0; i < characters.Count; i++)
        {
            if(characters[i].infection > 0)
            {
                infected++;
            }
        }
        if(infected == 0)
        {
            if(FindObjectsOfType(typeof(InfectionArea)).Length > 0)
            {
               // Application.LoadLevel(Application.loadedLevel);
            }
        }
	}

    #endregion

    #region public methods

    void TryCough(Vector2 _screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(_screenPos);

        Plane plane = new Plane(new Vector3(0, 1, 0), new Vector3(0, 0, 0));
        float rayLength = 0;
        if (plane.Raycast(ray, out rayLength))
        {
            Vector3 worldPos = ray.GetPoint(rayLength);
            Character character = GetClosestInfectedCharacter(worldPos, 2);
            if(character)
            {
                character.Cough();
            }
        }
    }

    Character GetClosestInfectedCharacter(Vector3 _pos, float _maxDist)
    {
        Character closestCharacter = null;
        float minDist = float.MaxValue;
        for(int i = 0; i < characters.Count; i++)
        {
            Character character = characters[i];
            if(character.infection > 0)
            {
                float dist = Vector3.Distance(_pos, character.transform.position);
                if(dist < minDist && dist < _maxDist)
                {
                    minDist = dist;
                    closestCharacter = character;
                }
            }
        }
        return closestCharacter;
    }

    #endregion
}
