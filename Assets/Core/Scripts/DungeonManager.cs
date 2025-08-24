using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public GameObject floorPrefab, wallPrefab, tileSpawnerPrefab, exitPrefab;
    public int totalFloorCount;
    
    [HideInInspector] public float minX, maxX, minY, maxY;
    
    List<Vector3> floorList = new List<Vector3>();

    void Start()
    {
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
    }

    void ExitDoorway()
    {
        Vector3 doorPosition = floorList[floorList.Count - 1];
        GameObject goDoor = Instantiate(exitPrefab, doorPosition, Quaternion.identity);
        goDoor.name = exitPrefab.name;
        goDoor.transform.SetParent(transform);
    }
}
