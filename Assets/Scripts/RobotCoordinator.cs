using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCoordinator : MonoBehaviour
{
    public GameObject[] robots;
    private CommunicationManager commManager;

    void Start()
    {
        commManager = GetComponent<CommunicationManager>();
        AssignNextTask();
    }

    void AssignNextTask()
    {
        foreach (GameObject robot in robots)
        {
            RobotController controller = robot.GetComponent<RobotController>();
            Vector3 nextTrashLocation = commManager.GetNextTrashLocation();
            if (nextTrashLocation != Vector3.zero)
            {
                controller.SetTargetPosition(nextTrashLocation);
                commManager.CollectTrash(nextTrashLocation);
            }
        }
    }
}
