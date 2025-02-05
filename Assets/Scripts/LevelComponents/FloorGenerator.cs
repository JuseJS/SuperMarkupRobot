using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
   [SerializeField] private GameObject floorPrefab;
   [SerializeField] private float minFloorHeight = 0.5f;

   public GameObject CreateFloor(Vector3 position, Vector2 dimensions, string floorName)
   {
       GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity);
       floor.transform.localScale = new Vector3(dimensions.x, minFloorHeight, dimensions.y);
       floor.name = floorName;
       return floor;
   }
}