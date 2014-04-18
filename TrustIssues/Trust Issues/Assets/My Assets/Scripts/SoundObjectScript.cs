using UnityEngine;
using System.Collections;

public class SoundObjectScript : MonoBehaviour {

    float timer = 1.5f;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            GameObject.Destroy(this.gameObject);
        }
	}
}
