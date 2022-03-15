using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatorBehaviour : MonoBehaviour {
    [SerializeField] private GameObject world;
    [SerializeField] private GameObject cell;
    [SerializeField] public int high = 500, width = 500;

    private List<List<GameObject>> worldList;
    private List<GameObject> infectedCells = new List<GameObject>();
    private List<GameObject> curedCells = new List<GameObject>();

    private int contagious = 0;
    private int lethal = 0;
    private int virusResistance = 0;

    private int cureLevel = 10;
    private int cureAdvance = 20;


    Vector2 virusPoint, curePoint;
    public float cureVirusDistance;
    void Start() {
        worldList = GenerateWorld();
        Vector2 VirusStartPoint = RandomStart();
        GameObject cell = worldList[(int)VirusStartPoint.x][(int)VirusStartPoint.y];
        cell.GetComponent<CellBehaviour>().Infect();
        infectedCells.Add(cell);

        Vector2 CureStartPoint = RandomStart();
        cell = worldList[(int)CureStartPoint.x][(int)CureStartPoint.y];
        cell.GetComponent<CellBehaviour>().SetCured();
        curedCells.Add(cell);
        cell.GetComponent<CellBehaviour>().UpdateColor();
        virusPoint = VirusStartPoint;
        curePoint = CureStartPoint;
        cureVirusDistance = Mathf.Sqrt(Mathf.Pow(VirusStartPoint.x - CureStartPoint.x, 2) + Mathf.Pow(VirusStartPoint.y - CureStartPoint.y, 2));
    }

    void Update() {
        for (int i = 0; i < infectedCells.Count; i++) {
            Action(infectedCells[i]);
        }

        for (int i = 0; i < curedCells.Count; i++) {
            CureAction(curedCells[i]);
        }
        cureLevel = (100 * curedCells.Count) / (high * width);
    }

    public void AddContagious(int increase) {
        contagious += increase;
    }

    public void AddLethal(int increase) {
        lethal += increase;
    }

    public void AddVirusResistance(int increase) {
        virusResistance += increase;
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
        WorldParameters worldParams = GameObject.Find("World").GetComponent<WorldParameters>();
        int random = Random.Range(0, worldParams.humansPerCell/2+1);
        // Decide if infect same cell or expand when half is infected
        if (virusResistance > cureLevel && cellBehaviour.cured) {
            cellBehaviour.cured = false;
            curedCells.Remove(cell);
        }
        if (random + cellBehaviour.infected < worldParams.humansPerCell) {
            cellBehaviour.Infect();
        } else {
            // Calculate expansion cell
            int new_x = cellBehaviour.x + Random.Range(-1, 2);
            int new_y = cellBehaviour.y + Random.Range(-1, 2);
            // Check new cell is in map
            if (new_x >= 0 && new_x < width && new_y >= 0 && new_y < high){
                // Get cell to infect
                GameObject newCell = worldList[new_x][new_y];
                CellBehaviour newCellBehaviour = newCell.GetComponent<CellBehaviour>();
                // If it's first infection, add cell to infected list
                if (newCellBehaviour.cured && virusResistance < cureLevel) {
                    return;
                }
                if (newCellBehaviour.Infect() == 1) {
                    infectedCells.Add(newCell);
                    //Get minimal distance
                    float distance = Mathf.Sqrt(Mathf.Pow(newCellBehaviour.x - curePoint.x, 2) + Mathf.Pow(newCellBehaviour.y - curePoint.y, 2));
                    if (distance < cureVirusDistance) {
                        cureVirusDistance = distance;
                        Vector2 newVirusPoint;
                        newVirusPoint.x = newCellBehaviour.x;
                        newVirusPoint.y = newCellBehaviour.y;
                        virusPoint = newVirusPoint;
                    }
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

    private void CureAction(GameObject cell) {
        CellBehaviour cellBehaviour = cell.GetComponent<CellBehaviour>();

        int random = Random.Range(0, 100);
        if (random > 100 - cureAdvance) {
            // Calculate expansion cell
            int new_x = cellBehaviour.x + Random.Range(-1, 2);
            int new_y = cellBehaviour.y + Random.Range(-1, 2);
            if (new_x >= 0 && new_x < width && new_y >= 0 && new_y < high) {
                // Get cell to infect
                GameObject newCell = worldList[new_x][new_y];
                CellBehaviour newCellBehaviour = newCell.GetComponent<CellBehaviour>();
                if (newCellBehaviour.cured == false && cellBehaviour.GetStatus() != 2 ) {
                    newCellBehaviour.SetCured();
                    curedCells.Add(newCell);
                    newCellBehaviour.UpdateColor();
                    //Get minimal distance
                    float distance = Mathf.Sqrt(Mathf.Pow(virusPoint.x - newCellBehaviour.x, 2) + Mathf.Pow(virusPoint.y - newCellBehaviour.y, 2));
                    if (distance < cureVirusDistance) {
                        cureVirusDistance = distance;
                        Vector2 newCurePoint;
                        newCurePoint.x = newCellBehaviour.x;
                        newCurePoint.y = newCellBehaviour.y;
                        curePoint = newCurePoint;
                    }
                }
            }
        }
    }
    
    public List<int> GetWorldStatus() {
        /* 0: no cure, no virus
         * 1: virus
         * 2: dead
         * 3: cure
         */
        List<int> worldStatus = new List<int>();

        for(int x = 0; x < worldList.Count; x++) {
            for(int y = 0; y < worldList[x].Count; y++) {
                worldStatus.Add(worldList[x][y].GetComponent<CellBehaviour>().GetStatus());
            }
        }

        return worldStatus;
    }
}
