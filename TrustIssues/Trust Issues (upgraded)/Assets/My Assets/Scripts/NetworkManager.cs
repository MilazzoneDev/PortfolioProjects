using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// states for the games and menues
public enum SetupState
{
	MainMenu,
	Lobby,
	Tag
}

public enum MenuState
{
	Name,
	Main,
	Connect
}


public class NetworkManager : MonoBehaviour {

	//network info
	private string connectionIP = "127.0.0.1";
	private int connectionPort = 25005;
	
	// name details
	private string _name  = "";
	private string tempName = "";
	private bool _isServer = false;
	// winndow details
	private int _mainWindowLeft;
	private int _mainWindowTop;
	private int _mainWindowHeight;
	private int _mainWindowWidth;
	private Rect _mainWindow;
	// refrence details to the tag mannager
	
	
	public string PlayerName
	{
		get{ return _name;}
	}
	
	
	private SetupState _gameState = SetupState.MainMenu;
	private MenuState _menueState = MenuState.Main;
	
	// Other Components
	private LobbyManager _lobby;
	private TrustManager _tagManager;
	
	// Use this for initialization
	void Start () 
	{
		// get previous player prefs
		_name = PlayerPrefs.GetString("playerName");
		if(_name == "" || _name == null)
		{
			_menueState = MenuState.Name;	
		}
		
		_tagManager = GameObject.Find("GameManager").GetComponent<TrustManager>();
		_lobby = GameObject.Find("GameManager").GetComponent<LobbyManager>();
		
		Application.runInBackground = true;
	}
	

	
	#region SERVER NETWORK
	
	void StartServer()
	{
		// open the server for eople to connect
		Network.InitializeServer(32, connectionPort, Network.HavePublicAddress());
		Debug.Log("Starting the server");
		
		_gameState = SetupState.Lobby;
		
		PlayerDetails me = new PlayerDetails();
		me.Name = _name;
		me.GameScore = 0;
		me.TotalScore = 0;
		me.ViewID = NetworkViewID.unassigned;
		me.Connected = true;
		
		_lobby.init(networkView, me);
		
	}
	
	void OnPlayerConnected (NetworkPlayer player)
	{
		Debug.Log ("Player " + player + " connected from " + player.ipAddress + ":" + player.port);
	}
	
	// validates and adds a player to the lobby
	[RPC]
	void validatePlayer(string name, NetworkMessageInfo info)
	{
		PlayerDetails nPlayer = new PlayerDetails();
		nPlayer.Name = name;
		nPlayer.GameScore = 0;
		nPlayer.TotalScore = 0;
		nPlayer.ViewID = NetworkViewID.unassigned;
		nPlayer.Connected = true;
		// if the player is a duplcate kick them
		if(!_lobby.addPlayer(nPlayer))
		{
			networkView.RPC("DuplicateLogin", info.sender, 0);
		}
	}
	
	// if a player is dysconected remove it
	[RPC]
	void playerDisconnect(string name)
	{
		if(name == _name)
		{
			_gameState = SetupState.MainMenu;
		}
		if(Network.peerType == NetworkPeerType.Server)
		{
			_lobby.RemovePlayer(name);
		}
	}
	
	
	#endregion
	
	
	#region CLIENT NETWORK
	// if the server gets disconected restart
	void OnDisconnectedFromServer()
	{
		Application.LoadLevel(Application.loadedLevel);
		_gameState = SetupState.MainMenu;
	}
	
	// the server disconcets leave
	[RPC]
	void serverDisconnect()
	{
		_gameState = SetupState.MainMenu;
		Network.Disconnect();
	}
	
	// when conected make details and 
	void OnConnectedToServer()
	{
		PlayerDetails me = new PlayerDetails();
		me.Name = _name;
		me.GameScore = 0;
		me.TotalScore = 0;
		me.ViewID = NetworkViewID.unassigned;
		me.Connected = true;
		
		_lobby.init(networkView, me);
		
		// connect
		_gameState = SetupState.Lobby;
		//NetworkMessageInfo nmi = new NetworkMessageInfo();
		networkView.RPC("validatePlayer", RPCMode.Server, _name);
	}
	
