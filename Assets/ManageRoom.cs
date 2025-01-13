using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ManageRoom : MonoBehaviourPunCallbacks
{
    public ConnectedToRoom ConnectedToRoom;
    public TMP_InputField roomToCreateName;
    public TMP_InputField roomToJoinName;
    public BaseTextHandler roomName;
    public BaseTextHandler playerName;
    public BaseTextHandler playerInRoomName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void CreateRoom()
    {
        
        // Set the player's name
        PhotonNetwork.LocalPlayer.NickName = PlayerPieceEnum.TopHat.ToString();
        PhotonNetwork.JoinOrCreateRoom(roomToCreateName.text, new RoomOptions(){MaxPlayers = 8}, TypedLobby.Default, null);
    }

    public void JoinRoom()
    {
        
        // Set the player's name
        PhotonNetwork.LocalPlayer.NickName = PlayerPieceEnum.BattleShip.ToString();
        PhotonNetwork.JoinRoom(roomToJoinName.text);
    }
    public void UpdateNickname(string newNickname)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.NickName = newNickname;
            Debug.Log($"Nickname updated to: {newNickname}");
        }
        else
        {
            Debug.LogWarning("Not connected to Photon. Connect before updating nickname.");
        }
    }
    public override void OnJoinedRoom()
    {
        ConnectedToRoom.SetCurrentImageIndex(0);
        roomName.SetText($"{PhotonNetwork.CurrentRoom.Name} - {PhotonNetwork.CurrentRoom.PlayerCount}");
        playerName.SetText(playerInRoomName.GetText());
        gameObject.SetActive(false);
        ConnectedToRoom.gameObject.SetActive(true);
        // PhotonNetwork.LoadLevel("MonopolyMainScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
