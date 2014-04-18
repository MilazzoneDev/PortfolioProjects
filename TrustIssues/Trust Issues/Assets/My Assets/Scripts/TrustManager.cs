using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum gameState { LOBBY, GAME_STARTED, TEAMS, GAME_OVER };
public enum playerState { UNSPAWNED, ALIVE, DEAD };

public struct PlayerStats
{
	public string name;
	public float life; // 0 - 100
	public int allegiance;
	public int weapon;
    public playerState currentStatus;

    public PlayerStats(string n, float l, int a, int w, playerState s)
    {
        name = n;
        life = l;
        allegiance = a;
        weapon = w;
        currentStatus = s;
    }
}

public class TrustManager : MonoBehaviour {
	
#region GAME CONSTANTS
	public const float _secondsPerGame = 300;
    public const float _introTime = 3;
    const float _gameOverTime = 10;
	
	const int SAB_DENOMINATOR = 3;
	
	public const int WRENCH_DAMAGE = 10;
	public const int CLEAVER_DAMAGE = 80;
	public const int PISTOL_DAMAGE = 20;
	public const int BOLT_GUN_DAMAGE = 100;
	
	public const int WRENCH_ID = 101;
	public const int PISTOL_ID = 102;
	public const int CLEAVER_ID = 103;
	public const int BOLT_GUN_ID = 104;
	
	public const int SABOTEUR_ID = 200;
	public const int BUILDER_ID = 201;
	
	public const float MAX_HEALTH_VISION = 4;
	
	private const float MAX_HEALTH = 100;
	
	private const float BUILD_TIMER = 4;
	private const float PICKUP_TIMER = 2;
#endregion
	
	float _currentTimer = 0;
	gameState currentGameState = gameState.LOBBY;
	private GameObject[] spawnPoints;
	private GameObject _me;
	private float Health;
	
	private GameObject[] GO_Players;
    
#region PREFABS AND MATERIALS
	public GameObject PlayerPrefab;
	public GameObject LazerPrefab;
	public GameObject MeleeBoxPrefab;
    public GameObject CorpsePrefab;
    public GameObject WeaponDropPrefab;
	public GameObject ToolBoxPrefab;
	public Texture2D Reticle;
	public Material[] _matts;
#endregion

	
#region EMOTIONS
	public const int SMILE_ID = 1;
	public const int ANGRY_ID = 2;
	public const int SAD_ID = 3;
	public const int SCARED_ID = 4;
	public const int QUESTION_ID = 5;
	public const int HELP_ID = 6;
#endregion
	
	private bool oldMouseDown = false;
	private bool currentMouseDown = false;
	
	private Dictionary<string, PlayerStats> _stats;

    public GUIStyle progress_empty;
    public GUIStyle progress_full;

    public Camera mainCam;

    private bool statsReceived = false;
    private string drawOtherHealthNow = "";
	
	private bool rocketBuilt = false;

    #region GUI
    //Health bar stuff
    private Texture2D whitePixel;
    private float barDisplay;

    Rect healthBar = new Rect(10, 10, Screen.width/3.2f, 70);
	Rect buildBar = new Rect(Screen.width/3, (Screen.height * 4)/7, Screen.width/3, 40);
    Rect otherHealthBar;

    private float barOtherDisplay;

    //Team name
    Vector2 teamPos = new Vector2(250, 0);
    Vector2 teamSize = new Vector2(300, 50);
    public Texture2D saboTex;
    public Texture2D buildTex;

    //Game over
    Vector2 endPos = new Vector2(150, 100);
    Vector2 endSize = new Vector2(500, 150);
    public Texture2D vicTex;
    public Texture2D defTex;
    bool victor;
	
	private Vector2 reticlePos;
	
	// style of in game text
	private GUIStyle _textStyle = new GUIStyle();
	
	WeaponDrop _weaponStandingOn = null;
	RocketBuilder _rocketPlatform;
	bool _caryingToolBox = false;
	
