using System;
using System.Collections.Generic;
using Mosframe;
using UnityEngine;

public class DynamicHScrollViewUtils<T>: DynamicHScrollView
{
    private RectTransform GetElementAt(int index)
    {
        if (index < 0 || index >= this.containers.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        var current = this.containers.First;
        for (int i = 0; i < index; i++)
        {
            current = current.Next;
        }

        return current.Value;
    }
    
    public T GetChildAt(int index)
    {
        return GetElementAt(index).GetComponentInChildren<T>();
    }

    public T CreateNewChildAtEnd()
    {
        return AddNewItem().GetComponentInChildren<T>();
    }
}
public class PlayersHorizontalView : DynamicHScrollViewUtils<PlayerSummaryButton>
{
    public PlayerSummaryButton GetPlayer(int playerIndex)
    {
        return GetChildAt(playerIndex);
    }

    public void UpdatePlayer(int playerIndex, MonopolyPlayer selfMadePlayer)
    {
        PlayerSummaryButton playerSummaryButton = GetPlayer(playerIndex);
        playerSummaryButton.UpdatePlayer(selfMadePlayer);
    }
}
