using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
   public GameObject tilePrefabwhite;
    public GameObject tilePrefabblack;
   public WebClient webClient;

   void Start()
   {
       webClient.createGrid = this;
   }

   public void CreateTiles(List<WebClient.RobotPosition> robotPositions)
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

         for (int x = 0; x < maxX; x++)
         {
                for (int y = 0; y < maxY; y++)
                {
                    Instantiate((x + y) % 2 == 0 ? tilePrefabwhite : tilePrefabblack, new Vector3(x, y, 2), Quaternion.identity);
                }
         }
   }
}
