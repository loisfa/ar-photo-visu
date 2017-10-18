using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using SimpleJSON;

public class MainScript : MonoBehaviour {

	public class Photo {
		private string name;
		private int width;
		private int height;
		private Byte[] image;

		public Photo(string name, int width, int height, Byte[] image) {
			this.name = name;
			this.width = width;
			this.height = height;
			this.image = image;
		}
		public Byte[] GetImage() {
			return this.image;
		}
		public string GetName() {
			return this.name;
		}
		public int GetWidth() {
			return this.width;
		}
		public int GetHeight() {
			return this.height;
		}
	}

	public  class PhotosHandler {
		private List<Photo> listPhotos;
		private int indexDisplayedPhoto;

		public PhotosHandler() {
			Init();
		}
		public void Init() {
			this.listPhotos = new List<Photo>();
			this.indexDisplayedPhoto = 0;
		}
		public void AddPhoto(Photo photo) {
			this.listPhotos.Add(photo);
		}
		public Photo GetDisplayedPhoto() {
			if (this.CheckWellDefined()) {
				return this.listPhotos[this.indexDisplayedPhoto];
			} else {
				return null;
			}
		}
		public void GoToNextPhoto() {
			if (this.CheckWellDefined()) {
				this.indexDisplayedPhoto = (this.indexDisplayedPhoto+1) % this.listPhotos.Count;
			}
		}
		public void GoToPreviousPhoto() {
			if (this.CheckWellDefined()) {
				this.indexDisplayedPhoto = (this.indexDisplayedPhoto-1) % this.listPhotos.Count;
			}
		}
		public bool CheckWellDefined() {
			if (this.listPhotos.Count>=1) {
				if (this.indexDisplayedPhoto>=0 && this.indexDisplayedPhoto<this.listPhotos.Count) {
					return true;
				} else {
					Debug.Log("indexDisplayedPhoto was wrong ("+this.indexDisplayedPhoto+"). Now set to 0.");
					this.indexDisplayedPhoto = 0;
					return true;
				}
			} else {
				Debug.Log("no photo in instance of PhotosHandler");
				return false;
			}
		}
	}

	public class VRAPIHandler {

		private string urlForSessionPhotoNames = @"https://photo-web-api.herokuapp.com/api/vr/smartphone/";
		private string urlForPhotos = @"https://photo-web-api.herokuapp.com/api/photos/";
		// http://localhost:8080

		public VRAPIHandler() {}

		public void PerformSessionRequest(string code, PhotosHandler photosHandler) {
			// should fill the listPhotos + display the first image on the plane
			List<string> listPhotoNames =  this.RequestPhotoNames(code);
			Debug.Log("got photo names");
			foreach (string photoName in listPhotoNames) {
				Debug.Log("Name: "+photoName);
				Photo photo =  this.RequestPhoto(photoName);
				photosHandler.AddPhoto(photo);
			}
			Debug.Log("session request "+code+" performed");
		}

		private List<string> RequestPhotoNames(string code) {
				ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
				string responseJson = string.Empty;
				string url = this.urlForSessionPhotoNames+"/"+code;
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.ContentType = "application/json; charset=utf-8";
				request.AutomaticDecompression = DecompressionMethods.GZip;
				using (HttpWebResponse response =  (HttpWebResponse)request.GetResponse())
				using (Stream stream =  response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
				    responseJson = reader.ReadToEnd();
						List<string> listPhotoNames = new List<string>();
						var obj = JSON.Parse(responseJson);
						for (int i=0; i<obj["listPhotoNames"].Count; i++) {
							listPhotoNames.Add(obj["listPhotoNames"][i].Value);
							Debug.Log("i: "+i+"/"+obj.Count+" ; photo:"+listPhotoNames[i]);
						}
						return listPhotoNames;
				}
		}

		private Photo RequestPhoto(string photoName) {
			photoName = photoName.Replace(" ", "+");
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			string url = this.urlForPhotos+"/"+photoName;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json; charset=utf-8";
			request.AutomaticDecompression = DecompressionMethods.GZip;
			using (HttpWebResponse response =  (HttpWebResponse)request.GetResponse()){
			   using (BinaryReader reader = new BinaryReader(response.GetResponseStream())) {
			      Byte[] image = reader.ReadBytes(1 * 1024 * 1024 * 10);
						string photoProperties = response.Headers["photoProperties"];
						var obj = JSON.Parse(photoProperties);
						string name = obj["name"].Value;
						int width = obj["dimensions"]["width"].AsInt;
						int height = obj["dimensions"]["height"].AsInt;
						Photo photo = new Photo(name, width, height, image);
						Debug.Log("got photo: "+name+" width:"+width);
						return photo;
			   }
			}
		}

