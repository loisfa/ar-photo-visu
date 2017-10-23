using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.IO;
using System;
using UnityEngine;
using SimpleJSON;
using PhotosHandlerNamespace;
using PhotoNamespace;


namespace ARAPIHandlerNamespace {

  public class ARAPIHandler {

    // prod
    private string urlForSessionPhotoNames = @"https://photo-web-api.herokuapp.com/api/ar/smartphone/";
  	private string urlForPhotos = @"https://photo-web-api.herokuapp.com/api/photos/";
    // localhost
    // private string urlForSessionPhotoNames = @"http://localhost:8080/api/ar/smartphone";
		// private string urlForPhotos = @"http://localhost:8080/api/photo";

		public ARAPIHandler() {}

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
			request.AutomaticDecompression = DecompressionMethods.GZip;
      request.ContentType = "application/json; charset=utf-8";
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

}
