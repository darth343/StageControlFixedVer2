using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

    int Gold;

	// Use this for initialization
	void Start () {
        Gold = 500;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void AddPlayerGold(int amount)
    {
        Gold += amount;
    }
    public void SpendPlayerGold(int amount)
    {
        Gold -= amount;
    }
}
