using UnityEngine;
using System.Collections.Generic;
using System;
public class DiceRoller : MonoBehaviour
{
    public DicesManager DicesManager;
    private int id;
    private int lastRoll = 0;
    private Rigidbody diceRigidbody; // Attach your dice Rigidbody
    public float throwForce = 100f;  // Adjustable force for throwing the dice
    public float torqueForce = 50f;  // Adjustable torque for random rotation
    public float stopThreshold = 5; // Threshold to consider the dice stopped
    public LayerMask groundMask; // Mask for ground detection (optional)

    private Vector3 startPosition;
    public Vector3Int DirectionValues;
    private Vector3Int OpposingDirectionValues;
    private Vector3 previousPosition;
    private Quaternion previousRotation;

   private float timer = 0f;
    private bool actionTriggered = false;
     readonly List<int> FaceRepresent = new List<int>() {0, 1, 2, 3, 4, 5, 6};
    private DateTime _lastActionTime;
    private readonly TimeSpan _cooldownPeriod = TimeSpan.FromSeconds(2); // 2-second cooldown
    // List to store currently collided objects
    private List<GameObject> currentCollisions = new List<GameObject>();
    void Start()
    {
        id = UnityEngine.Random.Range(1, 7);
        diceRigidbody = GetComponent<Rigidbody>();
        if (diceRigidbody == null)
        {
            Debug.LogError("Assign the Rigidbody of the dice in the inspector.");
        }
        OpposingDirectionValues = 7 * Vector3Int.one - DirectionValues;
        // Store the initial position and rotation
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        // Add to the list if not already present
        if (!currentCollisions.Contains(collidedObject))
        {
            currentCollisions.Add(collidedObject);
            //Debug.Log($"Started colliding with: {collidedObject.name}");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // (Optional) Log ongoing collisions if needed
        //Debug.Log($"Continuing collision with: {collision.gameObject.name}");
    }

    void OnCollisionExit(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        // Remove from the list when collision ends
        if (currentCollisions.Contains(collidedObject))
        {
            currentCollisions.Remove(collidedObject);
            //Debug.Log($"Stopped colliding with: {collidedObject.name}");
        }
    }
    public bool CanBeThrown(DateTime currentTime)
    {
        
        return !(currentTime - _lastActionTime < _cooldownPeriod);
    }
    public bool CanBeThrown()
    {
        return CanBeThrown(DateTime.Now);
    }
    public void ThrowDice()
    {
        DateTime currentTime = DateTime.Now;
        
        if (!CanBeThrown(currentTime))
        {
            Console.WriteLine("Action is on cooldown. Please wait.");
            return;
        }

        // Perform the action
        this._ThrowDice();

        // Update the last action time
        _lastActionTime = currentTime;
    }

    public void ThrowAnyway()
    {
        _ThrowDice();
    }
    private void _ThrowDice()
    {
        //diceRigidbody.useGravity = true;
        startPosition = diceRigidbody.transform.position;
        // Reset position and state
        ResetDice();

        // Random direction and torque
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(10f, 40f), UnityEngine.Random.Range(-10f, 10f)).normalized;
        Vector3 randomTorque = new Vector3(UnityEngine.Random.Range(-torqueForce, torqueForce), UnityEngine.Random.Range(-torqueForce, torqueForce), UnityEngine.Random.Range(-torqueForce, torqueForce));

        // Apply force and torque
        diceRigidbody.AddForce(randomDirection * throwForce, ForceMode.Impulse);
        diceRigidbody.AddTorque(randomTorque, ForceMode.Impulse);

    }

    void Update()
    {
        
        if (diceRigidbody.linearVelocity.magnitude < stopThreshold && diceRigidbody.angularVelocity.magnitude < stopThreshold)
        {        
            // Debug.Log($"id {id}, velocity: {diceRigidbody.linearVelocity.magnitude}, angular: {diceRigidbody.angularVelocity.magnitude}");

            if (currentCollisions.Count > 1)
            {
                ThrowAnyway();
            }
            else
            {
                // Check if position and rotation have remained the same for 1 second
                if (transform.position == previousPosition && transform.rotation == previousRotation)
                {
                    timer += Time.deltaTime; // Accumulate time
                    // Debug.Log($"id {id} all position is good {timer} {actionTriggered}");
                    int _lastRoll = GetFaceAccordingToXYZ();
                    if (_lastRoll == 0)
                    {
                        ThrowAnyway();
                    }
                    else if (timer >= 1f && !actionTriggered)
                    {
                        // Trigger the action if 1 second has passed
                        DoDieMovementEndedAction(_lastRoll); 
                        actionTriggered = true; // Prevent multiple triggers
                    }
                }
                else
                {
                    // Reset the timer if position or rotation changes
                    timer = 0f;
                    actionTriggered = false;
                }
             }
        }        
        // Store the initial position and rotation
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    
    }
    void DoDieMovementEndedAction(int gottenFace)
    {

        lastRoll = gottenFace;
        // Debug.Log($"Dice stopped! Top face: {lastRoll}") ;
        _lastActionTime = DateTime.Now - _cooldownPeriod - TimeSpan.FromMilliseconds(100);  // Forces cooldown to pass
        DicesManager.NotifyResponse(this);
    }

    public int LastRoll()
    {
        
        Debug.Assert(lastRoll > 0, "The value must be greater than 0");
        return lastRoll;
    }
    private int GetFaceAccordingToXYZ()
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

        return 0;
    }

    private void ResetDice()
    {
        // Reset dice position and state
        diceRigidbody.linearVelocity = Vector3.zero;
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
    public DiceRoller GetNewer(int index=1)
    {

        // Clone the GameObject associated with the element
        GameObject original = gameObject;
        GameObject clone = Instantiate(original, original.transform.parent);

        // Optionally modify the clone (e.g., rename it or change its properties)
        clone.name = original.name + index.ToString();

        DiceRoller clonedComponent = clone.GetComponent<DiceRoller>();
        return clonedComponent;
    }
}