using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerInfo : MonoBehaviour {

    int Gold;
    public Text goldtext; 
	// Use this for initialization
	void Start () {
        Gold = 5000;
        goldtext.text = Gold.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public bool AddPlayerGold(int amount)
    {
        Gold += amount;
        goldtext.text = Gold.ToString();
        return true;
    }
    public bool SpendPlayerGold(int amount)
    {
        if(Gold < amount)
            return false;

        Gold -= amount;
        goldtext.text = Gold.ToString();

        return true;
    }
}
