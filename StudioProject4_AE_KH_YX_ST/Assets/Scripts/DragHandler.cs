using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler {
    public static GameObject itemBeingDragged;
    public Vector3 startPosition;
    private Vector3 initialScale;
    public Vector3 SmallerScale = new Vector3(0.1f, 0.1f, 0.1f);

	#region IBeginDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData)
	{
        SceneData.sceneData.isHoldingCard = true;
		itemBeingDragged = gameObject;
		startPosition = transform.position;

        //dragged onto play area , render the hologram mode
        
        //startParent = transform.parent;
        //GetComponent<CanvasGroup>().blocksRaycasts = false;

        
	}
	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
        GameObject holobuild = itemBeingDragged.GetComponent<UnitCards>().GOModel;
        if (SceneData.sceneData.handhandler.onPlayArea(eventData.position) == true)//outside of panel
        {
            holobuild.SetActive(true);
            if (itemBeingDragged.transform.childCount > 0)
            {
                for (int i = 0; i < itemBeingDragged.transform.childCount; ++i)
                {
                    itemBeingDragged.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            //GameObject maxgrid = SharedData.instance.gridmesh.GetMaxGrid(getTerrainPos(), holobuild.GetComponent<Building>().size);
            Vector3 cursorpos = getTerrainPos();
            holobuild.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(cursorpos, holobuild.GetComponent<Building>().size);
          
        }
        else
        {
            if (itemBeingDragged.transform.childCount > 0)
            {
                for (int i = 0; i < itemBeingDragged.transform.childCount; ++i)
                {
                    itemBeingDragged.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            holobuild.SetActive(false);
            //toggleCardRender(true);
    
        }
        transform.position = Input.mousePosition;
    }

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
        //have to add mobile compatability later
        //check if card is in play area
        if (itemBeingDragged.transform.childCount > 0)
        {
            for (int i = 0; i < itemBeingDragged.transform.childCount; ++i)
            {
                itemBeingDragged.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        UnitCards spawnedcard = gameObject.GetComponent<UnitCards>(); 
        GameObject newbuilding = spawnedcard.GOModel;
        if (SceneData.sceneData.handhandler.onPlayArea(eventData.position) == true)
        {
            SceneData.sceneData.isHoldingCard = false;
            //SharedData.instance.buildhandler.build(spawnedcard.GOModel);

            newbuilding.SetActive(true);
            //sending a true with this statement occupy teh grids
            if (SceneData.sceneData.gridmesh.DerenderBuildGrids(true) && SceneData.sceneData.Player.SpendPlayerGold(spawnedcard.goldValue))//check if there are obstructions and build
            {
                itemBeingDragged = null;
                SceneData.sceneData.handhandler.RemoveCard(gameObject);
                newbuilding.GetComponent<Building>().SetBuilding();
            }
            else
            {
                newbuilding.SetActive(false);
                SceneData.sceneData.handhandler.ResetCardPos();
                SceneData.sceneData.gridmesh.DerenderBuildGrids(false);

            }
        }
        else//not in play area
        {
            //SharedData.instance.handhandler.RemoveCard(gameObject);
            newbuilding.SetActive(false);
            SceneData.sceneData.handhandler.ResetCardPos();
            SceneData.sceneData.gridmesh.DerenderBuildGrids(false);
            SceneData.sceneData.isHoldingCard = false;

            //UnityEngine.Debug.Log("released on panel");
        }

        //GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

	#endregion
    
    Vector3 getTerrainPos()
    {
        RaycastHit vHit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (SceneData.sceneData.ground.GetComponent<Collider>().Raycast(ray, out vHit, 1000.0f))
        {
            return vHit.point;

        }
        else
            return new Vector3(0, 0, 0);
    }
    void toggleCardRender(bool render)// this dosent fucking work
    {
        //itemBeingDragged.GetComponent<MeshRenderer>().enabled = false;

        for (int i = 0; i < itemBeingDragged.transform.GetChild(0).childCount; ++i)
        {
            itemBeingDragged.transform.GetChild(i).gameObject.SetActive(false);

        }
    }


    // Use this for initialization
    void Start () {
	
	}
}
