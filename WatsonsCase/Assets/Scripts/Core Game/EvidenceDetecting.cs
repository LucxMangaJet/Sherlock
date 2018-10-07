using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holmes_Menu;
//[ExecuteInEditMode]
public class EvidenceDetecting : MonoBehaviour
{
	[SerializeField] Camera _camera ,_mainCamera;
	[SerializeField] int resWidth = 600, resHeight = 400,brushSize=3;
	[SerializeField] Material mat;
	[SerializeField] GameObject ui_drawingAnalysis,ui_sprite,ui_saveDrawing;
    [SerializeField] Texture2D paperTex1, paperTex2;
	bool usable =true;

	//drawing capturing
	Drawing latestDrawing = null, latestDrawingModified=null;
	Vector3[]  viewSpaceDrawingRectCorners = new Vector3[4] ;
	int selectionState=0;
	bool inSelection = false;
	Vector3 selectionPoint1 = Vector3.zero, selectionPoint2=Vector3.zero;
	Texture2D tempText= null;

	public event DialogueHandler.EmptyDel enterDrawing;
	public event DialogueHandler.EmptyDel exitDrawing;

	void Start(){
		GetComponent<DialogueHandler> ().enterDialogueEvent += Disable;
		GetComponent<DialogueHandler> ().exitDialogueEvent += Enable;
		GetComponent<MenuHandler> ().enterMenu += Disable;
		GetComponent<MenuHandler> ().exitMenu += Enable;
        GetComponent<CutsceneHandler>().CutSceneStartEvent += EnterCutScene;
        GetComponent<CutsceneHandler>().CutSceneEndEvent += ExitCutscene;

      
    }

	void Update(){
		if ((Input.GetKeyDown (KeyCode.P)|| Input.GetKeyDown(KeyCode.Q)) &&usable) {
			ui_drawingAnalysis.SetActive (true);
			Disable ();
			inSelection = true;
			enterDrawing ();
			CalcDrawingRect ();
			latestDrawing = TakeScreenShot ();
			ui_sprite.GetComponent<Image> ().sprite = latestDrawing.sprite;
            GetComponent<SoundHandler>().PlayClip(SoundHandler.ClipEnum.UIDrawing, SoundHandler.OutputEnum.UI);
		}

	}



	void HandleSelection(){
		Vector3 mousePos =_mainCamera.ScreenToViewportPoint(Input.mousePosition);
		mousePos.z += 0.1f;
		if (!DrawingContainsPoint(mousePos)) {
		//	Debug.Log ("Click outside drawing: M: " + mousePos + "Drawing: ("+ viewSpaceDrawingRectCorners[0] + "," + viewSpaceDrawingRectCorners[1] + "," + viewSpaceDrawingRectCorners[2] + "," + viewSpaceDrawingRectCorners[3]  +")");
			return;
		}

		switch (selectionState) {
		case 0:
//			Debug.Log ("first Selection: " + mousePos);
			if (tempText != null) {
				Texture2D.DestroyImmediate (tempText);
				tempText = null;
			}
			ui_sprite.GetComponent<Image> ().sprite = latestDrawing.sprite;
			latestDrawingModified = null;
			selectionPoint1 = mousePos;
			selectionPoint2 = Vector3.zero;
			ui_saveDrawing.SetActive (false);
			selectionState = 1;
			break;
		
		case 1:
		//	Debug.Log ("second Selection: " + mousePos);
			selectionPoint2 = mousePos;
			selectionState = 0;

			Vector3 p1 = ViewPortPointToDrawingPoint (selectionPoint1), p2 = ViewPortPointToDrawingPoint (selectionPoint2);

			//draw rectangle
			tempText = Instantiate (latestDrawing.sprite.texture);
			Sprite temp = Sprite.Create (tempText, latestDrawing.sprite.rect, latestDrawing.sprite.pivot);
			Color[] pixels =	temp.texture.GetPixels ();
			Vector2 i0 = DrawingPointToPixel (p1, temp.texture.width, temp.texture.height);
			Vector2 i1 = DrawingPointToPixel (new Vector3 (p1.x, p2.y, 0), temp.texture.width, temp.texture.height);
			Vector2 i2 = DrawingPointToPixel (p2, temp.texture.width, temp.texture.height);
			Vector2 i3 = DrawingPointToPixel (new Vector3 (p2.x, p1.y, 0), temp.texture.width, temp.texture.height);

			if (i0.y > i1.y) {
				Vector2 t = i0, t1 = i2;
				i0 = i1;
				i1 = t;
				i2 = i3;
				i3 = t1;
			}
			if (i0.x > i2.x) {
				Vector2 t = i0, t1 = i2;
				i0 = i3;
				i3 = t;
				i2 = i1;
				i1 = t1;
			}

			for (int i = (int)i0.y; i <= i1.y; i++) {
				pixels [i * temp.texture.width + (int)i0.x] = Color.red;
				pixels [i * temp.texture.width + (int)i2.x] = Color.red;
			}
				
			for (int i = (int)i0.x; i <= i2.x; i++) {
				pixels [(int)i0.y * temp.texture.width + i] = Color.red;
				pixels [(int)i2.y * temp.texture.width + i] = Color.red;
			}

				
			//calculate evidences;
			List<DrawingEvidence> l = new List<DrawingEvidence> ();
			foreach (DrawingEvidence d in latestDrawing.evidences) {
				bool inMod = true;
				foreach (Vector2 dv in d.evidenceBorders) {
					Vector2 v = DrawingPointToPixel (dv, temp.texture.width, temp.texture.height);
					if (v.x < i0.x || v.y < i0.y || v.x > i2.x || v.y > i2.y)
						inMod = false;
				}
				if (inMod)
					l.Add (d);

			}


			temp.texture.SetPixels (pixels);
			temp.texture.Apply ();
			latestDrawingModified = new Drawing (temp, l.ToArray ());
			l.Clear ();
			ui_sprite.GetComponent<Image>().sprite = latestDrawingModified.sprite;
			ui_saveDrawing.SetActive (true);
			break;
		
		

		default:
			throw new UnityException ("Error");
		}
	}

