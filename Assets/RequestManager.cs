using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestManager : MonoBehaviourPunCallbacks
{
    // List to track multiple UUIDs of requests
    private readonly List<Guid> _requestsUuids = new List<Guid>();

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
    void SetRequestProcessingState(Photon.Realtime.Player player, Guid requestUuid, bool isProcessing)
    {
        // Get the 'requests' dictionary
        var requestsDict = GetRequestsDictionary(player);

        // Update or add the request with its processing state
        requestsDict[requestUuid.ToString()] = new ExitGames.Client.Photon.Hashtable
        {
            { "state", isProcessing ? "processing" : "not_processing" }
        };

        // Apply the updated properties to the player
        player.SetCustomProperties(player.CustomProperties);

        // Optionally log the state change for debugging
        Debug.Log($"Request {requestUuid} state set to {(isProcessing ? "processing" : "not_processing")}");
    }

    // Method to mark the request as processed
    void MarkRequestAsProcessed(Photon.Realtime.Player player, Guid requestUuid)
    {
        // Get the 'requests' dictionary
        var requestsDict = GetRequestsDictionary(player);

        // Update the state of the specific request to 'processed'
        requestsDict[requestUuid.ToString()] = new ExitGames.Client.Photon.Hashtable
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
    void ProcessRpcMethodWithAttribute(Photon.Realtime.Player player, string methodName, object[] parameters)
    {
        // Use Reflection to get the method info
        var method = this.GetType().GetMethod(methodName);

        // Call the original RPC method
        method.Invoke(this, parameters);

        // Get the request UUID from the parameters
        Guid requestUuid = (Guid)parameters[0]; // Assuming the first parameter is always the requestUuid

        // Mark the request as processed
        MarkRequestAsProcessed(player, requestUuid);
    }

    // Example RPC method (this will be called on all clients when invoked)
    void SomeRpcMethod(Photon.Realtime.Player player, Guid requestUuid, string param1, int param2)
    {
        Debug.Log($"RPC Method 'SomeRpcMethod' called with param1: {param1}, param2: {param2}");
        // Logic for handling the method here
    }

    void AnotherRpcMethod(Photon.Realtime.Player player, Guid requestUuid, int param1, bool param2)
    {
        Debug.Log($"RPC Method 'AnotherRpcMethod' called with param1: {param1}, param2: {param2}");
        // Logic for handling the method here
    }

    // Start method to initiate requests
    void Start()
    {
        // Example of starting multiple requests with dynamic RPC method names and parameters
        StartCoroutine(WaitForRequest(PhotonNetwork.LocalPlayer, "ProcessRpcMethodWithAttribute", "SomeRpcMethod", "param1", 123)); // Request without UUID
        StartCoroutine(WaitForRequest(PhotonNetwork.LocalPlayer, "ProcessRpcMethodWithAttribute", "AnotherRpcMethod", 456, true)); // Request without UUID

        Guid newRequestUuid1 = Guid.NewGuid(); // Generate a unique UUID for the first request
        StartCoroutine(WaitForRequest(PhotonNetwork.LocalPlayer, "ProcessRpcMethodWithAttribute", newRequestUuid1, "param1", 123)); // Request with UUID
    }

    // Coroutine to wait for a request to be processed and call the corresponding RPC (with UUID)
    public IEnumerator WaitForRequest(Photon.Realtime.Player player, string methodName, params object[] rpcParams)
    {
        Guid requestUuid = Guid.NewGuid(); // Automatically generate a new UUID for this request

        // Set custom properties for the request (Initially set to false)
        SetRequestProcessingState(player, requestUuid, false);

        // Add the request UUID to the list of requests
        _requestsUuids.Add(requestUuid);

        // Call the RPC method
        photonView.RPC(methodName, RpcTarget.All, player, rpcParams);

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
    bool IsRequestProcessed(Photon.Realtime.Player player, Guid requestUuid)
    {
        object result;
        if (player.CustomProperties.TryGetValue("requests", out result))
        {
            var requestsDict = result as ExitGames.Client.Photon.Hashtable;
            if (requestsDict != null && requestsDict.ContainsKey(requestUuid.ToString()))
            {
                var request = requestsDict[requestUuid.ToString()] as ExitGames.Client.Photon.Hashtable;
                if (request != null && request.ContainsKey("state"))
                {
                    return (string)request["state"] == "processed"; // Check if the request state is 'processed'
                }
            }
        }
        return false; // Default to false if not processed yet
    }

}