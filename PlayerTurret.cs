using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerTurret : MonoBehaviour {

    public GameObject bullet;
    public GameObject gunEnd;
    GameObject bulletRoom;
    float speed = 1000.0f;
    // Use this for initialization
    void Start()
    {
        if (bullet == null)
        {
           bullet = GameObject.FindGameObjectWithTag("bullet");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gunEnd != null)
        {
            if (bulletRoom == null)
            {
                GameObject clone;
                clone = Instantiate(bullet, gunEnd.transform.position, bullet.transform.rotation) as GameObject;
                clone.tag = "bulletPlayer";
                clone.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
                bulletRoom = clone;
                
                
            }
        }

    }

}
