using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    #region static

    public static GameManager instance;

    #endregion

    #region public variables

	public enum GameState
	{
		TITLE,
		GAME,
		WIN,
		FAILURE
	}

	public GameState gameState = GameState.TITLE;

	public GUITexture titleGUI;
	public GUITexture winGUI;
	public GUITexture failureGUI;

	public AudioSource popSoundPrefab;
	public AudioSource sneezeSoundPrefab;

	public TextMesh scoreText;
	public TextMesh timerText;

	public float gameTime = 60;
	public float timer;

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

	protected GameObject characterContainer;

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
		scoreText.text = "0/" + numberOfCharacters;
		SetGameState(GameState.TITLE);
	}
	
	// Update is called once per frame
	void Update () 
    {
		switch(gameState)
		{
		case GameState.TITLE:
		{
			if (Input.GetMouseButtonDown(0))
			{
				SetGameState(GameState.GAME);
			}
			break;
		}
		case GameState.GAME:
		{
			int infected = 0;
			for (int i = 0; i < characters.Count; i++)
			{
				if(characters[i] == null)
				{
					GameObject.Instantiate(popSoundPrefab);
					characters.RemoveAt(i--);
					continue;
				}
				//end game catch
				if(GameManager.instance.characters.Count <= 3)
				{
					characters[i].Infect(3);
				}
				if(characters[i].infection > 0)
				{
					infected++;
				}
			}

			if (Input.GetMouseButtonDown(0))
	        {
	            TryCough(Input.mousePosition);
	        }

			//handle fail
			timer += Time.deltaTime;
			if(timer >= gameTime || (infected == 0 && FindObjectsOfType(typeof(InfectionArea)).Length == 0))
			{
				timer = gameTime;
				SetGameState(GameState.FAILURE);
			}
			timerText.text = (gameTime - timer).ToString("0.0s");

			//handle win
			if(characters.Count == 0 )
	        {
				SetGameState(GameState.WIN);
	        }
			scoreText.text = characters.Count + "/" + numberOfCharacters;

			break;
		}
		case GameState.WIN:
		case GameState.FAILURE:
		{
			if (Input.GetMouseButtonDown(0))
			{
				//SetGameState(GameState.TITLE);
				Application.LoadLevel(Application.loadedLevel);
			}
			break;
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
				GameObject.Instantiate(sneezeSoundPrefab);
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

	void SetGameState(GameState _newState)
	{
		titleGUI.gameObject.SetActive(_newState == GameState.TITLE);
		winGUI.gameObject.SetActive(_newState == GameState.WIN);
		failureGUI.gameObject.SetActive(_newState == GameState.FAILURE);

		switch(_newState)
		{
		case GameState.TITLE:
		{
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
			timer = 0;
			break;
		}
		case GameState.GAME:
		{
			break;
		}
		case GameState.WIN:
		{
			break;
		}
		case GameState.FAILURE:
		{
			break;
		}
		}
		gameState = _newState;
	}

    #endregion
}
