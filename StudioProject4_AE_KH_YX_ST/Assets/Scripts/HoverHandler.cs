using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HoverHandler : MonoBehaviour {
    static public RectTransform selected;
    public int origin;
    public float temprotatex, temprotatey, temprotatez;
    public Vector3 rotate;
    float tempw;
	// Use this for initialization
	void Start () {
        
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseEnter()
    {

        if (SceneData.sceneData.isHoldingCard)
            return;

        selected = gameObject.GetComponent<RectTransform>();
        selected.localScale = new Vector3(2, 2, 1);
        tempw = selected.localRotation.w;
        rotate.z= selected.localRotation.z;
        rotate.x = selected.localRotation.x;
        rotate.y = selected.localRotation.y;
        selected.localRotation = Quaternion.Euler(0,0,0);
        selected.position = new Vector3(selected.position.x,Screen.height*0.3f, selected.position.z);

        origin = transform.GetSiblingIndex();

        //move to the front of the UI
        transform.SetAsLastSibling();

        //Debug.Log("on");

    }

    public void OnMouseExit()
    {
        selected = gameObject.GetComponent<RectTransform>();
        selected.rotation.Set(0, 0, rotate.z, tempw);
        selected.position = new Vector3(selected.position.x, (selected.position.y -100f), selected.position.z);
        selected.localScale = new Vector3(1, 1, 1);
        //Debug.Log("off");
    }

    public void PanelEnter()
    {
        //Debug.Log("onpanel");
        SceneData.sceneData.handhandler.SetOnplayArea(false);
       
    }

    public void PanelExit()
      {
          //Debug.Log("offpanel");
          SceneData.sceneData.handhandler.SetOnplayArea(true);
    }
}
