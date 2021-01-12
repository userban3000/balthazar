using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSystemUI : MonoBehaviour {
    
    [Header("Star System")]
    StarSystem star;

    [Header("Text References")]
    public Text systemName;
    public Text unitCount;
    public Text industry;
    public Text indGen;
    public Text indNeeded;
    public Text Affiliation;

    public void Customize ( StarSystem receivedStar ) {
        star = receivedStar;
    }

    private void Update () {
        systemName.text = star.systemName;
        unitCount.text = star.units.ToString();
        industry.text = star.industry.ToString();
        indGen.text = (star.industryPerTick * star.industryFactor * 50).ToString();
        indNeeded.text = star.industryToGenerateUnit.ToString();
        Affiliation.text = star.teamIndex.ToString();
    }

}
