using System;

namespace PhotoNamespace {

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

}
