using UnityEngine;
using System.Collections;

public class FPSPlyerSync : MonoBehaviour {
	
	private GameObject cam;
	private Transform _camPoint;
	private int currentWeapon = TrustManager.WRENCH_ID;
	private bool weaponChanged = false;
	
	
	private GameObject _wrench;
	private GameObject _pistol;
	private GameObject _boltgun;
	private GameObject _cleaver;
	
	private double _swingTime = -0.25;
	private bool _swinging = false;
	
	private GameObject _armJoint;
	
	private Quaternion _armPos;
	
	
	//Used for emotions
	private GameObject _smile;
	private GameObject _angry;
	private GameObject _sad;
	private GameObject _scared;
	private GameObject _question;
	private GameObject _help;
	
	private GameObject _toolBox;
	
	private GameObject _body;
	
	private GameObject _light;
	private GameObject _activeEmotion;
	
	private bool activeEmote;
	private float emoteDiration = 5;
	private float oldTimer = 0;
	
	// sets up the camera to be put into the player and synced when the player spawns
	void Start()
	{
		cam = GameObject.Find("GameCamera");
		_camPoint = transform.Find("Camera_Anchor");
		
		_wrench = transform.Find("PlayerBody/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Weapons/wrench").gameObject;
		_pistol = transform.Find("PlayerBody/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Weapons/RevolveMove").gameObject;
		_boltgun = transform.Find("PlayerBody/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Weapons/RGun").gameObject;
		_cleaver = transform.Find("PlayerBody/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Weapons/Cleaver").gameObject;
		
		_toolBox = transform.Find("ToolBox").gameObject;
		
		_armJoint = transform.Find("PlayerBody/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle").gameObject;
		_armPos= Quaternion.Euler(-13.74927f,79.94571f,-178.9141f);
		
		_smile = transform.Find ("Emotes/Smile").gameObject;
		_angry = transform.Find ("Emotes/Angry").gameObject;
		_sad = transform.Find ("Emotes/Sad").gameObject;
		_scared = transform.Find ("Emotes/Scared").gameObject;
		_question = transform.Find ("Emotes/Question").gameObject;
		_help = transform.Find ("Emotes/Help").gameObject;
		
		_light = transform.Find ("Emotes/Light").gameObject;
		_activeEmotion = null;
		
		_body = transform.Find("PlayerBody").gameObject;
		
		
		Debug.Log(_wrench);
		
		if(_wrench == null)
		{
			Debug.Log("Wrench is null");
		}
		activeEmote = false;
	}
	
	
	void Update()
	{
		if(networkView.isMine)
		{
			cam.transform.position = _camPoint.position;
			cam.transform.rotation = _camPoint.rotation;
		}
		/*if(_body.activeSelf)
		{*/
			if(_swinging == true)
			{
				moveArm();
			}
			if(weaponChanged)
			{
				switch(currentWeapon)
				{
					case TrustManager.WRENCH_ID:
						_wrench.SetActive(true);
						_pistol.SetActive(false);
						_boltgun.SetActive(false);
						_cleaver.SetActive(false);
						break;
					case TrustManager.PISTOL_ID:
						_wrench.SetActive(false);
						_pistol.SetActive(true);
						_boltgun.SetActive(false);
						_cleaver.SetActive(false);
						break;
					case TrustManager.BOLT_GUN_ID:
						_wrench.SetActive(false);
						_pistol.SetActive(false);
						_boltgun.SetActive(true);
						_cleaver.SetActive(false);
						break;
					case TrustManager.CLEAVER_ID:
						_wrench.SetActive(false);
						_pistol.SetActive(false);
						_boltgun.SetActive(false);
						_cleaver.SetActive(true);
						break;
				}
				weaponChanged = false;
			}
			if(activeEmote)
			{
				//make sure timer has started
				if(oldTimer == 0)
				{
					oldTimer = Time.time;
					_light.SetActive(true);
					
				}
				/*Quaternion emoteRotate = _activeEmotion.transform.rotation;
				if(!networkView.isMine)
				{
					emoteRotate.y = cam.transform.rotation.y + 90;
					emoteRotate.x = 90;
					emoteRotate.z = 0;
					_activeEmotion.transform.rotation = emoteRotate;
				}*/
				//check if timer has gone over time
				if(oldTimer + emoteDiration <= Time.time)
				{
					//if it has end it/set to null, then reset the emote
					oldTimer = 0;
					UseEmotion (0);
					activeEmote = false;
					_light.SetActive(false);
				}
			}
		
		
	}
	
	public void SwingAttack()
	{
		//_armJoint.transform.Rotate(
		_swinging = true;
	}
	
	private void moveArm()
	{
		_swingTime += Time.deltaTime;
		if(_swingTime < 0)
		{
			_armJoint.transform.Rotate(-3.0f,0.0f,0.0f);
		}
		else if(_swingTime < 0.25)
		{
			_armJoint.transform.Rotate(3.0f,0.0f,0.0f);
		}
		else
		{
			_swingTime = -0.25;
			_swinging = false;
			//_armJoint.transform.rotation = _armPos;
		}
		
	}
	
	public void ChangeWeapon(int weapoonID)
	{
		currentWeapon = weapoonID;
		weaponChanged = true;
	}
	
	
	public void ChangeToolBox(bool carrying)
	{
		if(carrying)
		{
			_toolBox.SetActive(true);
		}
		else
		{
			_toolBox.SetActive(false);
		}
	}
	
	public void UseEmotion(int emotionID)
	{
		if(_body.activeSelf)
		{
			switch(emotionID)
			{
				case TrustManager.SMILE_ID:
					_smile.SetActive(true);
					_activeEmotion = _smile;
					Debug.Log ("SIMLE EMOTE");
					break;
				case TrustManager.ANGRY_ID:
					_angry.SetActive(true);
					_activeEmotion = _angry;
					break;
				case TrustManager.SAD_ID:
					_sad.SetActive(true);
					_activeEmotion = _sad;
					break;
				case TrustManager.SCARED_ID:
					_scared.SetActive(true);
					_activeEmotion = _scared;
					break;
				case TrustManager.QUESTION_ID:
					_question.SetActive(true);
					_activeEmotion = _question;
					break;
				case TrustManager.HELP_ID:
					_help.SetActive(true);
					_activeEmotion = _help;
					break;
				default:
					_smile.SetActive(false);
					_angry.SetActive(false);
					_sad.SetActive(false);
					_scared.SetActive(false);
					_question.SetActive(false);
					_help.SetActive(false);
					activeEmote = false;
					Debug.Log ("emotion Reset");
					break;
			}
		}
	}
	
	public void ChangeEmotion(int emotionID)
	{
		if(!networkView.isMine)
		{
			UseEmotion (0);//return all to false
			activeEmote = true;
			UseEmotion (emotionID);
		}
	}
	
	public void Die()
	{
		_body.SetActive(false);	
		_toolBox.SetActive(false);
	}
	
	// keep the players details together
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(stream.isWriting)
		{
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;
			int curW = currentWeapon;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref curW);
		}
		else
		{
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			int curW = 0;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref curW);
			transform.position = pos;
			transform.rotation = rot;
			if(curW != currentWeapon)
			{
				currentWeapon = curW;
				weaponChanged = true;
			}
		}
	}
}
