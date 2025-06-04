using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;


public class EarthquakeDataFetcher : MonoBehaviour
{
    [Header("UI Elements")]
    // variable earthqakeText type TextMeshProUGUI that will be "storing" the data
    public TextMeshProUGUI earthquakeText;
    // assigns content from scroll view
    public RectTransform contentTransform;
    public ScrollRect scrollRect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Multiple functions are automatically run
        StartCoroutine(FetchEarthquakeData());
        
    }

    // IEnumerator stops all the background codes and runs this function first
    IEnumerator FetchEarthquakeData()
    {
        string url = "https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=2022-01-01&endtime=2022-12-31&minmagnitude=2&maxlatitude=15&minlatitude=13&maxlongitude=-88&minlongitude=-92";

        // using only in this function
        using(UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            // asks if request has gone though
            // if either the web cannot communicate or if cannot receive data  
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                // log error = defines what is wrong and define what that additional error is
                Debug.LogError("Error Fetching Earthquake Data" + request.error);
                // Displays text to users
                DisplayError("Error Fetching Earthquake Data");

            }
            else
            {
                string json = request.downloadHandler.text;
                ProcessEarthquakeData(json);
            }
            
        }
    }

    void ProcessEarthquakeData(string json)
    {
        EarthquakeData data = JsonUtility.FromJson<EarthquakeData>(json);
        if(data == null || data.features == null || data.features.Length == 0)
        {
            DisplayError("No Significant Activity Recorded");
            return;
        }

        // text will be displayed as bold
        string displayText = "<b>Report</b>\n";
        foreach(Feature feature in data.features)
        {
            string location = !string.IsNullOrEmpty(feature.properties.place) ? feature.properties.place : "Unknown Location";
            string magnitude = feature.properties.mag >= 0 ? feature.properties.mag.ToString("F1") : "Not Available";
            string time = feature.properties.time > 0 ? System.DateTimeOffset.FromUnixTimeMilliseconds(feature.properties.time).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") : "Unknown Time"; 
        
            displayText += $"\n<b>Location: </b> {location}\n";
            displayText += $"<b>Magnitude: </b> {magnitude}\n";
            displayText += $"<b>Time: </b> {time}\n";
        }

        UpdateUIText(displayText);
    }

    // displaying processed information
    void UpdateUIText(string text)
    {
        if (earthquakeText != null)
        {
            earthquakeText.text = text;
            StartCoroutine(AdjustContentSize());
        }
    }

    void DisplayError(string errorMessage)
    {
        if (earthquakeText != null)
        {
            earthquakeText.text = errorMessage;
            StartCoroutine(AdjustContentSize());
        }
    }

    IEnumerator AdjustContentSize()
    {
        yield return new WaitForEndOfFrame();

        float textHeight = earthquakeText.preferredHeight;
        float scrollViewHeight = scrollRect.GetComponent<RectTransform>().rect.height;

        // Force content to be either as tall as the text or at least slightly larger than the viewport
        float buffer = 10f; // Ensures overflow exists for scrolling
        float finalHeight = Mathf.Max(textHeight, scrollViewHeight + buffer);

        contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, finalHeight);

        Debug.Log($"Text: {textHeight}, Viewport: {scrollViewHeight}, Final Height: {finalHeight}");
    }


}
// Create a new class outside of C
[System.Serializable]
public class EarthquakeData
{
    public Feature[] features;
}

[System.Serializable]
public class Feature
{
    public Properties properties;
}

[System.Serializable]
public class Properties
{
    public string place;
    public float mag;
    public long time;
}
