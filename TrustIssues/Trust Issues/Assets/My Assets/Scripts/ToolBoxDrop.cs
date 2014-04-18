using UnityEngine;
using System.Collections;

public class ToolBoxDrop : MonoBehaviour {

    private TrustManager _trustManager;

    // Use this for initialization
    void Start()
    {
        _trustManager = GameObject.Find("GameManager").GetComponent<TrustManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void PickupBox()
    {
        Network.Destroy(networkView.viewID);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.collider.networkView.isMine && other.gameObject.GetComponent<FPSPlyerSync>() != null)
        {
            _trustManager.StandingOnToolBox(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.collider.networkView.isMine && other.gameObject.GetComponent<FPSPlyerSync>() != null)
        {
            _trustManager.StandingOnToolBox(null);
        }
    }
}
