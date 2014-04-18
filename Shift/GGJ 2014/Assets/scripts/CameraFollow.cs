using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject player;
	Vector3 velocity = Vector3.zero;
	public float smoothtime = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPos = player.transform.position;
		playerPos.x += 4;
		playerPos.z = transform.position.z;

		transform.position = Vector3.SmoothDamp(transform.position,playerPos,ref velocity,smoothtime);
	}
}