	ToolBoxDrop _amOnToolBox = null;
	bool _inBuildZone = false;
	float _progressBarValue = 0; 
	
	
    #endregion

    // Use this for initialization
    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        _stats = new Dictionary<string, PlayerStats>();

        reticlePos = new Vector2((Screen.width/2) - (Reticle.width/2),(Screen.height/2) - (Reticle.height/2));
        otherHealthBar = new Rect(reticlePos.x, reticlePos.y + Reticle.height + 10, 150, 25);
		
		_textStyle.normal.textColor = Color.black;
		_textStyle.fontSize = 20;
		
		_rocketPlatform = GameObject.Find("RocketPlatform").GetComponent<RocketBuilder>();
		
		Health = MAX_HEALTH;
		
		whitePixel = new Texture2D(1,1);
		whitePixel.SetPixel(0,0, Color.white);
		
		

    }
	
	// Update is called once per frame
	void Update () 
	{
        
		oldMouseDown = currentMouseDown;
		currentMouseDown = Input.GetMouseButtonDown(0);
        //Debug.Log(currentGameState);
		
		if(currentGameState == gameState.GAME_STARTED || currentGameState == gameState.TEAMS)
		{
            
			_currentTimer += Time.deltaTime;
			//cannot shoot, build, or pick up toolboxes until game has started
			if((currentGameState == gameState.TEAMS))//&&%%
			{
	            if (_stats[_me.name].currentStatus == playerState.ALIVE)
	            {
	                if (SingleClick())
	                {
	                    networkView.RPC("shootLazer", RPCMode.All, _me.transform.position, _me.transform.Find("Camera_Anchor").rotation, _me.name,_stats[_me.name].weapon);
                        if(_stats[_me.name].weapon == WRENCH_ID || _stats[_me.name].weapon == CLEAVER_ID)
                        {
                            networkView.RPC("playSound", RPCMode.All, _me.transform.position, 1);
						    _me.GetComponent<FPSPlyerSync>().SwingAttack();
                        }
                        if (_stats[_me.name].weapon == PISTOL_ID)
                        {
                            networkView.RPC("playSound", RPCMode.All, _me.transform.position, 4);
                        }
                        if (_stats[_me.name].weapon == BOLT_GUN_ID)
                        {
                            networkView.RPC("playSound", RPCMode.All, _me.transform.position, 3);
                        }
	                }
		            
					
					if(_inBuildZone && _caryingToolBox && Input.GetKey(KeyCode.E))
					{
						_progressBarValue -= Time.deltaTime;
						if(_progressBarValue < 0)
						{
							// it has been built
							_rocketPlatform.AddStage();
							_caryingToolBox = false;
							networkView.RPC("ChangeToolBox",RPCMode.All, _me.name, 0);
						}
					}
					else if(_amOnToolBox != null && Input.GetKey(KeyCode.E))
					{
						_progressBarValue -= Time.deltaTime;
						if(_progressBarValue < 0)
						{
							_amOnToolBox.PickupBox();
							_amOnToolBox = null;
							_caryingToolBox = true;
							networkView.RPC("ChangeToolBox",RPCMode.All, _me.name, 1);
						}
					}
				}
			}
			
			if(!_inBuildZone && _amOnToolBox == null && _weaponStandingOn != null && Input.GetKeyDown(KeyCode.E)&&_stats[_me.name].currentStatus== playerState.ALIVE)
			{
				int myOldGun = _stats[_me.name].weapon;
				// change my weppon
				ChangeWeapon(_me.name, _weaponStandingOn._weaponType);
				_weaponStandingOn.ChangeWeapon(myOldGun);
			}
			checkTeams();
			// only the server has this going on in it
			if(Network.peerType == NetworkPeerType.Server)
			{
				if(Input.GetMouseButtonDown(2))
				{
					// end the game fast server said so
					_currentTimer = _secondsPerGame;
				}
                checkWinCondition();
            }
			if(_stats[_me.name].currentStatus== playerState.ALIVE)
			{
				//used for emotions
				if(Input.GetKeyDown (KeyCode.Alpha1))
				{
					networkView.RPC("ChangeEmotion", RPCMode.All, _me.name, SMILE_ID);
					//Debug.Log("pressed 1");
				}
				if(Input.GetKeyDown (KeyCode.Alpha2))
				{
					networkView.RPC("ChangeEmotion", RPCMode.All, _me.name, ANGRY_ID);
					//Debug.Log("pressed 2");
				}
				if(Input.GetKeyDown (KeyCode.Alpha3))
				{
					networkView.RPC("ChangeEmotion", RPCMode.All, _me.name, SAD_ID);
					//Debug.Log("pressed 3");
				}
				if(Input.GetKeyDown (KeyCode.Alpha4))
				{
					networkView.RPC("ChangeEmotion", RPCMode.All, _me.name, SCARED_ID);
					//Debug.Log("pressed 4");
				}
				if(Input.GetKeyDown (KeyCode.Alpha5))
				{
					networkView.RPC("ChangeEmotion", RPCMode.All, _me.name, QUESTION_ID);
					//Debug.Log("pressed 5");
				}
				if(Input.GetKeyDown (KeyCode.Alpha6))
				{
					networkView.RPC("ChangeEmotion", RPCMode.All, _me.name, HELP_ID);
					//Debug.Log("pressed 6");
				}
			}
		}
        if (currentGameState == gameState.GAME_OVER)
        {
            _currentTimer += Time.deltaTime;
            if (_currentTimer >= _gameOverTime)
            {
                networkView.RPC("cleanUp", RPCMode.All);
                currentGameState = gameState.LOBBY;
            }
        }
	}
	
	// check to see if the mouse has been clicked once
	private bool SingleClick()
	{
		return !oldMouseDown && currentMouseDown;
	}
	// respawn the player to a new spawn point
	public void Respawn()
	{
		int pt = Random.Range(0,spawnPoints.Length);
		GameObject sPoint = spawnPoints[pt];
		_me.transform.position = sPoint.transform.position;
		_me.transform.rotation = sPoint.transform.rotation;
	}
	// make the scores for the players
	public void makeScoreDictionary(List<PlayerDetails> dets)
	{
		_stats.Clear();
		foreach(PlayerDetails pd in dets)
		{
			PlayerStats ps = new PlayerStats();
			ps.name = pd.Name;
			ps.life = MAX_HEALTH;
            ps.allegiance = BUILDER_ID;
            ps.weapon = WRENCH_ID;
            ps.currentStatus = playerState.ALIVE;
			_stats.Add(pd.Name, ps);
		}
	}

