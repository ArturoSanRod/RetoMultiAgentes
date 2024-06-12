/* 
COMMENT:
Este script crea el grid con prefabs cuadrados blancos y negros donde encima se posiciona la basura y el basurero
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    public GameObject tilePrefabWhite;
    public GameObject tilePrefabBlack;
    public GameObject trashPrefab;
    public GameObject trashBinPrefab;
    public WebClient webClient;

    // Aqui puse un diccionario para guardar los objetos de basura y asi poder rastrearlos y desactivarlos
    private Dictionary<Vector3, GameObject> trashPrefabs = new Dictionary<Vector3, GameObject>();

    void Start()
    {
        webClient.createGrid = this;
    }

    public void CreateTiles(List<WebClient.RobotPosition> robotPositions, List<WebClient.TrashPosition> trashPositions, WebClient.TrashBinPosition trashBinPosition)
    {
        int maxX = 0;
        int maxY = 0;

        foreach (var position in robotPositions)
        {
            if (position.x > maxX)
            {
                maxX = (int)position.x;
            }

            if (position.y > maxY)
            {
                maxY = (int)position.y;
            }
        }

        foreach (var position in trashPositions)
        {
            if (position.x > maxX)
            {
                maxX = (int)position.x;
            }

            if (position.y > maxY)
            {
                maxY = (int)position.y;
            }
        }

        maxX = Mathf.Max(maxX, (int)trashBinPosition.x);
        maxY = Mathf.Max(maxY, (int)trashBinPosition.y);

        for (int x = 0; x < maxX + 1; x++)
        {
            for (int y = 0; y < maxY + 1; y++)
            {
                Instantiate((x + y) % 2 == 0 ? tilePrefabWhite : tilePrefabBlack, new Vector3(x, y, 2), Quaternion.identity);
            }
        }

        foreach (var trash in trashPositions)
        {
            Vector3 trashPos = new Vector3(trash.x, trash.y, 0);
            if (!trashPrefabs.ContainsKey(trashPos))
            {
                GameObject trashObj = Instantiate(trashPrefab, trashPos, Quaternion.identity);
                trashPrefabs[trashPos] = trashObj;
            }
        }

        Instantiate(trashBinPrefab, new Vector3(trashBinPosition.x, trashBinPosition.y, 1), Quaternion.identity);
    }
}
