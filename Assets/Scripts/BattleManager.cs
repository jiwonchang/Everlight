using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    private bool battleActive;
    public GameObject battleScene;

    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;

    // we use a "List" here, since an "array" always has a fixed length, but we can have a variable number of combatants
    // if we use an array of size 9, for example, but then only have 5 combatants, we will need to check if in each slot there is actually a combatant (and not undefined).
    // this is because we will have filled the first 5 indices of the array, but the remaining 4 will be empty.
    // using a list, we get rid of this concern. this is because lists can have dynamic / flexible lengths
    public List<BattleChar> activeCombatants = new List<BattleChar>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // for testing purposes
        if (Input.GetKeyDown(KeyCode.T))
        {
            BattleStart(new string[] { "Eyeball", "Spider", "Skeleton" });
        }
    }

    public void BattleStart(string[] enemiesToSpawn)
    {
        if (!battleActive)
        {
            battleActive = true;

            // in game manager, set battle active = true so that the player cannot move
            GameManager.instance.battleActive = true;
            // make the battle scene (actually battle manager, in this case) spawn at the current camera position
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            // activate the battle scene
            battleScene.SetActive(true);
            // switch to battle music
            AudioManager.instance.PlayBGM(0);

            // load in the players heroes
            for (int i = 0; i < playerPositions.Length; i++)
            {
                // if player exists (i.e., is playable) in game, then load him/her in
                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy)
                {
                    for (int j = 0; j < playerPrefabs.Length; j++)
                    {
                        if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName)
                        {
                            BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                            // we do this in case we ever want to move a character around
                            // in such a case, we will have a reference to the player positions, so we can just move the playerPositions object, which will move the child as well
                            newPlayer.transform.parent = playerPositions[i];
                            // add the newly created battler to the list of active combatants
                            activeCombatants.Add(newPlayer);

                            CharacterStats playerStats = GameManager.instance.playerStats[i];
                            // set the stats of each new player/combatant here
                            activeCombatants[i].currentHP = playerStats.currentHP;
                            activeCombatants[i].maxHP = playerStats.maxHP;
                            activeCombatants[i].currentMP = playerStats.currentMP;
                            activeCombatants[i].maxMP = playerStats.maxMP;
                            activeCombatants[i].strength = playerStats.strength;
                            activeCombatants[i].defense = playerStats.defense;
                            activeCombatants[i].weaponPower = playerStats.weaponPower;
                            activeCombatants[i].armorPower = playerStats.armorPower;
                        }
                    }
                }
            }

            // load in the enemies
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i] != "") // just do a quick check to make sure the enemy is not blank/empty/undefined
                {
                    for (int j = 0; j < enemyPrefabs.Length; j++)
                    {
                        if (enemyPrefabs[j].charName == enemiesToSpawn[i])
                        {
                            BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                            // in case we need to ever move the enemy around
                            newEnemy.transform.parent = enemyPositions[i];
                            // add the newly created battler to the list of active combatants
                            activeCombatants.Add(newEnemy);
                        }
                    }
                }
            }
        }
    }
}
