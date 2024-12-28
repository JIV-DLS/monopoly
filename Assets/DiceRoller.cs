using UnityEngine;
using System.Collections.Generic;
using System;
public class DiceRoller : MonoBehaviour
{
    private int id;
    private string lastRoll = "";
    private Rigidbody diceRigidbody; // Attach your dice Rigidbody
    public float throwForce = 100f;  // Adjustable force for throwing the dice
    public float torqueForce = 50f;  // Adjustable torque for random rotation
    public float stopThreshold = 5; // Threshold to consider the dice stopped
    public LayerMask groundMask; // Mask for ground detection (optional)

    private Vector3 startPosition;
    public Vector3Int DirectionValues;
    private Vector3Int OpposingDirectionValues;

    readonly List<string> FaceRepresent = new List<string>() {"", "1", "2", "3", "4", "5", "6"};
    private DateTime _lastActionTime;
    private readonly TimeSpan _cooldownPeriod = TimeSpan.FromSeconds(2); // 2-second cooldown
    void Start()
    {
        id = UnityEngine.Random.Range(1, 7);
        diceRigidbody = GetComponent<Rigidbody>();
        if (diceRigidbody == null)
        {
            Debug.LogError("Assign the Rigidbody of the dice in the inspector.");
        }
        OpposingDirectionValues = 7 * Vector3Int.one - DirectionValues;
    }
    public void ThrowDice()
    {
        DateTime currentTime = DateTime.Now;
        
        if (currentTime - _lastActionTime < _cooldownPeriod)
        {
            Console.WriteLine("Action is on cooldown. Please wait.");
            return;
        }

        // Perform the action
        this._ThrowDice();

        // Update the last action time
        _lastActionTime = currentTime;
    }
    private void _ThrowDice()
    {
        //diceRigidbody.useGravity = true;
        startPosition = diceRigidbody.transform.position;
        // Reset position and state
        ResetDice();

        // Random direction and torque
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1, UnityEngine.Random.Range(-1f, 1f)).normalized;
        Vector3 randomTorque = new Vector3(UnityEngine.Random.Range(-torqueForce, torqueForce), UnityEngine.Random.Range(-torqueForce, torqueForce), UnityEngine.Random.Range(-torqueForce, torqueForce));

        // Apply force and torque
        diceRigidbody.AddForce(randomDirection * throwForce, ForceMode.Impulse);
        diceRigidbody.AddTorque(randomTorque, ForceMode.Impulse);

    }

    void Update()
    {
        // Debug.Log($"id {id}, velocity: {diceRigidbody.velocity.magnitude}, angular: {diceRigidbody.angularVelocity.magnitude}");
        string currentFace = GetFaceAccordingToXYZ();
        if (diceRigidbody.velocity.magnitude < stopThreshold && diceRigidbody.angularVelocity.magnitude < stopThreshold && lastRoll != currentFace)
        {
            lastRoll = currentFace;
            Debug.Log("Dice stopped! Top face: " + lastRoll);
            _lastActionTime = DateTime.Now - _cooldownPeriod - TimeSpan.FromMilliseconds(100);  // Forces cooldown to pass
        }
    }

    private string GetFaceAccordingToXYZ()
    {
        if (transform.hasChanged)
        {
            if (  Vector3.Cross(Vector3.up, transform.right).magnitude < 0.5f) //x axis a.b.sin theta <45
                //if ((int) Vector3.Cross(Vector3.up, transform.right).magnitude == 0) //Previously
            {
                if (Vector3.Dot(Vector3.up, transform.right) > 0)
                {
                    return FaceRepresent[DirectionValues.x];
                }
                else
                {
                    return FaceRepresent[OpposingDirectionValues.x];
                }
            }
            else if ( Vector3.Cross(Vector3.up, transform.up).magnitude <0.5f) //y axis
            {
                if (Vector3.Dot(Vector3.up, transform.up) > 0)
                {
                    return FaceRepresent[DirectionValues.y];
                }
                else
                {
                    return FaceRepresent[OpposingDirectionValues.y];
                }
            }
            else if ( Vector3.Cross(Vector3.up, transform.forward).magnitude <0.5f) //z axis
            {
                if (Vector3.Dot(Vector3.up, transform.forward) > 0)
                {
                   return FaceRepresent[DirectionValues.z];
                }
                else
                {
                    return FaceRepresent[OpposingDirectionValues.z];
                }
            }


            transform.hasChanged = false;
        }

        return "";
    }

    private void ResetDice()
    {
        // Reset dice position and state
        diceRigidbody.velocity = Vector3.zero;
        diceRigidbody.angularVelocity = Vector3.zero;
        diceRigidbody.transform.position = startPosition;
        diceRigidbody.transform.rotation = Quaternion.identity;
    }

    private int GetTopFace()
    {
        // Raycast from each face of the dice to determine the top face
        Vector3[] faceDirections = {
            Vector3.up,    // Top face
            Vector3.down,  // Bottom face
            Vector3.left,  // Left face
            Vector3.right, // Right face
            Vector3.forward, // Front face
            Vector3.back    // Back face
        };

        int faceIndex = -1;
        float maxDot = float.MinValue;

        for (int i = 0; i < faceDirections.Length; i++)
        {
            float dot = Vector3.Dot(diceRigidbody.transform.up, faceDirections[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                faceIndex = i + 1; // Assuming face values 1 to 6
            }
        }

        return faceIndex;
    }
}