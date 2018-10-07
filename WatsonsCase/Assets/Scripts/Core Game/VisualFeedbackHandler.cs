using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holmes_Menu;
using UnityEngine.UI;

/// <summary>
/// The Visual feedback handler displays Visal Feedback for most of the Evidence Actions.
/// It's called from the Evidence Handler
/// </summary>


public class VisualFeedbackHandler : MonoBehaviour {

	[SerializeField]
	GameObject VisualFeedbackCanvas;
	[SerializeField]
	GameObject VisualFeedbackText;

	public void ShowVisualFeedback (string FeedbackText) {

		//enable the necessary UI layers
		VisualFeedbackCanvas.SetActive(true);
		VisualFeedbackText.SetActive(true);

		//set the feedback text
		VisualFeedbackText.GetComponent<Text>().text = FeedbackText;

		//hide the text in 2 seconds
		Invoke("HideVisualFeedback",2f);
	}

	void HideVisualFeedback () {
		VisualFeedbackCanvas.SetActive(false);
		VisualFeedbackText.SetActive(false);
	}
}
