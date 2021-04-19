using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int numRoomsComplete = 0;
    private GameObject PlayerBeforeChange;
    private GameObject InventBefore;
    private GameObject PlayerAfterChange;
    private GameObject InventAfter;
    public int currHealth;
    public Stat currDamage;
    public Stat currSpeed;
    private string[] levels = new string[6] { "Level1", "Level3", "Level4", "Level5", "Level6", "Level7" };
    public GameObject PauseMenu;
    public bool isPaused;
    public TextMeshProUGUI levelText;
    private int levelsPassed;

    private GameManager()
    {
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            Instantiate(PauseMenu);
            Time.timeScale = 0f;
            isPaused = true;
        }
        if (levelsPassed == 0)
        {
            levelText.text = "";
        }
    }

    public void Unpause()
    {
        isPaused = false;
    }

    public void ResetLevelCount()
    {
        levelsPassed = 0;
        levelText.text = "";

    }


    public GameManager Instance() {return instance;}

    public void LoadNextRoom()
    {
        PlayerBeforeChange = GameObject.FindWithTag("Player");
        PlayerBeforeChange.GetComponent<PlayerController>().ExportStats(instance);
        numRoomsComplete += 1;
        if (numRoomsComplete % 6 == 0)
        {
            SceneManager.LoadScene("Level2");
        }
        else
        {
            System.Random rnd = new System.Random();
            SceneManager.LoadScene(levels[rnd.Next(0, 6)]);
        }
        PlayerAfterChange = GameObject.FindWithTag("Player");
        PlayerAfterChange.GetComponent<PlayerController>().ImportStats(instance);
        PlayerAfterChange.transform.position = new Vector3(0, 0, 0);
        levelsPassed++;
        levelText.text = "Levels Passed: " + levelsPassed;
    }
}
