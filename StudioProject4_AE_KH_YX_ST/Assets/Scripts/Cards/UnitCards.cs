using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CARD_TYPE
{
    BALLISTA_FACTORY,
    BUSTER_FACTORY,
    IRON_GOLEM_FACTORY,
    CLOCKWORK_FACTORY,
    RAILGUN_FACTORY,
    SPIDERTANK_FACTORY,
    NEWDECK
}


public class UnitCards : MonoBehaviour {

	public string buildingName;
	public string UnitType;
	public int goldValue;
    public float Time;
    public string cardDescr;
    public GameObject GOModel;
    RectTransform card;
    public Text nameText;
    public Text goldText;
    public Text timeText;
    public Text cardDescription;
    public CARD_TYPE cardType;

	// Use this for initialization
	void Start () {
        SetText();
        //take this out tlater
        GOModel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	}

    

    public void GenerateBuilding()
    {
        //create a building when drawn
        GOModel = Instantiate(GOModel);
    }

    public void ConstructBuilding()
    {

    }

    public void ResetCardPos()
    {
        SceneData.sceneData.handhandler.ResetCardPos();
    }

    public void SetText()
    {
        nameText.text = buildingName + "\n" + " Factory";
        goldText.text = goldValue.ToString();
        timeText.text = Time.ToString();
        cardDescription.text = cardDescr;
    }
		
}
