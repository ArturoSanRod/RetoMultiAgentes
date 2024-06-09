using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private StepCounter stepCounter;

    void Start()
    {
        stepCounter = FindObjectOfType<StepCounter>();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (transform.position == targetPosition)
        {
            isMoving = false;
            stepCounter.IncrementStep();
        }
    }
}