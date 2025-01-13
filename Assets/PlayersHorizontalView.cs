using System;
using System.Collections.Generic;
using Monopoly;
using Mosframe;
using UnityEngine;

public class PlayersHorizontalView : DynamicHScrollView
{
    
    public PlayerSummaryButton CreateNewChildAtEnd()
    {
        return AddNewItem().GetComponentInChildren<PlayerSummaryButton>();
    }
    public PlayerSummaryButton GetPlayer(int playerIndex)
    {
        return GetChildAt<PlayerSummaryButton>(playerIndex);
    }

    public void UpdatePlayer(int playerIndex, MonopolyPlayer selfMadePlayer)
    {
        PlayerSummaryButton playerSummaryButton = GetPlayer(playerIndex);
        playerSummaryButton.UpdatePlayer(selfMadePlayer);
    }
}
