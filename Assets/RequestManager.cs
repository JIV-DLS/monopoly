using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestManager : MonoBehaviourPunCallbacks
{
    // List to track multiple UUIDs of requests
    private readonly List<string> _requestsUuids = new List<string>();

    // This method marks the request as processed.
    // Helper method to get or create the 'requests' dictionary in the custom properties
    private ExitGames.Client.Photon.Hashtable GetRequestsDictionary(Photon.Realtime.Player player)
    {
        // Get the current custom properties of the given player
        var currentProperties = player.CustomProperties;

        // Ensure the 'requests' key exists (it might be null initially)
        if (!currentProperties.ContainsKey("requests"))
        {
            currentProperties["requests"] = new ExitGames.Client.Photon.Hashtable();
        }

        // Return the 'requests' dictionary
        return currentProperties["requests"] as ExitGames.Client.Photon.Hashtable;
    }

    // Method to set the request's processing state
    void SetRequestProcessingState(Photon.Realtime.Player player, string requestUuid, bool isProcessing)
    {
        // Get the 'requests' dictionary
        var requestsDict = GetRequestsDictionary(player);

        // Update or add the request with its processing state
        requestsDict[requestUuid] = new ExitGames.Client.Photon.Hashtable
        {
            { "state", isProcessing ? "processing" : "not_processing" }
        };

        // Apply the updated properties to the player
        player.SetCustomProperties(player.CustomProperties);

        // Optionally log the state change for debugging
        Debug.Log($"Request {requestUuid} state set to {(isProcessing ? "processing" : "not_processing")}");
    }

    // Method to mark the request as processed
    void MarkRequestAsProcessed(Photon.Realtime.Player player, string requestUuid)
    {
        // Get the 'requests' dictionary
        var requestsDict = GetRequestsDictionary(player);

        // Update the state of the specific request to 'processed'
        requestsDict[requestUuid] = new ExitGames.Client.Photon.Hashtable
        {
            { "state", "processed" }
        };

        // Apply the updated properties to the player
        player.SetCustomProperties(player.CustomProperties);

        // Optionally, remove the request from the list once it's processed
        _requestsUuids.Remove(requestUuid); // Remove from the list when done processing
        Debug.Log($"Request {requestUuid} has been marked as processed and removed from the list.");
    }

    // This method processes the RPC method call and ensures the request is marked as processed.
    [PunRPC]
    public IEnumerator ProcessRpcMethodWithAttribute(Photon.Realtime.Player player, string methodName, string requestUuid, object[] parameters)
    {
        if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            yield return null;
        }
        else
        {
            // Use Reflection to get the method info
            var method = GetType().GetMethod(methodName);

            // Call the original RPC method
            yield return method?.Invoke(this, parameters);
        
            // Mark the request as processed
            MarkRequestAsProcessed(player, requestUuid);
        }
    }

    // Start method to initiate requests

    // Coroutine to wait for a request to be processed and call the corresponding RPC (with UUID)
    public IEnumerator WaitForRequest(Photon.Realtime.Player player, string methodName, params object[] rpcParams)
    {
        string requestUuid = Guid.NewGuid().ToString(); // Automatically generate a new UUID as string for this request

        // Set custom properties for the request (Initially set to false)
        SetRequestProcessingState(player, requestUuid, false);

        // Add the request UUID to the list of requests
        _requestsUuids.Add(requestUuid);
        // Create a new array with one extra element

        // Call the RPC method
        photonView.RPC("ProcessRpcMethodWithAttribute", RpcTarget.All, player, methodName, requestUuid, rpcParams);

        // Wait while the request is being processed
        while (!IsRequestProcessed(player, requestUuid))
        {
            yield return null; // Wait for the next frame
        }

        // Request processed, remove it from the list
        _requestsUuids.Remove(requestUuid);
        Debug.Log($"Request {requestUuid} has been processed and removed from the list.");
    }

    // Method to check if the request has been processed
    bool IsRequestProcessed(Photon.Realtime.Player player, string requestUuid)
    {
        object result;
        if (player.CustomProperties.TryGetValue("requests", out result))
        {
            var requestsDict = result as ExitGames.Client.Photon.Hashtable;
            if (requestsDict != null && requestsDict.ContainsKey(requestUuid))
            {
                var request = requestsDict[requestUuid] as ExitGames.Client.Photon.Hashtable;
                if (request != null && request.ContainsKey("state"))
                {
                    return (string)request["state"] == "processed"; // Check if the request state is 'processed'
                }
            }
        }
        return false; // Default to false if not processed yet
    }
}