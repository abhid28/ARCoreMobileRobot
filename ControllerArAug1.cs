    using GoogleARCore.Examples.HelloAR;
    using GoogleARCore.Examples.AugmentedImage;
	using System.Collections.Generic;
	using GoogleARCore;
	using GoogleARCore.Examples.Common;
	using UnityEngine;
#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controls the HelloAR example.
/// </summary>
public class ControllerArAug1 : MonoBehaviour
{
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
    public GameObject carcenterPrefab;
    public GameObject carfrontPrefab;
    public GameObject destinationPrefab;
    public GameObject MarkerPrefab;

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

    private Dictionary<int, AugmentedImageVisualizer> m_Visualizers = new Dictionary<int, AugmentedImageVisualizer>();

    private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();


    //New variables
    BtAutoScript btmodule;
    buttonScript buttonModule;
    bool ButtonClicked;
    bool ButtonPressed;
    bool doneOnce = false;
    bool clearedALL = true;
    bool firstDetect = true;
    GameObject targetObject1;
    GameObject targetObject2;
    GameObject targetObject3;
    GameObject markerCenter;
    GameObject robotworkspace;
    public GameObject workspace;

    Anchor anchor1;
    Anchor anchor2;
    Anchor anchor3;
    Anchor anchor4;
    Anchor Markeranchor;
    Vector3 startPose;
    Vector3 endPose;
    bool ImageTracking = false;
    int count = 0;
    Vector3 thePosition;
    const int N = 3;

    float[,] rotTrans;
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
    // public int reachscale =2;
    // Vector3 offsetVector = new Vector3(0,0,-0.01);


    public void Start() {
        btmodule = gameObject.GetComponent<BtAutoScript>();
        buttonModule = gameObject.GetComponent<buttonScript>();
        //ButtonClicked = gameObject.GetComponent<buttonScript> ().isClicked;
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    /// 

    public void Update()
    {

        ButtonClicked = buttonModule.isClicked;
        // Debug.Log("Button status:"+ ButtonClicked);
        ButtonPressed = buttonModule.isPressed;
        _UpdateApplicationLifecycle();

        //--------------------------------------------------------------------------------------------------
        // ------------------------Marker detection------------------------------------------------------
        // --------------------------------------------------------------------------------------------------
        // Get updated augmented images for this frame.
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
                    markerCenter = Instantiate(MarkerPrefab, Markeranchor.transform.position, Markeranchor.transform.rotation);
                    //markerCenter.transform.parent = Markeranchor.transform;
                    Vector3 worklocation = (Markeranchor.transform.position + Markeranchor.transform.forward * 0.095f);
                    // adjusting the workspace. The origin of the workspace plane is ahead of the anchor origin
                    robotworkspace = Instantiate(workspace, worklocation, Markeranchor.transform.rotation);

                    firstDetect = false;
                    Debug.Log("markerCenterX:" + Markeranchor.transform.position.x * 100 + ", markerCenterY:" + Markeranchor.transform.position.y * 100 + ", markerCenterZ:" + Markeranchor.transform.position.z * 100);
                }



                visualizer = (AugmentedImageVisualizer)Instantiate(AugmentedImageVisualizerPrefab, anchor1.transform);
                visualizer.Image = image;
                m_Visualizers.Add(image.DatabaseIndex, visualizer);
                visualizer.gameObject.SetActive(false); // this line is fort not showing the frame.

                //------------------------- Adding a visual for showing the robot workspace----------------------------
                // Not updating it always. Detecting and storin the values only once at the beginning.
                //-----------------------------------------------------------------------------------------------------

                //markerCenter = Instantiate (MarkerPrefab,anchor1.transform.position, anchor1.transform.rotation);
                //markerCenter.transform.parent = anchor1.transform;
                //Vector3 worklocation = (anchor1.transform.position + anchor1.transform.forward*0.095f); // adjusting the workspace. The origin of the workspace plane is ahead of the anchor origin
                //robotworkspace = Instantiate (workspace,worklocation, anchor1.transform.rotation);//Quaternion.identity);
                //robotworkspace.transform.parent = anchor1.transform;
                //-------------------------------------------------------------------------------------------------
                //Debug.Log ("Marker  center pose:" + image.CenterPose.position);
                Debug.Log("Marker height:" + image.ExtentZ);
                Debug.Log("Marker width:" + image.ExtentX);


                Debug.Log("anchor1X:" + anchor1.transform.position.x * 100 + ", anchor1Y:" + anchor1.transform.position.y * 100 + ", anchor1Z:" + anchor1.transform.position.z * 100);
                //Debug.Log ("MarkerXangle=" + anchor1.transform.eulerAngles.x + ", MarkerYangle=" + anchor1.transform.eulerAngles.y+ ", MarkerZangle=" + anchor1.transform.eulerAngles.z);
            }
            else if (image.TrackingState == TrackingState.Stopped && visualizer != null)

