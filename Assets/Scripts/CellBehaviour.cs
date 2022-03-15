using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour {
    // worldList indeces
    public int x, y;
    public bool cured;

    // Number of humans in each state;
    public int infected, dead;
    private int healthy, humans;

    void Start() {
        WorldParameters worldParams = GameObject.Find("World").GetComponent<WorldParameters>();

        infected = 0;
        dead = 0;
        healthy = worldParams.humansPerCell;
        humans = worldParams.humansPerCell;
    }

    public void SetPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public int GetStatus() {
        /*
         * 0 -> Nobody is infected
         * 1 -> At least 1 human is infected
         * 2 -> All humans are dead
         * 3 -> is cured
         */
        if (infected == 0 && healthy > 0) {
            return 0;
        }

        if (infected > 0) {
            return 1;
        }

        if (cured) {
            return 3;
        }
        return 2;
    }

    public int Kill() {
        /* Kill an human, possible cases:
         * A human was successfully killed, return 1
         * A human was successfully killed an no humans remains alive, return 0
         */
        if (dead == humans) {
            // this function is just to prevent that the ammount of dead humans
            // remains the same in case they are all dead and the function is called unexpectedly
            return 1;
        }
        dead++;
        infected--;
        WorldParameters worldParams = GameObject.Find("World").GetComponent<WorldParameters>();
        worldParams.totalDead++;
        worldParams.totalInfected--;
        if (dead == humans || infected == 0) {
            return 1;
        }

        return 0;
    }

    public int Infect() {
        // Infect an human, if all humans were already infected, nothing happens (return 2)
        // If the human is the first to be infected, return 1, if not, return 0
        if (infected == humans || healthy == 0) {
            return 2;
        }

        int firstInfected = 0;
        if (infected == 0) {
            firstInfected = 1;
        }

        infected++;
        healthy--;
        WorldParameters worldParams = GameObject.Find("World").GetComponent<WorldParameters>();
        worldParams.totalInfected++;
        return firstInfected;
    }

    public void UpdateColor() {
        Material material = gameObject.GetComponent<MeshRenderer>().material;
        int deadValue, infectedValue;
        if (cured) {
            deadValue = 0;
            infectedValue = 255;
        } else {
            deadValue = 255 - 255 / humans * dead;
            infectedValue = 255 - 255 / humans * infected;
            int excess = infectedValue - (255 - deadValue);
            if (excess > 0) {
                infectedValue = excess;
            } else {
                infectedValue = 0;
            }
        }
        material.SetColor("_Color", new Color32((byte)deadValue, (byte)infectedValue, (byte)infectedValue, (byte)1));
    }

    public void SetCured() {
        cured = true;
        infected = 0;
    }
}
