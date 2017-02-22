using UnityEngine;
using System.Collections;

public class AnimateHoloUV : MonoBehaviour {
    public int matIndex = 0;
    public Vector2 AnimRate = new Vector2(1f, 2f);
    public string texturename = "_MainTex";
    Vector2 Offset = Vector2.zero;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Offset += (AnimRate * Time.deltaTime);
      if(gameObject.GetComponent<Renderer>().enabled)
          gameObject.GetComponent<Renderer>().material.SetTextureOffset(texturename, Offset);
	}
}