	[RPC]
	void setToGame()
	{
		_gameState = SetupState.Tag;
	}
	
	[RPC]
	void cleanUp()
	{
		_gameState = SetupState.Lobby;
		_tagManager.destroy();
	}
	
	[RPC]
	void DuplicateLogin(int num)
	{
		_menueState = MenuState.Name;
		_gameState = SetupState.MainMenu;
		Network.Disconnect();
	}
	
	#endregion
	
	
	#region UI STUFF
	
	void OnGUI()
	{
		// get main window location and size
		_mainWindowWidth = (int)(Screen.width * 0.66);
		_mainWindowHeight = (int)(Screen.height * 0.66);
		
		_mainWindowLeft = (int)(Screen.width * 0.17);
		_mainWindowTop = (int)(Screen.height * 0.17);
		
		_mainWindow = new Rect(_mainWindowLeft, _mainWindowTop, _mainWindowWidth, _mainWindowHeight);
		
		switch(_gameState)
		{
			case SetupState.MainMenu:
				switch(_menueState)
				{
					case MenuState.Name:
						_mainWindow = GUILayout.Window (003, _mainWindow, SetNameWindow, "Select Network Name");
						break;
					case MenuState.Main:
						_mainWindow = GUILayout.Window (001, _mainWindow, MainMenuWindow, "Trust Issues");
						break;
					case MenuState.Connect:
						_mainWindow = GUILayout.Window (002, _mainWindow, ConectToServerWindow, "Connect to Sever");
						break;
				}
				break;
				
			case SetupState.Lobby:
				_lobby.drawGUI(_mainWindow);
				break;
				
			case SetupState.Tag:
				_tagManager.drawGUI();
				break;
		}
		
		
		//_lobby.OnGUI();
	}
	
	void SetNameWindow(int windowID)
	{
		GUILayout.Label("Enter your player name: ");
		
		tempName = GUILayout.TextField(tempName, 25);
		
		if(GUILayout.Button("Set Name", GUILayout.Height(20)) || Event.current.keyCode == KeyCode.Return)
		{
			if(tempName != "")
			{
				_menueState = MenuState.Main;
				_name = tempName;
				PlayerPrefs.SetString("playerName", _name); 
				Debug.Log("My Name now is: " + _name);
			}
		}
		if(GUILayout.Button("Cancel", GUILayout.Height(20)))
		{
			if(_name != "")
			{
				_menueState = MenuState.Main;
			}
		}
	}
	
	void MainMenuWindow(int windowID)
	{
		GUILayout.Space(15);
		if (GUILayout.Button("Start Server", GUILayout.Height(_mainWindowHeight/3)))
		{
			StartServer();
		}
		if (GUILayout.Button("Join the Game", GUILayout.Height(_mainWindowHeight/3)))
		{
			Debug.Log("I wanna join as client");
			_menueState = MenuState.Connect;
		}
		if (GUILayout.Button("Change Name", GUILayout.Height(_mainWindowHeight/3)))
		{
			tempName = _name;
			_menueState = MenuState.Name;
		}
	}
	
	void ConectToServerWindow(int windowID)
	{
		GUILayout.Label("Enter Server IP");
		connectionIP = GUILayout.TextField(connectionIP);
		
		GUILayout.Label("Enter Server Port Number");
		connectionPort = int.Parse(GUILayout.TextField(connectionPort.ToString()));
	
		
		if (GUILayout.Button("Login", GUILayout.Height(_mainWindow.height * 0.25f)))
		{
			Debug.Log("LOGIN CLICK");
			Network.Connect(connectionIP, connectionPort);
		}
		
		if (GUILayout.Button("Go Back", GUILayout.Height(_mainWindow.height * 0.25f)))
		{
			_menueState = MenuState.Main;
		}
	}
	
	
	#endregion
}
