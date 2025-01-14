using UnityEngine;

public class DinoShadowAnim : MonoBehaviour
{
    [SerializeField] private Transform dinoBody; // Reference to the dinosaur's body
    [SerializeField] private Vector3 offset = new Vector3(0, 0.1f, 0); // Offset for the shadow position

    private void Update()
    {
        if (dinoBody != null)
        {
            // Update the shadow's position to follow the dinosaur with an offset
            transform.position = dinoBody.position + offset;

            // Maintain rotation but only allow changes on the Z-axis
            Vector3 shadowRotation = transform.eulerAngles;      // Get current shadow rotation
            shadowRotation.z = dinoBody.eulerAngles.z;           // Match the Z-axis rotation of the dinosaur
            transform.eulerAngles = shadowRotation;             // Apply the constrained rotation
        }
    }
}
