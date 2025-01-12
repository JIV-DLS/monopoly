using System.Collections;
using System.Collections.Generic;
using Monopoly;
using Photon.Pun;
using UnityEngine;

public class PlayerElementOnMap : MonoBehaviourPunWithInitComponent
{
    // Get the SpriteRenderer component of the current GameObject
    private SpriteRenderer _spriteRenderer;
    public bool actionCompleted { get; private set; } // Tracks if the remote action is completed
    private MonopolyPlayer _monopolyPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
        
    }
    [PunRPC]
    private void StartRemoteAction()
    {
        if (!photonView.IsMine)
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
        photonView.RPC("NotifyActionComplete", RpcTarget.Others);
    }

    [PunRPC]
    private void NotifyActionComplete()
    {
        actionCompleted = true;
    }
    public override void OtherInit()
    {
        base.OtherInit();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer component found on this GameObject.");
        }
    }

    public Sprite GetSprite()
    {
        return _spriteRenderer.sprite;
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
}
