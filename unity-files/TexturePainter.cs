using UnityEngine;
using PhotosHandlerNamespace;
using PhotoNamespace;

namespace TexturePainterNamespace {

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


}
