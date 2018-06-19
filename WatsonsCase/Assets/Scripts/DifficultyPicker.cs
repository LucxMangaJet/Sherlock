using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyPicker : MonoBehaviour {

    [SerializeField] int sceneToLoad;

    GameObject info;
	void Start () {
        info = GameObject.FindGameObjectWithTag("DontDestroyOnLoadObj");
        if (info == null)
            Debug.LogError("Unable to find information Object. Ignore this if scene was not loaded from menu.");
	}
	
	

    public void PickEasy()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void PickHard()
    {
        if (info != null)
        {
            info.transform.position = Vector3.one;
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}
