using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadingSpinnerTMP : MonoBehaviour
{
    public TextMeshProUGUI loadingTextTMP; // Reference to the TMP text
    public Image loadingCircle;            // Reference to the loading circle image
    public float rotationSpeed = 200f;     // Speed of the circle rotation

    void Start()
    {
        // Make sure the loading TMP text and circle are active
        if (loadingTextTMP != null)
        {
            loadingTextTMP.gameObject.SetActive(true);
        }

        if (loadingCircle != null)
        {
            loadingCircle.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // Rotate the loading circle
        if (loadingCircle != null)
        {
            loadingCircle.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    // Method to hide the loading UI when loading is complete
    public void HideLoadingScreen()
    {
        if (loadingTextTMP != null)
        {
            loadingTextTMP.gameObject.SetActive(false);
        }

        if (loadingCircle != null)
        {
            loadingCircle.gameObject.SetActive(false);
        }
    }
}


