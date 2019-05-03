using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Proyecto26;



//[System.Serializable]
//public class GPhoto
//{
//	public string baseUrl;
//}


public class setTextureFromWWW : MonoBehaviour {

	public int imageID;
    public float image_Scale = 0.0001f;
    public float growTime = 0.05f;
    private string imageURL;
	public ServerConnector serverConnector;
    private bool scaleFlag = false;
    float m_textureWidth;
    float m_textureHeight;
    IEnumerator getMyTextures() // CoRoutine for web GET
	{
		// Start a download of the given URL
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
			// Wait for download to complete
		yield return www.Send();
		if (www.isNetworkError) {
			Debug.Log (www.error);
		} else {
			Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            m_textureWidth = myTexture.width;
            m_textureHeight = myTexture.height;
            scaleFlag = true; 
            // assign texture
            Renderer renderer = GetComponent<Renderer>();
            
            
            renderer.material.mainTexture = myTexture;
			Debug.Log ("texture has been set");
		}
	}

	// Use this for initialization

	void ready (GameObject calledBy){
		Debug.Log ("I was awakened by "+calledBy.name);
		imageURL = serverConnector.photosUrls[imageID].baseUrl;
		StartCoroutine(getMyTextures());
	}
	void Start () {

	}
	// Update is called once per frame
	void Update () {
        if(scaleFlag)
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(m_textureWidth * image_Scale, transform.localScale.y, m_textureHeight * image_Scale),growTime);
    }
}
