using UnityEngine;
using PhotosHandlerNamespace;
using PhotoNamespace;

namespace SizeHandlerNamespace {

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

}
