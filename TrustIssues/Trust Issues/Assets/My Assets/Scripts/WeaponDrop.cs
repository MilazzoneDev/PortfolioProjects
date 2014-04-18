using UnityEngine;
using System.Collections;

public class WeaponDrop : MonoBehaviour
{

    public int _weaponType = TrustManager.BOLT_GUN_ID;

    private bool _changed;

    private TrustManager _trustManager;

    public GameObject WrenchModel;
    public GameObject PistolModel;
    public GameObject BoltGunModel;
    public GameObject CleaverModel;

    private GameObject _currentModel;

    // Use this for initialization
    void Start()
    {
        _changed = true;
        _weaponType = Random.Range(101, 105);

        _trustManager = GameObject.Find("GameManager").GetComponent<TrustManager>();
		if(Network.peerType == NetworkPeerType.Server)
		{
			networkView.RPC("SetWeaponType", RPCMode.All, _weaponType);
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (_changed)
        {
            if (_currentModel != null)
            {
                Destroy(_currentModel);
                _currentModel = null;
            }
            _changed = false;
            // change model
            switch (_weaponType)
            {
                // change
                case TrustManager.WRENCH_ID:
                    _currentModel = Instantiate(WrenchModel, this.transform.position, Quaternion.identity) as GameObject;
                    break;
                case TrustManager.PISTOL_ID:
                    _currentModel = Instantiate(PistolModel, this.transform.position, Quaternion.identity) as GameObject;
                    break;
                case TrustManager.BOLT_GUN_ID:
                    _currentModel = Instantiate(BoltGunModel, this.transform.position, Quaternion.identity) as GameObject;
                    break;
                case TrustManager.CLEAVER_ID:
                    _currentModel = Instantiate(CleaverModel, this.transform.position, Quaternion.identity) as GameObject;
                    break;
            }
        }
    }

    [RPC]
    void SetWeaponType(int type)
    {
        _changed = true;
        _weaponType = type;
    }

    public void ChangeWeapon(int ID)
    {
        networkView.RPC("SetWeaponType", RPCMode.All, ID);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.collider.networkView.isMine && other.gameObject.GetComponent<FPSPlyerSync>() != null)
        {
            _trustManager.StandingOnWeapon(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.collider.networkView.isMine && other.gameObject.GetComponent<FPSPlyerSync>() != null)
        {
            _trustManager.StandingOnWeapon(null);
        }
    }

}
