using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationHandler : MonoBehaviour {

	[SerializeField] bool inConversation=false;
	[SerializeField] TextAsset scheduleText;


	Dictionary<int,NavigationMovement> schedule = new Dictionary<int, NavigationMovement>(); 
	public NavigationMovement currentMovement=null;
	public int currentMovementIndx =0;
	NavMeshAgent agent;
	AnimationHandler anim;
	Holmes_Control.GameState gameState;
	Vector3 playerPos = Vector3.zero;
	float rotSpeed=4;

	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<AnimationHandler> ();
		gameState = GameObject.FindGameObjectWithTag("Main").GetComponent<Holmes_Control.GameState> ();
		if(scheduleText != null)
		TranslateTextToDictionary ();
		gameState.Time_NewMinute += TryUpdateCurrentMovement;
	}
	
	// Update is called once per frame
	void Update () {
		if (inConversation) {
			RotateTowards (playerPos);
		} else {
			if (agent.remainingDistance < 0.5f) {
				if (currentMovement != null) {
					if (currentMovementIndx < currentMovement.targets.Length - 1) {
						currentMovementIndx++;
						agent.SetDestination (currentMovement.targets [currentMovementIndx]);
					} else if (currentMovement.continuous) {
						currentMovementIndx = 0;
						agent.SetDestination (currentMovement.targets [currentMovementIndx]);
					}
				}
			}
		}
	}

				public void EnterConversation(){
		inConversation = true;
		agent.isStopped = true;
		playerPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
	}

	public void ExitConversation(){
		inConversation = false;
		agent.isStopped = false;
		playerPos = Vector3.zero;

	}

	void TryUpdateCurrentMovement(int c){
		NavigationMovement n;
		if(schedule.TryGetValue (c, out n)){
			currentMovement = n;
			currentMovementIndx = 0;
			agent.SetDestination(currentMovement.targets[currentMovementIndx]);
		}
	}


	// based on: https://answers.unity.com/questions/540120/how-do-you-update-navmesh-rotation-after-stopping.html
	void RotateTowards(Vector3 pos){
		if (pos == Vector3.zero)
			throw new UnityException ("Error in RotateTowards");
		Vector3 direction = (pos - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
	}

	void TranslateTextToDictionary(){
		string content = scheduleText.text;


		string[] s = DevideStringIntoLines (content);

		for (int i = 0; i < s.Length; i++) {
			string j = s [i];
			if (j.Length < 10)
				continue;
			
			int key;
			bool looping = false;
			Vector3[] positions;

			key = int.Parse ("" + j [0] + j [1]) * 60 + int.Parse ("" + j [3] + j [4]);
			looping = (j [j.Length - 1] == 'L');

			List<string> stringPositions = new List<string> ();
			string temp = "";
			for (int k = 7; k < j.Length; k++) {
				if (j [k] == '}') {
					stringPositions.Add (temp);
					break;
				} else if (j [k] == ',') {
					stringPositions.Add (temp);
					temp = "";
				} else {
					temp += j [k];
				}
			}

			positions = new Vector3[stringPositions.Count];

			for (int m = 0; m < stringPositions.Count; m++) {
				positions [m] = gameState.LocGet (stringPositions [m]);
			}
			schedule.Add (key, new NavigationMovement(looping,positions));
		}
		//todo
	}

	//Also removes comments
	string[] DevideStringIntoLines(string input){
		//devide in lines
		string[] s = input.Split ((char)13);

		//remove chars < 33
		for (int k = 0; k < s.Length; k++) {
			string j = s [k];

			string temp = "";

				for (int i = 0; i < j.Length; i++) {
					if ((int)j [i] > 33) {
						if ((int)j [i] == 47) { // stop reading when reaching "/" comment
							break;
						}
						temp += "" + j [i];
					}
				}
			s [k] = temp;
		}

		return s;
	}



    public void LoadNavigationMovement( NavigationMovement n ,int x)
    {
        currentMovement = n;
        currentMovementIndx = x;
        agent.SetDestination(currentMovement.targets[currentMovementIndx]);
    }
}


public class NavigationMovement{
    public bool continuous;
    public Vector3[] targets;

    public NavigationMovement(bool _continuous,Vector3[] _targets){
		continuous = _continuous;
		targets = _targets;
	}
}

[System.Serializable]
public class NavigationMovementSerializable
{
    public bool continuous;
    public Holmes_Control.Vector3Serializable[] targets;

    public NavigationMovementSerializable(NavigationMovement n)
    {
       

        continuous = n.continuous;
        targets = new Holmes_Control.Vector3Serializable[n.targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = Holmes_Control.Vector3Serializable.Make(n.targets[i]);
        }
    }

    static public NavigationMovementSerializable Make(NavigationMovement n)
    {
        return new NavigationMovementSerializable(n);
    }

    static public NavigationMovement Read(NavigationMovementSerializable n)
    {
        Vector3[] tar = new Vector3[n.targets.Length];
        for (int i = 0; i < tar.Length; i++)
        {
            tar[i] = Holmes_Control.Vector3Serializable.Read(n.targets[i]);
        }
        return new NavigationMovement(n.continuous, tar);
    }
}

