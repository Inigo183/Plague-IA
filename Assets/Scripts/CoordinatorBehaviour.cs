using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatorBehaviour : MonoBehaviour {
    [SerializeField] private GameObject world;
    [SerializeField] private GameObject cell;
    [SerializeField] private int high = 500, width = 500;

    private List<List<GameObject>> worldList;
    private List<GameObject> infectedCells = new List<GameObject>();

    private int contagious = 40;
    private int lethal = 10;

    void Start() {
        worldList = GenerateWorld();
        Vector2 startPoint = RandomStart();
        GameObject cell = worldList[(int)startPoint.x][(int)startPoint.y];
        cell.GetComponent<CellBehaviour>().Infect();
        infectedCells.Add(cell);
    }

    void Update() {
        for (int i = 0; i < infectedCells.Count; i++) {
            Action(infectedCells[i]);
        }
    }

    private Vector2 RandomStart() {
        /* Create a random point
         * Returns Vector2 point
         */
        Vector2 point;
        point.x = Random.Range(0, width);
        point.y = Random.Range(0, high);
        return point;
    }

    private List<List<GameObject>> GenerateWorld() {
        /* Create world list. Array 2d with all cells of the map
         * Returns thw world list
         */
        List<List<GameObject>> worldList = new List<List<GameObject>>();
        for (int x = 0; x < width; x++) {
            List<GameObject> cells = new List<GameObject>();
            for (int y = 0; y < high; y++) {
                float planeScale = cell.transform.localScale.x * 10f;
                GameObject newCell = Instantiate(
                    cell,
                    new Vector3(planeScale * x - (planeScale * width / 2), planeScale * y - (planeScale * high / 2), 0),
                    cell.transform.rotation
                );
                newCell.transform.parent = world.transform;
                newCell.GetComponent<CellBehaviour>().SetPosition(x, y);
                cells.Add(newCell);
            }
            worldList.Add(cells);
        }
        return worldList;
    }

    private void Action(GameObject cell) {
        /* Decide which action will execute program
         * Posible cases: Nothig
         *                Infectation
         *                Kill
         */
        int random = Random.Range(0, 100);
        if (random > (100 - contagious - lethal)) {
            if (random > 100 - lethal) {
                Kill(cell);
            }
            else {
                Infectation(cell);
            }
        }

        return;
    }

    private void Infectation(GameObject cell) {
        /* Add one infectation to, same cell or expand cells if at least half cell is infected
         */
        CellBehaviour cellBehaviour = cell.GetComponent<CellBehaviour>();
        int random = Random.Range(0, 5);
        // Decide if infect same cell or expand when half is infected
        if (random + cellBehaviour.infected < 10) {
            cellBehaviour.Infect();
        } else {
            // Calculate expansion cell
            int new_x = cellBehaviour.x + Random.Range(-1, 2);
            int new_y = cellBehaviour.y + Random.Range(-1, 2);
            // Check new cell is in map
            if (new_x >= 0 && new_x < width && new_y >= 0 && new_y < high){
                // Get cell to infect
                GameObject newCell = worldList[new_x][new_y];
                cellBehaviour = newCell.GetComponent<CellBehaviour>();
                // If it's first infection, add cell to infected list
                if (cellBehaviour.Infect() == 1) {
                    infectedCells.Add(newCell);
                }
            }
        }

        cellBehaviour.UpdateColor();
        return;
    }

    private void Kill(GameObject cell) {
        // Kill one human in cell
        CellBehaviour cellBehaviour = cell.GetComponent<CellBehaviour>();
        // If cell is totally dead or there is no more infected, remove form infected list
        if (cellBehaviour.Kill() == 1) {
            infectedCells.Remove(cell);
        }

        cellBehaviour.UpdateColor();
        return;
    }
}
