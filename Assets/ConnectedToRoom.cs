using System.Collections;
using System.Linq;
using Mosframe;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Timers;

class StartGameClickableButtonHandler:IClickableButtonHandler
{
    public void OnClick()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "start", true } });
        PhotonNetwork.LoadLevel("MonopolyMainScene");
    }
}
public class ConnectedToRoom : MonoBehaviourPunCallbacks
{
    
    private Timer _debounceTimer;
    private bool _isDebouncing = false;
    public BaseTextHandler playerName;
    public BaseTextHandler roomName;
    public DynamicVScrollView connectedToRoomScrollView;
    public ButtonHandler startGameButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startGameButton.Init();
        UpdatePlayerList();
        startGameButton.Handler = new StartGameClickableButtonHandler();
    }

    // Optionally, you can override events when master client changes
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        Debug.Log($"The new master client is: {newMasterClient.NickName}");
    }
    // Called when a player joins the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} has joined the room.");
        UpdatePlayerList();
    }

    // Called when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} has left the room.");
        UpdatePlayerList();
    }
    public void SetCurrentImageIndex(int index)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "image", index } });
    }
    // Listen for room custom property changes
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        foreach (DictionaryEntry entry in propertiesThatChanged)
        {
            Debug.Log($"Room property updated: {entry.Key} = {entry.Value}");
        }
        PhotonNetwork.LoadLevel("MonopolyMainScene");
    }
    // Listen for player custom property changes
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        foreach (DictionaryEntry entry in changedProps)
        {
            Debug.Log($"Player {targetPlayer.NickName} property updated: {entry.Key} = {entry.Value}");
        }

        UpdatePlayerList();
    }

    public ConnectedToRoom()
    {
        _debounceTimer = new Timer(2); // 1 second debounce delay
        _debounceTimer.Elapsed += OnDebounceElapsed;
        _debounceTimer.AutoReset = false; // Make sure it doesn't repeat
    }


    private void OnDebounceElapsed(object sender, ElapsedEventArgs e)
    {
        _isDebouncing = false;
        _debounceTimer.Stop();
    }
    // Update and log the player list
    private void UpdatePlayerList()
    {
        if (!_isDebouncing)
        {
            _isDebouncing = true;
            _debounceTimer.Start();
            
            roomName.SetText($"{PhotonNetwork.CurrentRoom.Name} - {PhotonNetwork.CurrentRoom.PlayerCount}");
            connectedToRoomScrollView.ClearAll(); //.Where(player=>Equals(player, PhotonNetwork.LocalPlayer))
            Debug.Log("Current Players in Room:");
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                RemotePlayerInSessionList remotePlayerInSessionList =
                    connectedToRoomScrollView.CreateNewChildAtEnd<RemotePlayerInSessionList>();
                // remotePlayerInSessionList.SetPlayerInfo(player.NickName, (int)player.CustomProperties["image"]);
                remotePlayerInSessionList.SetPlayerInfo(player.NickName, (int)player.CustomProperties["image"]);
                Debug.Log($"- {player.NickName} (ActorNumber: {player.ActorNumber}) {player.CustomProperties}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
