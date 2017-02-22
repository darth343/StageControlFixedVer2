using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour {
    public string m_sceneName;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GoToScene()
    {
        SceneManager.LoadScene(m_sceneName);
    }
}
