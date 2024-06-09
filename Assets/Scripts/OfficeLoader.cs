using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OfficeLoader : MonoBehaviour
{
    public GameObject emptyPrefab;
    public GameObject trashPrefab;
    public GameObject obstaclePrefab;
    public GameObject binPrefab;

    void Start()
    {
        LoadOffice("Assets/input.txt");
    }

    void LoadOffice(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        string[] dimensions = lines[0].Split(' ');
        int rows = int.Parse(dimensions[0]);
        int cols = int.Parse(dimensions[1]);

        for (int i = 1; i <= rows; i++)
        {
            string[] cells = lines[i].Split(' ');
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = new Vector3(j, 0, -i);
                char cell = cells[j][0];

                switch (cell)
                {
                    case '0':
                        Instantiate(emptyPrefab, position, Quaternion.identity, transform);
                        break;
                    case 'X':
                        Instantiate(obstaclePrefab, position, Quaternion.identity, transform);
                        break;
                    case 'P':
                        Instantiate(binPrefab, position, Quaternion.identity, transform);
                        break;
                    default:
                        Instantiate(trashPrefab, position, Quaternion.identity, transform);
                        break;
                }
            }
        }
    }
}
