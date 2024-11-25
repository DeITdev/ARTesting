using UnityEngine;
using UnityEngine.UI;

public class EmissionController : MonoBehaviour
{
    public Toggle redToggle; // Reference to the red lamp toggle
    public Toggle yellowToggle; // Reference to the yellow lamp toggle

    public Material redEmissionMaterial; // The material for the red emission
    public GameObject redEmissionObject; // The object that has the red emission material

    public Material yellowEmissionMaterial; // The material for the yellow emission
    public GameObject yellowEmissionObject; // The object that has the yellow emission material

    void Start()
    {
        // Set up toggle listeners
        redToggle.onValueChanged.AddListener(OnRedToggleValueChanged);
        yellowToggle.onValueChanged.AddListener(OnYellowToggleValueChanged);

        // Initialize emission states based on the toggle values
        OnRedToggleValueChanged(redToggle.isOn);
        OnYellowToggleValueChanged(yellowToggle.isOn);
    }

    // Method called when the red toggle value changes
    private void OnRedToggleValueChanged(bool isOn)
    {
        if (redEmissionObject != null && redEmissionMaterial != null)
        {
            if (isOn)
            {
                // Activate red emission
                EnableEmission(redEmissionMaterial, redEmissionObject, Color.red);
            }
            else
            {
                // Deactivate red emission
                DisableEmission(redEmissionMaterial, redEmissionObject);
            }
        }
    }

    // Method called when the yellow toggle value changes
    private void OnYellowToggleValueChanged(bool isOn)
    {
        if (yellowEmissionObject != null && yellowEmissionMaterial != null)
        {
            if (isOn)
            {
                // Activate yellow emission
                EnableEmission(yellowEmissionMaterial, yellowEmissionObject, Color.yellow);
            }
            else
            {
                // Deactivate yellow emission
                DisableEmission(yellowEmissionMaterial, yellowEmissionObject);
            }
        }
    }

    private void EnableEmission(Material material, GameObject emissionObject, Color color)
    {
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", color * Mathf.LinearToGammaSpace(1f)); // Adjust color intensity as needed
        emissionObject.GetComponent<Renderer>().material = material; // Apply the material
    }

    private void DisableEmission(Material material, GameObject emissionObject)
    {
        material.DisableKeyword("_EMISSION");
        emissionObject.GetComponent<Renderer>().material = material; // Apply the material
    }
}
