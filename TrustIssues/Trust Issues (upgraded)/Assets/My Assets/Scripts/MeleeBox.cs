using UnityEngine;
using System.Collections.Generic;

public class MeleeBox : MonoBehaviour {
	
	public List<GameObject> hitting;
	// Use this for initialization
	void Start () {
		hitting = new List<GameObject>();
	}
	
	void OnTriggerEnter(Collider other)
    {
        hitting.Add(other.gameObject);
		Debug.Log(other.gameObject.name);
    }
}
