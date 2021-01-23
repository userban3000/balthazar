using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour {
    
    public int speed;
    public int units;

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        this.transform.position += transform.forward * speed / 100;
    }

    public void Set(int count, PlayerData pd) {
        speed = 4;
        units = count;
    }

    private void OnCollisionEnter(Collision col) {
        if ( col.gameObject.CompareTag("Star System") ) {
            
        } else if ( col.gameObject.CompareTag("Army") ) {

        } else {
            throw new System.Exception("Army hit something that's neither a Star System, nor another Army. How did you even manage?");
        }
    }

}
