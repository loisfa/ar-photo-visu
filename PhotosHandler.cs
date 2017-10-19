using System.Collections.Generic;
using UnityEngine;
using PhotoNamespace;

namespace PhotosHandlerNamespace {

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
    public string GetIndexToString() {
      string indexToString = (this.indexDisplayedPhoto+1).ToString();
      string countToString = (this.listPhotos.Count).ToString();
      return "(" + indexToString + "/" + countToString + ")";
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

}
