using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public string charName;
    public int charLevel;
    public int currentExp;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    public int baseExp = 1000;

    public int currentHP;
    public int maxHP = 100;
    public int currentMP;
    public int maxMP = 50;

    public int strength;
    public int defense;
    public int weaponPower;
    public int armorPower;
    public string equippedWeapon;
    public string equippedArmor;
    public Sprite charImage;

    // Start is called before the first frame update
    void Start()
    {
        expToNextLevel = new int[maxLevel];
        expToNextLevel[1] = baseExp;
        for (int i = 2; i < expToNextLevel.Length; i++) {
            expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i-1] * 1.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.K))
        {
            AddExp(500);
        }
    }

    void AddExp(int expToAdd)
    {
        if (charLevel < maxLevel)
        {
            currentExp += expToAdd;
            if (currentExp >= expToNextLevel[charLevel]) {
                currentExp -= expToNextLevel[charLevel];
                charLevel++;

                // determine whether to add to str or def based on odd or even
                if (charLevel % 2 == 0)
                {
                    strength++;
                }
                else
                {
                    defense++;
                }

                maxHP = Mathf.FloorToInt(maxHP * 1.05f);
                currentHP = maxHP;
            }
        }

        if (charLevel >= maxLevel)
        {
            currentExp = 0;
        }
    }
}
