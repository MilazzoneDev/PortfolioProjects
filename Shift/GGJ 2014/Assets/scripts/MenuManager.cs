using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public static MenuManager _instance = null;

	// Use this for initialization
	void Start () {
		if(!_instance)
		{
			DontDestroyOnLoad(gameObject);
			_instance = this;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnLevelWasLoaded(int level)
	{
		Debug.Log ("level " + level + " was loaded");
		if(level == 0)
		{
			GameObject.Find("Main Camera").GetComponent<MainMenu>().cur = 3;
			Vector3 hide = new Vector3(0,0,2);
			GameObject.Find ("Main Menu").transform.position = hide;
		}
	}
}
