using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{

    private GameObject[] enemies;
    private GameObject player;
    private GameObject manager;
    public string next;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        manager = GameObject.FindWithTag("GameController");
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
         if (coll.CompareTag("Player"))
         {
             enemies = GameObject.FindGameObjectsWithTag("Enemy");
             if (enemies.Length == 0) {
                manager.GetComponent<GameManager>().Instance().LoadNextRoom();
            }
         }
    }

}
