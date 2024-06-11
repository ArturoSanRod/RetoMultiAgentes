using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Dimensions
{
    public int n;
    public int m;
}

[System.Serializable]
public class InitialData
{
    public Dimensions dimensions;
    public List<List<string>> terrain;
}

public class WebClient : MonoBehaviour
{
    public GameObject groundPrefab;
    public GameObject obstaclePrefab;
    public GameObject trashPrefab;
    public GameObject binPrefab;
    public GameObject robotPrefab;

    private void Start()
    {
        StartCoroutine(RequestInitialData());
    }

    IEnumerator RequestInitialData()
    {
        string url = "http://localhost:8585";
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.downloadHandler = new DownloadHandlerBuffer();
        www.uploadHandler = new UploadHandlerRaw(new byte[0]);
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            InitialData initialData = JsonUtility.FromJson<InitialData>(www.downloadHandler.text);
            GenerateTerrain(initialData);
        }
    }

    void GenerateTerrain(InitialData data)
    {
        for (int i = 0; i < data.dimensions.m; i++)
        {
            for (int j = 0; j < data.dimensions.n; j++)
            {
                Vector3 position = new Vector3(j, i, 0);
                string cell = data.terrain[i][j];

                if (cell == "X")
                {
                    Instantiate(obstaclePrefab, position, Quaternion.identity);
                }
                else if (cell == "P")
                {
                    Instantiate(binPrefab, position, Quaternion.identity);
                }
                else if (cell != "0")
                {
                    GameObject trash = Instantiate(trashPrefab, position, Quaternion.identity);
                }
                else
                {
                    Instantiate(groundPrefab, position, Quaternion.identity);
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            Vector3 robotPosition = new Vector3(i, 0, 0);
            Instantiate(robotPrefab, robotPosition, Quaternion.identity);
        }
    }
}
