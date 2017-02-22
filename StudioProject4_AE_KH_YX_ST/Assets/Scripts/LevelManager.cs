using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;
    public GameObject PlayerBase;
    public GameObject EnemyBase;

	// Use this for initialization
	void Start ()
    {
        if (instance == null)
        {
            instance = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}