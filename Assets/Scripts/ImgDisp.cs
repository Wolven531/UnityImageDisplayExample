using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace awillInc {
    public class ImgDisp : MonoBehaviour
    {
        /// <summary>
        /// The URL to use (rather than passing one into loadSpriteToObject())
        /// </summary>
        public string URL;
        /// <summary>
        /// This is used internally to track whether or not this image is in the process of loading
        /// </summary>
        private bool isLoading;

        /// <summary>
        /// This property can be used to tell whether or not the image is ready to display
        /// </summary>
        /// <returns type="bool">true if the image is ready for display; false otherwise</returns>
        public bool IsReady
        {
            get { return !isLoading; }
        }

        /// <summary>
        /// This method will attempt to load a given path into a ImgDisp enabled component
        /// </summary>
        /// <param name="targetURL" type="string">The URL of the image to load</param>
        /// <param name="imgComp" type="ImgDisp">An ImgDisp component on it, which will receive the loaded image</param>
        public static void loadSpriteToObject(string targetURL, ImgDisp imgComp)
        {
            imgComp.loadSpriteImage(targetURL, imgComp.gameObject);
        }
        
        void loadSpriteImage(string targetURL, GameObject g)
        {
            if (isLoading)
            {
                Debug.LogWarningFormat("Could not load targetURL because this image is already loading... targetURL={0}", targetURL);
                return;
            }
            if (string.IsNullOrEmpty(targetURL))
            {
                Debug.LogWarningFormat("targetURL was empty, so using this.URL which is = {0}", this.URL);
                targetURL = this.URL;
            }
            StartCoroutine(loadSpriteImageHelper(targetURL, g));
        }

        /// <summary>
        /// This method attempts to load a local file
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="targetURL"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerator HandleLocalLoad(string localPath, string targetURL, GameObject target)
        {
    #if UNITY_EDITOR
                targetURL = "file:///" + localPath;
    #elif UNITY_ANDROID
                targetURL = "file://" + localPath;
    #endif
            WWW www = new WWW(targetURL);
            Debug.LogFormat("Loading from disk... targetURL={0}", targetURL);
            yield return www;
            Debug.LogFormat("Load from disk completed. targetURL={0} size={1:N0} bytes", targetURL, www.bytesDownloaded);

            if (string.IsNullOrEmpty(www.text))
            {
                Debug.LogErrorFormat("Load err={0} targetURL={1}", www.error, targetURL);
                isLoading = false;
                yield break;
            }

            SetupSpriteDisp(target, www);
            isLoading = false;
        }

        /// <summary>
        /// This method attempts to download a file from the internet
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="targetURL"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerator HandleWebLoad(string localPath, string targetURL, GameObject target)
        {
            WWW www = new WWW(targetURL);
            Debug.LogFormat("Downloading from web... targetURL={0}", targetURL);
            yield return www;
            Debug.LogFormat("Download completed. targetURL={0} size={1:N0} bytes", targetURL, www.bytesDownloaded);

            if (string.IsNullOrEmpty(www.text))
            {
                Debug.LogErrorFormat("Download failed err={0} targetURL={1}", www.error, targetURL);
                isLoading = false;
                yield break;
            }

            // NOTE: save image to disk
            Debug.LogFormat("Saving image to disk... localPath={0}", localPath);
            System.IO.File.WriteAllBytes(localPath, www.bytes);
            Debug.LogFormat("Image save completed. localPath={0}", localPath);

            SetupSpriteDisp(target, www);
            isLoading = false;
        }

        /// <summary>
        /// This method is used to orchestrate the image loading
        /// </summary>
        /// <param name="targetURL"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerator loadSpriteImageHelper(string targetURL, GameObject target)
        {
            isLoading = true;
            string filename = System.IO.Path.GetFileName(targetURL);
            string localPath = Application.persistentDataPath + "/" + filename;

            // NOTE: if image is available from local, use it
            if (System.IO.File.Exists(localPath))
            {
                StartCoroutine(HandleLocalLoad(localPath, targetURL, target));
                yield break;
            }

            // NOTE: otherwise, check for internet
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("No Internet Connection");
                isLoading = false;
                yield break;
            }

            StartCoroutine(HandleWebLoad(localPath, targetURL, target));
        }

        /// <summary>
        /// This method is used to setup the sprite display after an image has successfully been loaded
        /// </summary>
        /// <param name="target"></param>
        /// <param name="www"></param>
        private void SetupSpriteDisp(GameObject target, WWW www)
        {
            RectTransform rt = target.GetComponent<RectTransform>();
            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            Rect r = rt.rect;
            Sprite sprite = Sprite.Create(
                www.texture,
                new Rect(0, 0, www.texture.width, www.texture.height),
                Vector2.one / 2,
                www.texture.width,
                0,
                SpriteMeshType.FullRect,
                Vector4.zero
            );

            sr.sprite = sprite;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(r.width, r.height);

            target.transform.localScale = Vector3.one;
            target.transform.localPosition = Vector3.zero;
            target.transform.position = Vector3.zero;

            rt.position = Vector3.zero;
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;

            // NOTE: awill: make SURE to let the parent transform know it needs
            // to refresh after the image is done
            LayoutRebuilder.MarkLayoutForRebuild(target.transform.parent.GetComponent<RectTransform>());
        }
    }
}
