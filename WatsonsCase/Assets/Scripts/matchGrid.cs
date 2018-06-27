using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class matchGrid : MonoBehaviour {

	[SerializeField]
	bool half = false;

	[SerializeField]
	bool clearDoubles = false;


    void Update()
	{
		//if (gameObject == Selection.activeGameObject) {
			if (half == false) {
				transform.position = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y), Mathf.Round (transform.position.z));
			} else {
				transform.position = new Vector3 (Mathf.Round (transform.position.x * 2) / 2, Mathf.Round (transform.position.y * 2) / 2, Mathf.Round (transform.position.z * 2) / 2);
			}
		//}

		//removes doubled scipts
		if (clearDoubles == true) {
			int count = 0;
			foreach (MonoBehaviour matchGrid in GetComponents<matchGrid>()) {
				count++;
				if (count > 1) {
					Component.DestroyImmediate (this);
				}
			}
			clearDoubles = false;
		}
	}

	Color red = new Color ();
}
	