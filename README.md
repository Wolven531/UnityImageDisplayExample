# ImageTester

## How To Use

- Download project (or just `ImgDisp.cs`)
- Select a game object in your scene (that needs the image)
- Add the `ImgDisp` script to it
- Turn that game object into a prefab
- Add `using awillInc;` to your C# script
- Add a `public` (or private with `[SerializeField]`) variable to your script
  - `public ImgDisp IMG_PREFAB;` is what I use in `Main.cs`
- Instantiate an instance of the class using the prefab:
  - `ImgDisp img = Instantiate(IMG_PREFAB, Vector3.zero, Quaternion.identity, imgContainer);`
  - In the example, `imgContainer` is used to hold images, but you can supply any `Transform` as the parent for the image
- Explicitly load the image by URL:
  - `ImgDisp.loadSpriteToObject(imageURLs[i], img);`

## Useful Information

- When you request an image, this component:
  - Checks to see if image exists locally
  - If it does, load it from disk and use it
  - If it does not, download it from web (if possible), save it to disk (if possible), and use the downloaded resource
