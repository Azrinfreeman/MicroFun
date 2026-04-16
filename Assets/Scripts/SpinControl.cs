using UnityEngine;

public class SpinControl : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isSpinning = false;

    [Header("Settings")]
    public float minSpinSpeed = 1000f; // Increased for more rotations
    public float maxSpinSpeed = 5000f;

    // Adding a "Jitter" makes it harder for the physics engine to repeat patterns
    public float maxRandomStartRotation = 360f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure your Angular Drag is set to something like 1 or 2 in the Inspector
        // so the wheel actually slows down naturally.
    }

    void Update()
    {
        // Use Mathf.Abs for angularVelocity because spinning clockwise results in negative velocity
        if (isSpinning && Mathf.Abs(rb.angularVelocity) <= 0.1f)
        {
            isSpinning = false;
            rb.angularVelocity = 0;
            CalculateResult();
        }
    }

    public void Spin()
    {
        if (isSpinning)
            return; // Prevent double-clicking while spinning

        // 1. Randomize the starting rotation slightly
        // This breaks the "pattern" of where the physics drag starts calculating
        transform.Rotate(0, 0, Random.Range(0f, maxRandomStartRotation));

        isSpinning = true;

        // 2. Apply a much larger range of torque
        // Using a negative or positive random range can also help if you want it to spin both ways
        float randomForce = Random.Range(minSpinSpeed, maxSpinSpeed);

        rb.AddTorque(randomForce, ForceMode2D.Impulse);
    }

    void CalculateResult()
    {
        // Normalize the angle to be between 0 and 360
        float finalAngle = transform.eulerAngles.z % 360;
        Debug.Log("Wheel stopped at angle: " + finalAngle);
    }
}
