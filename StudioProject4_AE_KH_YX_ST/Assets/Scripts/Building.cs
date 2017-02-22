using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
    //base class for all buildings
    //call the spawning stuff here samuel
    public enum BUILDSTATE
    {
        B_HOLOGRAM,
        B_CONSTRUCT,
        B_ACTIVE
    };
    //public GameObject Unit; //the unit that this building spawns, spawn script already requires a unit
    public float buildtime, spawntime;// time to construct the building/time it takes to spawn a single unit
    public int size;//building size
    public Material holo,undamaged,damaged;
    public BUILDSTATE b_state;
    public bool isfriendly;
    float timerB = 0.0f;
    ParticleSystem buildingTemp = null;
    public float buildTimer;
	// Use this for initialization
	void Start () {
        //b_state = BUILDSTATE.B_HOLOGRAM;

        Invoke("InstantiateParticles", 0.1f);
        isfriendly = true;//default to the player's units
	}

    void InstantiateParticles()
    {
        buildingTemp = Instantiate(SceneData.sceneData.buildingP);
    }

    Vector3 GetMaxPosOfBuilding(Vector3 position, int othersize)
    {
        Vector3 maxpos = position + new Vector3(SceneData.sceneData.gridmesh.GridSizeX * (othersize), 0, SceneData.sceneData.gridmesh.GridSizeX * (othersize));
        return maxpos;
    }

	// Update is called once per frame
	void Update () {

        switch (b_state)
        {
            case BUILDSTATE.B_HOLOGRAM:
                for (int i = 0; i < gameObject.transform.GetChild(0).childCount; ++i)
                {
                    gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<MeshRenderer>().material = holo;

                }

                break;
            case BUILDSTATE.B_CONSTRUCT:
                    timerB += Time.deltaTime;
                    if (timerB < buildTimer)
                    {
                        if (buildingTemp)
                        {
                            buildingTemp.Play();
                            buildingTemp.transform.position = gameObject.transform.position;
                            for (int i = 0; i < gameObject.transform.GetChild(0).childCount; ++i)
                            {
                                gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<MeshRenderer>().material = holo;
                            }
                        }
                    }
                    else if (timerB >= buildTimer)
                    {
                        if (buildingTemp)
                        {
                            buildingTemp.Stop();
                            buildingTemp.transform.position = gameObject.transform.position;
                            for (int i = 0; i < gameObject.transform.GetChild(0).childCount; ++i)
                            {
                                gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<MeshRenderer>().material = undamaged;
                            }
                            b_state = BUILDSTATE.B_ACTIVE;
                            Destroy(buildingTemp);
                        }
                    }
                break;
            case BUILDSTATE.B_ACTIVE:
                for (int i = 0; i < gameObject.transform.GetChild(0).childCount; ++i)
                {
                    gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<MeshRenderer>().material = undamaged;
                }

                if (isfriendly && GetComponent<Pathfinder>())
                {
                    GetComponent<Pathfinder>().FindPath(GetMaxPosOfBuilding(LevelManager.instance.EnemyBase.transform.position, LevelManager.instance.EnemyBase.GetComponent<Building>().size));
                }

                break;


        }
	
	}

    public void SetBuilding()
    {
        b_state = BUILDSTATE.B_CONSTRUCT;


    }
}
