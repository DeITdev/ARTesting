using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Import TextMeshPro namespace

public class APIDirect_ECU : MonoBehaviour
{
    private const string apiUrl = "https://192.168.100.10/data/tags?iotag";
    private const string specificValueUrl = "https://192.168.100.10/data/tags/WISE4010:Red/value"; // Specific endpoint for CurrentInjector value
    private const string username = "admin";
    private const string password = "00000000";
    private const string cookie = "SID=000ce2c1757f454ab2964fdd13cf9688db9";
    private const string referer = "https://192.168.100.10"; // The referer URL

    // Reference to the TextMeshPro object
    public TextMeshProUGUI currentInjectorText;

    private void Start()
    {
        // Start the API call coroutine
        StartCoroutine(FetchDataFromApi());
    }

    private IEnumerator FetchDataFromApi()
    {
        while (true) // Repeat every second
        {
            yield return FetchCurrentInjectorValue(); // Fetch CurrentInjector specific value
            yield return new WaitForSeconds(1); // Wait for 1 second before the next fetch
        }
    }

    private IEnumerator FetchCurrentInjectorValue()
    {
        UnityWebRequest request = UnityWebRequest.Get(specificValueUrl);

        // Add Basic Authentication header
        string auth = $"{username}:{password}";
        string encodedAuth = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(auth));
        request.SetRequestHeader("Authorization", "Basic " + encodedAuth);

        // Add Cookie header
        request.SetRequestHeader("Cookie", cookie);

        // Add Referer header
        request.SetRequestHeader("Referer", referer);

        // Allow untrusted SSL certificate
        request.certificateHandler = new AcceptAllCertificatesHandler();

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Log the response data
            Debug.Log("Current Injector Value API Response: " + request.downloadHandler.text);
            // Update TextMeshPro object with the current injector value
            currentInjectorText.text = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError($"Error fetching CurrentInjector value: {request.error} - Status Code: {request.responseCode}");
        }
    }
}

// Custom Certificate Handler to accept all certificates (use only for testing)
public class AcceptAllCertificatesHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // Always accept
    }
}
