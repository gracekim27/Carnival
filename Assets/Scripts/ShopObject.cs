using UnityEngine;

public class ShopObject : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f; // Height of the sine wave, controls how high the object moves
    [SerializeField] private float frequency = 1f; // Speed of the sine wave, controls how fast the object moves

    private Vector3 startPosition; // Starting position of the object

    void Start()
    {
        startPosition = transform.position; // Save the initial position to base the sine wave movement on it
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave pattern
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}