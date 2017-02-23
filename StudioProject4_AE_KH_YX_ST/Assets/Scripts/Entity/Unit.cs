using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UNIT_TYPE
{
    CLOCKWORK_SOLDIER,
}

public enum UNIT_TEAM
{
    BLUE_TEAM,
    RED_TEAM,
}

/**/
// Controller class of all units
/**/
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthBar))]
[RequireComponent(typeof(Flocking))]
[RequireComponent(typeof(VMovement))]
public class Unit : MonoBehaviour {
    public int m_attkRadius; // Attack radius which is how many grids surrounding the unit it can detect/see enemies
    public int m_attkDamage;
    private float m_attkDist; // Minimum distance before unit attacks closest enemy unit
    private Vector2 m_currGrid; // The current grid the unit is standing on
    private Vector2 m_oldGrid; // The grid the unit was standing on in the previous frame
    private Animator m_animator; // The animator of this model
    private bool m_switchAnimation; // Prevents the turning off of attack animation from running too many times  
    public AnimationClip m_attkAnim; // The attack animation
    private Timer m_timer;
    public static Component[] m_destroyerOfWorlds; // Stores and destroys all the components of an object
    public List<Vector3> PathToEnd = null;
    public int pathindex = 0;
    private UNIT_TEAM m_team; // The team this entity belongs to
    public bool m_isFriendly; // is the unit player or enemy team's?
    private GameObject m_targetEnemy; // The unit this unit is trying to kill
    private GameObject m_targetBuilding; // The building this unit is trying to kill
    private List<GameObject> m_targetList; // Queue data structure containing buildings or units that unit will attack
    private int m_targetIndex; // The current target in the list unit is focusing attack on
    [HideInInspector]
    public Building m_building; // The building which spawned this unit

    public void SetPath(List<Vector3> newPath)
    {
        PathToEnd = newPath;
        pathindex = newPath.Count - 1;
    }