		public bool MyRemoteCertificateValidationCallback(System.Object sender,
	    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
			    bool isOk = true;
			    // If there are errors in the certificate chain,
			    // look at each error to determine the cause.
			    if (sslPolicyErrors != SslPolicyErrors.None) {
			        for (int i=0; i<chain.ChainStatus.Length; i++) {
			            if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) {
			                continue;
			            }
			            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
			            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
			            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
			            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
			            bool chainIsValid = chain.Build ((X509Certificate2)certificate);
			            if (!chainIsValid) {
			                isOk = false;
			                break;
			            }
			        }
			    }
			    return isOk;
			}
	}

	public class TexturePainter {
		private PhotosHandler photosHandler;

		public TexturePainter(PhotosHandler photosHandler) {
			this.photosHandler = photosHandler;
		}

		public Texture2D GetTexture() {
			Photo photo = this.photosHandler.GetDisplayedPhoto();
			byte[] image = photo.GetImage();
			Debug.Log("Material was updated with image: "+photo.GetName());
			return this.LoadTexture(image);
		}

		private Texture2D LoadTexture(byte[] image) {
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(image); //..this will auto-resize the texture dimensions.;
			return texture2D;
		}
	}

	public class SizeHandler {
		private PhotosHandler photosHandler;
		private float scaleFactor=0.001f;
		public SizeHandler(PhotosHandler photosHandler) {
			this.photosHandler = photosHandler;
		}
		public Vector3 GetLocalScale() {
			Photo photo = this.photosHandler.GetDisplayedPhoto();
			float width = photo.GetWidth();
			float height = photo.GetHeight();
			return new Vector3(width, 1, height) * this.scaleFactor;
		}
	}


	public GameObject photoFrameObject;
	public Material materialPhotoFrame;
	private string codeText = "";
	private int screenWidth;
	private int screenHeight;
	private const int MAX_LENGTH_CODE = 4;
	private string displayedPhotoName = "";
	private VRAPIHandler vrAPIHandler;
	private PhotosHandler photosHandler;
	private TexturePainter texturePainter;
	private SizeHandler sizeHandler;


	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		this.screenWidth = Screen.width;
		this.screenHeight = Screen.height;
		this.vrAPIHandler = new VRAPIHandler();
		this.photosHandler = new PhotosHandler();
		this.texturePainter = new TexturePainter(this.photosHandler);
		this.sizeHandler = new SizeHandler(this.photosHandler);
	}

	// Update is called once per frame
	void Update () {}

	void OnGUI() {
		GUI.skin.textField.fontSize = 30;
    this.codeText = GUI.TextField(new Rect(10, 10, 100, 40), this.codeText, MAX_LENGTH_CODE);
		GUI.color = Color.green;
		if (GUI.Button(new Rect(115, 10, 40, 40), "OK"))
		{
			SubmittedCode(codeText);
		}
		GUI.color = Color.white;
		GUI.skin.label.fontSize = 24;
		GUIStyle centeredTextStyle = new GUIStyle("Label");
    centeredTextStyle.alignment = TextAnchor.MiddleCenter;

		GUI.Label(new Rect(0, this.screenHeight-40, this.screenWidth, 30),
			this.displayedPhotoName, centeredTextStyle
		);


		if (GUI.Button(new Rect(10, 60, 40, 100), "<"))
		{
			ClickedPreviousPhoto();
		}
		if (GUI.Button(new Rect(screenWidth-50, 60, 40, 100), ">"))
		{
			ClickedNextPhoto();
		}
  }

	void SubmittedCode(string codeText) {
		this.vrAPIHandler.PerformSessionRequest(codeText, this.photosHandler);
		this.DisplayPhoto();
	}
	void ClickedNextPhoto() {
		this.photosHandler.GoToNextPhoto();
		this.DisplayPhoto();
	}
	void ClickedPreviousPhoto() {
		this.photosHandler.GoToPreviousPhoto();
		this.DisplayPhoto();
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
		this.displayedPhotoName = this.photosHandler.GetDisplayedPhoto().GetName();
	}

}
