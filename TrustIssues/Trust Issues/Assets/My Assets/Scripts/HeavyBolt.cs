using UnityEngine;
using System.Collections;

public class HeavyBolt : MonoBehaviour {
	
	// this is used to kill the lazer shorlty after it is spawned into the world
	int framesAlive = 10;
	GameObject _bullet;
	
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		framesAlive--;
		if(framesAlive <= 0 && Network.peerType == NetworkPeerType.Server) // only the server kills it to avoid error
		{
			Network.Destroy(this.networkView.viewID);
		}
	}
}
