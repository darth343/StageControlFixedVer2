using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flocking : MonoBehaviour
{
    public float NeighborRadius = 200f;
    public float SeperationOffset = 800f;
    public List<GameObject> FlockingList = new List<GameObject>();
    public GameObject leader;
    public bool isleader = true;

    void Update()
    {
        updatelist();

        UpdateLeadership();

        DoFlock();

        if (isleader == false)
        {

            // Cap Maximum speed of followers
            gameObject.GetComponent<VMovement>().Velocity += cohesion(leader);//+ alignment(Force);
            float MAX = leader.GetComponent<VMovement>().speed * 2;
            gameObject.GetComponent<VMovement>().Velocity.x = Mathf.Clamp(gameObject.GetComponent<VMovement>().Velocity.x, -MAX, MAX);
            gameObject.GetComponent<VMovement>().Velocity.z = Mathf.Clamp(gameObject.GetComponent<VMovement>().Velocity.z, -MAX, MAX);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, 1000, 0), Color.blue);
        }
    }

    void UpdateLeadership()
    {
        if (!isleader)
        {
            if (leader.gameObject == null)
            {
                isleader = true;
            }
        }

        if (isleader)
        {
            for (int i = 0; i < FlockingList.Count; ++i)
            {
                if (FlockingList[i].GetComponent<Flocking>().isleader == true)
                {
                    FlockingList[i].GetComponent<Flocking>().isleader = false;
                    FlockingList[i].GetComponent<Flocking>().leader = gameObject;
                    break;
                }
            }
        }
    }

    void DoFlock()
    {
        Vector3 Force = new Vector3(0, 0, 0);
        for (int i = 0; i < FlockingList.Count; ++i)
        {
            if (FlockingList[i].GetComponent<Flocking>().isleader)
                continue;

            Vector3 Repel = new Vector3(0, 0, 0);
            Vector3 displacement = (FlockingList[i].transform.position - gameObject.transform.position);
            if (displacement.magnitude > 40)
            {
                Force.x = FlockingList[i].GetComponent<VMovement>().Velocity.x;
                Force.y = FlockingList[i].GetComponent<VMovement>().Velocity.y;
            }

            if (displacement.magnitude < 10)
            {
                //Debug.Log("repeling");
                Repel = displacement;
                FlockingList[i].GetComponent<VMovement>().Velocity += FlockingList[i].GetComponent<Flocking>().seperation(Repel);
            }
        }
    }

    void updatelist()
    {
        FlockingList.Clear();

        for (int i = 0; i < SceneData.sceneData.EntityList.transform.childCount; ++i)
        {
            if (gameObject == SceneData.sceneData.EntityList.transform.GetChild(i).gameObject)
                continue;
            if ((SceneData.sceneData.EntityList.transform.GetChild(i).position - transform.position).sqrMagnitude < NeighborRadius * NeighborRadius)
            {
                FlockingList.Add(SceneData.sceneData.EntityList.transform.GetChild(i).gameObject);
            }
        }
    }

    Vector3 seperation(Vector3 repel)
    {
        float temp = 0;

        if (repel != new Vector3(0, 0, 0))
        {
            temp = (repel.magnitude / SeperationOffset);
            repel = repel.normalized;
            repel = new Vector3(repel.x / temp, repel.y / temp, repel.z / temp);

            return repel.normalized;

        }
        return new Vector3(0, 0, 0);
    }

    Vector3 alignment(Vector3 force)
    {
        if (force != new Vector3(0, 0, 0))
        {
            return force.normalized;
        }
        return new Vector3(0, 0, 0);
    }

    Vector3 cohesion(GameObject leader)
    {
        //Vector3 temp = (leader.GetComponent<VMovement>().Velocity * -1f).normalized *0.2f;
        Vector3 CenterOfMass = leader.transform.position;
        return (CenterOfMass - gameObject.transform.position).normalized;
    }

}

//public class Flocking : MonoBehaviour
//{
//    float NeighborRadius = 50;
//    public List<GameObject> neighbours = new List<GameObject>();
//    public bool isLeader = false;
//    public float speed = 1f;

//    Vector3 value = new Vector3();

//    List<GameObject> UpdateNeighbours()
//    {
//        neighbours.Clear();
//        // iterate through entitylist
//        for (int i = 0; i < SceneData.sceneData.EntityList.transform.childCount; ++i)
//        {
//            if (gameObject == SceneData.sceneData.EntityList.transform.GetChild(i).gameObject)
//                continue;

