using UnityEngine;

public class WallGenerator : MonoBehaviour
{
   [SerializeField] private GameObject wallPrefab;
   [SerializeField] private float wallHeight = 5f;
   [SerializeField] private Vector2 defaultWallSize = new Vector2(2, 2);

   public GameObject CreateWall(Vector3 position, Vector3 scale, string wallName, Transform parent = null)
   {
       GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, parent);
       wall.transform.localScale = scale;
       wall.name = wallName;
       ConfigureWallMaterial(wall, scale);
       return wall;
   }

   public void CreateFloorWalls(Vector2 floorDimensions, float floorY, int floorNumber, Transform parent = null)
   {
       float halfWidth = floorDimensions.x / 2;
       float halfDepth = floorDimensions.y / 2;

       CreateWall(
           new Vector3(0, floorY, halfDepth), 
           new Vector3(floorDimensions.x, wallHeight, 1),
           $"Wall_{floorNumber}_North",
           parent
       );
       
       CreateWall(
           new Vector3(0, floorY, -halfDepth),
           new Vector3(floorDimensions.x, wallHeight, 1),
           $"Wall_{floorNumber}_South",
           parent
       );
       
       CreateWall(
           new Vector3(halfWidth, floorY, 0),
           new Vector3(1, wallHeight, floorDimensions.y),
           $"Wall_{floorNumber}_East",
           parent
       );
       
       CreateWall(
           new Vector3(-halfWidth, floorY, 0),
           new Vector3(1, wallHeight, floorDimensions.y),
           $"Wall_{floorNumber}_West",
           parent
       );
   }

   private void ConfigureWallMaterial(GameObject wall, Vector3 scale)
   {
       if (wall.TryGetComponent<Renderer>(out var renderer) && renderer.sharedMaterial != null)
       {
           Material wallMaterial = new Material(renderer.sharedMaterial);
           float textureRepeatX = Mathf.Max(scale.x, scale.z) / defaultWallSize.x;
           float textureRepeatY = scale.y / defaultWallSize.y;
           wallMaterial.mainTextureScale = new Vector2(textureRepeatX, textureRepeatY);
           renderer.material = wallMaterial;
       }
   }
}