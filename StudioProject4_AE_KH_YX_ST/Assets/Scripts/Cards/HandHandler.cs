using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandHandler : MonoBehaviour {

    //change to card objects later
  
    public List<GameObject> cardlist ;//not more than 5
    public int dist,handsize;//has to almosta lways be 5

    public RectTransform canvas,cardarea;
    float m,center,leftstart,maxdegree;
    public bool odd,inPlayArea;


	// Use this for initialization
	void Start () {
        dist = 100;
        handsize = cardlist.Count;
        m = cardarea.rect.height*0.02f;
        center = Screen.width / 2;
        inPlayArea = false;
        //cardarea.GetComponent<RectTransform>().rect.x = 


        if (handsize % 2 != 0)
            odd = true;
        else
            odd = false;

        maxdegree = 10 * (handsize / 2);
        SetCardPos();
	}
	
	// Update is called once per frame
	void Update () {
	 
	}

    public bool onPlayArea(Vector2 screenpoint)
    {
        if(RectTransformUtility.RectangleContainsScreenPoint(cardarea,screenpoint))
        { 
            return false;
        }
        else 
            return true;
        
    }

    public void RemoveCard(GameObject card)
    {
        card.SetActive(false);
        cardlist.Remove(card);
        handsize = cardlist.Count;
        ResetCardPos();
    }

    public void SetCardPos()
    {
        //when down to the last five cards(adda check later)
        // 
        //five card placement
        leftstart = -(dist*2) + ((5-handsize) * 25);
        for (int i = 0; i < cardlist.Count; i++)
        {
            Vector3 newpos = GetCurvePos(i);
            cardlist[i].GetComponent<RectTransform>().position = newpos;
            if (!odd)//no center
            {

                cardlist[i].GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 1), maxdegree - (i * 10 + i * 3), 0);

            }
            if(odd)
            {

                //if (i == handsize / 2 + 1)//center card
                cardlist[i].GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 1), maxdegree - (10 * i), 0);

            }
        }

    }

    public void ResetCardPos()
    {
        handsize = cardlist.Count;
        if (handsize % 2 != 0)
            odd = true;
        else
            odd = false;

        maxdegree = 10 * (handsize / 2);
        leftstart = -(dist * 2) + ((5 - handsize) * 25);
        for (int i = 0; i < cardlist.Count; i++)
        {
            Vector3 newpos = GetCurvePos(i);
            cardlist[i].GetComponent<RectTransform>().position = newpos;
            cardlist[i].GetComponent<RectTransform>().localScale = (new Vector3(1, 1, 1));


            cardlist[i].GetComponent<RectTransform>().rotation = Quaternion.identity;
            if (!odd)//no center
            {

                cardlist[i].GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 1), maxdegree - (i * 10 + i * 3), 0);

            }
            if (odd)
            {

                cardlist[i].GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 1), maxdegree - (10 * i), 0);

            }
        }

    }

    public void SetOnplayArea(bool var)
    {
        inPlayArea = var;
    }



    Vector3 GetCurvePos(int slotno)
    {
        //y = (wx)^2 - m
        // w->curvestrength,m->maximum heigh
         //if ( handsize % 2 == 0)//no center card
         //{ 
         
         //}
        //float lstart = dist*(-2+slotno);
        float x = leftstart + (slotno * dist),y, w = -0.0009f;
        y = (w*(x * x) ) + m;
        return (new Vector3(x+center, y, 0));
        
    }



}