	void CalcDrawingRect(){
		Vector3[] corners = new Vector3[4];

		ui_sprite.GetComponent<RectTransform> ().GetWorldCorners (corners);
		for (int i = 0; i < 4; i++) {
			
			viewSpaceDrawingRectCorners [i] = _mainCamera.WorldToViewportPoint (corners [i]);	
		}
	}

	bool DrawingContainsPoint(Vector3 p){
		Vector3[] pt = viewSpaceDrawingRectCorners;
		if (pt [0].x < p.x && pt [0].y < p.y && pt [2].x > p.x && pt [2].y > p.y)
			return true;
		return false;
	}

	Vector3 ViewPortPointToDrawingPoint(Vector3 p){
		Vector3 temp = new Vector3 (p.x - viewSpaceDrawingRectCorners [0].x, p.y - viewSpaceDrawingRectCorners [0].y, 0);
		temp.x *= 1 / (viewSpaceDrawingRectCorners [2].x - viewSpaceDrawingRectCorners [0].x);
		temp.y *= 1 / (viewSpaceDrawingRectCorners [2].y - viewSpaceDrawingRectCorners [0].y);
		return temp;
	}

	Vector2 DrawingPointToPixel(Vector3 p, int width,int height){
		return new Vector2(Mathf.Floor(p.x*width),Mathf.Floor(p.y*height));
	}

	public Drawing TakeScreenShot ()
	{
		//detect the Evidences in the camera spectrum
		GameObject[] allEvidences = GameObject.FindGameObjectsWithTag ("Evidence");
		List<ObjectEvidence> onScreenEvidences = new List<ObjectEvidence>();
		List<DrawingEvidence> evidences = new List<DrawingEvidence>();

		foreach (GameObject g in allEvidences) {
			ObjectEvidence o= g.GetComponent<ObjectEvidence> ();
			bool onScreen = true;
			foreach (Vector3 v in o.points){
				Vector3 pos = _camera.WorldToViewportPoint (v);
				if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1 || pos.z < _camera.nearClipPlane || pos.z > _camera.farClipPlane)
					onScreen = false;
					}

			if (onScreen)
				onScreenEvidences.Add (o);
		}


