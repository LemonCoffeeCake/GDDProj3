using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private int numRoomsComplete = 0;
    private GameObject PlayerBeforeChange;
    private GameObject InventBefore;
    private GameObject PlayerAfterChange;
    private GameObject InventAfter;
    public int currHealth;
    public Stat currDamage;
    public Stat currSpeed;
    public List<Item> currItems;
    public int currGold;

    private GameManager() {
    }

    void Awake(){
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != this){
            Destroy(gameObject);
        }
    }

    public GameManager Instance() {return instance;}

    public void LoadNextRoom(){
        PlayerBeforeChange = GameObject.FindWithTag("Player");
        InventBefore = GameObject.FindWithTag("Inventory");
        PlayerBeforeChange.GetComponent<PlayerController>().ExportStats(this);
        InventBefore.GetComponent<Inventory>().Instance().ExportStats(this);
        numRoomsComplete += 1;
        if(numRoomsComplete % 6 == 0) {
            SceneManager.LoadScene("Level2");
        } else {
            SceneManager.LoadScene("Level1");
        }
        PlayerAfterChange = GameObject.FindWithTag("Player");
        InventAfter = GameObject.FindWithTag("Inventory");
        PlayerAfterChange.GetComponent<PlayerController>().ImportStats(this);
        InventAfter.GetComponent<Inventory>().Instance().ImportStats(this);
    }
}
