using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveRandom : MonoBehaviour {

	List<PathfindNode> pathfindNodeList;
    GameObject[] pathfindNodes;
    GameObject[] otherSeekers;
	public float speed;
    float resumeSpeed;
	public PathfindNode currentNode;
	PathfindNode previousNode;
	PathfindNode avoidNode;
	PathfindNode tempNode;
	PathfindNode alertNode;
    PathfindNode seekNode;

    public bool alertCall;
    public bool capturePlayer;

    int layerMask = 1<<9;
    Ray seekingRay;
    float shortDist;
	RaycastHit hit;
	int randomNode;
	int randomMax;
    GameObject playerTarget;
    float alertTime;

	enum State
	{
		MoveToNode,
		MoveToPlayer,
		MoveToLastAlert,
		Stop
	}
	State currentState = State.MoveToNode;

	// Use this for initialization
	void Start () {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        capturePlayer = false;

        if (speed <= 0)
        {
            speed = 10.0f;
        }
        alertTime = 0.0f;
        otherSeekers = GameObject.FindGameObjectsWithTag("Seeker");
		float start = Mathf.Infinity;
        alertCall = false;


		pathfindNodes = GameObject.FindGameObjectsWithTag("PathNode");
		//Move seeker to nearest node
		if (currentNode == null) 
		{
			foreach(GameObject newNode in pathfindNodes)
			{
				float distance = Vector3.Distance(newNode.transform.position,transform.position); 
				if(distance < start)
				{
					start = distance;
					currentNode = newNode.GetComponent<PathfindNode>();
				}
			}
		}
		previousNode = currentNode;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 fwd = transform.TransformDirection (Vector3.forward);
		switch (currentState)
		{
		case State.MoveToNode:
			if (transform.position == currentNode.transform.position)
			{
				RandomMove();
			}
            
            foreach (GameObject seeker in otherSeekers)
            {
                if (seeker.GetComponent<MoveRandom>().alertCall == true)
                {
                    alertNode = seeker.GetComponent<MoveRandom>().seekNode;
                }
            }
            if (Physics.Linecast(transform.position, fwd, out hit) && hit.transform.CompareTag("Player"))
            {
                alertCall = true;
                seekNode = previousNode;
                currentState = State.MoveToPlayer;
                Debug.Log(currentState);
            }
            else if (alertNode != null)
            {
                currentState = State.MoveToLastAlert;
                alertTime = 0;
                Debug.Log(currentState);
            }
			break;

		
		case State.Stop:

            if (playerTarget != null && (Physics.Linecast(transform.position, playerTarget.transform.position, out hit)
                && hit.distance > 10.0f && hit.transform.CompareTag("Player")))
            {
                speed = resumeSpeed;
                alertCall = true;
                capturePlayer = false;
                seekNode = previousNode;
                currentState = State.MoveToPlayer;
                Debug.Log(currentState);
            }
            else
            {
                speed = 0;
            }
			break;

		default:
			break;
		}
		Debug.DrawRay (transform.position, Vector3.forward, Color.white);
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
		Debug.Log (currentNode);
	}

  

	void OnDrawGizmos()
	{
		//Show where the AI is going
		if (currentNode) 
		{
            if (currentState == State.MoveToNode)
            {
                Gizmos.color = Color.red;
            }
			Gizmos.DrawSphere (currentNode.transform.position, 2.0f);
		}
		//Show prvious Node
		if (previousNode)
		{
			Gizmos.color = Color.grey;
			Gizmos.DrawSphere (previousNode.transform.position, 2.0f);
		}
	}

}
