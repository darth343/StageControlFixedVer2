using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Card_Link : System.Object
{
    public CARD_TYPE type;
    public GameObject gm;
}

public class SharedData : MonoBehaviour {
    public static SharedData instance = null;

    public List<Card_Link> DatabasePopulater = null;
    public SortedList<CARD_TYPE, GameObject> CardDatabase = new SortedList<CARD_TYPE, GameObject>();

	// Use this for initialization
	void Start ()
    {
	    if(instance == null)
        {
            if (DatabasePopulater == null)
            {
                Debug.Log("Database populater is null");
            }

            foreach(Card_Link link in DatabasePopulater)
            {
                CardDatabase.Add(link.type, link.gm);
                //Debug.Log(link.type.ToString() + "  " + CardDatabase.Count);
            }
            instance = this;

            if (SceneData.sceneData)
            {
                if (SceneData.sceneData.PlayerDeck != null)
                    SceneData.sceneData.PlayerDeck.GetComponent<Deck>().GenerateDeck();
            }

            if (DatabasePopulater != null)
                DatabasePopulater.Clear();

        }else
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