#region Teams

    public void allocateTeams()
    {
		
		GO_Players = GameObject.FindGameObjectsWithTag("Player");
        //Server allocates the teams
        //1 in 5 players is a saboteur
        //the rest are builders
        if (Network.peerType == NetworkPeerType.Server)
        {
            Debug.Log("Assigning Team..");
            //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            float numPlayers = _stats.Count;
            int numSaboteurs = Mathf.CeilToInt(numPlayers / SAB_DENOMINATOR);
			int[] randInt = new int[numSaboteurs];
            //Now that we know the number of saboteurs in the game,
            //for as many saboteurs we should have, pick a player to 
            //be a saboter. If they are already a saboteur, try again
            for (int i = 0; i < numSaboteurs; i++)
            {
				randInt[i] = Mathf.FloorToInt(Random.Range(0, numPlayers));
				Debug.Log(randInt[i]);
				if(i > 0)
				{
					if(randInt[i] == randInt[i - 1])
						i--;
					else
						Debug.Log("Sabo at " + randInt[i]);
				}
				else
						Debug.Log("Sabo at " + randInt[i]);
				Debug.Log("saboteur chosen");
            }
			int count = 0;
			string[] flags = new string[numSaboteurs];
			foreach(KeyValuePair<string, PlayerStats> entry in _stats)
			{
				for(int i = 0; i < numSaboteurs; i++)
				{
					if(count == randInt[i])
					{
						flags[i] = entry.Value.name;
					}
				}
				count++;
			}
			for(int i = 0; i < numSaboteurs; i++)
			{
				setStats(flags[i], MAX_HEALTH, SABOTEUR_ID, _stats[flags[i]].weapon, playerState.ALIVE);
			}
			
            Debug.Log(numSaboteurs + " Saboteurs assigned");
        }
    }
    //When 30 seconds have passed, teams will be revealed
    private void checkTeams()
    {
        if (_currentTimer >= _introTime)
        {
            if (currentGameState == gameState.GAME_STARTED)
            {
                currentGameState = gameState.TEAMS;
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < players.Length; i++)
                {
                    adjustPlayer(players[i].name, players[i].networkView.viewID, _stats[players[i].name].allegiance - 200);
                }
            }
        }
    }

    private void checkWinCondition()
    {
        if (currentGameState == gameState.TEAMS)
        {
			if(_rocketPlatform.completed())
			{
				networkView.RPC("endGame", RPCMode.All, 1);
			}
			if(_currentTimer >= _secondsPerGame)
			{
				networkView.RPC("endGame", RPCMode.All, 0);
			}
			//Debug.Log(_rocketPlatform.completed());
        }
    }

