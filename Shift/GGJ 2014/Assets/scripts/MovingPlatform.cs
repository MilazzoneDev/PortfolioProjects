using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour {

	public float useSpeed;	
	public float directionSpeed = 9.0f;	
	float origY;
	float origX;	
	public float distance = 30.0f;	
	public bool VerticalOrHorizontal = false;
	List<string> typeList;
	public string type;
	public int current = 0;
	bool changeMotion = false;
	// Use this for initialization
	
	void Start () 		
	{		
		typeList = new List<string>();
		typeList.Add("Vertical");
		typeList.Add("Horizontal");
		typeList.Add("Freeze");
		type = typeList[current];
		origY = transform.position.y;
		origX = transform.position.x;
		useSpeed = directionSpeed;		
	}
	
	
	public void cycle(int specific)
	{		
		if(specific == 0)
		{
			if(current == 2)
			{
				changeMotion = true;
				if(origX == transform.position.x)
				{
					current = 1;
					type = "Horizontal";
				}
				else
				{
					current = 0;
					type = "Vertical";
				}
				changeMotion = true;
			}
			else if(current == 1)
			{
				changeMotion = true;
				current = specific;
			}

		}
		else if(specific == 1)
		{
			if(current == 2)
			{
				changeMotion = true;
				if(origY == transform.position.y)
				{
					current = 0;
					type = "Vertical";
				}
				else
				{
					current = 1;
					type = "Horizontal";
				}
			}
			else if(current == 0)
			{
				changeMotion = true;
				current = specific;
			}
		}
		else
		{
			current = 2;
			type = "Frozen";
		}
		Debug.Log (typeList[current]);
	}
	// Update is called once per frame
	
	void Update ()		
	{
		type = typeList[current];

		if(origX == transform.position.x && origY == transform.position.y)
		{
			if(changeMotion)
			{
				switch(current)
				{
					case 0:
						type = "Vertical";
						break;
					case 1:
						type = "Horizontal";
						break;
					default:
						type = "Frozen";
						break;
				}
			}
		}

		switch(type)
		{
			case "Vertical":
				Vertical();
				break;
			case "Horizontal":
				Horizontal();			
				break;
			case "Freeze":
				break;
			default:
				break;
		
		}		
	}

	void Horizontal()
	{
		if(origX - transform.position.x > distance)			
		{			
			useSpeed = directionSpeed; //flip direction			
		}		
		else if(origX - transform.position.x < -distance)			
		{			
			useSpeed = -directionSpeed; //flip direction			
		}		
		transform.Translate(useSpeed*Time.deltaTime,0,0);
	}
		
	void Vertical()
	{
		if(origY - transform.position.y > distance)
			useSpeed = directionSpeed; //flip direction
		else if(origY - transform.position.y < -distance)
			useSpeed = -directionSpeed; //flip direction
		transform.Translate(0,useSpeed * Time.deltaTime,0);
	
	}
}
