using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LongshoremanX : MonoBehaviour {

	GameObject[]pathNodes;
	GameObject[]coverNodes;
	List<GameObject> coverNodeList;
	
	public float speed;
	float resumeSpeed;
	public PathfindNode currentNode;
	PathfindNode previousNode;
	PathfindNode avoidNode;
	PathfindNode tempNode;
	public GameObject leftFoward;
	public GameObject rightFoward;

	GameObject bulletRoom;
	float bulletSpeed = 1000.0f;
	
	Vector3 a;
	Vector3 target;
    public bool dead;
	
	int layerMask = 1<<8;
	float shortDist;
	RaycastHit hit;
	
	
	int randomNode;
	int randomMax;
	
	GameObject playerTarget;
	public float detectDistance;
	float timeStamp;
	
	enum ShieldState
	{
		MoveRandom,
		MoveToPlayer,
		Dead
	}
	ShieldState currentState = ShieldState.MoveRandom;
	// Use this for initialization
	// Use this for initialization
	void Start () {
		playerTarget = GameObject.FindGameObjectWithTag("Player");
		
		if (speed <= 0)
		{
			speed = 2.0f;
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

		Vector3 fwd = transform.TransformDirection (Vector3.forward);
		Vector3 leftCast = transform.TransformDirection(Vector3.left);
		Vector3 rightCast = transform.TransformDirection(Vector3.right);
        ColorChange();
		switch (currentState) {
		case ShieldState.MoveRandom:
			a = target - transform.position;
			if (transform.position == currentNode.transform.position) {
				RandomMove ();
			}
			if (playerTarget.GetComponent<PlayerScript> ().fired == true) {
				currentState = ShieldState.MoveToPlayer;
				Debug.Log (currentState);
			}
			if ((Physics.Raycast (leftFoward.transform.position, fwd, out hit, detectDistance) || Physics.Raycast (rightFoward.transform.position, fwd, out hit, detectDistance)) && hit.transform.CompareTag ("Player")) {
				currentState = ShieldState.MoveToPlayer;
				Debug.Log (currentState);
			}
			transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, a, speed * Time.deltaTime, 0.0f));
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			break;
		case ShieldState.MoveToPlayer:
			a = playerTarget.transform.position;
			if(transform.position == currentNode.transform.position)
			{
				TargetMove();
			}
			transform.LookAt(playerTarget.transform.position);
			transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			break;
		case ShieldState.Dead:
			{
				speed = 0;
                dead = true;
			}
			break;
		default:
			break;
		}
		transform.position = Vector3.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);
	
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
			if (Vector3.Distance(currentNode.GetComponent<PathfindNode>().connections[i].transform.position, playerTarget.transform.position) < shortDist)
			{
				if (avoidNode != currentNode.GetComponent<PathfindNode>().connections[i])
				{
					if (previousNode != currentNode.GetComponent<PathfindNode>().connections[i] || currentNode.GetComponent<PathfindNode>().connections.Count != 1)
					{
						tempNode = currentNode.GetComponent<PathfindNode>().connections[i];
						shortDist = Vector3.Distance(currentNode.GetComponent<PathfindNode>().connections[i].transform.position, playerTarget.transform.position);
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
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "bulletPlayer") 
		{
			currentState = ShieldState.Dead;
		}
	}
    void ColorChange()
    {
        Color color = new Color(0.5f,0f,1.0f);
        Renderer rend = GetComponent<Renderer>();
        switch (currentState)
        {
            case ShieldState.MoveRandom:
                break;
            case ShieldState.MoveToPlayer:
                break;
            case ShieldState.Dead:
                color = Color.black;
                break;
            default:
                break;

        }
        rend.material.SetColor("_Color", color);
    }
}
