using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PLAY, PACMAN_DYING, PACMAN_DEAD, GAME_OVER, GAME_WON
    };
    public GameState gameState = GameState.PLAY;
    public List<GameObject> pills;
    public List<GameObject> ghosts;
    [Range(1,10)]
    public float ghostVulnerableDuration = 7.0f;

    public GameObject pacman;
    public AnimationClip pacmanDeathAnimation;
    private float respawnTime = 0;//when pacman should respawn from the PACMAN_DYING state
    public Image gameOverScreen;
    public Image gameWonScreen;
    [Range(1,5)]
    public float ghostVulnerableEndWarningDuration = 2.0f;

    public AudioSource pacmanKilledSound;
    public AudioSource gameWonSound;
    public AudioSource gameOverSound;
    

    private static GameManager instance;
    private float invulnerableTime = 0;

    // Start is called before the first frame update
    void Start ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gameOverScreen.enabled = false;
        gameWonScreen.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.PLAY:
                bool foundPill = false;
                foreach (GameObject pill in pills)
                {
                    if (pill.activeSelf)
                    {
                        foundPill = true;
                        break;
                    }
                }
                if (!foundPill)
                {
                    gameState = GameState.GAME_WON;
                }
                break;
            case GameState.PACMAN_DYING:
                if (Time.time > respawnTime)
                {
                    gameState = GameState.PACMAN_DEAD;
                    respawnTime = Time.time + 1;
                    pacman.SetActive(false);
                }
                break;
            case GameState.PACMAN_DEAD:
                if (Time.time > respawnTime)
                {
                    gameState = GameState.PLAY;
                    pacman.SetActive(true);
                    PlayerController playerController = pacman.GetComponent<PlayerController>();
                    playerController.setLivesLeft(playerController.livesLeft - 1);
                    if (playerController.livesLeft >= 0)
                    {
                        playerController.setAlive(true);
                    }
                    else
                    {
                        gameState = GameState.GAME_OVER;
                    }
                    pacman.transform.position = Vector2.zero;
                    foreach (GameObject ghost in ghosts)
                    {
                        ghost.GetComponent<GhostController>().reset();
                    }
                }
                break;
            case GameState.GAME_OVER:
                gameOverScreen.enabled = true;
                gameWonScreen.enabled = false;
                if (!gameOverSound.isPlaying)
                {
                    gameOverSound.Play();
                }
                if (Input.anyKeyDown)
                {
                    resetGame();
                    gameState = GameState.PLAY;
                    gameOverScreen.enabled = false;
                    gameWonScreen.enabled = false;
                }
                break;
            case GameState.GAME_WON:
                gameOverScreen.enabled = false;
                gameWonScreen.enabled = true;
                if (!gameWonSound.isPlaying)
                {
                    gameWonSound.Play();
                }
                if (Input.anyKeyDown)
                {
                    resetGame();
                    gameState = GameState.PLAY;
                    gameOverScreen.enabled = false;
                    gameWonScreen.enabled = false;
                }
                break;
        }
         //Ghost Vulnerability
        if (invulnerableTime > 0)
        {
            if (Time.time > invulnerableTime)
            {
                invulnerableTime = 0;
                foreach (GameObject ghost in ghosts)
                {
                    ghost.GetComponent<GhostController>().setVulnerable(false);
                    ghost.GetComponent<GhostController>().freeze(false);
                }
            }
            else if (Time.time > invulnerableTime - ghostVulnerableEndWarningDuration && (Time.time *10)%2 < 0.1f)
            {
                foreach (GameObject ghost in ghosts)
                {
                    if (ghost.GetComponent<GhostController>().vulnerable) {
                        ghost.GetComponent<GhostController>().blink();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public static void pacmanKilled()
    {
        instance.gameState = GameState.PACMAN_DYING;
        instance.pacman.GetComponent<PlayerController>().setAlive(false);
        instance.respawnTime = Time.time + instance.pacmanDeathAnimation.length;
        instance.pacmanKilledSound.Play();
        foreach (GameObject ghost in instance.ghosts)
        {
            ghost.GetComponent<GhostController>().freeze(true);
        }
    }

    public void resetGame()
    {
        pacman.transform.position = Vector2.zero;
        PlayerController playerController = pacman.GetComponent<PlayerController>();
        playerController.setLivesLeft(2);
        playerController.setAlive(true);
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().reset();
        }
        foreach (GameObject pill in pills)
        {
            pill.SetActive(true);
        }
    }

    public static void makeGhostsVulnerable()
    {
        instance.invulnerableTime = Time.time + instance.ghostVulnerableDuration;
        foreach(GameObject ghost in instance.ghosts)
        {
            ghost.GetComponent<GhostController>().setVulnerable(true);
        }
    }
}