//            if ((SceneData.sceneData.EntityList.transform.GetChild(i).position - gameObject.transform.position).sqrMagnitude < NeighborRadius * NeighborRadius)
//            {
//                neighbours.Add(SceneData.sceneData.EntityList.transform.GetChild(i).gameObject);
//            }
//        }

//        return neighbours;
//    }

//    public Vector3 computeAlignment()
//    {
//        value.Set(0, 0, 0);
//        foreach(GameObject neighbour in neighbours)
//        {
//            value.x += neighbour.GetComponent<VMovement>().Velocity.x;
//            value.y += neighbour.GetComponent<VMovement>().Velocity.y;
//            value.z += neighbour.GetComponent<VMovement>().Velocity.z;
//        }

//        value.x /= neighbours.Count;
//        value.y /= neighbours.Count;
//        value.z /= neighbours.Count;

//        return value.normalized;
//    }
//    public Vector3 computeSeperation()
//    {
//        value.Set(0, 0, 0);
//        foreach (GameObject neighbour in neighbours)
//        {
//            value.x += neighbour.transform.position.x - transform.position.x;
//            value.y += neighbour.transform.position.y - transform.position.y;
//            value.z += neighbour.transform.position.z - transform.position.z;
//        }

//        value.x /= neighbours.Count;
//        value.y /= neighbours.Count;
//        value.z /= neighbours.Count;
//        value.x *= -1;
//        value.y *= -1;
//        value.z *= -1;

//        return value.normalized;
//    }
//    public Vector3 computeCohesion()
//    {
//        value.Set(0, 0, 0);
//        foreach (GameObject neighbour in neighbours)
//        {
//            value.x += neighbour.transform.position.x;
//            value.y += neighbour.transform.position.y;
//            value.z += neighbour.transform.position.z;
//        }

//        value.x /= neighbours.Count;
//        value.y /= neighbours.Count;
//        value.z /= neighbours.Count;

//        value = new Vector3(value.x - transform.position.x, value.y - transform.position.y, value.z - transform.position.z);

//        return value.normalized;
//    }

//    void Update()
//    {
//        UpdateNeighbours();

//        bool leaderNearby = false;
//        int leaderindex = 0;
//        for (int i = 0; i < neighbours.Count; ++i)
//        {
//            if (neighbours[i].GetComponent<Flocking>().isLeader == true)
//            {
//                leaderNearby = true;
//                leaderindex = i;
//                break;
//            }
//            if(isLeader)
//            Debug.DrawLine(transform.position, neighbours[i].transform.position, Color.yellow);
//        }

//        if (leaderNearby == false)
//        {
//            isLeader = true;
//        }
//        else if(leaderNearby && isLeader)
//        {
//            //isLeader = true;
//            neighbours[leaderindex].GetComponent<Flocking>().isLeader = false;
//        }

//        if (isLeader == false)
//        {
//            Vector3 alignment = GetComponent<Flocking>().computeAlignment();
//            Vector3 seperation = GetComponent<Flocking>().computeSeperation();
//            //Vector3 seperation = new Vector3();// GetComponent<Flocking>().computeSeperation();
//            Vector3 cohesion = GetComponent<Flocking>().computeCohesion();
//            //Vector3 cohesion = new Vector3();// GetComponent<Flocking>().computeCohesion();

//            GetComponent<VMovement>().Velocity.x += alignment.x * SceneData.sceneData.AlignmentWeight + cohesion.x * SceneData.sceneData.CohesionWeight + seperation.x * SceneData.sceneData.SeperationWeight;
//            GetComponent<VMovement>().Velocity.y += alignment.y * SceneData.sceneData.AlignmentWeight + cohesion.y * SceneData.sceneData.CohesionWeight + seperation.y * SceneData.sceneData.SeperationWeight;
//            GetComponent<VMovement>().Velocity.z += alignment.z * SceneData.sceneData.AlignmentWeight + cohesion.z * SceneData.sceneData.CohesionWeight + seperation.z * SceneData.sceneData.SeperationWeight;
//        }

//        GetComponent<VMovement>().Velocity.Normalize();
//        Debug.DrawLine(transform.position + new Vector3(0, 5, 0), transform.position + GetComponent<VMovement>().Velocity * 10 + new Vector3(0, 5, 0), Color.blue);
//        transform.position += GetComponent<VMovement>().Velocity * speed;
//    }
//}
