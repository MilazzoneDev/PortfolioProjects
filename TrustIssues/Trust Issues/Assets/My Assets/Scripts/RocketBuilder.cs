using UnityEngine;
using System.Collections;

public class RocketBuilder : MonoBehaviour {
	
	private int _currentStage = 0;
	
	private TrustManager _trustManager;
	
	private GameObject _stage1;
	private GameObject _stage2;
	private GameObject _stage3;
	private GameObject _stage4;
	
	// Use this for initialization
	void Start () {
		_stage1 = transform.Find("Stage1").gameObject;
		_stage2 = transform.Find("Stage2").gameObject;
		_stage3 = transform.Find("Stage3").gameObject;
		_stage4 = transform.Find("Stage4").gameObject;
		
		_trustManager = GameObject.Find("GameManager").GetComponent<TrustManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	public bool completed()
	{
		return _currentStage >= 4;
	}
	
	public bool AddStage()
	{
		_currentStage++;
		networkView.RPC("setStageLevel", RPCMode.All, _currentStage);		
		return _currentStage >= 4;
	}
	
	[RPC]
	public void setStageLevel(int atStage)
	{
		_currentStage = atStage;
		switch(atStage)
		{
			case 0:
				_stage1.SetActive(false);
				_stage2.SetActive(false);
				_stage3.SetActive(false);
				_stage4.SetActive(false);
				break;
			case 1:
				_stage1.SetActive(true);
				_stage2.SetActive(false);
				_stage3.SetActive(false);
				_stage4.SetActive(false);
				break;
			case 2:
				_stage1.SetActive(true);
				_stage2.SetActive(true);
				_stage3.SetActive(false);
				_stage4.SetActive(false);
				break;
			case 3:
				_stage1.SetActive(true);
				_stage2.SetActive(true);
				_stage3.SetActive(true);
				_stage4.SetActive(false);
				break;
			case 4:
				_stage1.SetActive(true);
				_stage2.SetActive(true);
				_stage3.SetActive(true);
				_stage4.SetActive(true);
				break;
		}
	}
	
	void OnTriggerEnter(Collider other)
    {
        if (other.collider.networkView.isMine && other.gameObject.GetComponent<FPSPlyerSync>() != null)
        {
            _trustManager.InBuildZone(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.collider.networkView.isMine && other.gameObject.GetComponent<FPSPlyerSync>() != null)
        {
             _trustManager.InBuildZone(false);
        }
    }
}
