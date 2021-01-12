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
    public int systemID;
    public StarSystem[] Neighbors;

    [Header("Unit Generation Properties")]
    public int industryPerTick = 1;
    public int industryToGenerateUnit = 1000;
    public int industry = 0;

    [Header("System Properties")]
    public int teamIndex;

    private void Awake() {
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
