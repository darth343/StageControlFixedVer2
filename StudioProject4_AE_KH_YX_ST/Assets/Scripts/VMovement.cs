using UnityEngine;
using System.Collections;

public class VMovement : MonoBehaviour
{
    Vector3 Goal;
    public Vector3 Velocity;
    public float speed = 10f;
    Vector3 nextpos = new Vector3();
    Grid nextgrid;
    bool moveX = true;
    bool moveZ = true;
    public bool m_stopMove;
    public bool m_stopMove2;
	// Use this for initialization
	void Start () 
    {
        Velocity.Set(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        m_stopMove = false;
        m_stopMove2 = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_stopMove && !m_stopMove2)
        {
            //nextpos = transform.position + Velocity.normalized * speed * Time.deltaTime;
            //nextgrid = SceneData.sceneData.gridmesh.GetGridAtPosition(nextpos).GetComponent<Grid>();
            //if (nextgrid != SceneData.sceneData.gridmesh.GetGridAtPosition(transform.position).GetComponent<Grid>())
            //{
            //    if (nextpos.x > nextgrid.GetMinPos().x && nextpos.x < nextgrid.GetMaxPos().x)
            //    {
            //        if (transform.position.z > nextgrid.GetMinPos().z && transform.position.z < nextgrid.GetMaxPos().z)
            //        {
            //            moveX = false;
            //        }
            //    }
            //    if (nextpos.z > nextgrid.GetMinPos().z && nextpos.z < nextgrid.GetMaxPos().z)
            //    {
            //        if (transform.position.x > nextgrid.GetMinPos().x && transform.position.x < nextgrid.GetMaxPos().x)
            //        {
            //            moveZ = false;
            //        }
            //    }

            //    if (moveX)
            //    {
            //        gameObject.transform.position = new Vector3(nextpos.x, gameObject.transform.position.y, gameObject.transform.position.z);
            //    }

            //    if (moveZ)
            //    {
            //        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, nextpos.z);
            //    }
            //}
            //else
            transform.position = transform.position + Velocity.normalized * speed * Time.deltaTime;
        }
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, SceneData.sceneData.ground.SampleHeight(gameObject.transform.position), gameObject.transform.position.z);

        //if(nextpos.x > nextgrid.GetMinPos().x && nextpos.x < nextgrid.GetMaxPos().x)
        //{
        //    if (transform.position.z > nextgrid.GetMinPos().z && transform.position.z < nextgrid.GetMaxPos().z)
        //    {
        //        moveX = false;
        //    }
        //}

        //if (nextpos.z > nextgrid.GetMinPos().z && nextpos.z < nextgrid.GetMaxPos().z)
        //{
        //    if (transform.position.x > nextgrid.GetMinPos().x && transform.position.x < nextgrid.GetMaxPos().x)
        //    {
        //        moveZ = false;
        //    }
        //}

        //if (moveX)
        //{
        //    gameObject.transform.position = new Vector3(nextpos.x, gameObject.transform.position.y, gameObject.transform.position.z);
        //}

        //if (moveZ)
        //{
        //    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, nextpos.z);
        //}

        //gameObject.transform.position.Set(gameObject.transform.position.x, SceneData.sceneData.ground.SampleHeight(gameObject.transform.position), gameObject.transform.position.z);
        moveX = true;
        moveZ = true;
	}
}
