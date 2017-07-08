using System.Collections.Generic;
using UnityEngine;
using awillInc;

public class Main : MonoBehaviour {
	public ImgDisp IMG_PREFAB;
	public Transform imgContainer;
	public List<string> imageURLs = new List<string>{
		"https://43ch47qsavx2jcvnr30057vk-wpengine.netdna-ssl.com/wp-content/uploads/2015/02/Archangel_Water_Icon.png",
		"https://43ch47qsavx2jcvnr30057vk-wpengine.netdna-ssl.com/wp-content/uploads/2015/02/Ariel_Icon.png",
		"https://43ch47qsavx2jcvnr30057vk-wpengine.netdna-ssl.com/wp-content/uploads/2015/02/Archangel_Fire_Icon.png",
		"https://43ch47qsavx2jcvnr30057vk-wpengine.netdna-ssl.com/wp-content/uploads/2016/12/Velajuel_Icon.png"
	};

	void Start () {
		for(int i = 0; i < imageURLs.Count; i++)
		{
			ImgDisp img = Instantiate(IMG_PREFAB, Vector3.zero, Quaternion.identity, imgContainer);
			ImgDisp.loadSpriteToObject(imageURLs[i], img);
		}
	}
}
