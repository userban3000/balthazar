using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour {
    
    public int teamIndex;
    public int speed;
    public int units;

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        this.transform.position += transform.forward * speed / 100;
    }

    public void Set(int count, int pid) {
        //ignores the hover colliders
        Physics.IgnoreLayerCollision(10, 11);
        teamIndex = pid;
        speed = 4;
        units = count;
    }

    private void OnCollisionEnter(Collision col) {
        if ( col.gameObject.CompareTag("Star System") ) {
            StarSystem sys = col.gameObject.GetComponentInParent<StarSystem>();
            int sysTeam = sys.teamIndex;

            //if self-owned
            if ( sysTeam == teamIndex ) {
                Debug.Log(units + " friendly units arrived at " + sys.systemName);
                sys.units += units;
                Destroy(this.gameObject);
            } else {
                Debug.Log(units + " units attacked the enemy-controlled system " + sys.systemName);
                sys.units -= units;
                Destroy(this.gameObject);
            }

            return;
        } 
        
        if ( col.gameObject.CompareTag("Army") ) {
            
            return;
        }
        
    }

}
