using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
public class PathfindNode : MonoBehaviour {

	Transform target;
	GameObject[] otherPN;
	public List<PathfindNode> connections;
	public List<PathfindNode> forceConnection;
	public float nodeDistance;
	RaycastHit hit;
	int mask = 1<<8;

	
	public int maxConnections = 8;
	public Color connectionColor = Color.green;
	public bool forceLine = false;
	public bool coverNode = false;


	// Use this for initialization
	void Start () {
		if (nodeDistance <= 0) 
		{
			nodeDistance = 20;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnDrawGizmos()
	{
		if (coverNode == true) 
		{
			Gizmos.color = Color.cyan;
		} 
		else 
		{
			Gizmos.color = Color.yellow;
		}
		Gizmos.DrawSphere (transform.position, 0.5f);
		connectionColor = Color.green;
		otherPN = GameObject.FindGameObjectsWithTag ("PathNode");
		foreach (GameObject target in otherPN) 
		{
			if(target != null && target != this.transform)
			{
				if(connections.Contains(target.GetComponent<PathfindNode>()))
				{
					Gizmos.color = connectionColor;
					Gizmos.DrawLine (new Vector3(transform.position.x, transform.position.y, transform.position.z), 
					                 new Vector3(target.transform.position.x, target.transform.position.y + 1.0f, target.transform.position.z));
				}
				if(forceConnection.Contains(target.GetComponent<PathfindNode>()) && forceLine == true)
				{
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine (new Vector3(transform.position.x, transform.position.y, transform.position.z), 
					                 new Vector3(target.transform.position.x, target.transform.position.y + 1.0f, target.transform.position.z));
				}
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (coverNode == true) 
		{
			Gizmos.color = Color.magenta;
		} 
		else 
		{
			Gizmos.color = Color.red;
		}
		Gizmos.DrawSphere (transform.position, 0.75f);
		connectionColor = Color.black;
		otherPN = GameObject.FindGameObjectsWithTag ("PathNode");
		foreach (GameObject target in otherPN) 
		{
			if(target != null && target != this.transform)
			{
				if(connections.Contains(target.GetComponent<PathfindNode>()))
				{
					Gizmos.color = connectionColor;
					Gizmos.DrawLine (new Vector3(transform.position.x, transform.position.y, transform.position.z), 
					                 new Vector3(target.transform.position.x, target.transform.position.y + 1.0f, target.transform.position.z));
				}
				if(forceConnection.Contains(target.GetComponent<PathfindNode>()) && forceLine == true)
				{
					Gizmos.color = connectionColor;
					Gizmos.DrawLine (new Vector3(transform.position.x, transform.position.y, transform.position.z), 
					                 new Vector3(target.transform.position.x, target.transform.position.y + 1.0f, target.transform.position.z));
				}
			}
		}
	}

	public void BuildPaths()
	{
		coverNode = false;
		connections = new List<PathfindNode> ();
		otherPN = GameObject.FindGameObjectsWithTag ("PathNode");
		foreach (GameObject target in otherPN) 
		{
			if (target != null && target.transform != this.transform)
			{
				if(connections.Count < maxConnections)
				{
					if(!Physics.Linecast(transform.position, target.transform.position, mask) && Vector3.Distance(transform.position, target.transform.position) <= nodeDistance)
					{
						connections.Add(target.GetComponent<PathfindNode>());
                        #if UNITY_EDITOR
						EditorUtility.SetDirty (this);
						#endif
					}
                    if (Physics.Linecast(transform.position, target.transform.position) && forceLine == true && Vector3.Distance(transform.position, target.transform.position) <= nodeDistance)
					{
						forceConnection.Add(target.GetComponent<PathfindNode>());
                        #if UNITY_EDITOR
						EditorUtility.SetDirty(this);
						#endif
					}
					if (Physics.Linecast(transform.position, target.transform.position, mask) && forceLine == false && Vector3.Distance(transform.position, target.transform.position) <= nodeDistance)
					{
						if(target.transform.position.x == transform.position.x || target.transform.position.z == transform.position.z)
						{
							coverNode = true;
							#if UNITY_EDITOR
							EditorUtility.SetDirty(this);
							#endif
						}
					}
				}
			}
		}

	}

	void CoverNode ()
	{
		coverNode = false;
		connections = new List<PathfindNode> ();
		otherPN = GameObject.FindGameObjectsWithTag ("PathNode");
	}

	public void ClearPaths()
	{
		connections.Clear ();
		forceConnection.Clear ();
		forceLine = false;
		#if UNITY_EDITOR
		EditorUtility.SetDirty (this);
		#endif
	}
	
}
