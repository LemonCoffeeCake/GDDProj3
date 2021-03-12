using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{

    private GameObject[] enemies;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
         if (coll.CompareTag("Player"))
         {
             enemies = GameObject.FindGameObjectsWithTag("Enemy");
             if (enemies.Length == 0) {
                MoveRoom();
            }
         }
    }

    private void MoveRoom() {
        SceneManager.LoadScene("Level1");
    }
}
