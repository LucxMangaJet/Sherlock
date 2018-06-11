using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationHandler : MonoBehaviour {
	private Animator anim;
	private NavMeshAgent agent;
	[SerializeField] bool isWalking = false;
    [SerializeField] bool forceSpecialAnim = false;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();

        if (forceSpecialAnim)
            anim.SetBool("s1", true);
        
    }
	void Update(){


        if (forceSpecialAnim)
        {
            return;
        }

        if (isWalking) {
			if (agent.velocity.magnitude < 0.01)
				SetisWalking (false);
		} else {
            if (agent.velocity.magnitude > 0.01)
                SetisWalking (true);
		}
	

	}
	
	public void SetisWalking(bool b){
		isWalking = b;
		anim.SetBool ("isMoving", b);

	}
}
