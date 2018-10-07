using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Holmes_Menu;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Holmes_Control { 
public class SaveGameHandler : MonoBehaviour {

    public bool SceneIsNewGame = true;

	VisualFeedbackHandler visualFeedback;

	// Use this for initialization
	void Start () {
		visualFeedback = GetComponent<VisualFeedbackHandler>();
	}
	
    public void SaveGame()
        {
         Vector3 playerPos, playerRot;
         float time;
            bool hardMode;
         string[] variableKey;
         bool[] variableValue;
            UnitedEvidence[] evidences;
         List<CharacterSaveFormat> characters = new List<CharacterSaveFormat>();


            //player;
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            playerRot = GameObject.FindGameObjectWithTag("Player").transform.eulerAngles;
            //time;
            time = GetComponent<GameState>().time;
            //hard mode
            hardMode = GetComponent<GameState>().HardMode;
            //Variables;
            variableKey = GetComponent<GameState>().variables.Keys.ToArray<string>();
            variableValue = GetComponent<GameState>().variables.Values.ToArray<bool>();
            //Evidences;
            evidences = GetComponent<EvidenceHandler>().GetEvidences().ToArray();
            // Characters;
            // source of next line: https://answers.unity.com/questions/271910/c-sorting-a-list-of-gameobjects-alphabetically.html
            GameObject[] charObjects = GameObject.FindGameObjectsWithTag("Character").OrderBy(o => o.name).ToArray();

            foreach (GameObject g in charObjects)
            {
                characters.Add(g.GetComponent<Character>().GetCharacterInSaveFormat());
            }


              SaveFile save = new SaveFile(playerPos,playerRot,time,hardMode,variableKey,variableValue,evidences,characters);
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, save);
                byte[] bytz = ms.ToArray();
                  EncryptDecryptBytes(ref bytz);
            FileStream file = File.Create(Application.persistentDataPath + "/SaveGame "); //you can call it anything you want
            file.Write(bytz, 0, bytz.Length);
            file.Close();

            //save Pngs of drawings;
            for (int i = 0; i <evidences.Length; i++)
            {
                if (evidences[i].drawings != null)
                {
                    for (int j = 0; j < evidences[i].drawings.Length; j++)
                    {
                        byte[] bytes = evidences[i].drawings[j].texture.EncodeToPNG();
                        EncryptDecryptBytes(ref bytes);
                        File.WriteAllBytes(Application.persistentDataPath + "/" + i + j , bytes);
                    }
                }
            }

			// trigger visual feedback for saving the game
			visualFeedback.ShowVisualFeedback("Game saved!");

            Debug.Log("Saved Successfully at: " +Application.persistentDataPath + "/SaveGame");
        }


        public bool LoadSaveFile()
        {
            if (!File.Exists(Application.persistentDataPath + "/SaveGame"))
            {
                Debug.LogError("Failed loading: non existent Savefile");
                return false;
            }
            

            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/SaveGame");
            EncryptDecryptBytes(ref bytes);
            ms.Write(bytes, 0, bytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            SaveFile save = (SaveFile)bf.Deserialize(ms);
            

            GameObject[] charObjects = GameObject.FindGameObjectsWithTag("Character").OrderBy(o => o.name).ToArray();
            if(charObjects.Length != save.characters.Count)
            {
                Debug.LogError("Loading failed: There are not the same amount of characters in the save file as in the game");
                return false;
            }


            //player
            GameObject.FindGameObjectWithTag("Player").transform.position = Vector3Serializable.Read(save.playerPos);
            GameObject.FindGameObjectWithTag("Player").transform.eulerAngles = Vector3Serializable.Read(save.playerRot);
            //time
            GetComponent<GameState>().time = save.time;

            GetComponent<GameState>().HardMode = save.hardMode;
            //variables;
            Dictionary<string, bool> vars = new Dictionary<string, bool>();
            for (int i = 0; i < save.variableKey.Length; i++)
            {
                vars.Add(save.variableKey[i], save.variableValue[i]);
            }
            GetComponent<GameState>().variables = vars;
            //evidences
            List<UnitedEvidence> s = new List<UnitedEvidence>();

            for (int i = 0; i < save.evidences.Length; i++)
            {
                s.Add(UnitedEvidenceSerializable.Read(save.evidences[i],i));
            }

            GetComponent<EvidenceHandler>().SetEvidences(s);
            GetComponent<EvidenceHandler>().RefitEvidenceContent();

            //load characters
            for (int i = 0; i < charObjects.Length; i++)
            {
                charObjects[i].GetComponent<Character>().LoadCharacterFromSave(save.characters[i]);
            }

			// trigger visual feedback for saving the game
			visualFeedback.ShowVisualFeedback("Savefile loaded!");

            Debug.Log("Loading successful");
            return true;
        }


       public static void EncryptDecryptBytes (ref byte[] bytes)
        {
            const string Code = "FollowTheWhiteRabbit";
            byte[] CodeBytes = Encoding.ASCII.GetBytes(Code);

            for (int i = 0; i < CodeBytes.Length; i++)
            {
                CodeBytes[i] = (byte)~CodeBytes[i];
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i]^CodeBytes[i%CodeBytes.Length]);
            }
        }
    }
   
    [System.Serializable]
    public class SaveFile
    {
        public Vector3Serializable playerPos, playerRot;
        public float time;
        public bool hardMode;
        public string[] variableKey;
        public bool[] variableValue;
        public UnitedEvidenceSerializable[] evidences;
        public List<CharacterSaveFormat> characters;

        public SaveFile(Vector3 _playerPos, Vector3 _playerRot, float _time,bool _hardmode, string[] _variableKey, bool[] _variableValue, UnitedEvidence[] _evidences ,List<CharacterSaveFormat> _characters )
        {
            playerRot =Vector3Serializable.Make(_playerRot);
            playerPos = Vector3Serializable.Make(_playerPos);
            time = _time;
            hardMode = _hardmode;
            variableKey = _variableKey;
            variableValue = _variableValue;

            evidences = new UnitedEvidenceSerializable[_evidences.Length];

            for (int i = 0; i < _evidences.Length; i++)
            {
                evidences[i] = UnitedEvidenceSerializable.Make(_evidences[i]);
            }
           
            characters = _characters;
        }

    }
    [System.Serializable]
public class Vector3Serializable
    {
        public float x;
        public float y;
        public float z;

        public Vector3Serializable( float _x,float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static Vector3Serializable Make(Vector3 v)
        {
            return new Vector3Serializable(v.x, v.y, v.z);
        }

        public static Vector3 Read(Vector3Serializable v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

    }


}


