using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class LevelEditorProperties {

    //singleton
    private static LevelEditorProperties instance;


    private Dictionary<string, bool> variables;
    private List<string> textEvidences, objEvidences;
    private List<string> characters;


    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    public LevelEditorProperties()
    {
        variables = new Dictionary<string, bool>();
        textEvidences = new List<string>();
        objEvidences = new List<string>();
        characters = LevelEditorController.LoadCharacterNames();

    }

    public static void Setup()
    {
        if(instance == null)
        {
            instance = new LevelEditorProperties();
        }
    }

    public static Dictionary<string, bool> GetVariables()
    {
        return instance.variables;
    }

    public static List<string> GetTextEvidences()
    {
        return instance.textEvidences;
    }

    public static List<string> GetObjEvidences()
    {
        return instance.objEvidences;
    }

    public static List<string> GetCharacters()
    {
        return instance.characters;
    }

    public static void AddCharacter(string name)
    {
        if (!instance.characters.Contains(name))
        {
            instance.characters.Add(name);
        }
    }

    public static void RemoveCharacter(int indx)
    {
        string name = instance.characters[indx];
        instance.characters.RemoveAt(indx);


        LevelEditorController.DeleteCharacterData(name);
    }

    public static void AddOrChangeVariable(string name,bool state)
    {
        instance.variables[name] = state;
    }

    public static void RemoveVariable(string name)
    {
        if (!instance.variables.Remove(name))
        {
            throw new System.Exception("Trying to remove inexisting variable.");
        }
        
    }

    public static void AddTextEvidence(string name)
    {
        if (!instance.textEvidences.Contains(name))
        {
            instance.textEvidences.Add(name);
        }
        
    }

    public static void RemoveTextEvidence(string name)
    {
        instance.textEvidences.Remove(name);
    }

    public static void AddObjEvidence(string name)
    {
        if (!instance.objEvidences.Contains(name))
        {
            instance.objEvidences.Add(name);
        }

    }

    public static void RemoveObjEvidence(string name)
    {
        instance.objEvidences.Remove(name);
    }

    public static void SetSpawn(Transform _transform)
    {
        instance.spawnPosition = _transform.position;
        instance.spawnRotation = _transform.rotation;

        Transform tr = GameObject.FindGameObjectWithTag("Main").transform.parent;
        tr.position = _transform.position;
        tr.rotation = _transform.rotation;
    }

}