            {
                m_Visualizers.Remove(image.DatabaseIndex);
                //GameObject.Destroy (markerCenter.gameObject);
                markerCenter.gameObject.SetActive(false);

                GameObject.Destroy(visualizer.gameObject);
                visualizer.gameObject.SetActive(false);
            }

            //Debug.Log ("The current marker image center: X=" + image.CenterPose.position.x * 100 + ", Y=" + image.CenterPose.position.y * 100 + ", Z=" + image.CenterPose.position.z * 100);
        }



        //-----------------------------------------------------------------------------
        // Script for ground plane detection starts here.
        //-----------------------------------------------------------------------------
        // Hide snackbar when currently tracking at least one plane.
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

        // If the player has not touched the screen, we are done with this update.
        // Touch touch;
        // if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        //{
        //    return;
        // }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (ButtonClicked && !doneOnce && clearedALL) {
            //Debug.Log ("Button clicked");
            //if (Frame.Raycast (touch.position.x, touch.position.y, raycastFilter, out hit)) {
            if (Frame.Raycast((float)Screen.width / 2, (float)Screen.height / 2, raycastFilter, out hit)) {

                // Use hit pose and camera pose to check if hittest is from the back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) && Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0) {
                    Debug.Log("Hit at back of the current DetectedPlane");
                } else {

                    count = count + 1;
                    if (count == 1) {
                        anchor2 = hit.Trackable.CreateAnchor(hit.Pose);
                        targetObject1 = Instantiate(carcenterPrefab, hit.Pose.position, hit.Pose.rotation);
                        targetObject1.transform.parent = anchor2.transform;
                        
                    }
                    else if (count == 2)
                    {
                        anchor3 = hit.Trackable.CreateAnchor(hit.Pose);
                        targetObject2 = Instantiate(carfrontPrefab, hit.Pose.position, hit.Pose.rotation);
                        targetObject2.transform.parent = anchor3.transform;
                       
                    }
                    else
                    {
                        anchor4 = hit.Trackable.CreateAnchor(hit.Pose);
                        targetObject3 = Instantiate(destinationPrefab, hit.Pose.position, hit.Pose.rotation);
                        targetObject3.transform.parent = anchor4.transform;
                        count = 0;
                        doneOnce = true; }

                    // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
                    //targetObject.transform.Rotate (0, k_ModelRotation, 0, Space.Self);

                    //................................. Non homogenous transformation.....................................
                    //............................Sending Marker (angle,position) and object's pick and pose (position) to the Manipulator
                    //.....................................................................................................

                    if (doneOnce) {
                        float alphaX, alphaY, alphaZ;
                        alphaX = anchor1.transform.eulerAngles.x;
                        alphaY = anchor1.transform.eulerAngles.y;
                        alphaZ = anchor1.transform.eulerAngles.z;
                        Rx = anchor1.transform.position.x;
                        Ry = anchor1.transform.position.y;
                        Rz = anchor1.transform.position.z;
                        Ox = anchor2.transform.position.x;
                        Oy = anchor2.transform.position.y;
                        Oz = anchor2.transform.position.z;
                        Fx = anchor3.transform.position.x;
                        Fy = anchor3.transform.position.y;
                        Fz = anchor3.transform.position.z;
                        Dx = anchor4.transform.position.x;
                        Dy = anchor4.transform.position.y;
                        Dz = anchor4.transform.position.z;
                        // [,] rotTrans = new float[3, 4]{}
                        float deg2rad = 0.01745329251f; //3.14159265359 / 180;
                        float alpha = alphaX * deg2rad;
                        float beta = alphaY * deg2rad;
                        float gamma = alphaZ * deg2rad;


                        float[,] rot = new float[3, 3]{ {Mathf.Cos(alpha)* Mathf.Cos(beta), (Mathf.Cos(alpha) * Mathf.Sin(beta)* Mathf.Sin(gamma) - Mathf.Sin(alpha)* Mathf.Cos(gamma)), (Mathf.Cos(alpha) * Mathf.Sin(beta)* Mathf.Cos(gamma) + Mathf.Sin(alpha)* Mathf.Sin(gamma))},
                                              {Mathf.Sin(alpha)* Mathf.Cos(beta), ((Mathf.Sin(alpha) * Mathf.Sin(beta)* Mathf.Sin(gamma)) - (Mathf.Cos(alpha) * Mathf.Cos(gamma)) ),((Mathf.Sin(alpha) * Mathf.Sin(beta)* Mathf.Cos(gamma)) - (Mathf.Cos(alpha) * Mathf.Sin(gamma)))},
                                              {-Mathf.Sin(beta), Mathf.Cos(beta)* Mathf.Sin(gamma), Mathf.Cos(beta)* Mathf.Cos(gamma)} };

                        //Transposing rot, rRc= Transpose of cRr
                        //Finding : Calcualting rRc
                        for (int i = 0; i < 3; i++)
                            for (int j = 0; j < 3; j++)
                                rotTrans[j, i] = rot[i, j];


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

                        xcenter = centerpointlocation[0, 0] * 10;  //converting to mm scale by multipying by 10
                        ycenter = centerpointlocation[1, 0] * 10;

                        xdestination = destinationpointlocation[0, 0] * 10;  //converting to mm scale by multipying by 10
                        ydestination = destinationpointlocation[1, 0] * 10;
                        xfront = frontpointlocation[0, 0] * 10;  //converting to mm scale by multipying by 10
                        yfront = frontpointlocation[1, 0] * 10;
                        float turnangle = (Mathf.Atan2((ycenter - yfront), (xcenter - xfront) * Mathf.Rad2Deg) * 10.0f) / 10.0f;
                        float turnangle2 = (Mathf.Atan2((ydestination - yfront), (xdestination - xfront) * Mathf.Rad2Deg) * 10.0f) / 10.0f;
                        float distance = Mathf.Sqrt(Mathf.Pow((ydestination - ycenter), 2F) + Mathf.Pow((xdestination - xcenter), 2F));
                        float turnangle12 = (180 - turnangle2 + turnangle);
                        string turnangle12s = turnangle12.ToString();
                        string distances = distance.ToString();
                        btmodule.sendMsg(turnangle12s);
                        btmodule.sendMsg(distances);

                        //string rxs = Rx.ToString();
                        //string rys = Ry.ToString();
                        //btmodule.sendMsg(rxs);
                        //btmodule.sendMsg(rys);

                        /*float distance = 400;
                        float turnangle12 =100;
                        string turnangle12s = turnangle12.ToString();
                        string distances = distance.ToString();
                        btmodule.sendMsg(turnangle12s);
                        btmodule.sendMsg(distances);*/


                        doneOnce = false;
						clearedALL = false;
					   
					} //if (doneOnce) 

				   } //else
				}
			}//ButtonClicked


		// Fore resetting and selecting new object
		if (ButtonPressed) {

				Debug.Log ("Resetted the object");
				//robot.SetActive (false);
           		//targetObject1.SetActive (false);
    		    //targetObject2.SetActive (false);
			    GameObject.Destroy (targetObject1);
			    GameObject.Destroy (targetObject2);
                GameObject.Destroy(targetObject3);
            clearedALL = true;
				//doneOnce = false;
			    count =0;

			}


	  // Show the fit-to-scan overlay if there are no images that are Tracking.
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

