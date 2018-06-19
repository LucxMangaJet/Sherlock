using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour {
    public int i;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
           
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SceneManager.LoadScene(i);
        }
    }

  public void Load()
    {
        SceneManager.LoadScene(i);
    }
  public void Quit()
    {
        Application.Quit();
    }
}
