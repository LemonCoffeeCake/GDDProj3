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
    private string[] levels1 = new string[6] { "Level1", "Level3", "Level4", "Level5", "Level6", "Level7" };
    private string[] levels2 = new string[6] { "Level8", "Level9", "Level10", "Level11", "Level12", "Level13" };
    public GameObject PauseMenu;
    public bool isPaused;
    public TextMeshProUGUI levelText;
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
        if (numRoomsComplete == 0)
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
        numRoomsComplete = 0;
        levelText.text = "";

    }


    public GameManager Instance() {return instance;}

    public void LoadNextRoom()
    {
        PlayerBeforeChange = GameObject.FindWithTag("Player");
        PlayerBeforeChange.GetComponent<PlayerController>().ExportStats(instance);
        numRoomsComplete += 1;
        if (numRoomsComplete == 7 || numRoomsComplete == 15) {
            SceneManager.LoadScene("Level2");
        } else if (numRoomsComplete == 8) {
            SceneManager.LoadScene("Level14");
        } else if (numRoomsComplete == 16) {
            SceneManager.LoadScene("Level15");
        } else if (numRoomsComplete == 17) {
            SceneManager.LoadScene("Win");
        } else if (numRoomsComplete < 7) {
            System.Random rnd = new System.Random();
            SceneManager.LoadScene(levels1[rnd.Next(0, 6)]);
        } else {
            System.Random rnd = new System.Random();
            SceneManager.LoadScene(levels2[rnd.Next(0, 6)]);
        }
        PlayerAfterChange = GameObject.FindWithTag("Player");
        PlayerAfterChange.GetComponent<PlayerController>().ImportStats(instance);
        PlayerAfterChange.transform.position = new Vector3(0, 0, 0);
        levelText.text = "Levels Passed: " + numRoomsComplete;
    }
}
