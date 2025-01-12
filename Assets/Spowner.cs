using Photon.Pun;
using UnityEngine;

public class Spowner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.Instantiate("BattleShip", transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
