using UnityEngine;
using UnityEngine.UI;

public class playercam : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensitivity = 150f;  // Global sensitivity
    public Slider sensSlider;         // Drag your slider here in Inspector

    [Header("References")]
    public Transform orientation;

    float xRotation;
    float yRotation;

    void Start()
    {
        // Load saved sensitivity or fallback to default
        float savedSens = PlayerPrefs.GetFloat("Sensitivity", sensitivity);
        sensitivity = savedSens;

        if (sensSlider != null)
        {
            sensSlider.value = sensitivity;
            sensSlider.onValueChanged.AddListener(UpdateSensitivity);
        }
    }

    void UpdateSensitivity(float newSens)
    {
        sensitivity = newSens;
        PlayerPrefs.SetFloat("Sensitivity", newSens);
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
