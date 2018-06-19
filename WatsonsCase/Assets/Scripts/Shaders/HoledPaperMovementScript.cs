using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoledPaperMovementScript : MonoBehaviour {
    public bool active = true;
    [Range(0, 10)]
    public float speed = 1;
    float _Mov = 0;
    Material mat;
	
	void Start () {
        mat = GetComponent<MeshRenderer>().material;
	}
	
	
	void Update () {
        if (active)
        {
            _Mov += speed * Time.deltaTime;
            mat.SetFloat("_Mov", _Mov);
        }
	}
}
