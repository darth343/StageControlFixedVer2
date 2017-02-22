using UnityEngine;
using System.Collections;


public class Grid : MonoBehaviour
{
    public enum GRID_STATE
    {
        AVAILABLE,
        UNAVAILABLE,
        ISPATH,
        INOPENLIST,
        INCLOSELIST,
    }

    public Material[] materials = new Material[5];
    public Vector2 position;
    public Vector3[] Points = new Vector3[5];
    public GRID_STATE state;

    public Vector3 GetWorldPosition()
    {
        return new Vector3(position.x * gameObject.transform.parent.GetComponent<GridArray>().GridSizeX + gameObject.transform.parent.GetComponent<GridArray>().GridSizeX * 0.5f, 0, position.y * gameObject.transform.parent.GetComponent<GridArray>().GridSizeZ + gameObject.transform.parent.GetComponent<GridArray>().GridSizeZ * 0.5f);
    }

    public Vector3 GetMinPos()
    {
        return new Vector3(transform.position.x - SceneData.sceneData.gridmesh.GridSizeX, transform.position.y, transform.position.z - SceneData.sceneData.gridmesh.GridSizeZ); 
    }

    public Vector3 GetMaxPos()
    {
        return new Vector3(transform.position.x + SceneData.sceneData.gridmesh.GridSizeX, transform.position.y, transform.position.z + SceneData.sceneData.gridmesh.GridSizeZ); 
    }

    public GRID_STATE CollidedWithTerrain()
    {
        if (SharedData.instance == null)
            Debug.Log("Null shared data");
        Terrain ground = SceneData.sceneData.ground;
        Vector3 minPos = GetWorldPosition() - (new Vector3(gameObject.transform.parent.GetComponent<GridArray>().GridSizeX * 0.5f, 0, gameObject.transform.parent.GetComponent<GridArray>().GridSizeZ * 0.5f));
        Vector3 maxPos = GetWorldPosition() + (new Vector3(gameObject.transform.parent.GetComponent<GridArray>().GridSizeX * 0.5f, 0, gameObject.transform.parent.GetComponent<GridArray>().GridSizeZ * 0.5f));

        if (0.05 < ground.SampleHeight(minPos) &&  0.05 < ground.SampleHeight(maxPos))
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,ground.SampleHeight(maxPos)+0.1f, gameObject.transform.position.z);
            return GRID_STATE.UNAVAILABLE;
        }
        return GRID_STATE.AVAILABLE;
    }

    void OnValidate()
    {
        UpdateAvailability();
    }

    public void ChangeState(GRID_STATE newstate)
    {
        state = newstate;
        UpdateAvailability();
    }

    public void EnableRendering(bool enabled)
    {
        GetComponent<LineRenderer>().enabled = enabled;
    }

    public void UpdateAvailability()
    {
        if (GetComponent<LineRenderer>() == null)
        {
            return;  
        }

       switch(state)
       {
               case GRID_STATE.AVAILABLE:
            {
                GetComponent<LineRenderer>().material = materials[0];
            }
            break;
               case GRID_STATE.UNAVAILABLE:
            {
                GetComponent<LineRenderer>().material = materials[1];
            }
            break;
               case GRID_STATE.ISPATH:
            {
                GetComponent<LineRenderer>().material = materials[2];
            }
            break;
               case GRID_STATE.INOPENLIST:
            {
                GetComponent<LineRenderer>().material = materials[3];
            }
            break;
               case GRID_STATE.INCLOSELIST:
            {
                GetComponent<LineRenderer>().material = materials[4];
            }
            break;
        }
    }

    public void Select()
    {

    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
