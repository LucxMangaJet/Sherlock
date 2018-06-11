using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Holmes_Import;

namespace Holmes_Editor{
public class Editor_TestDialogue : EditorWindow {

		private TextAsset t;
		private bool debug;

	[MenuItem("Holmes/TestTXTDialogues")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(Editor_TestDialogue));
	}

	private void OnGUI()
	{
			if (t == null)
		{
				t = new TextAsset();
		}

		GUI.enabled = true;
			GUILayout.Label ("Use this window to test your Dialogue .txt files");
			GUILayout.Label ("(Notice: Rerun test after fixing a mistake)");
			t = EditorGUILayout.ObjectField("Insert txt here: ", t, typeof(TextAsset), false) as TextAsset;
			debug = EditorGUILayout.Toggle ("Debug: ", debug);

		if (GUILayout.Button("Test"))
		{
				GameObject g = Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube));
				g.AddComponent<DialogueParser> ();
				DialogueParser p = g.GetComponent<DialogueParser> ();
				p.RunTest (t, debug);
				Destroy (g);
				Debug.Log ("Test Compleated");
		}
	}

}

}