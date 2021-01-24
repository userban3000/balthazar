using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StarSystem : MonoBehaviour {
    
    [Header("System Stats")]
    public string systemName;
    public int units = 0;
    public float industryFactor = 1f;
    public float scienceFactor = 1f;

    [Header("Mapping")]
    public bool[] hasNeighbor = new bool[6];

    [Header("Unit Generation Properties")]
    public int industryPerTick = 0;
    public int industryToGenerateUnit = 20000;
    public int industry = 0;
    public Army armyPrefab;
    public int unitsToSend;

    [Header("System Properties")]
    public bool useTeamColoring;
    public int teamIndex;
    public int health;

    [Header("Effects")]
    public ParticleSystem particle;

    public void UpdateTeam(int newTeamIndex) {
        teamIndex = newTeamIndex;
        Material mat = GetComponentInChildren<Renderer>().material;
        mat.color = PlayerData.playerColors[teamIndex];
    }

    private void FixedUpdate() {
        Tick(); 
    }

    private void Tick () {
        industry += (int)(industryPerTick * industryFactor);
        
        if ( industry >= industryToGenerateUnit ) {
            industry = 0;
            units++;
        }
    }

}
