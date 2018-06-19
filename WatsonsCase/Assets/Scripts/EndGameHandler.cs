﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameHandler : MonoBehaviour {

    [SerializeField] Character holmes;
    EvidenceHandler evidenceHandler;
    Holmes_Control.GameState state;
    int scoreChoice = 0, scoreEvidences = 0,scoreChoiceAndEvidences, scoreDialogs = 0,finalScore;

    /*
     * Write reassumiing
     * write Sherlock comment
     * Calculate Dialogs score
     * calculate final score
     */

    // Use this for initialization
    void Start() {
        evidenceHandler = GetComponent<EvidenceHandler>();
        state = GetComponent<Holmes_Control.GameState>();
        evidenceHandler.GivenInEvidence += CollectGivenInEvidence;
        state.updatedVarEvent += CheckVarUpdate;
    }


    void CheckVarUpdate(string s, bool b)
    {
        if (s == "E_Conclusion")
        {
            CalculateScores();
            SetConclusionDialog();
            Debug.Log("Starting Conclusion");
        } else if (s == "E_ConfirmEnd")
        {
            Debug.Log("End Confirmed. Starting Credits");
            SceneManager.LoadScene(7);
        }
    }

    void CalculateScores()
    {
        Dictionary<string, bool> vars = state.variables;
        //Calculate score Choices
        if (vars["EA_When_24"])
            scoreChoice += 25;
        if (vars["EA_How_Bayonett"])
            scoreChoice += 25;
        if (vars["EA_Who_Clarice"])
            scoreChoice += 25;
        if (vars["EA_Why_Clarice_Compositions"])
            scoreChoice += 25;

        scoreChoiceAndEvidences = Mathf.RoundToInt((scoreChoice * 65 + scoreEvidences * 35) / 100);
        //scoreDialogs 13 in total

        for (int i = 0; i < 13; i++)
        {
            if (vars["SCORE_" + i])
                scoreDialogs += 8;
        }
        Debug.Log(scoreDialogs);
        
        scoreDialogs = Mathf.Clamp(scoreDialogs,0, 100 );

        //final score
        finalScore = Mathf.RoundToInt((scoreChoiceAndEvidences * 40 + scoreDialogs *60) / 100);
    }


    void SetConclusionDialog()
    {
        Dictionary<string, bool> vars = state.variables;
        string Dialog;
        string when, how, who, why;

        #region Defining When How Who Why
        //when
        if (vars["EA_When_24"])
        {
            when = "2 and 4pm";
        }
        else
        {
            when = "5 and 6pm";
        }

        //how
        if (vars["EA_How_Bayonett"])
        {
            how = "bayonett";
        } else if (vars["EA_How_Knife"])
        {
            how = "kitchen knife";
        }else if (vars["EA_How_Pipe"])
        {
            how = "heating pipe";
        }
        else
        {
            how = "fireplace poker";
        }

        //who

        if (vars["EA_Who_Clarice"])
        {
            who = "Clarice";
        }
        else if (vars["EA_Who_Elisabeth"])
        {
            who = "Elisabeth";
        } else if (vars["EA_Who_Robert"])
        {
            who = "Robert";
        }
        else
        {
            who = "Theodore";
        }

        //why
        if (vars["EA_Why_Clarice_Compositions"])
        {
            why = "he was selling her compositions as his own";
        } else if (vars["EA_Why_Clarice_Love"])
        {
            why = "they had an affair and he wanted to stop it";
        } else if (vars["EA_Why_Elisabeth_Love"])
        {
            why = "she discovered that he was having an affair";
        } else if (vars["EA_Why_ELisabeth_Abuse"])
        {
            why = "he was being abusive towards her and she couldm't take it anymore";
        } else if (vars["EA_Why_Robert_Anger"])
        {
            why = "Richard wanted to expell him";
        } else if (vars["EA_Why_Robert_Revenge"])
        {
            why = "Robert wanted to revenge his father killed by Richard during the war";
        } else if (vars["EA_Why_Theodore_Jelousy"])
        {
            why = "Richard wanted to expell him, which drove him mad";
        }
        else
        {
            why = "Richard was selling his compositions as his own";
        }

        #endregion

        Dialog = "Let's reassume what you said until now... " + who + " killed Richard between " + when + " with a " + how + " because " + why + ". ";

        Dialog += "SCORES:. Conclusion Score: " + scoreChoiceAndEvidences + "%. Dialogs Score: " + scoreDialogs + "%. <b>Final Score: " + finalScore + "%</b>. ";

        holmes.dialogueOptions[holmes.dialogueOptions.Length - 1].answer = Dialog;

    }

    void CollectGivenInEvidence(string[] s, string phase)
    {
        //calculate Evidences score

       
        switch (phase)
        {
            //rigormortis
            case "EE_When":
                if (StringArrayContainsElement(s, "RigorMortis"))
                    scoreEvidences += 25;
                break;

            //bloody bayonet and stabwounds
            case "EE_How":
                bool[] res = StringArrayContainsElements(s, new string[2] { "BloodyBayonett", "StabWounds" });
                foreach (bool b in res)
                {
                    if (b)
                        scoreEvidences += Mathf.CeilToInt(25 / res.Length);
                }
                break;
            //female murderer and siblingswroteonlist
            case "EE_Who":
                 res = StringArrayContainsElements(s, new string[2] {"SiblingsWroteOnList", "FemaleMurderer" });
                foreach (bool b in res)
                {
                    if (b)
                        scoreEvidences += Mathf.CeilToInt(25 / res.Length);
                }
                break;
            //confession Clarice
            case "EE_Why":
                if (StringArrayContainsElement(s, "ConfessionWhy"))
                    scoreEvidences += 25;
                break;

            default:
                break;
        }

        scoreEvidences = Mathf.Clamp(scoreEvidences,0, 100);
    }



    // extra methods 
    bool[] StringArrayContainsElements(string[] container, string[] toContain)
    {
        bool[] result = new bool[toContain.Length];
        


        for (int i = 0; i < toContain.Length; i++)
        {
            result[i] = false;
            foreach (string s in container)
            {
                if (s == toContain[i])
                    result[i] = true;
            }
        }


        return result;
    }

    bool StringArrayContainsElement(string[] container, string toContain)
    {
        bool result = false;
            foreach (string s in container)
            {
                if (s == toContain)
                    result = true;
            }

        return result;
    }

}
