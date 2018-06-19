using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour {
	Rigidbody rb;
	public float lockCd=0;
	float freezeLock =0;
	public bool toOpen=true;

	public bool isLocked =false;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v	= transform.localEulerAngles;
		if (freezeLock <= 0) {
			if (v.y < 7 || v.y > 353) {
				rb.angularVelocity = Vector3.zero;
				transform.localEulerAngles = Vector3.zero;
				toOpen = true;
			} else if (v.y < 277 && v.y > 263) {
				rb.angularVelocity = Vector3.zero;
				transform.localEulerAngles = new Vector3 (0, -90, 0);
				toOpen = false;
			} 
		} else {
			freezeLock -= Time.deltaTime;
		}
		if(lockCd>0){
			lockCd -= Time.deltaTime;
	}



	}

	public void Open(){
		if (isLocked)
			return;
		lockCd = 1f;
		rb.AddForceAtPosition (transform.right * 45, transform.GetChild (0).position);
	}

	public void Interact(){
		if (isLocked) {
			return;
		}

		if (lockCd > 0)
			return;
		rb.angularVelocity = Vector3.zero;
		lockCd = 1f;
		freezeLock = 0.2f;
		if (toOpen) {
			rb.AddForceAtPosition (transform.right * 45, transform.GetChild (0).position);
		} else {
			rb.AddForceAtPosition (-transform.right * 45, transform.GetChild (0).position);
		}
	}
}
