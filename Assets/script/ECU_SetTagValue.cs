using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class ECU_SetTagValue : MonoBehaviour
{
    // Base API URL
    private string baseUrl = "http://192.168.1.14/WaWebService/Json/SetTagValue/TrainerKit/";

    private string username = "admin";

    // UI Elements
    public TMP_Dropdown dropDownValue; // Dropdown for "mati" (0) and "menyala" (1)
    public TMP_Dropdown dropDownDevice; // Dropdown for device selection

    // Public method to be called when the submit button is clicked
    public void OnSubmitButtonClicked()
    {
        // Start the GET request coroutine
        StartCoroutine(SendGetRequest());
    }

    // Coroutine to send the GET request
    private IEnumerator SendGetRequest()
    {
        // Get the selected values from the dropdowns
        int switchValue = GetSwitchValue();
        string deviceName = GetDeviceName();

        // Construct the full API URL with the selected device and value
        string fullUrl = baseUrl + deviceName + "/" + switchValue;

        // Create the UnityWebRequest for the GET method
        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            // Add basic authentication with the username only
            string auth = username + ":";
            string authBase64 = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(auth));
            request.SetRequestHeader("Authorization", "Basic " + authBase64);

            // Send the GET request and wait for the response
            yield return request.SendWebRequest();

            // Check for a successful response
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("GET Request Success: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("GET Request Error: " + request.error);
            }
        }
    }

    // Method to get the switch value based on the first dropdown selection
    private int GetSwitchValue()
    {
        // Returns 0 for "mati" and 1 for "menyala"
        return dropDownValue.value; // Assumes dropdown options are 0 for "mati" and 1 for "menyala"
    }

    // Method to get the device name based on the second dropdown selection
    private string GetDeviceName()
    {
        // Returns the selected device based on the dropdown option
        switch (dropDownDevice.value)
        {
            case 0:
                return "StopButton";
            case 1:
                return "ToggleSwitch01";
            case 2:
                return "ToggleSwitch02";
            default:
                return "StopButton"; // Default case, should not happen
        }
    }
}
