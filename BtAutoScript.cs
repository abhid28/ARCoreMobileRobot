using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TechTweaking.Bluetooth;
using UnityEngine.UI;


public class BtAutoScript : MonoBehaviour {

	private  BluetoothDevice device;
	public Text statusText;

	void Awake ()
	{
		device = new BluetoothDevice ();

		if (BluetoothAdapter.isBluetoothEnabled ()) {
			connect ();
		} else {

			//BluetoothAdapter.enableBluetooth(); //you can by this force enabling Bluetooth without asking the user
			statusText.text = "Status : Please enable your Bluetooth";

			BluetoothAdapter.OnBluetoothStateChanged += HandleOnBluetoothStateChanged;
			BluetoothAdapter.listenToBluetoothState (); // if you want to listen to the following two events  OnBluetoothOFF or OnBluetoothON

			BluetoothAdapter.askEnableBluetooth ();//Ask user to enable Bluetooth

		}
	}

	void Start ()
	{
		BluetoothAdapter.OnDeviceOFF += HandleOnDeviceOff;//This would mean a failure in connection! the reason might be that your remote device is OFF

		BluetoothAdapter.OnDeviceNotFound += HandleOnDeviceNotFound; //Because connecting using the 'Name' property is just searching, the Plugin might not find it!(only for 'Name').
	}

	private void connect ()
	{


		statusText.text = "Status : Trying To Connect";
		device.MacAddress = "00:14:03:06:0A:40";

		//device.Name = "HC-05";
	    device.ReadingCoroutine = ManageConnection;
		statusText.text = "Status : Trying to connect";
		device.connect ();

	}


	//############### Handlers/Recievers #####################
	void HandleOnBluetoothStateChanged (bool isBtEnabled)
	{
		if (isBtEnabled) {
			connect ();
			//We now don't need our recievers
			BluetoothAdapter.OnBluetoothStateChanged -= HandleOnBluetoothStateChanged;
			BluetoothAdapter.stopListenToBluetoothState ();
		}
	}

	//This would mean a failure in connection! the reason might be that your remote device is OFF
	void HandleOnDeviceOff (BluetoothDevice dev)
	{
		if (!string.IsNullOrEmpty (dev.Name)) {
			statusText.text = "Status : can't connect to '" + dev.Name + "', device is OFF ";
		} else if (!string.IsNullOrEmpty (dev.MacAddress)) {
			statusText.text = "Status : can't connect to '" + dev.MacAddress + "', device is OFF ";
		}
	}

	//Because connecting using the 'Name' property is just searching, the Plugin might not find it!.
	void HandleOnDeviceNotFound (BluetoothDevice dev)
	{
		if (!string.IsNullOrEmpty (dev.Name)) {
			statusText.text = "Status : Can't find a device with the name '" + dev.Name + "', device might be OFF or not paird yet ";

		} 
	}

	public void disconnect ()
	{
		if (device != null)
			device.close ();
	}

	//############### Reading Data  #####################
	//Please note that you don't have to use this Couroutienes/IEnumerator, you can just put your code in the Update() method.
	IEnumerator  ManageConnection (BluetoothDevice device)
	{
		statusText.text = "Status :Connected & Can read";

		while (device.IsReading) {

			byte [] msg = device.read ();
			if (msg != null) {


				string content = System.Text.ASCIIEncoding.ASCII.GetString (msg);
				statusText.text = "MSG : " + content;
			}
			yield return null;
		}

		statusText.text = "Status : Done Reading";
	}

	public void sendMsg(string val) {
		if (device != null) {

			//int Zvalue = zval;
			/// string Zsend = Zvalue.ToString();
			device.send(System.Text.Encoding.ASCII.GetBytes(val));
			device.send (System.Text.Encoding.ASCII.GetBytes ("\n"));
			// device.send (System.Text.Encoding.ASCII.GetBytes ("Hello\n"));
		}
	}


	//############### Deregister Events  #####################
	void OnDestroy ()
	{
		BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
		BluetoothAdapter.OnDeviceNotFound -= HandleOnDeviceNotFound;

	}


}
