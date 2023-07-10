using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMagicSelect : MonoBehaviour
{
    public string spellName;
    public int spellCost;
    public Text nameText;
    public Text costText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Press()
    {
        if (BattleManager.instance.activeCombatants[BattleManager.instance.currentTurn].currentMP >= spellCost)
        {
            BattleManager.instance.magicMenu.SetActive(false);
            BattleManager.instance.OpenTargetMenu(spellName);
            BattleManager.instance.activeCombatants[BattleManager.instance.currentTurn].currentMP -= spellCost;
            if (BattleManager.instance.activeCombatants[BattleManager.instance.currentTurn].currentMP < 0)
            {
                BattleManager.instance.activeCombatants[BattleManager.instance.currentTurn].currentMP = 0;
            }
        } else
        {
            // let player know they don't have enough MP for the spell
        }
    }
}