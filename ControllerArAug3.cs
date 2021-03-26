    using GoogleARCore.Examples.HelloAR;
    using GoogleARCore.Examples.AugmentedImage;
	using System.Collections.Generic;
	using GoogleARCore;
	using GoogleARCore.Examples.Common;
	using UnityEngine;
    using System;
#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controls the HelloAR example.
/// </summary>

public class ControllerArAug3 : MonoBehaviour { }
 /* {
  float tv, mb, ydestination, yfront, xdestination, xfront;
{
    float xcenter = 4.0f;
    float ycenter = 0.0f;
    xfront = 0.0f;
    yfront = 0.0f;
    xdestination = 4.0f;
   ydestination = 4.0f;
    tv = (ydestination - ycenter);
    mb = (xdestination - xcenter);
    float dcdistance = Math.Sqrt(Math.Pow(tv, 2F) + Math.Pow(mb, 2F));
    float dfdistance = Math.Sqrt(Math.Pow((ydestination - yfront), 2F) + Math.Pow((xdestination - xfront), 2F));
    float fcdistance = Math.Sqrt(Math.Pow((yfront - ycenter), 2F) + Math.Pow((xfront - xcenter), 2F));
    float dcdistances = Math.Pow((ydestination - ycenter), 2F) + Math.Pow((xdestination - xcenter), 2F);
    float dfdistances = Math.Pow((ydestination - yfront), 2F) + Math.Pow((xdestination - xfront), 2F);
    float fcdistances = Math.Pow((yfront - ycenter), 2F) + Math.Pow((xfront - xcenter), 2F);
    float turnangleratio = ((dcdistances + fcdistances - dfdistances) / (2 * dcdistance * fcdistance));
    float turnanglerad = (Math.Acos(turnangleratio) * 10.0f) / 10.0f;
    float turnangle = turnanglerad * 57;
    float distance = Math.Sqrt(Math.Pow((ydestination - ycenter), 2F) + Math.Pow((xdestination - xcenter), 2F));
   //Console.Write;
}*/
