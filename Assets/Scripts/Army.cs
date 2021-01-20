using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour {
    
    public int speed;

    public int health;
    public int damage;

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        this.transform.position += Vector3.forward * speed / 100;
    }

    public void Set(int count, PlayerData pd) {

        speed = 2;
        health = 0;
        damage = 0;

    }

}
