using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class InitialData
{
    public Dimensions dimensions;
    public List<List<string>> Terrain;
    public List<AgentPosition> AgentPositions;
}
[System.Serializable]
public class Dimensions
{
    public int[] DimensionsArray = new int[2];
}
[System.Serializable]
public class AgentPosition 
{
    public string type;
    public int x;
    public int y;
    public int z;
}

public class WebClient : MonoBehaviour
{
    public GameObject groundPrefab;
    public GameObject obstaclePrefab;
    public GameObject trashPrefab;
    public GameObject binPrefab;
    public GameObject agentPrefab;

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
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log("Received JSON: " + www.downloadHandler.text);
            InitialData initialData = JsonUtility.FromJson<InitialData>(www.downloadHandler.text);

            if (initialData != null && initialData.dimensions != null && initialData.Terrain != null)
            {
                GenerateTerrain(initialData);
            }
            else
            {
                Debug.LogError("InitialData or its properties are null");
            }
        }
    }

    void GenerateTerrain(InitialData data)
    {
        int height = data.dimensions.DimensionsArray[0];
        int width = data.dimensions.DimensionsArray[1];


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                string cell = data.Terrain[i][j];
                Vector3 position = new Vector3(j, i, 0);

                switch (cell)
                {
                    case "X":
                        Instantiate(obstaclePrefab, position, Quaternion.identity);
                        break;
                    case "P":
                        Instantiate(binPrefab, position, Quaternion.identity);
                        break;
                    case "0":
                        Instantiate(groundPrefab, position, Quaternion.identity);
                        break;
                    default:
                        Debug.LogError("Unknown cell type: " + cell);
                        break;
                }
            }
        }


        foreach (var agent in data.AgentPositions)
        {
            Vector3 agentPosition = new Vector3(agent.x, agent.y, agent.z);
            GameObject prefabToUse = agentPrefab; 

            switch (agent.type)
            {
                case "A":
                    prefabToUse = agentPrefab;
                    break;
                case "X":
                    prefabToUse = obstaclePrefab;
                    break;
                case "P":
                    prefabToUse = binPrefab;
                    break;
            }

            Instantiate(prefabToUse, agentPosition, Quaternion.identity);
        }
    }
}
