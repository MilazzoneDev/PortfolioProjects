using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public bool rotate = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(rotate)
			transform.Rotate(0,0,-.5f);

	}
}
