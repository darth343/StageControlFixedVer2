using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour
{
    // How many seconds before next spawn
    public float m_secondsToSpawn;
    private Timer m_timer;
    // The controller that the spawned entity belongs to like EnemyController
    private static GameObject m_controller;
    private static bool m_initController = false;
    // The entity that the building spawns
    public GameObject m_entity;
    // Which direction away from building does user want to spawn units? left, right, up, down
    public string m_orientationX;
    public string m_orientationZ;
    // How many grids away from the building does user want to spawn units
    public int m_offsetGridX;
    public int m_offsetGridZ;
    // How many to spawn in a flock
    public int m_spawnAmt;
    public int m_spawnLimit;
    private int m_originalAmt;
    private int m_currAmt;
    // All the spawned entities are put into this list
    public static List<GameObject> m_entityList;
    // The building of the spawner
    private Building m_building;
    public List<GameObject> m_tempList; // Stores only entities spawned by this building
    public static int m_team1MAX = 50; // Max entities team 1 can have inside scene
    public static int m_team2MAX = 50; // Max entities team 1 can have inside scene

    void Start()
    {
        m_tempList = new List<GameObject>();
        m_timer = this.gameObject.AddComponent<Timer>();
        m_timer.Init(0, m_secondsToSpawn, 0);
        m_building = GetComponent<Building>();
        if (m_initController == false)
        {
            m_entityList = new List<GameObject>();
            m_controller = new GameObject();
            m_controller.name = "EntityList";
            GameObject temp = new GameObject();
            Canvas temp_canvas = temp.AddComponent<Canvas>();
            temp_canvas.transform.SetParent(m_controller.transform);
            temp_canvas.renderMode = RenderMode.WorldSpace;
            m_initController = true;
            SceneData.sceneData.EntityList = m_controller;
        }
        m_originalAmt = m_spawnAmt;
        if (m_spawnLimit < m_spawnAmt)
            m_spawnLimit = m_spawnAmt + m_spawnLimit;
        m_currAmt = 0;
        //temp.worldCamera = 
        //SharedData.instance.gridmesh.GetOccupiedGrids(transform.position, transform.localScale);
    }

    void Update()
    {
        if (m_building.isfriendly && m_entityList.Count > m_team1MAX)
            return;
        if (!m_building.isfriendly && m_entityList.Count > m_team2MAX)
            return;
        if (m_building.b_state == Building.BUILDSTATE.B_ACTIVE)
            m_timer.Update();
        //SharedData.instance.gridmesh.RenderBuildGrids(transform.position, transform.localScale);
        if (m_timer.can_run && m_spawnAmt > 0 && m_entityList.Count < Building.MAX_UNIT && GetComponent<Pathfinder>().PathFound && m_currAmt < m_spawnLimit)
        {
            GameObject spawn;
            for (int i = 0; i < m_spawnAmt; ++i)
            {
                spawn = (GameObject)Instantiate(m_entity); // Create a copy of the original "hell"spawn
                spawn.transform.SetParent(m_controller.transform);
                spawn.GetComponent<Health>().MAX_HEALTH = 100;
                spawn.GetComponent<Unit>().SetPath(GetComponent<Pathfinder>().PathToEnd);
                spawn.GetComponent<Unit>().m_building = GetComponent<Building>();
                ++m_currAmt;
                GameObject handle, handleChild;
                handle = new GameObject();
                handleChild = new GameObject();
                Image img, imgChild;
                //handle = handleChild = (GameObject)Instantiate(m_entity);
                handle.AddComponent<Image>();
                img = handle.GetComponent<Image>();
                img.transform.SetParent(spawn.transform.parent.GetChild(0));
                img.rectTransform.sizeDelta = new Vector2(50, 10);
                img.rectTransform.pivot = new Vector2(0f, 0.5f);
                img.color = Color.red;
                handleChild.AddComponent<Image>();
                imgChild = handleChild.GetComponent<Image>();
                imgChild.transform.SetParent(img.transform);
                imgChild.rectTransform.sizeDelta = new Vector2(50, 10);
                imgChild.rectTransform.pivot = new Vector2(0f, 0.5f);
                imgChild.color = Color.green;
                //spawn.AddComponent<HealthBar>(); // Give it a healthbar
                Vector2 this_grid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position);
                //SharedData.instance.gridmesh.GetGridAtPosition(transform.position);

                //Debug.Log(transform.position);
                int orientationX;
                int orientationZ;
                switch (m_orientationX)
                {
                    case "right":
                        orientationX = -1;
                        break;
                    default:
                        orientationX = 1;
                        break;
                }
                if (m_orientationZ == "up")
                    orientationZ = -1;
                else
                    orientationZ = 1;
                Vector3 spawn_pos = SceneData.sceneData.gridmesh.GetPositionAtGrid((int)this_grid.x + m_offsetGridX * orientationX, (int)this_grid.y + m_offsetGridZ * orientationZ); // is actually the grid this object is on's z position + 30, not y
                spawn_pos.y = SceneData.sceneData.gridmesh.GetTerrainHeightAtGrid(spawn_pos);
                spawn.transform.position = spawn_pos;
                if (m_building.isfriendly)
                    spawn.GetComponent<Unit>().m_isFriendly = true;
                m_entityList.Add(spawn);
                m_tempList.Add(spawn);
            }
            m_timer.Reset();

            if (m_entityList.Count < m_spawnLimit && m_currAmt > m_spawnLimit)
            {
                m_currAmt = 0;
            }
            //if (m_entityList.Count > 10)
            //{
            //    m_spawnAmt = 0;
            //}
            //else if (m_spawnAmt == 0)
            //    m_spawnAmt = m_originalAmt;

        }
    }
}
