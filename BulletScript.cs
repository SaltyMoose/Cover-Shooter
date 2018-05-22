using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
