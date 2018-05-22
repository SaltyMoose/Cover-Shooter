using UnityEngine;
using System.Collections;

public class SceneScript : MonoBehaviour {
    public GameObject player;
    public GameObject[] coverShooters;
    public GameObject shieldMan;
    public GameObject gameOver;
    public GameObject gameWon;

    bool coverShootersDead;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        coverShooters = GameObject.FindGameObjectsWithTag("Cower");
        shieldMan = GameObject.FindGameObjectWithTag("Shieldman");
        gameOver.SetActive(false);
        gameWon.SetActive(false);

        coverShootersDead = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (player.GetComponent<PlayerScript>().dead == true)
        {
            gameOver.SetActive(true);
        }
        if (shieldMan.GetComponent<LongshoremanX>().dead == true)
        {
            coverShootersDead = true;
            foreach (GameObject coverShooter in coverShooters)
            {
                if (coverShooter.GetComponent<CoverScript>().dead == false)
                {
                    coverShootersDead = false;
                }
            }
            if (coverShootersDead == true)
            {
                gameWon.SetActive(true);
            }
        }
	}
}