	// Use this for initialization
	void Start () {
        m_currGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position);
        m_oldGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position);

        Vector2 temp1 = m_currGrid;
        int x = (int)m_currGrid.x;
        int z = (int)m_currGrid.y;
        for (int i = x - m_attkRadius; i <= x + m_attkRadius; ++i) // Print out the attack radius which is a ring around the unit
        {
            for (int j = z - m_attkRadius; j <= z + m_attkRadius; ++j)
            {
                temp1.x = i;
                temp1.y = j;
                //SceneData.sceneData.gridmesh.HighlightUnitPosition(temp1);
            }
        }

        Vector2 farGrid; // Stores furthest grid unit can detect enemy 
        farGrid.x = m_currGrid.x + m_attkRadius; // The grid which unit is on is offset by number of grids away which he can sense so we can get furthest away grid
        farGrid.y = m_currGrid.y;
        Vector3 furthestPoint = SceneData.sceneData.gridmesh.GetPositionAtGrid((int)farGrid.x, (int)farGrid.y); // Get the position of the furthest away grid
        m_attkDist = (furthestPoint - transform.position).sqrMagnitude; // sqrmagnitude is less expensive since we are just doing distance checks
        m_animator = transform.GetChild(0).GetComponent<Animator>();
        m_switchAnimation = false;
        AnimationClip clip = GetAnimationClipFromAnimatorByName(m_animator, "Attack");
        if (clip)
            m_attkAnim = clip;
        m_timer = this.gameObject.AddComponent<Timer>();
        //m_timer.Init(0, m_attkAnim.length * 2, m_attkAnim.length * 2);
        m_timer.Init(0, m_attkAnim.length * 2, 7);
        m_timer.can_run = true;
        m_destroyerOfWorlds = new Component[100];
        m_targetEnemy = null;
        m_targetBuilding = null;
        m_targetList = new List<GameObject>();
        m_targetIndex = 0;
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        //
        //
        // PUT ATTACK STUFF IN A FUNCTION INSTEAD OF FLOODING UPDATE
        //
        //

        // if current grid unit is standing on is not the old one, meaning he has moved
        m_currGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position); // Get unit's current grid
        if (!m_oldGrid.Equals(m_currGrid)) // If unit was on another grid in the previous frame
        {
            Vector2 temp1 = m_currGrid;
            int x = (int)m_currGrid.x;
            int z = (int)m_currGrid.y;
            for (int i = x - m_attkRadius; i <= x + m_attkRadius; ++i)
            {
                for (int j = z - m_attkRadius; j <= z + m_attkRadius; ++j)
                {
                    temp1.x = i;
                    temp1.y = j;
                    //SceneData.sceneData.gridmesh.HighlightUnitPosition(temp1);
                }
            }
            m_oldGrid = m_currGrid;
        }

             for (int i = 0; i < Spawn.m_entityList.Count; ++i)
        {
            GameObject ent = (GameObject)Spawn.m_entityList[i];
            if (ent.GetComponent<Unit>().m_isFriendly != m_isFriendly) // if the entity being checked is not himself and is in the opponent's team
            {
                float dist = (ent.transform.position - transform.position).sqrMagnitude;
                if (dist < m_attkDist) // An enemy has drawn close to the unit, attack it
                {
                    m_timer.Update();
                    //Vector2 enemyGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(ent.transform.position); // The commented out code below is to check and kill all enemies in a grid
                    //for (int j = 0; j < Spawn.m_entityList.Count; ++j)
                    {
                        //GameObject enemy = (GameObject)Spawn.m_entityList[j];
                        //if (SceneData.sceneData.gridmesh.GetGridIndexAtPosition(enemy.transform.position).x == enemyGrid.x && SceneData.sceneData.gridmesh.GetGridIndexAtPosition(enemy.transform.position).y == enemyGrid.y && m_timer.can_run)
                        {
                            ent.GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, m_attkDamage);
                            m_timer.Reset();
                        }
                    }
                    m_animator.SetBool("b_attack", true); // Play attack animation
                    m_switchAnimation = true;
                    if (m_targetEnemy == null)
                    {
                        m_targetEnemy = ent;
                        //m_targetList.Add(ent);
                        GetComponent<VMovement>().m_stopMove = true;
                    }
                }
                else if (m_switchAnimation)
                {
                    m_animator.SetBool("b_attack", false);
                    m_switchAnimation = false;
                }
            }
            if ((m_targetEnemy == null || m_targetEnemy.GetComponent<Health>().GetHealth() < 0) && GetComponent<VMovement>().m_stopMove)
            {
                m_targetEnemy = null;
                GetComponent<VMovement>().m_stopMove = false;
            }
            else if (m_targetEnemy != null && GetComponent<VMovement>().m_stopMove)
            {
                if (Vector3.Distance(m_targetEnemy.transform.position, transform.position) > Mathf.Sqrt(m_attkDist))
                {
                    m_targetEnemy = null;
                    GetComponent<VMovement>().m_stopMove = false;
                }
            }
        }

        if (GetComponent<Flocking>().isleader)
        {
            if (PathToEnd.Count > 0)
            {
                GetComponent<VMovement>().Velocity = (PathToEnd[pathindex] - transform.position).normalized;
                if ((PathToEnd[pathindex] - transform.position).sqrMagnitude < GetComponent<VMovement>().speed * GetComponent<VMovement>().speed)
                {
                    --pathindex;
                    if (pathindex <= 0)
                    {
                        PathToEnd.Clear();
                    }
                }
            }
        }

        for (int i = 0; i < Building.m_buildingList.Count; ++i)
        {
            GameObject ent = Building.m_buildingList[i];
            if (ent.GetComponent<Building>().isfriendly != m_isFriendly && m_targetBuilding == null) // if not from the same team
            {
                // need to make the list of ent to kill
                if (Vector3.Distance(ent.transform.position, transform.position) < Mathf.Sqrt(m_attkDist)) // distance check to see if building to attack is nearby
                {
                    m_animator.SetBool("b_attack", true); // Play attack animation
                    m_switchAnimation = true;
                    if (m_targetBuilding == null)
                    {
                        Debug.Log(ent.GetComponent<Building>().gameObject.name);
                        m_targetBuilding = ent;
                    }
                }
                else if (m_switchAnimation)
                {
                    m_animator.SetBool("b_attack", false);
                    m_switchAnimation = false;
                }
            }
        }

        if ((m_targetBuilding == null || m_targetBuilding.GetComponent<Health>().GetHealth() < 0) && GetComponent<VMovement>().m_stopMove2)
        {
            m_targetBuilding = null;
            GetComponent<VMovement>().m_stopMove2 = false;
        }
        else if (m_targetBuilding != null && m_building)
        {
            m_building.m_isDistract = true;
            m_building.GetComponent<Pathfinder>().FindPath(m_building.GetMaxPosOfBuilding(m_targetBuilding.transform.position, m_targetBuilding.GetComponent<Building>().size));
            SetPath(m_building.GetComponent<Pathfinder>().PathToEnd);
            Debug.Log("decreasing");
            m_targetBuilding.GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, m_attkDamage);
            GetComponent<VMovement>().m_stopMove2 = true;
            if (m_targetBuilding.GetComponent<Health>().GetHealth() < 0)
                m_building.m_isDistract = false;
        }

        if (GetComponent<Health>().GetHealth() < 0) // Unit died
        {
            RemoveEntity(this.gameObject);
            UnityEngine.Object.Destroy(this.gameObject);
            m_destroyerOfWorlds = GetComponents(typeof(Component));
            for (int i = 0; i < m_destroyerOfWorlds.Length; ++i)
            {
                if (m_destroyerOfWorlds[i].gameObject.activeSelf)
                    UnityEngine.Object.Destroy(m_destroyerOfWorlds[i]);
            }
        }
 
    }

    internal static AnimationClip GetAnimationClipFromAnimatorByName(Animator animator, string name)
    {
        //can't get data if no animator
        if (animator == null)
            return null;

        //favor for above foreach due to performance issues
        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (animator.runtimeAnimatorController.animationClips[i].name.Equals(name))
                return animator.runtimeAnimatorController.animationClips[i];
        }

        //Debug.LogError("Animation clip: " + name + " not found");
        return null;
    }

    // Remove a gameobject from Spawn.m_entityList
    public static void RemoveEntity(GameObject go, List<GameObject> list = null)
    {
        if (list == null)
            Spawn.m_entityList.Remove(go);
        else
            list.Remove(go);
    }

}

