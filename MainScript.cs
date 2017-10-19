using UnityEngine;
using UnityEngine.UI;
using PhotoNamespace;
using PhotosHandlerNamespace;
using VRAPIHandlerNamespace;
using TexturePainterNamespace;
using SizeHandlerNamespace;

public class MainScript : MonoBehaviour {

	public GameObject photoFrameObject;
	public Material materialPhotoFrame;
	public Text photoNameText;
	public InputField codeInput;
	private VRAPIHandler vrAPIHandler;
	private PhotosHandler photosHandler;
	private TexturePainter texturePainter;
	private SizeHandler sizeHandler;

	// Use this for initialization
	void Start () {
		this.vrAPIHandler = new VRAPIHandler();
		this.photosHandler = new PhotosHandler();
		this.texturePainter = new TexturePainter(this.photosHandler);
		this.sizeHandler = new SizeHandler(this.photosHandler);
		Debug.Log("photoNameText.text:"+this.photoNameText.text);
	}

	// Update is called once per frame
	void Update () {}

	void OnGUI() {}

	public void SubmittedCode() {
		this.vrAPIHandler.PerformSessionRequest(this.codeInput.text, this.photosHandler);
		this.DisplayPhoto();
	}
	public void ClickedNextPhoto() {
		this.photosHandler.GoToNextPhoto();
		this.DisplayPhoto();
	}
	public void ClickedPreviousPhoto() {
		this.photosHandler.GoToPreviousPhoto();
		this.DisplayPhoto();
	}
	public void ClickedRotation() {
		float angle = 90;
		this.RotateRight(90);
	}

	void DisplayPhoto() {
		this.ApplyTexture();
		this.AdaptSize();
		this.WritePhotoName();
	}
	void ApplyTexture() {
		this.materialPhotoFrame.mainTexture = this.texturePainter.GetTexture();
	}
	void AdaptSize() {
		this.photoFrameObject.transform.localScale = this.sizeHandler.GetLocalScale();
	}
	void WritePhotoName() {
		string photoName = this.photosHandler.GetDisplayedPhoto().GetName();
		string index = this.photosHandler.GetIndexToString();
		this.photoNameText.text = photoName + " " + index;
	}
	void RotateRight(float angle) {
		this.photoFrameObject.transform.Rotate(0,angle,0);
	}

}