		//Checking for obstructed vision;
		foreach (ObjectEvidence o in onScreenEvidences) {
			bool clearSight = true;
			foreach (Vector3 v in o.points){
				Vector3 posCam = _camera.gameObject.transform.position;
				if(Physics.Raycast(posCam, v-posCam, Vector3.Magnitude(v-posCam))){
					clearSight = false;
				}
			}

			if (clearSight) {
				Vector2[] borders = new Vector2[4];
				for (int i = 0; i < 4; i++) {
					borders [i] = _camera.WorldToViewportPoint (o.points [i]);
				}
				evidences.Add (new DrawingEvidence(o.myName,borders));
			}
		}

//		Debug.Log ("All: "+allEvidences.Length);
//		Debug.Log ("InCam: "+ onScreenEvidences.Count);
//		Debug.Log ("Visible: " + evidences.Count);
		//taking the screenshot
		//Modified from: https://answers.unity.com/questions/733240/how-to-take-a-screenshot-and-apply-it-as-a-texture.html
		Texture2D _screenShot;
		RenderTexture rt = new RenderTexture (resWidth, resHeight, 24);
		_camera.targetTexture = rt;
		_screenShot = new Texture2D (resWidth, resHeight, TextureFormat.RGBA32, false);
		_camera.Render ();
		RenderTexture.active = rt;
		_screenShot.ReadPixels (new Rect (0, 0, resWidth, resHeight), 0, 0);
		_screenShot.Apply ();	
		_camera.targetTexture = null;
		RenderTexture.active = null;
		Destroy (rt);


        Color[] pixels = _screenShot.GetPixels();
        //very resource intesive!! ToSolve
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(0, 0, 0, 1 - pixels[i].r);
        }
        _screenShot.SetPixels(pixels);
        _screenShot.Apply();
        //
        Sprite tempSprite = Sprite.Create (_screenShot, new Rect (0, 0, resWidth, resHeight), new Vector2 (0, 0));


		return new Drawing (tempSprite, evidences.ToArray ());
	}

    /// <summary>
    /// Multiply two textures together and return the result on the first texture. Textures have to have the same size.
    /// </summary>
    /// <param name="refTex"></param>
    /// <param name="tex2"></param>
    public void MultiplyTextures( ref Texture2D refTex, Texture2D tex2 )
    {

        Color[] pixels = refTex.GetPixels();
        Color[] pixels2 = tex2.GetPixels();

        if (pixels.Length != pixels2.Length)
        {
            throw new UnityException("Trying to multiply 2 textures of different sizes:" + pixels.Length + ": " + pixels2.Length);
        }

        for (int i = 0; i < pixels.Length; i++) { 
            pixels[i] = new Color(pixels[i].r*pixels2[i].r, pixels[i].g * pixels2[i].g , pixels[i].b *pixels2[i].b);
        }

        refTex.SetPixels(pixels);
        refTex.Apply();
    }

	public void OperateButton(bool save ){
		if (save) {
			Texture2D saveText = Instantiate (latestDrawing.sprite.texture);
			Sprite saveSprite = Sprite.Create (saveText, latestDrawing.sprite.rect, latestDrawing.sprite.pivot);
			GetComponent<EvidenceHandler> ().AddDrawing (new Drawing (saveSprite, latestDrawing.evidences));
            GetComponent<EvidenceHandler>().RefitEvidenceContent();
        }
			
		ui_drawingAnalysis.SetActive (false);
		Texture2D.Destroy (latestDrawing.sprite.texture);
		latestDrawing = null;
		latestDrawingModified = null;
		selectionState = 0;
		Enable ();
		inSelection = false;
		exitDrawing ();
	}	

	public void Enable(){
		usable = true;
	}

	public void Disable(){
		usable = false;
	}

    public void EnterCutScene(string s)
    {
        Disable();
    }

    public void ExitCutscene(string s)
    {
        Enable();
    }
}

[System.Serializable]
public class Drawing{
	public Sprite sprite;
	public DrawingEvidence[] evidences;

	public Drawing(Sprite _sprite, DrawingEvidence[] _evidences){
		sprite = _sprite;
		evidences = _evidences;
	}

}

[System.Serializable]
public struct DrawingEvidence{
	public string name;
	public Vector2[] evidenceBorders;
 		public DrawingEvidence(string _name, Vector2[] _evidenceBorders){
		name = _name;
		evidenceBorders = _evidenceBorders;
	}
}
