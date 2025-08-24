using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public GameObject[] randomItems;
    public GameObject floorPrefab, wallPrefab, tileSpawnerPrefab, exitPrefab;
    [Range(50, 5000)] public int totalFloorCount;
    [Range(0, 100)] public int itemSpawnPercent;
    
    [HideInInspector] public float minX, maxX, minY, maxY;
    
    List<Vector3> floorList = new List<Vector3>();
    LayerMask floorMask;
    LayerMask wallMask;

    void Start()
    {
        floorMask = LayerMask.GetMask("Floor");
        wallMask = LayerMask.GetMask("Wall");
        RandomWalker();
    }

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void RandomWalker()
    {
        Vector3 currentPosition = Vector3.zero;
        floorList.Add(currentPosition);
        while (floorList.Count < totalFloorCount)
        {
            switch (Random.Range(1, 5))
            {
                case 1:
                    currentPosition += Vector3.up;
                    break;
                case 2:
                    currentPosition += Vector3.right;
                    break;
                case 3:
                    currentPosition += Vector3.down;
                    break;  
                case 4:
                    currentPosition += Vector3.left;
                    break;
            }

            bool inFloorList = false;
            for (int i = 0; i < floorList.Count; i++)
            {
                if (Vector3.Equals(currentPosition, floorList[i]))
                {
                    inFloorList = true;
                    break;
                }
            }

            if (!inFloorList)
            {
                floorList.Add(currentPosition);
            }
        }

        for (int i = 0; i < floorList.Count; i++)
        {
            GameObject goTile = Instantiate(tileSpawnerPrefab, floorList[i], Quaternion.identity);
            goTile.name = tileSpawnerPrefab.name;
            goTile.transform.SetParent(transform);
        }
        // After all the tiles are instantiated, place an exit doorway
        StartCoroutine(DelayProgress());
    }

    IEnumerator DelayProgress()
    {
        while (FindObjectsOfType<TileSpawner>().Length > 0)
        {
            yield return null;
        }

        ExitDoorway();
        Vector2 hitSize = Vector2.one * 0.8f;

        for (int x = (int)minX - 2; x <= (int)maxX + 2; x++)
        {
            for (int y = (int)minY - 2; y <= (int)maxY + 2; y++)
            {
                Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(x, y), hitSize, 0, floorMask);
                if (hitFloor)
                {
                    // so its not the same position as the exit doorway
                    if (!Vector2.Equals(hitFloor.transform.position, floorList[floorList.Count - 1]))
                    {
                        Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, 0, wallMask);
                        Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, 0, wallMask);
                        Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, 0, wallMask);
                        Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, 0, wallMask);
                        
                        RandomItems(hitFloor, hitTop, hitRight, hitBottom, hitLeft);
                    }
                }
            }
        }
    }

    void RandomItems(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft)
    {
        // if there is a wall on any one of our sides,
        // and there is no walls opposing each other on the top and bottom,
        // and there is no walls opposing each other on the left and right
        if ((hitTop || hitRight || hitBottom || hitLeft) && !(hitTop && hitBottom) && !(hitRight && hitLeft))
        {
            int roll = Random.Range(0, 100);
            if (roll <= itemSpawnPercent)
            {
                int itemIndex = Random.Range(0, randomItems.Length);
                GameObject goItem = Instantiate(randomItems[itemIndex], hitFloor.transform.position, Quaternion.identity);
                goItem.name = randomItems[itemIndex].name;
                goItem.transform.SetParent(hitFloor.transform);
            }
        }
    }

    void ExitDoorway()
    {
        Vector3 doorPosition = floorList[floorList.Count - 1];
        GameObject goDoor = Instantiate(exitPrefab, doorPosition, Quaternion.identity);
        goDoor.name = exitPrefab.name;
        goDoor.transform.SetParent(transform);
    }
}
