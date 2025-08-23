using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    private DungeonManager dungeonManager;

    private void Awake()
    {
        dungeonManager =  FindObjectOfType<DungeonManager>();
        GameObject goFloor = Instantiate(dungeonManager.floorPrefab, transform.position, Quaternion.identity);
        goFloor.name = dungeonManager.floorPrefab.name;
        goFloor.transform.SetParent(dungeonManager.transform);
        if (transform.position.x > dungeonManager.maxX)
        {
            dungeonManager.maxX = transform.position.x;
        }
        if (transform.position.x < dungeonManager.minX)
        {
            dungeonManager.minX = transform.position.x;
        }
        if (transform.position.y > dungeonManager.maxY)
        {
            dungeonManager.maxY = transform.position.y;
        }
        if (transform.position.y < dungeonManager.minY)
        {
            dungeonManager.minY = transform.position.y;
        }
    }

    void Start()
    {
        LayerMask envMask = LayerMask.GetMask("Walls", "Floor");
        Vector2 hitSize = Vector2.one * 0.8f;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2 targetPos = new Vector2(transform.position.x + x, transform.position.y + y);
                Collider2D hit = Physics2D.OverlapBox(targetPos, hitSize, 0, envMask);
                if (!hit)
                {
                    // add a wall
                    GameObject goWall = Instantiate(dungeonManager.wallPrefab, targetPos, Quaternion.identity);
                    goWall.name = dungeonManager.wallPrefab.name;
                    goWall.transform.SetParent(dungeonManager.transform);
                }
            }
        }
        
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
