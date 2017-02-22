using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
	public int posX;
    public int posY;

	public int GetNodeID()
    {
        return posX + posY * 5000;
    }

    public Node parent = null;

    // Distance from Start Node
    public float G;
    // Distance from End Node
    public float H;
	public float getF(){ return G + H;}
    public float Distance(Node End)
	{
		float x = Mathf.Abs(posX - End.posX);
        float y = Mathf.Abs(posY - End.posY);
		return (x + y) * 10;
	}
}

public class Pathfinder : MonoBehaviour 
{
    List<Node> OpenList = new List<Node>();
    List<Node> VisitedList = new List<Node>();
    bool InitializedStartandGoal = false;
    Node StartNode = new Node();
    Node EndNode = new Node();

    public bool PathFound = true;
    public List<Vector3> PathToEnd = new List<Vector3>();
    //public Vector3 StartPos = new Vector3();
    public Vector3 EndPos = new Vector3();

	// Use this for initialization
	void Start ()
    {
	    
	}

    void OnValidate()
    {
    }

    public void Reset()
    {
        PathFound = false;
        InitializedStartandGoal = false;

        foreach (Node node in OpenList)
        {
            Grid tempGrid = SceneData.sceneData.gridmesh.gridmesh[node.posX, node.posY].GetComponent<Grid>();
            tempGrid.GetComponent<Grid>().ChangeState(Grid.GRID_STATE.AVAILABLE);
        }

        foreach (Node node in VisitedList)
        {
            Grid tempGrid = SceneData.sceneData.gridmesh.gridmesh[node.posX, node.posY].GetComponent<Grid>();
            tempGrid.GetComponent<Grid>().ChangeState(Grid.GRID_STATE.AVAILABLE);
        }
    }

    public void FindPath(Vector3 endposition)
    {
        for (int i = 0; i < 10; ++i)
        {
            if (!InitializedStartandGoal)
            {
                OpenList.Clear();
                VisitedList.Clear();
                PathToEnd.Clear();
                PathFound = false;
            }

            if (!InitializedStartandGoal && !PathFound)
            {
                Grid StartGrid = SceneData.sceneData.gridmesh.GetGridAtPosition(transform.position).GetComponent<Grid>();
                Grid EndGrid = SceneData.sceneData.gridmesh.GetGridAtPosition(endposition).GetComponent<Grid>();
                StartNode.posX = (int)SceneData.sceneData.gridmesh.GetGridPosition(StartGrid).x;
                StartNode.posY = (int)SceneData.sceneData.gridmesh.GetGridPosition(StartGrid).y;
                StartNode.G = 0;
                StartNode.H = StartNode.Distance(EndNode);
                EndNode.posX = (int)SceneData.sceneData.gridmesh.GetGridPosition(EndGrid).x;
                EndNode.posY = (int)SceneData.sceneData.gridmesh.GetGridPosition(EndGrid).y;;
                EndNode.G = EndNode.Distance(StartNode);
                EndNode.H = 0;
                OpenList.Add(StartNode);
                InitializedStartandGoal = true;
            }
            if (InitializedStartandGoal && !PathFound)
            {
                ContinueSearch();
            }
        }
    }

