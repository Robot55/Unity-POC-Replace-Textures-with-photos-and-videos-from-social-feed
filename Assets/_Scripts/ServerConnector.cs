using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Proyecto26;
using Newtonsoft.Json;


[System.Serializable]
public class ServerRoute // used to return an array of photo urls
{
	public string name;
	public string serviceRoute;
}
[System.Serializable]
public class GPhoto // used to return an array of photo urls
{
	public string baseUrl;
}
[System.Serializable]
public class APIResponse  // used for pages that return this SPECIFIC json model
{
	public string error; 
	public int errorCode;
}
[System.Serializable]
public class NewPairing  // used for pages that return this SPECIFIC json model
{
	public string url; 
	public string hash;
}


public class ServerConnector : MonoBehaviour
{
	public enum Service { Google, Instagram };
	
	[Header("Choose Which Service to Connect to")]
	public Service service;
	// ==== Server Vars
	public bool localhost = false;
	public string serverBaseURL = "https://photos-sandbox-server.herokuapp.com";
	public string routeToPhotos;
	public ServerRoute googleRoute = new ServerRoute {serviceRoute = "/gphotos", name = "Google"};
	public ServerRoute instagramRoute = new ServerRoute {serviceRoute = "/instaphotos", name = "Instagram"};
	public ServerRoute activeRoute;
	public string clientHash = "";
	public float serverPollingInterval = 4.0f;

	
	// == Server Vars End
	private Service lastService;

	[Header("Drag Photos Container from Hierarchy")]
	public GameObject photosContainer;
	[Header("populated from Server. Look, Don't touch!")]
	public int timeOutErrorCounter = 0;
	public APIResponse apiResponse;
	private NewPairing newPairing;
	public GPhoto[] photosUrls; // This array will collect all the photos urls

    void Start()
    {
		Debug.Log("=======================\n Server Connector Started");
		if (localhost){
			serverBaseURL = "http://localhost:3000";
			Debug.Log("=======================\n Working in Localhost / DEV mode Disable localhost checkbox in Unity editor");
		}
		updateServiceRoute();
		InvokeRepeating("pollAPI", 1.0f, serverPollingInterval);
		
		if (!PlayerPrefs.HasKey("hash")){
				Debug.Log("playerPrefs hash is empty");
				createNewPairing();
		} else {
			Debug.Log("playerPrefs hash is: "+PlayerPrefs.GetString("hash"));
			clientHash = PlayerPrefs.GetString("hash");
			updateRouteToPhotos();
		}
	}

    // Update is called once per frame
    void Update()
    {
       if (lastService==service) return;
	   lastService=service;
	   updateServiceRoute();
	   print ("<<< Service has been set to: " + activeRoute.name);
    }
	
	void grabPhotoUrls(){
		print("Getting Google photo urls...");
		RestClient.GetArray<GPhoto>(routeToPhotos).Then(payLoad => { // this is the REST code to get string array from JSON
			Debug.Log(payLoad.Length+" photos retrieved from server");
			photosUrls=payLoad;
			tellPhotosURLsAreLive();
			CancelInvoke("pollAPI");
		});  

	}
	void grabInstagramPhotosUrls(){
		print("Getting Instagram photo urls...");
		RestClient.GetArray<GPhoto>(routeToPhotos).Then(payLoad => { // this is the REST code to get string array from JSON
			Debug.Log(payLoad.Length+" photos retrieved from server");
			photosUrls=payLoad;
			tellPhotosURLsAreLive();
			CancelInvoke("pollAPI");
		});  

	}

	void tellPhotosURLsAreLive(){
		print("...Photo urls received");
		photosContainer.BroadcastMessage ("ready", this.gameObject); // tells all photos in container that urls are available
	}

	void pollAPI(){
		print("polling API invoked");
		RestClient.Get(serverBaseURL+activeRoute.serviceRoute+"?hash="+clientHash).Then(res => {
				Debug.Log("ATTEMPT RESPONSE: \n" + res.Text);
				if (res.Text.Contains("baseUrl") && res.Text.Contains("https://")){
					Debug.Log("photo urls accessible - we can stop polling now");
					SendMessage("grabPhotoUrls");
				}
				if (res.Text.Contains("errorCode"))
					{
						print ("some error found");
						apiResponse = JsonUtility.FromJson<APIResponse>(res.Text);
						SendMessage("handleErrors", apiResponse);
					}
		});
	}

	void handleErrors(APIResponse apiResponse){
		print ("Error Handling Started");
		string error = apiResponse.error;
		int errorCode = apiResponse.errorCode;
		print("Error ("+errorCode+") returned from server:");
		print(error);
		
		switch (errorCode)
      	{
         case 503:
			timeOutErrorCounter++ ;
			if (timeOutErrorCounter>=3){
				print("<color=red>ERROR RESOLUTION:</color> 3 timeouts occured. Attempt to create new pairing");
				CancelInvoke("pollAPI");
				timeOutErrorCounter=0;
				SendMessage("createNewPairing");
			}
            break;
         case 401:
		 	timeOutErrorCounter++ ;
		 	if (timeOutErrorCounter>=3){
				print("<color=red>ERROR RESOLUTION:</color> Bad hash or invalid token. Attempt to create new pairing");
				CancelInvoke("pollAPI");
				timeOutErrorCounter=0;
				SendMessage("createNewPairing");
			}	
            break;
         default:
            Debug.LogWarning("The error is unknown.");
            break;   
      }
	}

	void createNewPairing(){
		Debug.Log("CreateNewPairing Started");
		RestClient.Get(serverBaseURL+"/createNewPairing").Then(res => {
				Debug.Log("Response from CreateNewPairing: \n" + res.Text);
				newPairing = JsonUtility.FromJson<NewPairing>(res.Text) ; 
				Debug.Log("Response api: \n" + newPairing.url +"\n" + newPairing.hash );
				clientHash=newPairing.hash;
				PlayerPrefs.SetString("hash", clientHash);
				updateRouteToPhotos();
				print("opening web browser");
				Application.OpenURL(newPairing.url+"?name="+activeRoute.name);
				StartCoroutine(enableServerPolling(5));
			
			});
	}
	void updateRouteToPhotos(){
		print("updating route to photos");
		routeToPhotos = serverBaseURL+activeRoute.serviceRoute+"?hash="+clientHash; 
	}

	void updateServiceRoute(){
		print("updating service route");
		if (service==Service.Google){
			activeRoute=googleRoute;
		} else {
			if (service==Service.Instagram){
				activeRoute = instagramRoute;
			}
		}
		print ("serviceRoute set to google: "+activeRoute.serviceRoute); 
	}

	IEnumerator enableServerPolling(float timeToWait) // CoRoutine for web GET
	{
		print ("<b><color=purple>waiting <" +timeToWait +"> seconds for user to login in browser</color></b>");
		yield return new WaitForSeconds(timeToWait);
		if(!IsInvoking("pollAPI")){
			print ("pollAPI currently NOT invoking. Attempting to Invoke");
			InvokeRepeating("pollAPI", 1.0f, serverPollingInterval);
		}
	} 
}
