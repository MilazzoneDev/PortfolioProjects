using UnityEngine;
using System.Collections;

public class EndLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		Debug.Log ("end level collision");
		if(col.gameObject.name == "Character")
		{
			if(Application.loadedLevel != Application.levelCount-1)
			{
				Debug.Log ("Next level");
				Application.LoadLevel(Application.loadedLevel+1);
			}
			else
			{
				Debug.Log ("end game");
				Application.LoadLevel (0);
			}
		}
	}
}
