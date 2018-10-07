using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holmes_Control;
public class GatingHandler : MonoBehaviour {
    GameState state;
    // Use this for initialization

    [SerializeField] int caseClues = 0;
	void Start () {
        state = GetComponent<GameState>();
        state.updatedVarEvent += CheckVarUpdate;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void CheckVarUpdate(string s, bool b)
    {
        Debug.Log("Setting: " + s + " as " + b);
        if( s == "OsgarNoAlibi" || s == "LyingOsgar" || s == "OsgarWantsCompany" || s == "OsgarPaysFred"|| s == "OsgarBlackmail" || s== "FoundSword")
        {
            if (b)
            {
                caseClues += 1;
            }
        }

        if (s == "FoundKey" &&b)
        {
            Debug.Log("Key Found Opening respective door");
        }

        if(caseClues > 5)
        {
            Debug.LogError("CASE SOLVED");
        }
    }
}
