using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UNIT_TYPE
{
    CLOCKWORK_SOLDIER,
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
    private float m_attkDist; // Minimum distance before unit attacks closest enemy unit
    private Vector2 m_currGrid; // The current grid the unit is standing on
    private Vector2 m_oldGrid; // The grid the unit was standing on in the previous frame
    private Animator m_animator; // The animator of this model
    private bool m_switchAnimation; // Prevents the turning off of attack animation from running too many times  
    private AnimationClip m_attkAnim; // The attack animation
    private Timer m_timer;
    Component[] m_destroyerOfWorlds; // Stores and destroys all the components of an object
    public List<Vector3> PathToEnd = null;
    private int pathindex = 0;

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
        //m_attkAnim = GetAnimationClipFromAnimatorByName(m_animator, "Attack");
        m_timer = this.gameObject.AddComponent<Timer>();
        //m_timer.Init(0, m_attkAnim.length, 0);
        m_timer.Init(0, 20, 0);
        m_timer.can_run = true;
        m_destroyerOfWorlds = new Component[100];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //
        //
        // PUT ATTACK STUFF IN A FUNCTION INSTEAD OF FLOODING UPDATE
        //
        //
        if (GetComponent<Health>().GetHealth() < 0)
        {
            RemoveEntity(this.gameObject);
            UnityEngine.Object.Destroy(this.gameObject);
            m_destroyerOfWorlds = GetComponents(typeof(Component));
            for (int i = 0; i < m_destroyerOfWorlds.Length; ++i)
            {
                UnityEngine.Object.Destroy(m_destroyerOfWorlds[i]);
            }
        }

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
            if (ent != this.gameObject)
            {
                float dist = (ent.transform.position - transform.position).sqrMagnitude;
                if (dist < m_attkDist) // An enemy has drawn close to the unit, attack it
                {
                    m_timer.Update();
                    Vector2 enemyGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(ent.transform.position);
                    for (int j = 0; j < Spawn.m_entityList.Count; ++j)
                    {
                        GameObject enemy = (GameObject)Spawn.m_entityList[j];
                        if (SceneData.sceneData.gridmesh.GetGridIndexAtPosition(enemy.transform.position).x == enemyGrid.x && SceneData.sceneData.gridmesh.GetGridIndexAtPosition(enemy.transform.position).y == enemyGrid.y && m_timer.can_run)
                        {
                            enemy.GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, 25);
                            m_timer.Reset();
                        }
                    }
                    m_animator.SetBool("b_attack", true); // Play attack animation
                    m_switchAnimation = true;
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

        Debug.LogError("Animation clip: " + name + " not found");
        return null;
    }

    // Remove a gameobject from Spawn.m_entityList
    static void RemoveEntity(GameObject go)
    {
        Spawn.m_entityList.Remove(go);
    }

}
