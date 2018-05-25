using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectEvidence : MonoBehaviour {
	public string myName;
	public Vector3[] points = new Vector3[4];
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

			for (int i = 0; i < 4; i++) {
				points [i] = transform.GetChild (i).position;

		}
	}

	void OnDrawGizmos(){

			Gizmos.color = Color.red;
			Gizmos.DrawLine (points [0], points [1]);
			Gizmos.DrawLine (points [1], points [3]);
			Gizmos.DrawLine (points [2], points [3]);
			Gizmos.DrawLine (points [2], points [0]);

	}
}
