using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class LampController : MonoBehaviour
{
    public Toggle redToggle;
    public Toggle yellowToggle;

    private const string RED_API_URL = "https://192.168.100.10/data/tags/WISE4010:Red/value";
    private const string YELLOW_API_URL = "https://192.168.100.10/data/tags/WISE4010:Yellow/value";
    private const string USERNAME = "admin";
    private const string PASSWORD = "00000000";

    void Start()
    {
        // Assign toggle change listeners
        redToggle.onValueChanged.AddListener(delegate { UpdateLampState(RED_API_URL, "Red", redToggle.isOn); });
        yellowToggle.onValueChanged.AddListener(delegate { UpdateLampState(YELLOW_API_URL, "Yellow", yellowToggle.isOn); });
    }

    private void UpdateLampState(string apiUrl, string lampColor, bool isOn)
    {
        string value = isOn ? "1" : "0"; // Set value based on the state of the Toggle
        StartCoroutine(SendApiRequest(apiUrl, value, lampColor));
    }

    private IEnumerator SendApiRequest(string apiUrl, string value, string lampColor)
    {
        string body = $"{{\"name\": \"WISE4010:{lampColor}\", \"type\": \"IO Tag\", \"value\": \"{value}\", \"quality\": \"0000H\", \"timestamp\": \"1728962178.070078\", \"readwrite\": \"3\"}}";

        Debug.Log("Request Body: " + body); // Log the request body

        using (UnityWebRequest request = UnityWebRequest.Put(apiUrl, body))
        {
            // Set the request headers
            request.SetRequestHeader("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(USERNAME + ":" + PASSWORD)));
            request.SetRequestHeader("Referer", "https://192.168.100.10");
            request.SetRequestHeader("Cookie", "SID=000cff38b8189b0c3b1b1cf8ce1dedcb6b3");
            request.SetRequestHeader("Content-Type", "application/json");

            // Bypass SSL certificate validation
            request.certificateHandler = new BypassCertificateHandler();

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            // Handle response
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}

// Custom Certificate Handler to bypass SSL validation
public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Bypass all certificate validation
        return true;
    }
}