    void ContinueSearch()
    {
        if (OpenList.Count == 0)
        {
            InitializedStartandGoal = false;
            PathFound = true;
            return;
        }

        Node currentNode = getNextNodeFromOpenList();

        if (currentNode != null)
        {
            if (currentNode.GetNodeID() == EndNode.GetNodeID())
            {
                Debug.Log("Found");
                PathFound = true;
                Node getPath;
                for (getPath = currentNode; getPath != null; getPath = getPath.parent)
                {
                    Vector3 Waypoint = new Vector3(getPath.posX * SceneData.sceneData.gridmesh.GridSizeX, 1, getPath.posY * SceneData.sceneData.gridmesh.GridSizeZ);
                    Waypoint.y = SceneData.sceneData.ground.SampleHeight(Waypoint);
                    //SceneData.sceneData.gridmesh.gridmesh[getPath.posX, getPath.posY].GetComponent<Grid>().ChangeState(Grid.GRID_STATE.ISPATH);
                    PathToEnd.Add(Waypoint);
                }
            }
            else 
            {
                OpenNode(currentNode.posX + 1, currentNode.posY, 10, currentNode);
                OpenNode(currentNode.posX - 1, currentNode.posY, 10, currentNode);
                OpenNode(currentNode.posX, currentNode.posY + 1, 10, currentNode);
                OpenNode(currentNode.posX, currentNode.posY - 1, 10, currentNode);

                OpenNode(currentNode.posX - 1, currentNode.posY - 1, 14, currentNode);
                OpenNode(currentNode.posX - 1, currentNode.posY + 1, 14, currentNode);
                OpenNode(currentNode.posX + 1, currentNode.posY + 1, 14, currentNode);
                OpenNode(currentNode.posX + 1, currentNode.posY - 1, 14, currentNode);
            }
        }
        else 
        {
            Debug.Log("PATHFINDER BUG BUG BUG");
        }

    }

    void OpenNode(int posX, int posY, float newCost, Node parent)
    {
        if (posX < 0 || posX > SceneData.sceneData.gridmesh.m_rows - 1|| posY < 0 || posY > SceneData.sceneData.gridmesh.m_columns - 1)
        {
            //Debug.Log("X:" + posX + "Y:" + posY);
            return;
        }

        if (SceneData.sceneData.gridmesh.gridmesh[posX, posY])
        {
            //Debug.Log("Index X:" + posX + "Index Z: " + posY);
            if (SceneData.sceneData.gridmesh.gridmesh[posX, posY].GetComponent<Grid>().state == Grid.GRID_STATE.UNAVAILABLE)
            {
                return;
            }
            else 
            {
                //Debug.Log();
            }
            //Grid not walkable
        }
        Node newNode = new Node();
        newNode.posX = posX;
        newNode.posY = posY;
        foreach (Node node in VisitedList)
        {
            if (node.GetNodeID() == newNode.GetNodeID())
            {
                // Dont retrace back
                return;
            }
        }
        newNode.parent = parent;
        newNode.G = newCost;
        newNode.H = newNode.Distance(EndNode);

        foreach(Node node in OpenList)
        {
            // if new adjacent node is already in the openlist
            // check to see if current processing path to adjacent node is shorter than prev path
            if (newNode.GetNodeID() == node.GetNodeID())
            {
                float newF = newNode.G + newCost + newNode.H;
                if (node.getF() > newF)
                {
                    node.G = newNode.G + newCost;
                    node.parent = parent;
                }
                else 
                {
                    return;
                }
            }
        }
        //Grid tempGrid = SceneData.sceneData.gridmesh.gridmesh[posX, posY].GetComponent<Grid>();
        //tempGrid.GetComponent<Grid>().ChangeState(Grid.GRID_STATE.INOPENLIST);
        OpenList.Add(newNode);
    }

    Node getNextNodeFromOpenList()
    {
        float lowestF = 9999;
        Node nextnode = null;

        foreach(Node node in OpenList)
        {
            if (node.getF() < lowestF)
            {
                lowestF = node.getF();
                nextnode = node;
            }
            else if (node.getF() == lowestF)
            {
                nextnode = node;
            }
        }

        if (nextnode != null)
        {
            OpenList.Remove(nextnode);
            //Grid tempGrid = SceneData.sceneData.gridmesh.gridmesh[nextnode.posX, nextnode.posY].GetComponent<Grid>();
            //tempGrid.GetComponent<Grid>().ChangeState(Grid.GRID_STATE.INCLOSELIST);
            VisitedList.Add(nextnode);
        }

        return nextnode;

    }

	// Update is called once per frame
	void Update ()
    {
	}
}
