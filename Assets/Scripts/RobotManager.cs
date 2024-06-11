using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    public GameObject robotPrefab;
    public float speed = 2.0f;  // Añadir un parámetro de velocidad
    private Dictionary<int, GameObject> robots = new Dictionary<int, GameObject>();
    private Dictionary<int, Vector3> targetPositions = new Dictionary<int, Vector3>();

    public void UpdateRobotPositions(List<WebClient.RobotPosition> robotPositions)
    {
        foreach (var position in robotPositions)
        {
            if (!robots.ContainsKey(position.id))
            {
                GameObject newRobot = Instantiate(robotPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
                robots[position.id] = newRobot;
                targetPositions[position.id] = new Vector3(position.x, position.y, 0);
            }
            else
            {
                targetPositions[position.id] = new Vector3(position.x, position.y, 0);
            }
        }
    }

    void Update()
    {
        foreach (var robot in robots)
        {
            int id = robot.Key;
            GameObject robotObj = robot.Value;

            if (targetPositions.ContainsKey(id))
            {
                Vector3 targetPosition = targetPositions[id];
                robotObj.transform.position = Vector3.MoveTowards(robotObj.transform.position, targetPosition, speed * Time.deltaTime);
            }
        }
    }
}
