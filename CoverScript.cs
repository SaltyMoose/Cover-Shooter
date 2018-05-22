using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverScript : MonoBehaviour {

	GameObject[]pathNodes;
	GameObject[]coverNodes;
	public GameObject partnerAI;
	List<GameObject> coverNodeList;

	public float speed;
	float resumeSpeed;
	public PathfindNode currentNode;
	public PathfindNode coverSpot;
	PathfindNode previousNode;
	PathfindNode avoidNode;
	PathfindNode tempNode;
    public GameObject leftFoward;
    public GameObject rightFoward;
    public GameObject bullet;
	public GameObject gunPoint;
	bool playerInSight;

	Vector3 a;
	Vector3 target;

	int layerMask = 1<<8;
	float shortDist;
	RaycastHit hit;

    float waitTime = 0;
    float lookTime = 0;

	int randomNode;
	int randomMax;

	GameObject bulletRoom;
	float bulletSpeed = 1000.0f;

	GameObject playerTarget;
	public float detectDistance;
	float timeStamp;
    public bool dead;

	enum CoverState
	{
		MoveRandom,
		MoveToCover,
		WaitCover,
		CheckInCover,
		Dead
	}
	CoverState currentState = CoverState.MoveRandom;
	// Use this for initialization
	void Start () {
		playerTarget = GameObject.FindGameObjectWithTag("Player");
        
		if (bullet == null) 
		{
			bullet = GameObject.FindGameObjectWithTag("bullet");
		}

		if (speed <= 0)
		{
			speed = 5.0f;
		}
		float start = Mathf.Infinity;
		coverNodeList = new List<GameObject>();
		
		pathNodes = GameObject.FindGameObjectsWithTag("PathNode");

		foreach (GameObject pathNode in pathNodes) {
			if(pathNode.GetComponent<PathfindNode>().coverNode == true)
			{
				coverNodeList.Add(pathNode);
			}
		}

		coverNodes = coverNodeList.ToArray ();
		//Move seeker to nearest node
		if (currentNode == null) 
		{
			foreach(GameObject newNode in pathNodes)
			{
				float distance = Vector3.Distance(newNode.transform.position,transform.position); 
				if(distance < start)
				{
					start = distance;
					currentNode = newNode.GetComponent<PathfindNode>();
					target = currentNode.GetComponent<PathfindNode>().transform.position;
				}
			}
		}
		previousNode = currentNode;
        dead = false;
	}
	
	// Update is called once per frame
	void Update () {
        ColorChange();
		a = target - transform.position;
		Vector3 fwd = transform.TransformDirection (Vector3.forward);
        Vector3 leftCast = transform.TransformDirection(Vector3.left);
        Vector3 rightCast = transform.TransformDirection(Vector3.right);

		switch (currentState) {
		case CoverState.MoveRandom:
			if (transform.position == currentNode.transform.position)
			{
				RandomMove();
			}
            if (playerTarget.GetComponent<PlayerScript>().fired == true)
            {
                currentState = CoverState.MoveToCover;
                NewCover();
                Debug.Log(currentState);
            }
            if ((Physics.Raycast(leftFoward.transform.position, fwd, out hit, detectDistance) || Physics.Raycast(rightFoward.transform.position, fwd, out hit, detectDistance)) && hit.transform.CompareTag("Player"))
			{
				currentState = CoverState.MoveToCover;
                NewCover();
				Debug.Log(currentState);
			}
			transform.position = Vector3.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, a, speed * Time.deltaTime, 0.0f));
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			break;
		case CoverState.MoveToCover:
            if (partnerAI.transform.position == coverSpot.transform.position)
            {
                NewCover();
            }
			else if(transform.position == coverSpot.transform.position)
			{
				currentState = CoverState.WaitCover;
                Debug.Log(currentState);
                waitTime = 0;

			}
			else if(transform.position == currentNode.transform.position)
			{
				TargetMove();
			}
			transform.position = Vector3.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, a, speed * Time.deltaTime, 0.0f));
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			break;
		case CoverState.WaitCover:
            waitTime += Time.deltaTime;
			Vector3 hideCover = new Vector3(coverSpot.transform.position.x, 0.0f, coverSpot.transform.position.z);
			FaceCover();
            if (Physics.Raycast(leftFoward.transform.position, leftCast, out hit, detectDistance) && hit.transform.tag == "Player")
			{
				currentState = CoverState.MoveToCover;
                NewCover();
				Debug.Log(currentState);
			}
            else if (Physics.Raycast(rightFoward.transform.position, rightCast, out hit, detectDistance) && hit.transform.tag == "Player")
            {
                currentState = CoverState.MoveToCover;
                NewCover();
                Debug.Log(currentState);
            }
            else if (waitTime >= 5.0f)
            {
                currentState = CoverState.CheckInCover;
                lookTime = 0.0f;
                playerInSight = false;
                Debug.Log(currentState);
            }
			transform.position = Vector3.MoveTowards(transform.position, hideCover, speed * Time.deltaTime);
			break;
		case CoverState.CheckInCover:
            Vector3 lookCover = new Vector3(coverSpot.transform.position.x, 0.5f, coverSpot.transform.position.z);
            lookTime += Time.deltaTime;
            if (Physics.Raycast(leftFoward.transform.position, leftCast, out hit, detectDistance)  && hit.transform.tag == "Player")
            {
                currentState = CoverState.MoveToCover;
                NewCover();
                Debug.Log(currentState);
            }
            else if (Physics.Raycast(rightFoward.transform.position, rightCast, out hit, detectDistance) && hit.transform.tag == "Player")
            {
                currentState = CoverState.MoveToCover;
                NewCover();
                Debug.Log(currentState);
            }
            else if (lookTime >= 10.0f)
            {
				if(playerInSight == true)
				{
	                currentState = CoverState.WaitCover;
	                waitTime = 0.0f;
	                Debug.Log(currentState);
				}
				else
				{
					currentState = CoverState.MoveRandom;
				}
            }

			if ((Physics.Raycast(leftFoward.transform.position, fwd, out hit, detectDistance) || Physics.Raycast(rightFoward.transform.position, fwd, out hit, detectDistance)) && hit.transform.tag == "Player")
			{
				Shoot();
				playerInSight = true;
			}
            transform.position = Vector3.MoveTowards(transform.position, lookCover, speed * Time.deltaTime);
            break;
		case CoverState.Dead:
            dead = true;
			break;
		default:
			break;
		};
        
		Debug.DrawRay (transform.position,fwd,Color.black);
	}

	void RandomMove()
	{
		//float shortDist = Mathf.Infinity;
		
		randomMax = currentNode.GetComponent<PathfindNode> ().connections.Count;
		tempNode = currentNode;
		if (randomMax > 1)
		{
			do
			{
				do
				{
					randomNode = Random.Range(0, randomMax);
				} while (currentNode.GetComponent<PathfindNode>().connections[randomNode] == previousNode);
				tempNode = currentNode.GetComponent<PathfindNode>().connections[randomNode];
			} while (tempNode == currentNode);
		}
		else if(randomMax == 1)
		{
			tempNode = previousNode;
		}
		previousNode = currentNode;
		currentNode = tempNode;
		target = currentNode.GetComponent<PathfindNode>().transform.position;
		Debug.Log (currentNode);
	}
	void TargetMove()
	{
		shortDist = Mathf.Infinity;
		tempNode = currentNode;
		for(int i = 0; i <= currentNode.GetComponent<PathfindNode> ().connections.Count-1; i++)
		{
			//Go to node base on where the player was last at.
			if (Vector3.Distance(currentNode.GetComponent<PathfindNode>().connections[i].transform.position, coverSpot.transform.position) < shortDist)
			{
				if (avoidNode != currentNode.GetComponent<PathfindNode>().connections[i])
				{
					if (previousNode != currentNode.GetComponent<PathfindNode>().connections[i] || currentNode.GetComponent<PathfindNode>().connections.Count != 1)
					{
						tempNode = currentNode.GetComponent<PathfindNode>().connections[i];
						shortDist = Vector3.Distance(currentNode.GetComponent<PathfindNode>().connections[i].transform.position, coverSpot.transform.position);
					}
				}
			}
			
		}
		if (tempNode == previousNode)
		{
			avoidNode = currentNode;
		}
		previousNode = currentNode;
		currentNode = tempNode;
		target = currentNode.GetComponent<PathfindNode>().transform.position;
		Debug.Log(currentNode);
	}
	void FaceCover()
	{
		Vector3 leftCast = transform.TransformDirection (Vector3.left);
		Vector3 rightCast = transform.TransformDirection (Vector3.right);
		Vector3 fwd = transform.TransformDirection (Vector3.forward);
		if ((Physics.Raycast (transform.position, leftCast, out hit, 5.0f) || Physics.Raycast (transform.position, rightCast, out hit, 5.0f)) && hit.transform.tag == "Obstacle") 
		{
			a = -hit.normal;
		} 
        else if ((Physics.Raycast (transform.position, fwd, out hit, 5.0f)) && hit.transform.tag == "Obstacle") 
		{
            a = -hit.normal;
		}
		transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, a, speed * Time.deltaTime, 0.0f));
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
	}
    void NewCover()
    {
        float shortDist = Mathf.Infinity;
        coverSpot = null;
        foreach (GameObject coverNode in coverNodes)
        {
            if (Physics.Linecast(coverNode.transform.position, playerTarget.transform.position, layerMask))
            {
                if (Vector3.Distance(coverNode.transform.position, transform.position) < shortDist)
                {
                    if (partnerAI.GetComponent<CoverScript>().coverSpot != coverNode.GetComponent<PathfindNode>())
                    {
                        coverSpot = coverNode.GetComponent<PathfindNode>();
                        shortDist = Vector3.Distance(coverNode.transform.position, playerTarget.transform.position);
                    }
                }
            }

        }
    }
    void ColorChange()
    {
        Color color = Color.white;
        Renderer rend = GetComponent<Renderer>();
        switch (currentState)
        {
            case CoverState.MoveRandom:
                color = Color.green;
                break;
            case CoverState.MoveToCover:
                color = Color.yellow;
                break;
            case CoverState.WaitCover:
                color = Color.red;
                break;
            case CoverState.CheckInCover:
                color = new Color(1.0f, 0.5f, 0.0f, 1.0f);
                break;
			case CoverState.Dead:
				color = Color.black;
				break;
			default:
				break;

        }
        rend.material.SetColor("_Color", color);
    }

	void Shoot()
	{
		if (bulletRoom == null)
		{
			GameObject clone;
			clone = Instantiate(bullet, gunPoint.transform.position, bullet.transform.rotation) as GameObject;
			clone.tag = "bulletEnemy";
			clone.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
			bulletRoom = clone;
			
			
		}
	}
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "bulletPlayer") 
		{
			currentState = CoverState.Dead;
		}
	}
}
