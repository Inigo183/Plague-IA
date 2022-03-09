using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatorBehaviour : MonoBehaviour {
    public GameObject world;
    public GameObject human;
    public int high = 500, width = 500;
    
    private List<List<GameObject>> worldList;

    void Start() {
        worldList = GenerateWorld();
        Vector2 startPoint = RandomStart();
        Debug.Log(startPoint);
        Debug.Log(worldList);
    }
    
    void Update() {

    }

    private Vector2 RandomStart() {
        Vector2 point;
        point.x = Random.Range(0, 1000);
        point.y = Random.Range(0, 1000);
        return point;
    }

    private List<List<GameObject>> GenerateWorld() {
        List<List<GameObject>> worldList = new List<List<GameObject>>();
        for(int x = 0; x < width; x++) {
            List<GameObject> humans = new List<GameObject>();
            for (int y = 0; y < high; y++) {
                GameObject newHuman = Instantiate(human, new Vector3(0.1f*x - (0.1f*width/2), 0.1f*y - (0.1f*high/2), 0), Quaternion.identity);
                newHuman.transform.parent = world.transform;
                humans.Add(newHuman);
            }
            worldList.Add(humans);
        }
        return worldList;
    }
}
