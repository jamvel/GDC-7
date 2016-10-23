using UnityEngine;
using System.Collections;

public class Magic {
    [HideInInspector] public string id;
    [HideInInspector] public int level;
    [HideInInspector] public int power;
    [HideInInspector] public int cost;
    [HideInInspector]public int damage;

    public Magic(int level , int cost , int power){
        this.id = id;
        this.level = level;
        this.cost = cost;
        this.damage = damage;
    }

}
