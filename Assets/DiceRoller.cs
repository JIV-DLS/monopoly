using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    private int id;
    private Rigidbody diceRigidbody; // Attach your dice Rigidbody
    public float throwForce = 100f;  // Adjustable force for throwing the dice
    public float torqueForce = 50f;  // Adjustable torque for random rotation
    public float stopThreshold = 5; // Threshold to consider the dice stopped
    public LayerMask groundMask; // Mask for ground detection (optional)

    private bool hasStopped = false; // To track if the dice has stopped
    private Vector3 startPosition;

    void Start()
    {
        id = Random.Range(1, 7);
        diceRigidbody = GetComponent<Rigidbody>();
        if (diceRigidbody == null)
        {
            Debug.LogError("Assign the Rigidbody of the dice in the inspector.");
        }
        ThrowDice();
    }

    public void ThrowDice()
    {
        //diceRigidbody.useGravity = true;
        startPosition = diceRigidbody.transform.position;
        // Reset position and state
        ResetDice();

        // Random direction and torque
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized;
        Vector3 randomTorque = new Vector3(Random.Range(-torqueForce, torqueForce), Random.Range(-torqueForce, torqueForce), Random.Range(-torqueForce, torqueForce));

        // Apply force and torque
        diceRigidbody.AddForce(randomDirection * throwForce, ForceMode.Impulse);
        diceRigidbody.AddTorque(randomTorque, ForceMode.Impulse);

        hasStopped = false;
    }

    void Update()
    {
        if (!hasStopped)
        {
            // Check if the dice has stopped moving
            
            Debug.Log($"id {id}, velocity: {diceRigidbody.velocity.magnitude}, angular: {diceRigidbody.angularVelocity.magnitude}");
            if (diceRigidbody.velocity.magnitude < stopThreshold && diceRigidbody.angularVelocity.magnitude < stopThreshold)
            {
                hasStopped = true;
                Debug.Log("Dice stopped! Top face: " + GetTopFace());
                
                // diceRigidbody.useGravity = false;
            }
        }
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