﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;

public class MenuScript : MonoBehaviour
{

    [SerializeField] int newGameSceneIndex, loadGameSceneIndex;
    [SerializeField] GameObject StartMenu, OptionsMenu, NewGameText, VolumeText, ContinueButton, InformationCarrierPref;

    //relsolutions
    [SerializeField] Dropdown resDropdown;
    Resolution[] resolutions;
    //Volume Slider
    [SerializeField] AudioMixer mixer;
    //async loading
    // private bool UpdatedText = false;
    // private AsyncOperation asyncScene;


    void Start()
    {
        //resDropdown
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();
        List<string> options = new List<string>();
        int myResIndx = 0;
        foreach (Resolution r in resolutions)
        {
            if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height)
                myResIndx = options.Count;


            options.Add(r.width + " x " + r.height);
        }
        resDropdown.AddOptions(options);
        resDropdown.value = myResIndx;
        resDropdown.RefreshShownValue();

        //Async loading
        //LoadSceneAsync (sceneToLoadIndex);

        //Volume
        SetVolume(0);

        //LoadGame / continue
        GameObject info = Instantiate(InformationCarrierPref, null);
        DontDestroyOnLoad(info);

        if (File.Exists(Application.persistentDataPath + "/SaveGame"))
        {
            Debug.Log("Savefile found: allowing Continue");
            ContinueButton.SetActive(true);
        }
        else
        {
            Debug.Log("Savefile not found");
        }


    }

    //menuVisuals
    public void EnterSettings()
    {
        StartMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void QuitSettings()
    {
        StartMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    //Quality
    public void SetQuality(int qualityIndx)
    {
        QualitySettings.SetQualityLevel(qualityIndx);
        Debug.Log("Set Quality to: " + QualitySettings.names[qualityIndx]);
    }

    //Resolutions 
    public void SetResolution(int resIndx)
    {
        Resolution res = resolutions[resIndx];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        Debug.Log("Resolution set to: " + res.width + " x " + res.height);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen set to: " + isFullscreen);
    }

    //Audio
    public void SetVolume(float volume)
    {
        mixer.SetFloat("Volume", volume);
        VolumeText.GetComponent<Text>().text = (int)((volume + 80) * (1 / 0.8)) + "%";
    }

    //quit
    public void Quit()
    {
        Debug.Log("Application.Quit()");
        Application.Quit();
    }

    //scene loading
    public void ChangeScene(int sceneIndx)
    {
        SceneManager.LoadScene(sceneIndx);
    }


    public void LoadGame()
    {
        GameObject.FindGameObjectWithTag("DontDestroyOnLoadObj").name = "Load";
        SceneManager.LoadScene(loadGameSceneIndex);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(newGameSceneIndex);
    }

    public void LoadCustomGame()
    {
        string scenePath= Application.persistentDataPath + "/customlevelscenes";
        string assetsPath = Application.persistentDataPath + "/customlevelassets";
        AssetBundle aa = AssetBundle.LoadFromFile(assetsPath);
        AssetBundle ab = AssetBundle.LoadFromFile(scenePath);
        string[] scenes = ab.GetAllScenePaths();
        string sceneName = Path.GetFileNameWithoutExtension(scenes[0]);
        SceneManager.LoadScene(sceneName);
    }
}
