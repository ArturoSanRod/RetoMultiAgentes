using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationManager : MonoBehaviour
{
    private Dictionary<Vector3, int> trashLocations = new Dictionary<Vector3, int>();

    public void ReportTrash(Vector3 position, int amount)
    {
        if (!trashLocations.ContainsKey(position))
        {
            trashLocations[position] = amount;
        }
    }

    public Vector3 GetNextTrashLocation()
    {
        foreach (var location in trashLocations)
        {
            if (location.Value > 0)
            {
                return location.Key;
            }
        }
        return Vector3.zero;
    }

    public void CollectTrash(Vector3 position)
    {
        if (trashLocations.ContainsKey(position))
        {
            trashLocations[position]--;
        }
    }
}
