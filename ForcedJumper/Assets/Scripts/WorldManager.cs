using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public static int modulo = 2;

    public GameObject enemyPrefab;
    public GameObject floorPrefab;
    public int chunkHeight;
    public int chunkWidth;
    public int boxCountHorizontal = 2;

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
        float skinWidth = 0.05f;
        float boxSize = (float)(chunkWidth - skinWidth * 2 * boxCountHorizontal) / (float)boxCountHorizontal;
        float stepSize = boxSize + skinWidth * 2;
        Vector2 topLeft = new Vector2(-chunkWidth / 2 + stepSize / 2, yStart);
        List<GameObject> boxes = new List<GameObject>();
        int boxCountVertical = (int)(chunkHeight / stepSize);
        for(int x = 0; x < boxCountHorizontal; x++)
        {
            for(int y = 0; y < boxCountVertical; y++)
            {
                Vector2 position = topLeft + new Vector2(x, -y) * stepSize;

                if (Random.Range(0, 1f) < 0.5f)
                {
                    GameObject newBox = Instantiate(enemyPrefab, position, Quaternion.Euler(Vector2.zero));
                    newBox.transform.localScale = Vector3.one * boxSize;
                    //Health
                    newBox.GetComponent<Health>().health = Random.Range(0, modulo);

                    boxes.Add(newBox);
                }
            }
        }
        return boxes;
    }
    public List<GameObject> GenerateTop(int yStart)
    {
        float skinWidth = 0.05f;
        float boxSize = (float)(chunkWidth - skinWidth*2*boxCountHorizontal) / (float)boxCountHorizontal;
        float stepSize = boxSize + skinWidth*2;
        Vector2 topLeft = new Vector2(-chunkWidth / 2 + stepSize/2, yStart);
        List<GameObject> boxes = new List<GameObject>();
        for (int x = 0; x < boxCountHorizontal; x++)
        {

            Vector2 position = topLeft + Vector2.right * stepSize*x;
            GameObject newBox = Instantiate(enemyPrefab, position, Quaternion.Euler(Vector2.zero));
            newBox.transform.localScale = Vector3.one * boxSize;
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
