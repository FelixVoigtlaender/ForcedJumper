using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public GameObject enemyPrefab;
    public GameObject floorPrefab;
    public int chunkHeight;
    public int chunkWidth;

    public int nextChunk = 0;

    List<GameObject> topChunk = new List<GameObject>();
    List<GameObject> middleChunk = new List<GameObject>();
    List<GameObject> bottomChunk = new List<GameObject>();

    public void Start()
    {
        GenerateTop(nextChunk - chunkHeight/4);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector2.up * (nextChunk + chunkHeight /2 ) , new Vector2(chunkWidth, chunkHeight));
    }

    public void FixedUpdate()
    {
        if(PlayerController.instance.transform.position.y < nextChunk)
        {
            KillChunk(topChunk);

            topChunk = middleChunk;
            middleChunk = bottomChunk;
            bottomChunk = GenerateChunk((int)nextChunk - chunkHeight);
            nextChunk -= chunkHeight;
            
        }
    }

    public List<GameObject> GenerateChunk(int yStart)
    {
        Vector2 topLeft = new Vector2(-chunkWidth / 2 + 0.5f, yStart);
        List<GameObject> boxes = new List<GameObject>();
        for(int x = 0; x < chunkWidth; x++)
        {
            for(int y = 0; y < chunkHeight; y++)
            {
                Vector2 position = topLeft + new Vector2(x, -y);

                if (Random.Range(0, 1f) < 0.5f)
                {
                    GameObject newBox = Instantiate(enemyPrefab, position, Quaternion.Euler(Vector2.zero));
                    //Health
                    newBox.GetComponent<Health>().health = Random.Range(0, 5);

                    boxes.Add(newBox);
                }
            }
        }
        return boxes;
    }
    public List<GameObject> GenerateTop(int yStart)
    {
        Vector2 topLeft = new Vector2(-chunkWidth / 2 + 0.5f, yStart);
        List<GameObject> boxes = new List<GameObject>();
        for (int x = 0; x < chunkWidth; x++)
        {

            Vector2 position = topLeft + new Vector2(x, yStart);
            GameObject newBox = Instantiate(enemyPrefab, position, Quaternion.Euler(Vector2.zero));

            boxes.Add(newBox);

        }
        return boxes;
    }


    public void KillChunk(List<GameObject> boxes)
    {
        foreach(GameObject ob in boxes)
        {
            Destroy(ob);
        }
        boxes.Clear();
    }
}
