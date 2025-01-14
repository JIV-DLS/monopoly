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
        transform.rotation= Quaternion.Euler(
            UnityEngine.Random.Range(0f, 360f),
            UnityEngine.Random.Range(0f, 360f),
            UnityEngine.Random.Range(0f, 360f)
        );
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

    public DiceRoller GetNewer(int index = 1)
    {
        // Clone the GameObject associated with the element
        GameObject original = gameObject;
        GameObject clone = Instantiate(original, original.transform.parent);

        // Optionally modify the clone (e.g., rename it or change its properties)
        clone.name = original.name + index.ToString();

        // Get the stride value from the original object's scale
        float stride = original.transform.localScale.x; // Assuming the cube's size is the same for x, y, and z

        // Set the new position to avoid collision
        Vector3 newPosition = original.transform.position + new Vector3(index * stride, 0, 0);
        clone.transform.position = newPosition;

        // Get the cloned component
        DiceRoller clonedComponent = clone.GetComponent<DiceRoller>();
        return clonedComponent;
    }
    public IEnumerator<int> ManualSleep(float seconds)
    {
        float elapsedTime = 0f;

        while (elapsedTime < seconds)
        {
            // Yield 0 while the time hasn't reached the target
            yield return 0;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;
        }
    }
    // Simulates rolling the dice and stores the result
    public IEnumerator<int> Roll()
{
    // Return 0 initially as a placeholder
    int faceAccordingToXYZ;
    ThrowAnyway();
    while (true)
    {
        if (diceRigidbody.linearVelocity.magnitude < stopThreshold && diceRigidbody.angularVelocity.magnitude < stopThreshold)
        {        
            // Debug.Log($"id {id}, velocity: {diceRigidbody.linearVelocity.magnitude}, angular: {diceRigidbody.angularVelocity.magnitude}");

            if (currentCollisions.Count > 1)
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return 0; // Wait for 50 frames
                }
                ThrowAnyway();
            }
            else
            {
                // Check if position and rotation have remained the same for 1 second
                if (transform.position == previousPosition && transform.rotation == previousRotation)
                {
                    faceAccordingToXYZ = GetFaceAccordingToXYZ();
                    if (faceAccordingToXYZ == 0)
                    {
                        ThrowAnyway();
                    }
                    else
                    {
                        // Sleep for 0.5 seconds to make sure the position and rotation don't change
                        var sleeper = ManualSleep(.7f);
                        while (sleeper.MoveNext())
                        {
                            yield return 0;
                        }
                        // After sleeping, check again and then break
                        if (transform.position == previousPosition && transform.rotation == previousRotation)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // Update previous position and rotation if they changed
                    previousPosition = transform.position;
                    previousRotation = transform.rotation;
                }
            }
        }

        // Store the initial position and rotation for the next iteration
        previousPosition = transform.position;
        previousRotation = transform.rotation;
        yield return 0;
    }

    yield return faceAccordingToXYZ; // Return the rolled value
}

    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}