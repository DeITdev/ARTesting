using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq; // For using Newtonsoft.Json
using System.Collections.Generic; // For using List
using UnityEngine.UI; // For UI elements like Image

public class APIScada_ECU : MonoBehaviour
{
    private string url = "http://192.168.1.49/WaWebService/Json/GetTagValue/TrainerKit";
    private string username = "admin";
    private string password = ""; // No password required

    // UI Elements
    public TextMeshProUGUI text;
    public Image square1; // Image for StopButton
    public Image square2; // Image for ToggleSwitch01
    public Image square3; // Image for ToggleSwitch02

    // Start is called before the first frame update
    void Start()
    {
        // Start the repeated request to get data every 5 seconds
        StartCoroutine(PostRequestRoutine());
    }

    // Coroutine to make the request every 5 seconds
    IEnumerator PostRequestRoutine()
    {
        while (true)
        {
            // Make the POST request
            yield return StartCoroutine(PostRequest());

            // Wait for 5 seconds before the next request
            yield return new WaitForSeconds(2f);
        }
    }

    // Coroutine to send the POST request and handle the response
    IEnumerator PostRequest()
    {
        // Create the JSON body for the POST request, including the updated tags
        string jsonBody = @"{
            ""Tags"": [
                {""Name"": ""StartButton""},
                {""Name"": ""StopButton""},
                {""Name"": ""ToggleSwitch01""},
                {""Name"": ""ToggleSwitch02""}
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
                // Parse the response JSON using Newtonsoft.Json
                JObject responseJson = JObject.Parse(request.downloadHandler.text);

                // Variable to hold the display text
                string displayText = "";

                // Extract the values for the specific tags and update the corresponding images
                foreach (var tag in responseJson["Values"])
                {
                    string tagName = tag["Name"].ToString();
                    int tagValue = tag["Value"].ToObject<int>();

                    // Update the display text
                    displayText += $"{tagName}: {tagValue}\n";

                    // Update specific squares based on the tag name
                    if (tagName == "StopButton")
                    {
                        UpdateStatusColorBasedOnValue(tagValue, square1);
                    }
                    else if (tagName == "ToggleSwitch01")
                    {
                        UpdateStatusColorBasedOnValue(tagValue, square2);
                    }
                    else if (tagName == "ToggleSwitch02")
                    {
                        UpdateStatusColorBasedOnValue(tagValue, square3);
                    }

                    Debug.Log($"Tag: {tagName}, Value: {tagValue}");
                }

                // Update the text UI element with the formatted data
                text.text = displayText;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    // Method to update the status color based on the value (0 = red, 1 = green)
    private void UpdateStatusColorBasedOnValue(int value, Image image)
    {
        if (value == 1)
        {
            image.color = Color.green; // Output is 1 (true)
        }
        else
        {
            image.color = Color.red; // Output is 0 (false)
        }
    }
}