/*     for (int i = 0; i < Spawn.m_entityList.Count; ++i)
        {
            GameObject ent = (GameObject)Spawn.m_entityList[i];
            if (ent.GetComponent<Unit>().m_isFriendly != m_isFriendly) // if the entity being checked is not himself and is in the opponent's team
            {
                float dist = (ent.transform.position - transform.position).sqrMagnitude;
                if (dist < m_attkDist) // An enemy has drawn close to the unit, attack it
                {
                    m_timer.Update();
                    //Vector2 enemyGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(ent.transform.position); // The commented out code below is to check and kill all enemies in a grid
                    //for (int j = 0; j < Spawn.m_entityList.Count; ++j)
                    {
                        //GameObject enemy = (GameObject)Spawn.m_entityList[j];
                        //if (SceneData.sceneData.gridmesh.GetGridIndexAtPosition(enemy.transform.position).x == enemyGrid.x && SceneData.sceneData.gridmesh.GetGridIndexAtPosition(enemy.transform.position).y == enemyGrid.y && m_timer.can_run)
                        {
                            ent.GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, m_attkDamage);
                            m_timer.Reset();
                        }
                    }
                    m_animator.SetBool("b_attack", true); // Play attack animation
                    m_switchAnimation = true;
                    //if (m_targetEnemy == null)
                    {
                        //m_targetEnemy = ent;
                        m_targetList.Add(ent);
                        GetComponent<VMovement>().m_stopMove = true;
                    }
                }
                else if (m_switchAnimation)
                {
                    m_animator.SetBool("b_attack", false);
                    m_switchAnimation = false;
                }
            }
            if ((m_targetEnemy == null || m_targetEnemy.GetComponent<Health>().GetHealth() < 0) && GetComponent<VMovement>().m_stopMove)
            {
                m_targetEnemy = null;
                GetComponent<VMovement>().m_stopMove = false;
            }
        }

        if (GetComponent<Flocking>().isleader)
        {
            if (PathToEnd.Count > 0)
            {
                GetComponent<VMovement>().Velocity = (PathToEnd[pathindex] - transform.position).normalized;
                if ((PathToEnd[pathindex] - transform.position).sqrMagnitude < GetComponent<VMovement>().speed * GetComponent<VMovement>().speed)
                {
                    --pathindex;
                    if (pathindex <= 0)
                    {
                        PathToEnd.Clear();
                    }
                }
            }
        }

        for (int i = 0; i < Building.m_buildingList.Count; ++i)
        {
            GameObject ent = Building.m_buildingList[i];
            if (ent.GetComponent<Building>().isfriendly != m_isFriendly && m_targetBuilding == null) // if not from the same team
            {
                // need to make the list of ent to kill
                if (Vector3.Distance(ent.transform.position, transform.position) < Mathf.Sqrt(m_attkDist)) // distance check to see if building to attack is nearby
                {
                    m_animator.SetBool("b_attack", true); // Play attack animation
                    m_switchAnimation = true;
                    if (m_targetBuilding == null)
                    {
                        Debug.Log(ent.GetComponent<Building>().gameObject.name);
                        m_targetBuilding = ent;
                    }
                }
                else if (m_switchAnimation)
                {
                    m_animator.SetBool("b_attack", false);
                    m_switchAnimation = false;
                }
            }
        }

        if ((m_targetBuilding == null || m_targetBuilding.GetComponent<Health>().GetHealth() < 0) && GetComponent<VMovement>().m_stopMove)
        {
            m_targetBuilding = null;
            GetComponent<VMovement>().m_stopMove = false;
        }
        else if (m_targetBuilding != null)
        {
            m_building.m_isDistract = true;
            m_building.GetComponent<Pathfinder>().FindPath(m_building.GetMaxPosOfBuilding(m_targetBuilding.transform.position, m_targetBuilding.GetComponent<Building>().size));
            SetPath(m_building.GetComponent<Pathfinder>().PathToEnd);

            m_targetBuilding.GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, m_attkDamage);
            GetComponent<VMovement>().m_stopMove = true;
            if (m_targetBuilding.GetComponent<Health>().GetHealth() < 0)
                m_building.m_isDistract = false;
        }

        if (GetComponent<Health>().GetHealth() < 0) // Unit died
        {
            RemoveEntity(this.gameObject);
            UnityEngine.Object.Destroy(this.gameObject);
            m_destroyerOfWorlds = GetComponents(typeof(Component));
            for (int i = 0; i < m_destroyerOfWorlds.Length; ++i)
            {
                if (m_destroyerOfWorlds[i].gameObject.activeSelf)
                    UnityEngine.Object.Destroy(m_destroyerOfWorlds[i]);
            }
        }
	}
 */

