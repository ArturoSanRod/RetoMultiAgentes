/*
COMMENT: Este script gestiona a los prefabs de los 5 robots, y los mueve. Además desactiva la basura cuando el robot la recoge
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    public GameObject robotPrefab;
    public float speed = 2.0f;  // velocidad
    private Dictionary<int, GameObject> robots = new Dictionary<int, GameObject>(); // diccionario de robots
    private Dictionary<int, Vector3> targetPositions = new Dictionary<int, Vector3>(); // diccionario de posiciones
    public CreateGrid createGrid;

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

                // Si el robot llega a la posición de la basura, desactivar la basura
                if (robotObj.transform.position == targetPosition)
                {
                    DeactivateTrashAtPosition(targetPosition);
                }
            }
        }
    }

    void DeactivateTrashAtPosition(Vector3 position) 
    {
        GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("Trash");
        foreach (GameObject trash in trashObjects)
        {   
            if (trash.transform.position == position)
            {
                trash.SetActive(false);
            }
        }
    }
}
