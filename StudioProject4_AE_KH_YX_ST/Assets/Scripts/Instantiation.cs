using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Instantiation : MonoBehaviour {

    public GameObject building;
    public Terrain ground;
    Vector3 cursorpos;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
            build();
	}

    public void build()
    {
        RaycastHit vHit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (ground.GetComponent<Collider>().Raycast(ray, out vHit, 1000.0f))
        {
            GameObject temp =  Instantiate(building);
            temp.transform.position = vHit.point;
        }
    }


    public void build(GameObject attatchedbuilding)
    {
        RaycastHit vHit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (ground.GetComponent<Collider>().Raycast(ray, out vHit, 1000.0f))
        {
            GameObject temp = Instantiate(attatchedbuilding);
            temp.transform.position = vHit.point;
        }
    }

    void Create()
    {
        GameObject go = (GameObject)Instantiate(building);
    }
}
