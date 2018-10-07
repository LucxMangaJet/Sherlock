using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Holmes_Control
{
    public class GameState : MonoBehaviour
    {
        public bool savable;
        public bool SaveFileLoaded = false;
        public float time = 12 * 60 * 60;
        public int timeInMin;
        public bool HardMode=false;
        public Dictionary<string, Vector3> locationsByName;
        public Dictionary<string, bool> variables;
        [SerializeField] GameObject locationContainer;
        [SerializeField] List<string> textEvidences, objEvidences;
        [SerializeField] bool LoadVarsAndEvidencesFromLevelEditor = false;
        [SerializeField] TextAsset i_VarsandEvidences;


        public delegate void intDelegate(int i);
        public delegate void UpdateVarDel(string s, bool b);
        public event intDelegate Time_NewMinute;
        public event UpdateVarDel updatedVarEvent;

        void Awake()
        {
            variables = new Dictionary<string, bool>();
            locationsByName = new Dictionary<string, Vector3>();
            textEvidences = new List<string>();
            objEvidences = new List<string>();
            //Time_NewMinute += Save;

            if (LoadVarsAndEvidencesFromLevelEditor)
            {
                LoadVariablesFromLevelEditor();   
            }
            else
            {
                ReadVariablesAndEvidences();
            }
            
            SaveLocationsInDictionary();

            GameObject info = GameObject.FindGameObjectWithTag("DontDestroyOnLoadObj");
            if (info != null)
            {
                if (info.transform.position.x == 1)
                {
                    HardMode = true;
                }

                if (savable&&info.name=="Load")
                {
                    Debug.Log("Loading Savefile");
                    GetComponent<SaveGameHandler>().LoadSaveFile();
                    SaveFileLoaded = true;
                }
            }
            else
            {
                Debug.LogError("ERROR: Loading instruction not found: NOT loading savefile. (Ignore this if you started the scene indipendently without going through the menu)");
            }
        }

        

        void Update()
        {
            float oldTime = time;
            time += Time.deltaTime;
            timeInMin = (int)Mathf.Floor(time / 60);

            if (timeInMin > Mathf.Floor(oldTime / 60))
            {
                if (Time_NewMinute != null)
                {
                    Time_NewMinute(timeInMin);
                }
            }

        }

        // variables
        public bool VarGet(string key)
        {
            bool j = false;
            if (!variables.TryGetValue(key, out j))
                throw new DialogueParserException("Variable: " + key + " not found ");
            return j;
        }

        public void VarSet(string key, bool value)
        {
            if (variables.ContainsKey(key))
            {
                variables[key] = value;
                if (updatedVarEvent != null)
                {
                    updatedVarEvent(key, value);
                }
            }
            else
            {
                throw new DialogueParserException("Variable: " + key + " not found ");
            }
        }

        public bool VarCheck(string key)
        {
            return variables.ContainsKey(key);
        }


        public bool EvidenceTextCheck(string s)
        {
            return textEvidences.Contains(s);
        }

        public bool EvidenceObjCheck(string s)
        {
            return objEvidences.Contains(s);
        }

        public bool EvidencesCheck(string s)
        {
            return textEvidences.Contains(s) || objEvidences.Contains(s) || s == "ANY";
        }

        public void CallUpdateVarEvent(string s, bool b)
        {
            updatedVarEvent(s, b);
        }

        public Vector3 LocGet(string key)
        {
            Vector3 j = Vector3.zero;
            if (!locationsByName.TryGetValue(key, out j))
                throw new DialogueParserException("Location: " + key + " not found ");
            return j;
        }


        public void Save(int i)
        {
            if (savable)
            {
                Debug.Log("Saving occured at: " + i);
             GetComponent<SaveGameHandler>().SaveGame();
            }  
        }

        void SaveLocationsInDictionary()
        {
            for (int i = 0; i < locationContainer.transform.childCount; i++)
            {
                GameObject g = locationContainer.transform.GetChild(i).gameObject;
                locationsByName.Add(g.name, g.transform.position);
            }
        }

        void ReadVariablesAndEvidences()
        {
            string s = i_VarsandEvidences.text;
            string g = "";
            for (int i = 0; i < s.Length; i++)
            {
                char j = s[i];
                if (j.GetHashCode() > 32)
                {
                    g += j;
                }
            }

            string[] o = new string[3];
            int indx = -1;
            bool inName = false;
            o[0] = ""; o[1] = ""; o[2] = "";

            for (int i = 0; i < g.Length; i++)
            {
                if (indx > 2)
                    break;

                char j = g[i];
                if (j == ':')
                {
                    if (inName)
                    {
                        indx++;
                        inName = false;
                    }
                    else
                    {
                        inName = true;
                    }
                }
                else
                {
                    if (!inName)
                        o[indx] += j;
                }
            }

            string[] k = o[0].Split(';');
            //vars
            foreach (string l in k)
            {
                if (l.Length > 2)
                {

                    if (l.Contains("!"))
                    {
                        variables.Add(l.Replace("!", ""), false);
                    }
                    else
                    {
                        variables.Add(l, true);
                    }

                }
            }

            k = o[1].Split(';');
            //textEvidence
            foreach (string u in k)
            {
                if (u.Length > 2)
                {
                    textEvidences.Add(u);
                    //	Debug.Log (u);
                }
            }

            k = o[2].Split(';');
            //textEvidence
            foreach (string w in k)
            {
                if (w.Length > 2)
                {
                    objEvidences.Add(w);
                    //	Debug.Log (w);
                }
            }
        }

        private void LoadVariablesFromLevelEditor()
        {
            
        }
    }




}
