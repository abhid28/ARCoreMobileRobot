    using GoogleARCore.Examples.HelloAR;
    using GoogleARCore.Examples.AugmentedImage;
	using System.Collections.Generic;
	using GoogleARCore;
	using GoogleARCore.Examples.Common;
	using UnityEngine;
    using UnityEngine.UI;
using System.Threading;
using System.Collections;
#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

	/// <summary>
	/// Controls the HelloAR example.
	/// </summary>
	public class ControllerArAug4 : MonoBehaviour
	{
    //public GameObject robot;
    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera FirstPersonCamera;

		/// <summary>
		/// A prefab for tracking and visualizing detected planes.
		/// </summary>
		public GameObject DetectedPlanePrefab;

		/// <summary>
		/// A model to place when a raycast from a user touch hits a plane.
		/// </summary>
		public GameObject pickPrefab;
	    public GameObject placePrefab;
	    public GameObject MarkerPrefab;
    public GameObject destinationPrefab;
    public GameObject robot;
    /// <summary>
    /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
    /// </summary>
    public GameObject SearchingForPlaneUI;

		/// <summary>
		/// The rotation in degrees need to apply to model when the Andy model is placed.
		/// </summary>
		private const float k_ModelRotation = 180.0f;

		/// <summary>
		/// A list to hold all planes ARCore is tracking in the current frame. This object is used across
		/// the application to avoid per-frame allocations.
		/// </summary>
		private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

		/// <summary>
		/// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
		/// </summary>
		private bool m_IsQuitting = false;

	    //
		public AugmentedImageVisualizer AugmentedImageVisualizerPrefab;

		/// <summary>
		/// The overlay containing the fit to scan user guide.
		/// </summary>
		public GameObject FitToScanOverlay;

		private Dictionary<int, AugmentedImageVisualizer> m_Visualizers= new Dictionary<int, AugmentedImageVisualizer>();

		private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();


		//New variables
		BtAutoScript btmodule;
		buttonScript buttonModule;
		bool ButtonClicked;
		bool ButtonPressed;
		bool onetime =false;
	    bool clearedALL =true;
	    bool firstDetect = true;
		GameObject targetObject1;
	    GameObject targetObject2;
	    GameObject markerCenter;
	    GameObject robotworkspace;
	    public GameObject workspace;
    float turnangle;
     Text xcenter1;
     Text ycenter1;
     Text xdestination1;
    Text ydestination1;
    Text turnangle1;
    public Text distance1;
    public GameObject fir;
    public GameObject fir1;
    public GameObject fir2;
    public GameObject fir3;
    Anchor anchor1;
	    Anchor anchor2;
	    Anchor anchor3;
    Anchor anchor4;
    Anchor Markeranchor;
    GameObject targetObject3;
    Anchor anchor5;
    GameObject targetObject4;

    Vector3 startPose;
	    Vector3 endPose;
    Vector3 hitone;
    Quaternion hitone1;
    Vector3 hitone2;
    Quaternion hitone3;
    bool ImageTracking = false;
	    int count=0;
	    Vector3 thePosition;
	   // public int reachscale =2;
	   // Vector3 offsetVector = new Vector3(0,0,-0.01);
	     const int N = 3;

    //float[,] rotTrans;
  //  float alphaX, alphaY, alphaZ;
    float Rx, Ry, Rz;
    float Ox, Oy, Oz;
    float Fx, Fy, Fz;
    float Dx, Dy, Dz;
   float xcenter;
    float ycenter;
     float xfront;
   float yfront;
    float xdestination;
    float ydestination;
    private float movementSpeed = 1f;
    Transform sunrise;
   Transform sunset;


    // Time to move from sunrise to sunset position, in seconds.
    public float journeyTime = 0.1f;

    // The time at which the animation started.
    private float startTime;

    public void Start(){
			btmodule= gameObject.GetComponent<BtAutoScript>();
			buttonModule = gameObject.GetComponent<buttonScript> ();
        startTime = Time.time;

        }

	
    	public void Update()
		{

			ButtonClicked = buttonModule.isClicked;
		   // Debug.Log("Button status:"+ ButtonClicked);
			ButtonPressed = buttonModule.isPressed;
			_UpdateApplicationLifecycle();

		
		Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

		// Create visualizers and anchors for updated augmented images that are tracking and do not previously
		// have a visualizer. Remove visualizers for stopped images.
		foreach (var image in m_TempAugmentedImages)

		{
			AugmentedImageVisualizer visualizer = null;
			m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
			if (image.TrackingState == TrackingState.Tracking && visualizer == null)
			//if (image.TrackingState == TrackingState.Tracking && markerCenter==null )
			{
				// Create an anchor to ensure that ARCore keeps tracking this augmented image.
				anchor1 = image.CreateAnchor(image.CenterPose);
				ImageTracking = true;

				//------------------------- Adding a visual for showing the robot workspace----------------------------
				// Not updating it always. Detecting and storing the values only once at the beginning.
				//-----------------------------------------------------------------------------------------------------


				if (firstDetect) {
					Markeranchor = image.CreateAnchor(image.CenterPose);
					//markerCenter = Instantiate (MarkerPrefab,image.CenterPose.position, image.CenterPose.rotation);
					markerCenter = Instantiate (MarkerPrefab,Markeranchor.transform.position, Markeranchor.transform.rotation);
					//markerCenter.transform.parent = Markeranchor.transform;
					Vector3 worklocation = (Markeranchor.transform.position + Markeranchor.transform.forward*0.095f); 
					// adjusting the workspace. The origin of the workspace plane is ahead of the anchor origin
					robotworkspace = Instantiate (workspace,worklocation, Markeranchor.transform.rotation);

					firstDetect = false;
					Debug.Log ("markerCenterX:" + Markeranchor.transform.position.x*100 + ", markerCenterY:" + Markeranchor.transform.position.y*100 + ", markerCenterZ:" + Markeranchor.transform.position.z*100);
				}
					


				visualizer = (AugmentedImageVisualizer)Instantiate(AugmentedImageVisualizerPrefab, anchor1.transform);
			    visualizer.Image = image;
				m_Visualizers.Add(image.DatabaseIndex, visualizer);
				visualizer.gameObject.SetActive (false); 

				Debug.Log ("anchor1X:" + anchor1.transform.position.x*100 + ", anchor1Y:" + anchor1.transform.position.y*100 + ", anchor1Z:" + anchor1.transform.position.z*100);
				
			}
			else if (image.TrackingState == TrackingState.Stopped && visualizer != null)
				
			{
				m_Visualizers.Remove(image.DatabaseIndex);
				//GameObject.Destroy (markerCenter.gameObject);
				markerCenter.gameObject.SetActive (false);
						
				GameObject.Destroy(visualizer.gameObject);
				visualizer.gameObject.SetActive (false);
			}

			
		}



		
			Session.GetTrackables<DetectedPlane>(m_AllPlanes);
			bool showSearchingUI = true;
			for (int i = 0; i < m_AllPlanes.Count; i++)
			{
				if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
				{
					showSearchingUI = false;

					break;
				}
			}

			SearchingForPlaneUI.SetActive(showSearchingUI);

			
			TrackableHit hit;
			TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

		if (ButtonClicked && !onetime && clearedALL ) {
			
				if (Frame.Raycast ((float)Screen.width/2,(float)Screen.height/2, raycastFilter, out hit)) {   
				    
					
					if ((hit.Trackable is DetectedPlane) && Vector3.Dot (FirstPersonCamera.transform.position - hit.Pose.position,hit.Pose.rotation * Vector3.up) < 0) {
						Debug.Log ("Hit at back of the current DetectedPlane");
					} else {
						
					     count = count + 1;
                    if (count == 1)
                    {
                        anchor2 = hit.Trackable.CreateAnchor(hit.Pose);
                        targetObject1 = Instantiate(pickPrefab, hit.Pose.position, hit.Pose.rotation);
                        targetObject1.transform.parent = anchor2.transform;

                        hitone = hit.Pose.position;
                        hitone1 = hit.Pose.rotation;


                    }


                    else if (count == 2)
                    {
                        anchor3 = hit.Trackable.CreateAnchor(hit.Pose);
                        targetObject2 = Instantiate(placePrefab, hit.Pose.position, hit.Pose.rotation);
                        targetObject2.transform.parent = anchor3.transform;

                    }
                    else
                    {
                        anchor4 = hit.Trackable.CreateAnchor(hit.Pose);
                        targetObject3 = Instantiate(destinationPrefab, hit.Pose.position, hit.Pose.rotation);
                        targetObject3.transform.parent = anchor4.transform;
                        count = 0;
                        onetime = true;
                        targetObject4 = Instantiate(robot, hitone, hitone1);
                        //targetObject4.transform.parent = anchor2.transform;
                        //targetObject4.transform.parent = anchor2.transform;
                        //Quaternion turnangletarget = Quaternion.Euler(0, 0, 90);
                        //targetObject4.transform.rotation = Quaternion.Slerp(hit.Pose.rotation, turnangletarget, .3F);
                        //count = count + 1;
                        fir.GetComponent<Text>().text = "fir";
                        hitone2 = hit.Pose.position;
                        hitone3 = hit.Pose.rotation;
                        fir1.GetComponent<Text>().text = "fir1";
                    }



                    if (onetime) {
                        fir2.GetComponent<Text>().text = "fir2";
                        float alphaX, alphaY, alphaZ;
                        alphaX = anchor1.transform.eulerAngles.x;
                        alphaY = anchor1.transform.eulerAngles.y;
                        alphaZ = anchor1.transform.eulerAngles.z;
                        Rx = anchor1.transform.position.x * 100; 
                        Ry = anchor1.transform.position.y * 100;
                        Rz = anchor1.transform.position.z * 100;
                        Ox = anchor2.transform.position.x * 100;
                        Oy = anchor2.transform.position.y * 100;
                        Oz = anchor2.transform.position.z * 100;
                        Fx = anchor3.transform.position.x * 100;
                        Fy = anchor3.transform.position.y * 100;
                        Fz = anchor3.transform.position.z * 100;
                        Dx = anchor4.transform.position.x * 100;
                        Dy = anchor4.transform.position.y * 100;
                        Dz = anchor4.transform.position.z * 100;
                       
                        float deg2rad = 0.01745329251f; //3.14159265359 / 180;
                        float alpha = alphaX * deg2rad;
                        float beta = alphaY * deg2rad;
                        float gamma = alphaZ * deg2rad;


                        float[,] rot = new float[3, 3]{ {Mathf.Cos(alpha)* Mathf.Cos(beta), (Mathf.Cos(alpha) * Mathf.Sin(beta)* Mathf.Sin(gamma) - Mathf.Sin(alpha)* Mathf.Cos(gamma)), (Mathf.Cos(alpha) * Mathf.Sin(beta)* Mathf.Cos(gamma) + Mathf.Sin(alpha)* Mathf.Sin(gamma))},
                                              {Mathf.Sin(alpha)* Mathf.Cos(beta), ((Mathf.Sin(alpha) * Mathf.Sin(beta)* Mathf.Sin(gamma)) + (Mathf.Cos(alpha) * Mathf.Cos(gamma)) ),((Mathf.Sin(alpha) * Mathf.Sin(beta)* Mathf.Cos(gamma)) - (Mathf.Cos(alpha) * Mathf.Sin(gamma)))},
                                              {-Mathf.Sin(beta), Mathf.Cos(beta)* Mathf.Sin(gamma), Mathf.Cos(beta)* Mathf.Cos(gamma)} };

                       
                        float[,] rotTrans = new float[3, 3]{{ rot[0, 0], rot[1, 0], rot[2, 0] },
                                              {rot[0, 1], rot[1, 1],rot[2, 1]},
                                              {rot[0, 2], rot[1, 2], rot[2, 2]} };

                        float[,] centerpoint = new float[3, 1] { { (rotTrans[0, 0] * Ox + rotTrans[0, 1] * Oy + rotTrans[0, 2] * Oz)},
                                { (rotTrans[1,0] * Ox + rotTrans[1,1] * Oy + rotTrans[1,2] * Oz) },
                                { (rotTrans[2,0] * Ox + rotTrans[2,1] * Oy + rotTrans[2,2] * Oz) } };
                        float[,] frontpoint = new float[3, 1] { { (rotTrans[0, 0] * Fx + rotTrans[0, 1] * Fy + rotTrans[0, 2] * Fz)},
                                { (rotTrans[1,0] * Fx + rotTrans[1,1] * Fy + rotTrans[1,2] * Fz) },
                                { (rotTrans[2,0] * Fx + rotTrans[2,1] * Fy + rotTrans[2,2] * Fz) } };
                        float[,] destinationpoint = new float[3, 1] { { (rotTrans[0, 0] * Dx + rotTrans[0, 1] * Dy + rotTrans[0, 2] * Dz)},
                                { (rotTrans[1,0] * Dx + rotTrans[1,1] * Dy + rotTrans[1,2] * Dz) },
                                { (rotTrans[2,0] * Dx + rotTrans[2,1] * Dy + rotTrans[2,2] * Dz) } };
                        float[,] B = new float[3, 1] { {  -(rotTrans[0,0] * Rx + rotTrans[0,1] * Ry + rotTrans[0,2] * Rz)},
                                { -(rotTrans[1,0] * Rx + rotTrans[1,1] * Ry + rotTrans[1,2] * Rz) },
                                {   -(rotTrans[2,0] * Rx + rotTrans[2,1] * Ry + rotTrans[2,2] * Rz) } };
                        float[,] centerpointlocation = new float[3, 1] { { centerpoint[0, 0] + B[0, 0] }, { centerpoint[1, 0] + B[1, 0] }, { centerpoint[2, 0] + B[2, 0] } };
                        float[,] frontpointlocation = new float[3, 1] { { frontpoint[0, 0] + B[0, 0] }, { frontpoint[1, 0] + B[1, 0] }, { frontpoint[2, 0] + B[2, 0] } };
                        float[,] destinationpointlocation = new float[3, 1] { { destinationpoint[0, 0] + B[0, 0] }, { destinationpoint[1, 0] + B[1, 0] }, { destinationpoint[2, 0] + B[2, 0] } };

                        // sending the object v

                        xcenter = centerpointlocation[0, 0];  
                        ycenter = centerpointlocation[2, 0];

                        xdestination = destinationpointlocation[0, 0];  
                        ydestination = destinationpointlocation[2, 0];
                        xfront = frontpointlocation[0, 0];
                        yfront = frontpointlocation[2, 0];
                        float dcdistance= Mathf.Sqrt(Mathf.Pow((ydestination - ycenter), 2F) + Mathf.Pow((xdestination - xcenter), 2F));
                        float dfdistance = Mathf.Sqrt(Mathf.Pow((ydestination - yfront), 2F) + Mathf.Pow((xdestination - xfront), 2F));
                        float fcdistance = Mathf.Sqrt(Mathf.Pow((yfront - ycenter), 2F) + Mathf.Pow((xfront - xcenter), 2F));
                        float dcdistances = Mathf.Pow((ydestination - ycenter), 2F) + Mathf.Pow((xdestination - xcenter), 2F);
                        float dfdistances = Mathf.Pow((ydestination - yfront), 2F) + Mathf.Pow((xdestination - xfront), 2F);
                        float fcdistances = Mathf.Pow((yfront - ycenter), 2F) + Mathf.Pow((xfront - xcenter), 2F);
                        float turnangleratio= ((dcdistances + fcdistances - dfdistances)/ (2 * dcdistance * fcdistance));
                        float turnanglerad = (Mathf.Acos(turnangleratio) * 10.0f) / 10.0f;
                        turnangle = turnanglerad * Mathf.Rad2Deg;
                        //Quaternion turnangletarget = Quaternion.Euler(0, turnangle, 0);

                        //targetObject4 = Instantiate(robot, hitone.transform.position, hitone.transform.rotation);
                        //targetObject4.transform.parent = anchor2.transform;
                        // The center of the arc
                        /*Vector3 center = (targetObject1.transform.position + targetObject3.transform.position) * 0.5F;
                        targetObject4.transform.rotation = Quaternion.Slerp(hitone.transform.rotation, turnangletarget, 1.0F);
                        // move the center a bit downwards to make the arc vertical
                        center -= new Vector3(0, 1, 0);
                        // Interpolate over the arc relative to center
                        Vector3 riseRelCenter = targetObject1.transform.position - center;
                        Vector3 setRelCenter = targetObject3.transform.position - center;

                        // The fraction of the animation that has happened so far is
                        // equal to the elapsed time divided by the desired time for
                        // the total journey.
                        float fracComplete = (Time.time - startTime) / journeyTime;

                        targetObject4.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, 0.01f);
                        targetObject4.transform.position += center;
                        */

                        if ((yfront > ycenter && xdestination < xcenter) || (yfront < ycenter && xdestination > xcenter))
                            turnangle = 360 - turnangle;
                        float distance = Mathf.Sqrt(Mathf.Pow((ydestination - ycenter), 2F) + Mathf.Pow((xdestination - xcenter), 2F));
                       
                        
                        //string xcenters = xcenter.ToString();
                        //string ycenters = ycenter.ToString();
                        //btmodule.sendMsg(xcenters);
                        //btmodule.sendMsg(ycenters);
                        //string xfronts = xfront.ToString();
                        //string yfronts = yfront.ToString();
                        //btmodule.sendMsg(xfronts);
                        //btmodule.sendMsg(yfronts);

                        //string xdestinations = xdestination.ToString();
                        //string ydestinations = ydestination.ToString();
                        //btmodule.sendMsg(xdestinations);
                        //btmodule.sendMsg(ydestinations);
                        //xcenter1.text = xcenter.ToString();
                        //ycenter1.text = ycenter.ToString();
                        //xfront1.text = xfront.ToString();
                        //yfront1.text = yfront.ToString();
                       // xdestination1.text = xdestination.ToString();
                       // ydestination1.text = ydestination.ToString();
                        //turnangle1.text = turnangle.ToString();
                        //distance1.text = distance.ToString();
                        string turnangles = turnangle.ToString();
                        string distances = distance.ToString();
                        btmodule.sendMsg(turnangles);
                        btmodule.sendMsg(distances);
                        //robot.transform.rotation = Quaternion.Euler(new Vector3(0, (0), turnangle));
                        float xaxis = distance*Mathf.Cos(turnangle);
                        float yaxis = distance*Mathf.Sin(turnangle);
                        //robot.transform.position = robot.transform.position + new Vector3(xaxis, yaxis, 0);
                        //float distance = rotTrans[1, 0];
                        //float turnangle12 = rotTrans[1, 2];
                        //string turnangle12s = turnangle12.ToString();
                        //string distances = distance.ToString();
                        //btmodule.sendMsg(turnangle12s);
                        //btmodule.sendMsg(distances);
                        onetime = false;
                        clearedALL = false;
                        //Quaternion turnangletarget = Quaternion.Euler(0, 0, 90);
                        //targetObject4.transform.rotation = Quaternion.Slerp(hitone1, turnangletarget, 1.0f);
                        fir3.GetComponent<Text>().text = "fir3";
                        distance1.text = distance.ToString();
                         StartCoroutine(TimerRoutine());

                    } //if (onetime) 

                } //else
				}
			}//ButtonClicked



		if (ButtonPressed) {

				Debug.Log ("Resetted the object");

			    GameObject.Destroy (targetObject1);
			    GameObject.Destroy (targetObject2);
            GameObject.Destroy(targetObject3);
            clearedALL = true;
				
			    count =0;

			}


	  
		foreach (var visualizer in m_Visualizers.Values)
		{
			if (visualizer.Image.TrackingState == TrackingState.Tracking)



			{
				FitToScanOverlay.SetActive(false);
				return;
			}

		}

		FitToScanOverlay.SetActive(true);

        //		if (ImageTracking) {
        //			FitToScanOverlay.SetActive(false);
        //			return;
        //		}
        //		FitToScanOverlay.SetActive(true);
        //

    } //void update

  IEnumerator TimerRoutine()
    {
        yield return new WaitForSeconds(5);
        Quaternion turnangletarget = Quaternion.Euler(0, turnangle, 0);
        targetObject4.transform.rotation = Quaternion.Slerp(hitone1, turnangletarget, 1.0f);
       StartCoroutine(TimerRoutine1());
    }
   IEnumerator TimerRoutine1()
    {
        yield return new WaitForSeconds(5);
        
        targetObject4.transform.position = Vector3.Slerp(hitone, hitone2, 1.0f);
        

    }
    
    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
		{
			// Exit the app when the 'back' button is pressed.
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}

			// Only allow the screen to sleep when not tracking.
			if (Session.Status != SessionStatus.Tracking)
			{
				const int lostTrackingSleepTimeout = 15;
				Screen.sleepTimeout = lostTrackingSleepTimeout;
			}
			else
			{
				Screen.sleepTimeout = SleepTimeout.NeverSleep;
			}

			if (m_IsQuitting)
			{
				return;
			}

			// Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
			if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
			{
				_ShowAndroidToastMessage("Camera permission is needed to run this application.");
				m_IsQuitting = true;
				Invoke("_DoQuit", 0.5f);
			}
			else if (Session.Status.IsError())
			{
				_ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
				m_IsQuitting = true;
				Invoke("_DoQuit", 0.5f);
			}
		}

		/// <summary>
		/// Actually quit the application.
		/// </summary>
		private void _DoQuit()
		{
			Application.Quit();
		}

		/// <summary>
		/// Show an Android toast message.
		/// </summary>
		/// <param name="message">Message string to show in the toast.</param>
		private void _ShowAndroidToastMessage(string message)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			if (unityActivity != null)
			{
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
					{
						AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
							message, 0);
						toastObject.Call("show");
					}));
			}
		}
	}

