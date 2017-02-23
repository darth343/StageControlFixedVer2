using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Building : MonoBehaviour
{
    //base class for all buildings
    //call the spawning stuff here samuel, mai la, mai la, wa mai la
    public enum BUILDSTATE
    {
        B_HOLOGRAM,
        B_CONSTRUCT,
        B_ACTIVE
    };
    //public GameObject Unit; //the unit that this building spawns, spawn script already requires a unit
    public float buildtime, spawntime;// time to construct the building/time it takes to spawn a single unit
    // Max number of troops the building can spawn
    public static int MAX_UNIT = 30;
    public int size;//building size
    public Material holo, undamaged, damaged;
    public BUILDSTATE b_state;
    public bool isfriendly;
    public bool m_isDistract;
    float timerB = 0.0f;
    ParticleSystem buildingTemp = null;
    Image buildTimerTemp = null;
    public float buildTimer;
    private static GameObject m_buildingControl;
    private static bool m_initController = true;
    public static List<GameObject> m_buildingList; // List of all the buildings in the scene 

    // Use this for initialization
    void Start()
    {
        //b_state = BUILDSTATE.B_HOLOGRAM;

        Invoke("InstantiateParticles", 0.1f);
        //isfriendly = true;//default to the player's units
        m_isDistract = false;
        // The controller all building belongs to
        if (m_initController)
        {
            m_buildingControl = new GameObject();
            m_buildingControl.name = "Building Controller";
            GameObject temporary = new GameObject();
            Canvas temporary_canvas = temporary.AddComponent<Canvas>();
            temporary_canvas.transform.SetParent(m_buildingControl.transform);
            temporary_canvas.renderMode = RenderMode.WorldSpace;
            m_buildingList = new List<GameObject>();
            m_initController = false;
        }
        transform.SetParent(m_buildingControl.transform);
        GameObject handle, handleChild;
        handle = new GameObject();
        handleChild = new GameObject();
        Image img, imgChild;
        //handle = handleChild = (GameObject)Instantiate(m_entity);
        handle.AddComponent<Image>();
        img = handle.GetComponent<Image>();
        img.transform.SetParent(transform.parent.GetChild(0));
        img.rectTransform.sizeDelta = new Vector2(50, 10);
        img.rectTransform.pivot = new Vector2(0f, 0.5f);
        img.color = Color.red;
        handleChild.AddComponent<Image>();
        imgChild = handleChild.GetComponent<Image>();
        imgChild.transform.SetParent(img.transform);
        imgChild.rectTransform.sizeDelta = new Vector2(50, 10);
        imgChild.rectTransform.pivot = new Vector2(0f, 0.5f);
        imgChild.color = Color.green;
        m_buildingList.Add(gameObject);
    }

    void InstantiateParticles()
    {
        buildingTemp = Instantiate(SceneData.sceneData.buildingP);
        buildTimerTemp = Instantiate(SceneData.sceneData.buildTimer);
        buildTimerTemp.transform.SetParent(SceneData.sceneData.UI.transform);
        buildTimerTemp.enabled = false;    
    }
    

    public Vector3 GetMaxPosOfBuilding(Vector3 position, int othersize)
    {
        Vector3 maxpos = position + new Vector3(SceneData.sceneData.gridmesh.GridSizeX * (othersize), 0, SceneData.sceneData.gridmesh.GridSizeX * (othersize));
        return maxpos;
    }

    // Update is called once per frame
    void Update()
    {

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
                        buildTimerTemp.fillAmount -= 1.0f / buildTimer * Time.deltaTime;
                        buildTimerTemp.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
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

                if (!m_isDistract)
                {
                    if (isfriendly && GetComponent<Pathfinder>() && LevelManager.instance.EnemyBase)
                    {
                        GetComponent<Pathfinder>().FindPath(GetMaxPosOfBuilding(LevelManager.instance.EnemyBase.transform.position, LevelManager.instance.EnemyBase.GetComponent<Building>().size));
                    }
                    else if (!isfriendly && GetComponent<Pathfinder>() && LevelManager.instance.PlayerBase)
                    {
                        GetComponent<Pathfinder>().FindPath(GetMaxPosOfBuilding(LevelManager.instance.PlayerBase.transform.position, LevelManager.instance.PlayerBase.GetComponent<Building>().size));
                    }
                }

                if (GetComponent<Health>().GetHealth() < 0)
                {
                    Unit.m_destroyerOfWorlds = new Component[100];
                    Unit.RemoveEntity(gameObject, m_buildingList);
                    UnityEngine.Object.Destroy(this.gameObject);
                    //Unit.m_destroyerOfWorlds = GetComponents(typeof(Component));
                    //for (int i = 0; i < Unit.m_destroyerOfWorlds.Length; ++i)
                    //{
                    //    if (Unit.m_destroyerOfWorlds[i].gameObject.activeSelf)
                    //        UnityEngine.Object.Destroy(Unit.m_destroyerOfWorlds[i]);
                    //}
                }

                break;


        }

    }

    public void SetBuilding()
    {
        b_state = BUILDSTATE.B_CONSTRUCT;
        buildTimerTemp.enabled = true;  
    }
}
