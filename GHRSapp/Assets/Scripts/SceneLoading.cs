using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    // Update is called once per frame
    public void LoadEarthquakeScene()
    {
        SceneManager.LoadScene("EarthquakeData");
    }

}
