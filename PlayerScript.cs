using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {
    public float speed = 5.0f;
	float rotateSpeed = 100.0f;
	Vector3 a;
    public bool fired;
    public bool dead;
    enum LifeState
    {
        Alive,
        Dead
    }
    LifeState currentState = LifeState.Alive;
	// Update is called once per frame
	void Start()
	{
        fired = false;
        dead = false;
	}

	void FixedUpdate () {
        ColorChange();
        switch(currentState)
        {
            case LifeState.Alive:
                float moveSideways = Input.GetAxis("Vertical") * speed;
                float moveForwardBack = Input.GetAxis("Horizontal") * speed;
		        float rotatePlayer = Input.GetAxis ("Rotate") * rotateSpeed;
                moveSideways *= Time.deltaTime;
                moveForwardBack *= Time.deltaTime;
		        rotatePlayer *= Time.deltaTime;

		        transform.Translate (moveForwardBack, 0, moveSideways);
			
		        transform.Rotate (0, rotatePlayer, 0);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    fired = true;
                }
                else
                {
                    fired = false;
                }
                break;
            case LifeState.Dead:
                dead = true;
                break;
        }
	}

    void ColorChange()
    {
        Color color = Color.white;
        Renderer rend = GetComponent<Renderer>();
        switch (currentState)
        {
            case LifeState.Alive:
                break;
            case LifeState.Dead:
                color = Color.black;
                break;
            default:
                break;

        }
        rend.material.SetColor("_Color", color);
    }
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "bulletEnemy")
		{
            currentState = LifeState.Dead;
		}
	}

}
