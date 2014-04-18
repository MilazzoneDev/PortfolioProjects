using UnityEngine;
using System.Collections;

public class PlayerSync : MonoBehaviour {

	public GameObject cam;
	// sets up the camera to be put into the player and synced when the player spawns
	void Start()
	{
		cam = GameObject.Find("GameCamera");
		//Debug.Log ("CAM: " + cam);
	}
	
	void Update()
	{
		//this.gameObject.GetComponentInChildren("Camera"
		if(networkView.isMine)
		{
			cam.transform.position = this.transform.position + new Vector3(0,0.5f,0);
			cam.transform.rotation = this.transform.rotation;
		}
	}
	
	// keep the players details together
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(stream.isWriting)
		{
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
		}
		else
		{
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			transform.position = pos;
			transform.rotation = rot;
			
		}
	}
}
