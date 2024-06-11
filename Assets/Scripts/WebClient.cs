using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    public RobotManager robotManager;
    public CreateGrid createGrid;

    [System.Serializable]
    public class RobotPosition
    {
        public int id;
        public float x;
        public float y;
    }

    [System.Serializable]
    public class RobotPositionList
    {
        public List<RobotPosition> robots;
    }

    IEnumerator SendData(string data)
    {
        WWWForm form = new WWWForm();
        string url = "http://localhost:8585";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                RobotPositionList positions = JsonUtility.FromJson<RobotPositionList>("{\"robots\":" + www.downloadHandler.text + "}");
                    robotManager.UpdateRobotPositions(positions.robots);
                    createGrid.CreateTiles(positions.robots);
                
            }
        }
    }

    void Start()
    {
        // Simula el envío de datos iniciales si es necesario
        string json = "{}";
        StartCoroutine(SendData(json));
    }

    void Update()
    {
        // Puedes llamar a SendData periódicamente si quieres actualizar constantemente la posición de los robots
        if (Time.frameCount % 60 == 0) // Cada segundo
        {
            StartCoroutine(SendData("{}"));
        }
    }
}