/*
        GameObject target = null;
        if (m_targetList[m_targetIndex] != null)
            target = m_targetList[m_targetIndex];
        //else
        {
            for (int i = 0; i < Spawn.m_entityList.Count; ++i)
            {
                GameObject ent = (GameObject)Spawn.m_entityList[i];
                if (/*ent != this.gameObject/ && ent.GetComponent<Unit>().m_isFriendly != m_isFriendly) // if the entity being checked is not himself and is in the opponent's team
                {
                    float dist = (ent.transform.position - transform.position).sqrMagnitude;
                    if (dist < m_attkDist) // An enemy has drawn close to the unit, attack it
                    {
                        m_timer.Update();
                        //Vector2 enemyGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(ent.transform.position); // The commented out code below is to check and kill all enemies in a grid
                        //for (int j = 0; j < Spawn.m_entityList.Count; ++j)
                        {
                            //GameObject enemy = (GameObject)Spawn.m_entityList[j];
                            //if (SceneData.sceneData.gridmesh.GetGridIndexAtPosition(enemy.transform.position).x == enemyGrid.x && SceneData.sceneData.gridmesh.GetGridIndexAtPosition(enemy.transform.position).y == enemyGrid.y && m_timer.can_run)
                            {
                                ent.GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, m_attkDamage);
                                m_timer.Reset();
                            }
                        }
                        m_animator.SetBool("b_attack", true); // Play attack animation
                        m_switchAnimation = true;
                        m_targetList.Add(ent);
                    }
                    else if (m_switchAnimation)
                    {
                        m_animator.SetBool("b_attack", false);
                        m_switchAnimation = false;
                    }
                }
            }

            if (GetComponent<Flocking>().isleader)
            {
                if (PathToEnd.Count > 0)
                {
                    GetComponent<VMovement>().Velocity = (PathToEnd[pathindex] - transform.position).normalized;
                    if ((PathToEnd[pathindex] - transform.position).sqrMagnitude < GetComponent<VMovement>().speed * GetComponent<VMovement>().speed)
                    {
                        --pathindex;
                        if (pathindex <= 0)
                        {
                            PathToEnd.Clear();
                        }
                    }
                }
            }

            for (int i = 0; i < Building.m_buildingList.Count; ++i)
            {
                GameObject ent = Building.m_buildingList[i];
                if (ent.GetComponent<Building>().isfriendly != m_isFriendly) // if not from the same team
                {
                    // need to make the list of ent to kill
                    if (Vector3.Distance(ent.transform.position, transform.position) < Mathf.Sqrt(m_attkDist)) // distance check to see if building to attack is nearby
                    {
                        m_animator.SetBool("b_attack", true); // Play attack animation
                        m_switchAnimation = true;
                        Debug.Log(ent.GetComponent<Building>().gameObject.name);
                        m_targetList.Add(ent);
                    }
                    else if (m_switchAnimation)
                    {
                        m_animator.SetBool("b_attack", false);
                        m_switchAnimation = false;
                    }
                }
            }

            // set target to null
            if (target != null && target.GetComponent<Health>().GetHealth() < 0 && GetComponent<VMovement>().m_stopMove == true)
            {
                m_targetList.Remove(target);
                //if (m_targetList[m_targetIndex + 1] != null)
                //    ++m_targetIndex;
                m_building.m_isDistract = false;
                GetComponent<VMovement>().m_stopMove = false;
            }
            else if (target != null)
            {
                m_building.m_isDistract = true;
                if (target.GetComponent<Building>())
                    m_building.GetComponent<Pathfinder>().FindPath(m_building.GetMaxPosOfBuilding(target.transform.position, target.GetComponent<Building>().size));
                SetPath(m_building.GetComponent<Pathfinder>().PathToEnd);
                target.GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, m_attkDamage);
                if (Vector3.Distance(target.transform.position, transform.position) < Mathf.Sqrt(m_attkDist))
                    GetComponent<VMovement>().m_stopMove = true;
            }

            if (GetComponent<Health>().GetHealth() < 0) // Unit died
            {
                RemoveEntity(this.gameObject);
                UnityEngine.Object.Destroy(this.gameObject);
                m_destroyerOfWorlds = GetComponents(typeof(Component));
                for (int i = 0; i < m_destroyerOfWorlds.Length; ++i)
                {
                    if (m_destroyerOfWorlds[i].gameObject.activeSelf)
                        UnityEngine.Object.Destroy(m_destroyerOfWorlds[i]);
                }
            }
        }
	}*/
