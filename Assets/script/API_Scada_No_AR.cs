using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq; // For using Newtonsoft.Json
using System.Collections.Generic; // For using List
using UnityEngine.UI; // For UI elements like Image

public class API_Scada_No_AR : MonoBehaviour
{
    private string url = "http://192.168.1.49/WaWebService/Json/GetTagValue/ModSim32bit";
    private string username = "admin";
    private string password = ""; // No password required

    // Variables to store the values of Pressure1, Pressure2, Pressure3, and Switch
    private List<float> pressureValues = new List<float>();
    private bool switchValue; // To store the value of the Switch

    // UI Elements
    public TextMeshProUGUI text;
    public Image statusColor; // The Image component representing the color status

    // Start is called before the first frame update
    void Start()
    {
        // Start the repeated request to get data every 2 seconds
        StartCoroutine(PostRequestRoutine());
    }

    // Coroutine to make the request every 2 seconds
    IEnumerator PostRequestRoutine()
    {
        while (true)
        {
            // Make the POST request
            yield return StartCoroutine(PostRequest());

            // Wait for 2 seconds before the next request
            yield return new WaitForSeconds(2f);
        }
    }

    // Coroutine to send the POST request and handle the response
    IEnumerator PostRequest()
    {
        // Create the JSON body for the POST request, including the Switch tag
        string jsonBody = @"{
            ""Tags"": [
                {""Name"": ""Pressure1""},
                {""Name"": ""Pressure2""},
                {""Name"": ""Pressure3""},
                {""Name"": ""Switch""}
            ]
        }";

        // Convert the JSON body into a byte array
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        // Create the UnityWebRequest for POST method
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            // Set the request body
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Add basic authentication (username only, no password)
            string auth = username + ":" + password;
            string authBase64 = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
            request.SetRequestHeader("Authorization", "Basic " + authBase64);

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Clear the pressureValues list to store fresh data
                pressureValues.Clear();

                // Parse the response JSON using Newtonsoft.Json
                JObject responseJson = JObject.Parse(request.downloadHandler.text);

                // Extract the values for the specific tags (Pressure1, Pressure2, Pressure3, Switch)
                foreach (var tag in responseJson["Values"])
                {
                    string tagName = tag["Name"].ToString();
                    if (tagName == "Switch")
                    {
                        // Parse the Switch value as a boolean (1 = true, 0 = false)
                        switchValue = tag["Value"].ToObject<int>() == 1;
                        Debug.Log($"Tag: {tagName}, Value: {switchValue}");

                        // Update the color based on the Switch value
                        UpdateStatusColorBasedOnSwitchValue(switchValue);
                    }
                    else
                    {
                        // Parse the pressure values as floats
                        float tagValue = tag["Value"].ToObject<float>();
                        pressureValues.Add(tagValue);
                        Debug.Log($"Tag: {tagName}, Value: {tagValue}");
                    }
                }

                // Format the pressure values to two decimal places
                List<string> formattedPressureValues = new List<string>();
                foreach (float value in pressureValues)
                {
                    formattedPressureValues.Add(value.ToString("F2")); // Format each value to 2 decimal places
                }

                // Print all stored pressure values together and the switch value
                Debug.Log("Pressure Values: " + string.Join(", ", formattedPressureValues));
                Debug.Log("Switch Value: " + switchValue);
                text.text = "Pressure Values: " + string.Join(", ", formattedPressureValues) + "\nSwitch Value: " + switchValue;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    // Method to update the status color based on the Switch value
    private void UpdateStatusColorBasedOnSwitchValue(bool switchValue)
    {
        if (switchValue)
        {
            statusColor.color = Color.green; // Switch is ON (true)
        }
        else
        {
            statusColor.color = Color.red; // Switch is OFF (false)
        }
    }
}