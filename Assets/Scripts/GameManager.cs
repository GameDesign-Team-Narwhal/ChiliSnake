using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject restartLevelText;
    public GameObject instructionsText;

    public GameObject controlsOverlay;
    public GameObject snakeHeadPrefab;

    public GameObject pauseMenu;

    public Vector2 playerStartCoordinates;
    public uint numSegmentsNeeded = 10;
    public int nextLevelIndex = 1;
    public uint snakeSpeed = 4;
    public uint snakeStartingSegments = 5;
    public uint snakeSegmentOffset = 1;
    public bool showControlsOverlay = false;
    private bool gameStarted = false;

    private float gameStartTime;
    public SnakeHead snakeHeadInstance;

	void Awake ()
    {
        instance = this;
	}
	
    void Start()
    {
        StartGame();

        //when the level is loaded, show the controls hint
        if(showControlsOverlay)
        {
            controlsOverlay.GetComponent<Animator>().SetTrigger("Flash");
        }
    }

	// Update is called once per frame
	void Update ()
    {

        if(!gameStarted)
        {
            if(Application.isMobilePlatform)
            {
                if(Input.touches.Length > 0)
                {
                    StartGame();
                }
            }
            else
            {
                if(Input.anyKeyDown)
                {
                    StartGame();
                }
            }
        }
        else if(Time.time - gameStartTime > 2)
        {
            instructionsText.SetActive(false);
        }

        
	}

    //callbacks from SnakeHead
    public void OnPlayerDeath()
    {
        gameStarted = false;

        instructionsText.SetActive(false);
        restartLevelText.SetActive(true);
    }

    public void OnAddSegment()
    {
        if(snakeHeadInstance.bodySegments.Count >= numSegmentsNeeded)
        {
            WinLevel();
        }
    }

    void WinLevel()
    {
        //TODO: win animation

        StartCoroutine(LoadNextLevelAfterAnim());
    }

    IEnumerator LoadNextLevelAfterAnim()
    {
        yield return new WaitForSeconds(1.5f);

        Application.LoadLevel(nextLevelIndex);

        yield break;
    }

    void StartGame()
    {
        if(gameStarted)
        {
            Debug.LogError("StartGame() called when the game is already started!");
        }

        restartLevelText.SetActive(false);

        //get rid of old crying snake head
        if(snakeHeadInstance != null && snakeHeadInstance.gameObject != null)
        {
            GameObject.Destroy(snakeHeadInstance.gameObject);
        }

        snakeHeadInstance = ((GameObject)GameObject.Instantiate(snakeHeadPrefab, playerStartCoordinates, Quaternion.identity)).GetComponent<SnakeHead>();
        snakeHeadInstance.segmentOffset = snakeSegmentOffset;
        snakeHeadInstance.bodySegmentsToSpawn = snakeStartingSegments;
        snakeHeadInstance.speed = snakeSpeed;

        gameStarted = true;
        gameStartTime = Time.time;

        instructionsText.GetComponent<Text>().text = string.Format("Become {0} Segments Long!", numSegmentsNeeded);
        instructionsText.SetActive(true);
        
    }

    public void OnPause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void OnResume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
