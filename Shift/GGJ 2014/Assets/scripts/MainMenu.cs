using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {


	private GameObject Icon;
	public List<Vector3> Positions;

	public int cur = 0;

	void Start()
	{

		Icon = GameObject.Find ("Mark");
		Positions = new List<Vector3>();
		Positions.Add(new Vector3(-1.8f,1,0f));
		Positions.Add(new Vector3(-1.8f,-0.42f,0.0f));
		Positions.Add(new Vector3(-1.8f,-1.77f,0.0f));
		Positions.Add(new Vector3(-1.8f,-2.7f,0.0f));
	}
	
	/*void OnGUI()
	{
		//draws texture
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), backgroundTexture);

		GUI.Box(new Rect(Current.x,Current.y,100,200),IconTexture);
		//draws buttons
		//if(GUI.Button(new Rect(Screen.width * .1f, Screen.height * .5f, Screen.width * .3f, Screen.height * .1f), "Play Game"))
		//{
		//	//Debug.Log("Clicked Play Game");
		//	//Application.LoadLevel("gameJamPrototype");
		//}
		//
		//if(GUI.Button(new Rect(Screen.width * .1f, Screen.height * guiPlacementY, Screen.width * .3f, Screen.height * .1f), "Options"))
		//{
		//	//Debug.Log("Clicked Options");
		//}
		Current = Positions[cur];
	}*/
	void Update()
	{

		Icon.transform.position = Positions[cur];


		if(GameObject.Find ("Main Menu").transform.position.z == 0)
		{
			if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				if(cur !=0 )
				{
					cur--;
				}
				else
				{
					cur = 2;
				}
			}
			if(Input.GetKeyDown(KeyCode.DownArrow))
			{
				if(cur != 2 )
				{
					cur++;
				}
				else
				{
					cur = 0;
				}
			}
		}
			
		if(Input.GetKeyDown(KeyCode.Return))
		{
			if( cur == 0)
			{
				Application.LoadLevel(1);
			}
			else if( cur == 1)
			{
				Vector3 hide = new Vector3(0,0,2);
				GameObject.Find ("Main Menu").transform.position = hide;
				cur = 3;
			}
			else if ( cur == 2)
			{
				Application.Quit();
			}
			else if (cur == 3)
			{
				Vector3 show = new Vector3(0,0,0);
				GameObject.Find ("Main Menu").transform.position = show;
				cur = 1;
			}
		}
	}
}
