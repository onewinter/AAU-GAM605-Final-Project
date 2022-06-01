using System.Collections;
using TMPro;
using UnityEngine;

public delegate void GameResetHandler();

public class GameManager : MonoBehaviour
{
    // event other scripts will subscribe to to reset positions/variables upon game restart
    public event GameResetHandler onGameReset;

    // ui & sound elements
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject titleText;
    [SerializeField] private float titleDelay = .3f;
    [SerializeField] private Color titleColor1;
    [SerializeField] private Color titleColor2;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject menuMain;
    [SerializeField] private GameObject menuControls;
    [SerializeField] private GameObject menuCredits;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private GameObject musicToggleText;

    private AudioSource audioSource;
    private GameObject currentMenu;
    private GameObject previousMenu;
    private bool isPaused;
    private bool musicPaused;
    private int score;
    private IEnumerator fadeTitle;

    // ui button functions
    public void OpenMenuMain() => OpenMenu(menuMain);
    public void OpenMenuCredits() => OpenMenu(menuCredits);
    public void OpenMenuControls() => OpenMenu(menuControls);
    public void QuitGame() => Application.Quit();
    
    // sound manager used by all scripts
    public void PlayAudioClip(AudioClip audioClip)
    {
        if (isPaused) return;
        audioSource.PlayOneShot(audioClip);
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        fadeTitle = FadeTitle();
    }

    private void Start()
    {
        // init the score gui
        UpdateScore(score);
        
        // open the main menu
        currentMenu = menuMain;
        OpenMenu(menuMain);
    }


    public void CloseMenu()
    {
        // if the menu we're closing is the main menu, unpause and return to the game
        if (currentMenu == menuMain)
        {
            // close all menus
            menuPanel.SetActive(false);
            hudPanel.SetActive(true);
            isPaused = false;
            Time.timeScale = 1f;
            // stop the title flash coroutine
            StopCoroutine(fadeTitle);
        }
        else
        {
            // otherwise, close the submenu and return to the main menu
            OpenMenu(menuMain);    
        }
    }

    // toggle the game music on and off by swapping out the audio clip
    public void ToggleMusic()
    {
        if(musicPaused)
        {
            musicPaused = false;
            audioSource.clip = musicClip;
            audioSource.Play();
            musicToggleText.GetComponent<TextMeshProUGUI>().text = "Music: On";
        }
        else
        {
            musicPaused = true;
            audioSource.clip = null;
            musicToggleText.GetComponent<TextMeshProUGUI>().text = "Music: Off";
        }
    }

    // open the specified menu
    void OpenMenu(GameObject menu)
    {
        // pause bool will return at beginning of any update loops
        isPaused = true;
        // time scale will catch anything else we missed and pause coroutines
        Time.timeScale = 0f;
        menuPanel.SetActive(true);
        hudPanel.SetActive(false);
        previousMenu = currentMenu;
        currentMenu = menu;
        // nullable as previous menu may be null
        previousMenu?.SetActive(false);
        currentMenu.SetActive(true);
        // coroutine to flash the title text's color
        StartCoroutine(fadeTitle);
    }

    // make the title text dance between colors like it's electrified
    IEnumerator FadeTitle()
    {
        // pretty much an infinite loop unless random hits 1.0f perfectly; restarted with each menu open and stopped when game is unpaused
        while (titleText.GetComponent<TextMeshProUGUI>().color != titleColor2)
        {
            titleText.GetComponent<TextMeshProUGUI>().color = Color.Lerp(titleColor1, titleColor2, Random.Range(0f,1f));
            yield return new WaitForSecondsRealtime(titleDelay);
        }

    }

    private void Update()
    {
        // when game is unpaused, Escape pauses and opens main menu; when game is paused, Escape closes the current menu
        switch (isPaused)
        {
            case false when Input.GetKeyDown(KeyCode.Escape):
                OpenMenu(menuMain);
                break;
            case true when Input.GetKeyDown(KeyCode.Escape):
                CloseMenu();
                break;
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    // update health and score in the gui
    public void UpdateHealth(int newHealth) => healthText.GetComponent<TextMeshProUGUI>().text = newHealth.ToString();
    public void UpdateScore(int newScore) => scoreText.GetComponent<TextMeshProUGUI>().text = newScore.ToString();
    
    // add to current score and update gui
    public void AddToScore(int scoreToAdd)
    {
        score += scoreToAdd;
        UpdateScore(score);
    }

    // reset the game after a win or loss
    public void ResetGame(bool didWeWin)
    {
        // display win/loss message
        if (didWeWin)
        {
            OpenMenu(menuWin);
            Debug.Log("Game win condition reached.");
        }
        else
        {
            OpenMenu(menuLose);
            Debug.Log("Game loss condition reached.");
        }

        // fire our game reset event to set everything back to start
        onGameReset.Invoke();
        // zero out the score and update the gui
        AddToScore(-score);
    }
}