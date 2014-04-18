using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// used to store player details between rounds
public struct PlayerDetails
{
	public string Name;
	public NetworkViewID ViewID;
	public int GameScore;
	public int TotalScore;
	public bool Connected;
}

public class LobbyManager : MonoBehaviour
{
	private bool _isHost = false;
	
	NetworkView _networkView;
	
	private Dictionary<string,PlayerDetails> _players;
	
	// current lobby details
	public List<PlayerDetails> Lobby;
	private string _lobbyText = "";
	
	// chat info
	private string _chatText = "";
	private string _messageText = "";
	
	// ui details
	private Rect _mainRect;
	private Vector2 _chatScroll = new Vector2(0,0);
	private GUIStyle _chatStyle = new GUIStyle();
	
	private PlayerDetails _me;
	
	public string ChatText
	{
		get {return _chatText;}
	}
	public string LobbyText
	{
		get {return _lobbyText;}
	}
	
	// constructor
	void start()
	{
		Lobby = new List<PlayerDetails>();
		_players = new Dictionary<string, PlayerDetails>();
		_chatStyle.normal.textColor = Color.white;
	}
	
	#region CHAT STUFF
	[RPC]
	void SendChatMessage(string message, string pName)
	{
		_chatText = _chatText + "\n" + name + ": " + message;
		_chatScroll.y = float.MaxValue;	
	}
	
	[RPC]
	void SendLobbyInfo(string lobby, string chat)
	{
		_lobbyText = lobby;
		_chatText = chat;
	}
	
	// sets the scores for the people in the chat
	public void SetChatScores(Dictionary<string,int> scores)
	{
		foreach(KeyValuePair<string, int> item in scores)
		{
			PlayerDetails pd = _players[item.Key];
			pd.TotalScore = item.Value;
			Debug.Log(item.Value);
			_players[item.Key] = pd;
		}
		
		buildLobby();
	}
	
	#endregion
	
	// sets up the lobby
	public void init(NetworkView nv, PlayerDetails me)
	{
		// clear the game
		Lobby = new List<PlayerDetails>();
		_players = new Dictionary<string, PlayerDetails>();
	
		_networkView = nv;
		_me = me;
		addPlayer(_me);
		
		reset();
	}
	
	// resets the lobby to start state
	public void reset()
	{
		_chatText = "";
		_messageText = "";
		_players.Clear();
		_me.TotalScore = 0;
		_me.GameScore = 0;
		addPlayer(_me);
		buildLobby();
	}
	
	// adds a player to the loby returns if succesful
	// will return false on duplicates
	public bool addPlayer(PlayerDetails pd)
	{
		if(_players.ContainsKey(pd.Name))
		{
			if(_players[pd.Name].Connected)
			{
				return false;
			}
			else
			{
				pd.Connected = true;
				pd.GameScore = _players[pd.Name].GameScore;
				pd.TotalScore = _players[pd.Name].TotalScore;
				_players[pd.Name] = pd;
				buildLobby();
				return true;
			}
		}
		
		pd.Connected = true;
		_players.Add(pd.Name, pd);
		buildLobby();
		return true;
	}
	
	// attemps to remove a player from the lobby
	public bool RemovePlayer(string name)
	{
		if(_players.ContainsKey(name))
		{
			PlayerDetails logout = _players[name];
			logout.Connected = false;
			_players[name] = logout;
			buildLobby();
			return true;
		}
		return false;
	}
	
	// rebuils the lobby if it has been edited and sends a singal to all 
	// clients about the new lobby
	private void buildLobby()
	{
		Lobby.Clear();
		_lobbyText = "";
		foreach(PlayerDetails pd in _players.Values)
		{
			if(pd.Connected)
			{
				Lobby.Add(pd);
				_lobbyText += pd.Name + ": " + pd.TotalScore + "\n";
			}
		}
		if(Network.peerType == NetworkPeerType.Server)
		{
			_networkView.RPC("SendLobbyInfo",RPCMode.All,_lobbyText, _chatText);
		}
	}
	
	#region GUI functions
	
	// draws ui
	private void drawLobbyWindow(int windowID)
	{
		GUILayout.Space(10);
		GUILayout.TextArea(_lobbyText);
		
	}
	
	// draws the chat ui
	private void drawChatWindow(int windowID)
	{
		_chatScroll = GUILayout.BeginScrollView(_chatScroll,
								GUILayout.Width(_mainRect.width));
		GUILayout.Label(_chatText + "\n", _chatStyle);
		GUILayout.EndScrollView();
		
		//draw the text entry box
		GUILayout.BeginHorizontal();
		
		_messageText = GUILayout.TextField(_messageText, GUILayout.Width(_mainRect.width * 0.9f)); // GUILayout.Width(windowWidth-sendBtnWidth-padding)
		if(GUILayout.Button("Send", GUILayout.Width(_mainRect.width * 0.1f))|| Event.current.keyCode == KeyCode.Return)
		{
			// send button pressed
			if(_messageText != "")
			{
				_networkView.RPC("SendChatMessage", RPCMode.All, _messageText, _me.Name); 
				_messageText = "";
			}
		}
		
		GUILayout.EndHorizontal();
	}
	
	private void drawConsole(int WindowID)
	{
		if(Network.peerType == NetworkPeerType.Server)
		{
			if(GUILayout.Button("Start Game")) // start the games
			{
				GameObject.Find("GameManager").GetComponent<TrustManager>().makeScoreDictionary(Lobby);// set up the game players
				_networkView.RPC("networkSpawn", RPCMode.All); // tell everyone to spawn
				_networkView.RPC("beginGame", RPCMode.All);   // begin the game
				_networkView.RPC("setToGame", RPCMode.All);   // set the game state(might be able to be removed)
			}
			if(GUILayout.Button("Disconnect"))
			{
				_networkView.RPC("serverDisconnect", RPCMode.All); 
			}
		}
		else
		{
			if(GUILayout.Button("Disconnect"))
			{
				_networkView.RPC("playerDisconnect", RPCMode.All, _me.Name); 
				Network.Disconnect();
			}
		}
		
	}
	
	
	// used to draw the gui for the lobby
	public void drawGUI(Rect mainRect)
	{
		_mainRect = mainRect;
		GUILayout.Window(100,new Rect(0,_mainRect.y, _mainRect.x, _mainRect.height), drawLobbyWindow, "Lobby");
		GUILayout.Window(101,_mainRect, drawChatWindow, "Chat");
		GUILayout.Window(102,new Rect(0,_mainRect.y + _mainRect.height, _mainRect.x, _mainRect.y), drawConsole, "Actions");
	}
	
	#endregion
	
	
}