#endregion
    [RPC]
    void die(string name, NetworkViewID viewID, int weapon)
    {
        GameObject deadPlayer = NetworkView.Find(viewID).gameObject;
        Vector3 pos = deadPlayer.transform.position;
        Quaternion rot = deadPlayer.transform.rotation;
        if (_me.name == name)
        {
            //Disable everyone else's colliders
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].networkView.viewID.isMine)
                {
                    players[i].GetComponent<CharacterController>().enabled = false;
                }
            }
			 //Create a corpse and a weapon drop
	        Network.Instantiate(CorpsePrefab, pos, rot, 0);
	        GameObject wDP = (GameObject)Network.Instantiate(WeaponDropPrefab, pos, rot, 0);
			Network.Instantiate(ToolBoxPrefab, pos, rot, 0);
			
	        wDP.GetComponent<WeaponDrop>().ChangeWeapon(weapon);
        }
        //disable the renderer and collider of the dead player
        else
        {
            GameObject.Find(name + "/PlayerBody/Bip001/Bip001 Pelvis").GetComponent<SkinnedMeshRenderer>().enabled = false;
            deadPlayer.GetComponent<CharacterController>().enabled = false;
        }
		if(_stats.ContainsKey(name))
		{
			FPSPlyerSync fps = GameObject.Find (name).GetComponent<FPSPlyerSync>();
			fps.Die();
		}
       
    }


    // shoot the lazer on the server and check colisions 
    [RPC]
	void shootLazer(Vector3 pos, Quaternion roto, string name, int weapon)
	{
		// this will only work on the server
		if(Network.peerType == NetworkPeerType.Server)
		{
			if(weapon == WRENCH_ID || weapon == CLEAVER_ID)
			{
				/// do a meele attack
				Vector3 boxPos = pos + (roto * new Vector3(0,0,1));
				GameObject hit = GameObject.Instantiate(MeleeBoxPrefab, boxPos, roto) as GameObject;
				foreach(GameObject go in GO_Players)
				{
					if(hit.collider.bounds.Contains(go.transform.position) && go.name != name)
					{
						// object hit by colider
						 switch (weapon)
	                    {
	                        case WRENCH_ID:
	                            setHealth(go.name, _stats[go.name].life - WRENCH_DAMAGE);
                                networkView.RPC("playSound", RPCMode.All, _me.transform.position, 2);
	                            break;
	                        case CLEAVER_ID:
	                            setHealth(go.name, _stats[go.name].life - CLEAVER_DAMAGE);
                                networkView.RPC("playSound", RPCMode.All, _me.transform.position, 2);
	                            break;
	                    }
						Debug.Log(go.name);
					}
				}
				
				GameObject.Destroy(hit);
				
			}
			else
			{
				RaycastHit hit = new RaycastHit();
				Ray lazer = new Ray(pos, roto * new Vector3(0,0,1));
				
				GameObject laz = (GameObject)Network.Instantiate(LazerPrefab, pos + new Vector3(0,0.3f,0), roto, 1);
				//LayerMask notGuns =  1 << 8;// << 8;
				if(Physics.Raycast(lazer,out hit))//,notGuns)) // if something was hit
				{
					laz.transform.localScale = new Vector3(1,1,(float)hit.distance/2);
					if(_stats.ContainsKey(hit.collider.name)) // we hit someone valid send a signal
					{
						//networkView.RPC("playerHit",RPCMode.All, hit.collider.name, PISTOL_ID);
	                    //update the player in the dictionary
	                    //_stats[hit.collider.name] = new PlayerStats(hit.collider.name, _stats[hit.collider.name].life - PISTOL_DAMAGE, _stats[hit.collider.name].allegiance, _stats[hit.collider.name].weapon);
	                    switch (weapon)
	                    {
	                        case PISTOL_ID:
	                            setHealth(hit.collider.name, _stats[hit.collider.name].life - PISTOL_DAMAGE);
	                            break;
	                        case BOLT_GUN_ID:
	                            setHealth(hit.collider.name, _stats[hit.collider.name].life - BOLT_GUN_DAMAGE);
	                            break;
	                    }
					}
				}
				else // no hit but still make a lazer
				{
					laz.transform.localScale = new Vector3(1,1,(float)20);
				}
			}
			
			
		}
	}
	
	// tell everyone someone has been hit if they are respawn
	// give the person who hit the others give them poitns
	[RPC]
	void playerHit(string hitPlayer, int type)
	{
		if(_me.name == hitPlayer)
		{
			switch(type)
			{
				case WRENCH_ID:
					Health -= WRENCH_DAMAGE;
					break;
				case CLEAVER_ID:
					Health -= CLEAVER_DAMAGE;
					break;
				case PISTOL_ID:
					Health -= PISTOL_DAMAGE;
					break;
				case BOLT_GUN_ID:
					Health -= BOLT_GUN_DAMAGE;
					break;
			}
			if(Health <= 0)
			{
				Health = MAX_HEALTH;
				Respawn();
			}
			
			return;
		}
	
	}
	// spawn in yourelf into the game
	[RPC]
	void networkSpawn()
	{
		int pt = Random.Range(0,spawnPoints.Length);
		GameObject sPoint = spawnPoints[pt];
		_me = (GameObject)Network.Instantiate(PlayerPrefab, sPoint.transform.position, sPoint.transform.rotation,0);
		string nm =  GameObject.Find("GameManager").GetComponent<NetworkManager>().PlayerName;
		_me.name = nm;
		//int matNumHead = Random.Range(0,_matts.Length);
		//int matNumBody = Random.Range(0,_matts.Length);
		//int matNumArms = Random.Range(0,_matts.Length);
		//int matNumLegs = Random.Range(0,_matts.Length);
		
		networkView.RPC("adjustPlayer", RPCMode.All, nm, _me.networkView.viewID, 1);

	}
	// set the person colors corectly 
	[RPC]
	void adjustPlayer(string name, NetworkViewID nid, int material)
	{
		NetworkView nv = NetworkView.Find(nid);
		nv.name = name;
		if(!_stats.ContainsKey(name))
		{
			PlayerStats ps;
			ps.allegiance = BUILDER_ID;
			ps.life = MAX_HEALTH;
			ps.name = name;
			ps.weapon = WRENCH_ID;
            ps.currentStatus = playerState.ALIVE;
			_stats.Add(name, ps);
		}
         if (_stats[_me.name].allegiance == SABOTEUR_ID) 
            GameObject.Find(name+"/PlayerBody/Bip001/Bip001 Pelvis").GetComponent<SkinnedMeshRenderer>().material = _matts[material];
		//GameObject.Find(name+"/Head").renderer.material = _matts[matNumHead];
		//GameObject.Find(name+"/Body").renderer.material = _matts[matNumBody];
		//GameObject.Find(name+"/Arm1").renderer.material = _matts[matNumArms];
		//GameObject.Find(name+"/Arm2").renderer.material = _matts[matNumArms];
		//GameObject.Find(name+"/Leg1").renderer.material = _matts[matNumLegs];
		//GameObject.Find(name+"/Leg2").renderer.material = _matts[matNumLegs];
		GO_Players = GameObject.FindGameObjectsWithTag("Player");
	}

    [RPC]
    void playSound(Vector3 location, int type)
    {
        switch (type)
        {
            case 0:
                gameObject.GetComponent<SoundfxScript>().instantiateSound(location, SoundfxScript.soundType.DEATH);
                break;
            case 1:
                gameObject.GetComponent<SoundfxScript>().instantiateSound(location, SoundfxScript.soundType.SWOOSH);
                break;
            case 2:
                gameObject.GetComponent<SoundfxScript>().instantiateSound(location, SoundfxScript.soundType.MELEE_HIT);
                break;
            case 3:
                gameObject.GetComponent<SoundfxScript>().instantiateSound(location, SoundfxScript.soundType.RIVET_GUN);
                break;
            case 4:
                gameObject.GetComponent<SoundfxScript>().instantiateSound(location, SoundfxScript.soundType.PISTOL);
                break;
        }
       
    }

	// start the game
	[RPC]
	void beginGame()
	{
		Screen.lockCursor = true;
		currentGameState = gameState.GAME_STARTED;
		_currentTimer = 0;
		Health = 100;
        allocateTeams();
	}

    [RPC]
    void endGame(int vic)
    {
        if (vic == 0)
            endGame(false);
        else
            endGame(true);
    }
	// end the game and remove yourself
	public void endGame(bool vic)
	{
        victor = vic;
		Screen.lockCursor = false;
		currentGameState = gameState.GAME_OVER;
		_currentTimer = 0;
		
	}

    public void destroy()
    {
        Destroy(GameObject.Find(_me.name));
    }
	
		// Determine if we should draw another players health
    void otherHealth()
    {
        RaycastHit hit;
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        
		if(Physics.Raycast(ray, out hit, MAX_HEALTH_VISION) && _stats.ContainsKey(hit.collider.name))
		{
			drawOtherHealthNow = hit.collider.name;
            //networkView.RPC("requestStats", RPCMode.All, name, networkView.viewID);
		}
		else
		{
			drawOtherHealthNow = "";
		}
    }
	
	#region UI STUFF
	// ui stuff

    void drawProgressBar(float progress, Rect bar, Color color)
    {
        GUI.BeginGroup(bar);
        //Draw background
		bar.x = 0;
		bar.y = 0;
        whitePixel.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        whitePixel.Apply();
        GUI.DrawTexture(bar, whitePixel, ScaleMode.StretchToFill);
        //Draw progress
        whitePixel.SetPixel(0, 0, color);
        whitePixel.Apply();
        GUI.DrawTexture(new Rect(0, 0, bar.width * progress, bar.height), whitePixel, ScaleMode.StretchToFill);
        GUI.EndGroup();
    }

	void drawHealth()
	{
        drawProgressBar(barDisplay, healthBar, new Color(0.0f, 1.0f, 0.0f, 1.0f));
	}
	
	// 
    void drawOtherHealth()
    {
		barOtherDisplay = _stats[drawOtherHealthNow].life / MAX_HEALTH;
        drawProgressBar(barOtherDisplay, otherHealthBar, new Color(0.0f, 1.0f, 0.0f, 0.3f));
       
    }
	
	
	// ui time
	void drawTime(int windowID)
	{
		GUILayout.TextArea("" + ((int)(_secondsPerGame - _currentTimer)));
	}

    void drawTeam()
    {
        if(currentGameState == gameState.TEAMS)
        {
            if(_stats[_me.name].allegiance == BUILDER_ID)
                GUI.Box(new Rect(teamPos.x, teamPos.y, teamSize.x, teamSize.y), buildTex);
            else if(_stats[_me.name].allegiance == SABOTEUR_ID)
                GUI.Box(new Rect(teamPos.x, teamPos.y, teamSize.x, teamSize.y), saboTex);
        }
    }

    void endGameUI()
    {
        if (currentGameState == gameState.GAME_OVER)
        {
            if((victor && _stats[_me.name].allegiance == BUILDER_ID) || (!victor && _stats[_me.name].allegiance == SABOTEUR_ID))
                GUI.Box(new Rect(teamPos.x, teamPos.y, teamSize.x, teamSize.y), vicTex);
            else
                GUI.Box(new Rect(teamPos.x, teamPos.y, teamSize.x, teamSize.y), defTex);
        }
    }
	
	void drawPickupWeppon()
	{
		
		GUI.BeginGroup(new Rect(otherHealthBar.x - 90, otherHealthBar.y - 90, 300, 50));
		switch(_weaponStandingOn._weaponType)
		{
			case WRENCH_ID:
				GUI.Label(new Rect(0, 0, 200, 100), "Press \"E\" to pickup Wrench", _textStyle);
				break;
			case PISTOL_ID:
				GUI.Label(new Rect(0, 0, 200, 100), "Press \"E\" to pickup Pistol", _textStyle);
				break;
			case CLEAVER_ID:
				GUI.Label(new Rect(0, 0, 200, 100), "Press \"E\" to pickup Cleaver",  _textStyle);
				break;
			case BOLT_GUN_ID:
				GUI.Label(new Rect(0, 0, 200, 100), "Press \"E\" to pickup Bolt Gun", _textStyle);
				break;
		}
		
		GUI.EndGroup();
	}
	
	void drawPickupToolBox()
	{
		GUI.BeginGroup(new Rect(buildBar.x+ 60, buildBar.y - 25, buildBar.width, buildBar.height));
		GUI.Label(new Rect(0, 0, 200, 100), "Hold \"E\" to PickupToolBox", _textStyle);
		GUI.EndGroup();
		
		drawProgressBar((PICKUP_TIMER - _progressBarValue)/PICKUP_TIMER, buildBar, new Color(0.0f, 0.0f, 1.0f, 1.0f));
	}
	
	void drawBuildRocket()
	{
		GUI.BeginGroup(new Rect(buildBar.x+ 60, buildBar.y - 25, buildBar.width, buildBar.height));
		GUI.Label(new Rect(0, 0, 200, 100), "Hold \"E\" to build Rocket", _textStyle);
		GUI.EndGroup();
		
		drawProgressBar((BUILD_TIMER - _progressBarValue)/BUILD_TIMER, buildBar, new Color(0.0f, 0.0f, 1.0f, 1.0f));
		
	}
	
	// final ui
	public void drawGUI()
	{
		int sc25 = (int)(Screen.width * 0.25f);
        //the player's health
        barDisplay = Health / MAX_HEALTH;
		//GUILayout.Window(200,new Rect(0,0, sc25, 40), drawHealth, "Health");
        drawHealth();
		
		
		GUILayout.Window(201,new Rect(Screen.width - sc25,0, sc25, 40), drawTime, "Time Left");
		GUI.DrawTexture(new Rect((Screen.width/2) - (Reticle.width/2),(Screen.height/2) - (Reticle.height/2),
								Reticle.width, Reticle.height), Reticle); // draw the reticle
		
        otherHealth();
        if (drawOtherHealthNow != "")
		{
            drawOtherHealth();
		}
		
		if(_inBuildZone && _caryingToolBox)
		{
			drawBuildRocket();
		}
		else if(_amOnToolBox != null)
		{
			drawPickupToolBox();
		}
		else if(_weaponStandingOn != null)
		{
			drawPickupWeppon();
		}
		
        drawTeam();
        endGameUI();			
	}
	
	#endregion

    #region playerStats Stuff
    public float setHealth(string name, float newHealth)
    {
        if (newHealth >= 0)
        {
            setStats(name, newHealth, _stats[name].allegiance, _stats[name].weapon, _stats[name].currentStatus);
        }
        else
        {
            setStats(name, newHealth, _stats[name].allegiance, _stats[name].weapon, playerState.DEAD);
            networkView.RPC("playSound", RPCMode.All, _me.transform.position, 0);
            networkView.RPC("die", RPCMode.All, name, GameObject.Find(name).networkView.viewID, _stats[name].weapon);
        }
        return newHealth;
    }
	

    public void setStats(string name, float newHealth, int newAllegiance, int newWeapon, playerState newState)
    {
        if (newState == playerState.UNSPAWNED)
        {
            networkView.RPC("remoteSetStats", RPCMode.All, name, newHealth, newAllegiance, newWeapon, 0);
        }
        else if (newState == playerState.ALIVE)
        {
            networkView.RPC("remoteSetStats", RPCMode.All, name, newHealth, newAllegiance, newWeapon, 1);
        }
        else
        {
            networkView.RPC("remoteSetStats", RPCMode.All, name, newHealth, newAllegiance, newWeapon, 2);
        }
    }

    [RPC]
    public void remoteSetStats(string name, float newHealth, int newAllegiance, int newWeapon, int newState)
    {
        if (newState == 0)
        {
            _stats[name] =  new PlayerStats(name, newHealth, newAllegiance, newWeapon, playerState.UNSPAWNED);
        }
        else if (newState == 1)
        {
            _stats[name] = new PlayerStats(name, newHealth, newAllegiance, newWeapon, playerState.ALIVE);
        }
        else 
        {
            _stats[name] = new PlayerStats(name, newHealth, newAllegiance, newWeapon, playerState.DEAD);
        }
        if (name == _me.name)
        {
            Health = newHealth;
        }
        //if (Health <= 0)
        //{
        //    setHealth(name, MAX_HEALTH);
        //    Respawn();
        //}
    }
	
	[RPC]
	public void ChangeWeapon(string name, int weaponID)
	{
		if(_stats.ContainsKey(name))
		{
			PlayerStats psn = _stats[name];
			psn.weapon = weaponID;
			_stats[name] = psn;
			FPSPlyerSync fps =  GameObject.Find(name).GetComponent<FPSPlyerSync>();
			if(fps != null)
			{
				fps.ChangeWeapon(weaponID);
			}
		}
	}
	
	[RPC]
	public void ChangeToolBox(string name, int caryingBox)
	{
		if(_stats.ContainsKey(name))
		{
			FPSPlyerSync fps =  GameObject.Find(name).GetComponent<FPSPlyerSync>();
			if(fps != null)
			{
				fps.ChangeToolBox(caryingBox == 1);
			}
		}
	}
	
	[RPC]
	public void ChangeEmotion(string name, int emotionID)
	{
		if(_stats.ContainsKey(name))
		{
			FPSPlyerSync fps = GameObject.Find (name).GetComponent<FPSPlyerSync>();
			if(fps != null)
			{
				fps.ChangeEmotion(emotionID);	
			}
		}
	}

	
	public void StandingOnWeapon(WeaponDrop nGun)
	{
		_weaponStandingOn = nGun;
	}
	
	public void StandingOnToolBox(ToolBoxDrop on)
	{
		if(!_caryingToolBox)
		{
	 		_amOnToolBox = on; 
			_progressBarValue = PICKUP_TIMER;
		}
	}
	
	public void InBuildZone(bool buildZone)
	{
		_inBuildZone = buildZone;
		_progressBarValue = BUILD_TIMER;
	}
    #endregion
}
