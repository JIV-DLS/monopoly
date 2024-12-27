using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSleepThreeshold : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 previousVelocity; // To store the velocity from the previous frame
    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0.01f; // Set to a low value to stop movement sooner

        previousVelocity = Vector3.zero; // Initialize the previous velocity
    }

    // Update is called once per frame
    void Update()
    {
        // Current velocity
        Vector3 currentVelocity = rb.velocity;

        // Speed is the magnitude of the velocity
        float speed = currentVelocity.magnitude;

        // Acceleration is the change in velocity over time
        Vector3 accelerationVector = (currentVelocity - previousVelocity) / Time.deltaTime;
        float acceleration = accelerationVector.magnitude;

        // Log the speed and acceleration
        Debug.Log($"Speed: {speed} m/s, Acceleration: {acceleration} m/s²");

        // Update the previous velocity for the next frame rb.velocity.magnitude < 0.5f &&
        previousVelocity = currentVelocity;
        
        if ( acceleration < 0.5f)
        {
            Debug.Log($"changing because acceleration is: {acceleration} m/s²");
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep(); // Optionally force it to sleep
        }
    }
}
