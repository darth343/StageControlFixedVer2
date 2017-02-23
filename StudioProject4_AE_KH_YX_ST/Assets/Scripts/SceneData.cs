using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SceneData : MonoBehaviour
{
    public static SceneData sceneData;
    public GameObject EntityList;
    public Canvas UI;
    public GridArray gridmesh;
    public Text debuginfo;
    public Terrain ground;
    public PlayerInfo Player;

    //Decks
    public GameObject PlayerDeck = null;
    public bool isHoldingCard = false;

    //handlers
    public HandHandler handhandler;
    public DragHandler draghandler;

    //Flocking Settings
    public float CohesionWeight = 1f;
    public float SeperationWeight = 1f;
    public float AlignmentWeight = 1f;


    //Particles
    public ParticleSystem buildingP;

    public Image buildTimer;

    void SnapBasesToGrid()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.PlayerBase.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(LevelManager.instance.PlayerBase.transform.position, 4);
            SceneData.sceneData.gridmesh.DerenderBuildGrids(true);
            LevelManager.instance.EnemyBase.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(LevelManager.instance.EnemyBase.transform.position, 4);
            SceneData.sceneData.gridmesh.DerenderBuildGrids(true);      
        }
    }

    void Awake()
    {
        sceneData = this;
        Invoke("SnapBasesToGrid", 1);
    }

    void Start()
    {

    }
}
