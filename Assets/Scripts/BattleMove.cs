using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// we remove MonoBehavior
// this means that we can't add this into Unity as a component
// Instead, we add in System.Serializable, we CAN make this a list of object-like properties in our Battle Manager
[System.Serializable]
public class BattleMove
{

    public string moveName;
    public int movePower;
    public int moveCost;
    public AttackEffect moveEffect;

}
