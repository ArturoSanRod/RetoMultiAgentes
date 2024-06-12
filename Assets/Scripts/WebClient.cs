using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class WebClient : MonoBehaviour
{
    public RobotManager robotManager;
    public CreateGrid createGrid;
    public TextMeshProUGUI stepsText; // Referencia al componente TextMeshProUGUI
    public TextMeshProUGUI completedText; // Referencia al componente TextMeshProUGUI

    [System.Serializable]
    public class RobotPosition
    {
        public int id;
        public float x;
        public float y;
    }

    [System.Serializable]
    public class TrashPosition
    {
        public float x;
        public float y;
        public int trash_amount;
    }

    [System.Serializable]
    public class TrashBinPosition
    {
        public float x;
        public float y;
    }

    [System.Serializable]
    public class PositionData
    {
        public List<RobotPosition> robots;
        public List<TrashPosition> trash;
        public TrashBinPosition trashbin;
        public int steps;
        public bool completed;
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
                PositionData positions = JsonUtility.FromJson<PositionData>(www.downloadHandler.text);
                robotManager.UpdateRobotPositions(positions.robots);
                createGrid.CreateTiles(positions.robots, positions.trash, positions.trashbin);

                // Actualizar el texto de pasos y si ya termino
                stepsText.text = "Steps: " + positions.steps;
                completedText.text = "Completed: " + positions.completed;
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
        if (Time.frameCount % 60 == 0) // Cada segundo se actualiza la información mandando un mensaje vacío
        {
            StartCoroutine(SendData("{}"));
        }
    }
}
