using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ManageRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomToCreateName;
    public TMP_InputField roomToJoinName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomToCreateName.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomToJoinName.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MonopolyMainScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
