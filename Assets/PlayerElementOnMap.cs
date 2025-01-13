using System.Collections;
using System.Collections.Generic;
using Monopoly;
using Photon.Pun;
using UnityEngine;

public class PlayerElementOnMap : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Drag TMP_InputField here in Inspector

    // Get the SpriteRenderer component of the current GameObject
    public bool actionCompleted { get; private set; } // Tracks if the remote action is completed
    private MonopolyPlayer _monopolyPlayer;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    [PunRPC]
    private void StartRemoteAction()
    {
        if (true)
        {
            Debug.LogError("Received remote action request, but this is not the owner!");
            return;
        }

        // Simulate a remote action
        StartCoroutine(RemoteActionRoutine());
    }

    private IEnumerator RemoteActionRoutine()
    {
        yield return _monopolyPlayer.Play(); // Simulate a 2-second action
        // Notify all players that the action is complete
        //photonView.RPC("NotifyActionComplete", RpcTarget.Others);
    }

    [PunRPC]
    private void NotifyActionComplete()
    {
        actionCompleted = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTransform(Vector3 vector3)
    {
        transform.position = vector3;
    }

    public void SetMonopolyPlayer(MonopolyPlayer monopolyPlayer)
    {
        _monopolyPlayer = monopolyPlayer;
    }

    public void SetActionCompleted(bool actionCompletedToSet)
    {
        this.actionCompleted = actionCompletedToSet;
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
