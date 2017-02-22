using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Deck_Detail : System.Object
{
    public int CardAmount;
    public CARD_TYPE CardType;
}

public class Deck : MonoBehaviour
{

    public List<Deck_Detail> CardsToInclude = null;
    public List<GameObject> Cards;
    public HandHandler handHandler;

    // Use this for initialization
    void Start()
    {

    }

    public void GenerateDeck()
    {
        if (CardsToInclude == null)
            return;

        Cards = new List<GameObject>();

        foreach (Deck_Detail detail in CardsToInclude)
        {
            GameObject CardinDatabase = null;
            SharedData.instance.CardDatabase.TryGetValue(detail.CardType, out CardinDatabase);
            if (CardinDatabase != null)
            {
                for (int i = 0; i < detail.CardAmount; ++i)
                {
                    GameObject newcard = Instantiate(CardinDatabase);
                    Cards.Add(newcard);
                    newcard.transform.SetParent(SceneData.sceneData.UI.transform);
                }
            }
        }

        ShuffleDeck();

        if (CardsToInclude != null)
            CardsToInclude.Clear();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ShuffleDeck()
    {
        for (int i = 0; i < 50; ++i)
        {
            int randomIndex1 = Random.Range(0, Cards.Count);
            int randomIndex2 = Random.Range(0, Cards.Count);
            GameObject temp = Cards[randomIndex1];
            Cards[randomIndex1] = Cards[randomIndex2];
            Cards[randomIndex2] = temp;

        }
    }

    public void DrawCard()
    {
        
        if (Cards.Count <= 30 && Cards.Count > 0)
        {
            GameObject firstCard = Cards.ElementAt(0);
            if (SceneData.sceneData.handhandler.handsize < 5)
            {
                SceneData.sceneData.handhandler.cardlist.Add(firstCard);
                SceneData.sceneData.handhandler.ResetCardPos();
                firstCard.GetComponent<UnitCards>().GenerateBuilding();
                Cards.Remove(firstCard);
            }
        }
        
        else if (Cards.Count <= 0)
        {
            if (SceneData.sceneData.handhandler.handsize < 5)
            {
                GameObject NewDeckCard = GameObject.Find("NewDeckCard");
                SceneData.sceneData.handhandler.cardlist.Add(NewDeckCard);
                SceneData.sceneData.handhandler.ResetCardPos();
            }
        }
    }
}
