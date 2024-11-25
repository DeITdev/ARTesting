using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScadaSetTagValue : MonoBehaviour
{
    // API request settings
    private string apiUrl = "http://192.168.1.49/WaWebService/Json/SetTagValue/ModSim32bit/Switch/";
    private string username = "admin";

    // UI Elements
    public Image statusColor;
    public TMPro.TMP_Dropdown dropDownStatus;

    // Public method to be called when the submit button is clicked
    public void OnSubmitButtonClicked()
    {
        // Start the GET request coroutine based on the dropdown value
        StartCoroutine(SendGetRequest(GetSwitchStatus()));
    }

    // Method to handle the On button click
    public void OnButtonClicked()
    {
        // Set the switch value to 1 (active/true) when the On button is clicked
        StartCoroutine(SendGetRequest(1));
    }

    // Method to handle the Off button click
    public void OffButtonClicked()
    {
        // Set the switch value to 0 (inactive/false) when the Off button is clicked
        StartCoroutine(SendGetRequest(0));
    }

    // Coroutine to send the GET request with a specific switch value
    private IEnumerator SendGetRequest(int switchValue)
    {
        // Construct the full API URL with the switch value
        string fullUrl = apiUrl + switchValue;

        // Create the UnityWebRequest for the GET method
        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            // Add basic authentication with the username only
            string auth = username + ":";
            string authBase64 = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(auth));
            request.SetRequestHeader("Authorization", "Basic " + authBase64);

            // Send the GET request and wait for the response
            yield return request.SendWebRequest();

            // Check for successful response
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

    // Helper method to parse the switch status value from the API response
    private bool ParseSwitchStatusFromResponse(string response)
    {
        // Simple example of parsing the response. In a real-world scenario, you might need to use a JSON parser.
        return response.Contains("\"SwitchStatus\": true");
    }

    // Method to return the switch status value based on the dropdown selection
    private int GetSwitchStatus()
    {
        return dropDownStatus.value; // Returns 0 or 1 based on the dropdown selection
    }
}
