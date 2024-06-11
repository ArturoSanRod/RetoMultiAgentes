using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class InitialData
{
    public Dimensions Dimensions;
    public List<List<string>> Terrain;
}

[System.Serializable]
public class Dimensions
{
    public int[] DimensionsArray;
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
            Debug.Log(initialData);
            if (initialData != null && initialData.Dimensions != null && initialData.Terrain != null)
            {
                Debug.Log(initialData.Dimensions.DimensionsArray.Length);
                GenerateTerrain(initialData);
            }
            else
            {
                Debug.LogError("InitialData or its properties are null");
            }
        }
        Debug.Log("Request completed");
    }

    void GenerateTerrain(InitialData data)
    {
        if (data.Dimensions.DimensionsArray.Length != 2)
        {
            Debug.LogError("Dimensions array does not have the correct length");
            return;
        }

        int height = data.Dimensions.DimensionsArray[0];
        int width = data.Dimensions.DimensionsArray[1];
        
        if (height != data.Terrain.Count)
        {
            Debug.LogError("Height dimension does not match the number of rows in Terrain");
            return;
        }

        for (int i = 0; i < height; i++)
        {
            if (data.Terrain[i].Count != width)
            {
                Debug.LogError($"Width dimension does not match the number of columns in row {i} of Terrain");
                return;
            }

            for (int j = 0; j < width; j++)
            {
                string cell = data.Terrain[i][j];
                Vector3 position = new Vector3(j, i, 0);

                if (cell == "X")
                {
                    Instantiate(obstaclePrefab, position, Quaternion.identity);
                    Debug.Log("Obstacle at " + position);
                }
                else if (cell == "P")
                {
                    Instantiate(binPrefab, position, Quaternion.identity);
                    Debug.Log("Bin at " + position);
                }
                else if (cell != "0" && cell != "X" && cell != "P")
                {
                    Instantiate(trashPrefab, position, Quaternion.identity);
                    Debug.Log("Trash at " + position + " with type " + cell);
                }
                else if (cell == "0")
                {
                    Instantiate(groundPrefab, position, Quaternion.identity);
                    Debug.Log("Ground at " + position);
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            Vector3 robotPosition = new Vector3(i, 0, 0);
            Instantiate(robotPrefab, robotPosition, Quaternion.identity);
            Debug.Log("Robot at " + robotPosition);
        }
    }
}
