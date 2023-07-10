using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // keep track of whose turn it is
    public int currentTurn;
    // to be used while we're waiting for a turn to end (either waiting for input from player, or for enemy to execute their turn)
    public bool turnWaiting;
    public GameObject uiButtonsHolder;

    public BattleMove[] movesList;
    public GameObject enemyAttackEffect; // enemy turn indicator; shows whose turn it is that is attacking
    
    public DamageNumber damageNumberDisplay;

    public Text[] playerName, playerHP, playerMP;

    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;

    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;

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

        // handle what should happen at every turn
        if (battleActive)
        {
            if (turnWaiting)
            {
                // if player's turn, display battle menu buttons
                if (activeCombatants[currentTurn].isPlayer)
                {
                    uiButtonsHolder.SetActive(true);
                } else
                {
                    // if not player, hide the buttons
                    uiButtonsHolder.SetActive(false);

                    // enemy should attack (or otherwise execute their turn)
                    StartCoroutine(EnemyMoveCoroutine());
                }
            }

            // for testing purposes
            if (Input.GetKeyDown(KeyCode.N))
            {
                NextTurn();
            }
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

            // when the battle starts, we should immediately have it be someone's turn
            turnWaiting = true;
            // currentTurn = 0;
            currentTurn = Random.Range(0, activeCombatants.Count); // just for testing purposes, pick randomly
        
            UpdateUIStats();
        }
    }

    public void NextTurn()
    {
        currentTurn++;
        if (currentTurn >= activeCombatants.Count)
        {
            currentTurn = 0;
        }

        turnWaiting = true;
        // as we go to next turn, update the battle information
        UpdateBattle();
        UpdateUIStats();
    }

    // whenever we go to next turn, we should confirm that the battle should still continue (e.g., both sides are still not defeated)
    // also, we should update any information to be displayed
    public void UpdateBattle()
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeCombatants.Count; i++)
        {
            // first, check that none of the active combatants have any weird stat values (e.g., health below 0)
            if (activeCombatants[i].currentHP < 0)
            {
                activeCombatants[i].currentHP = 0;
            }

            // handle dead combatants
            if (activeCombatants[i].currentHP == 0)
            {
                // nothing for now
            } else
            {
                if (activeCombatants[i].isPlayer)
                {
                    allPlayersDead = false;
                } else
                {
                    allEnemiesDead = false;
                }
            }
        }

        // if either side is completely defeated, handle battle end
        if (allEnemiesDead || allPlayersDead)
        {
            if (allEnemiesDead)
            {
                // end battle in victory
            } else
            {
                // end battle in defeat
            }

            battleScene.SetActive(false);
            GameManager.instance.battleActive = false;
            battleActive = false;
        } else // even if battle is not over, make sure that only live combatants get turns
        {
            while (activeCombatants[currentTurn].currentHP == 0)
            {
                currentTurn++; // skip to next turn, as long as we encounter dead combatants
                if (currentTurn >= activeCombatants.Count)
                {
                    currentTurn = 0;
                }
            }
        }
    }

    // note: a coroutine (ienumerator) is something that can happen outside the normal order of things in Unity.
    // e.g., can run without completely blocking Unity's other processes
    public IEnumerator EnemyMoveCoroutine()
    {
        turnWaiting = false;
        // wait 1 second
        yield return new WaitForSeconds(1f);
        // then, call enemy attack function
        EnemyAttack();
        // then, wait 1 second
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack()
    {
        // first, pick which player to attack
        List<int> players = new List<int>();
        for (int i = 0; i < activeCombatants.Count; i++)
        {
            if (activeCombatants[i].isPlayer && activeCombatants[i].currentHP > 0)
            {
                players.Add(i);
            }
        }
        int selectedTarget = players[Random.Range(0, players.Count)]; // for now, choose among players randomly
        
        // second, apply damage to the targeted player
        // activeCombatants[selectedTarget].currentHP -= 30;

        int movePower = 0;
        // at least for now, pick a random move out of the moves available
        int selectAttack = Random.Range(0, activeCombatants[currentTurn].movesAvailable.Length);
        for (int i = 0; i < movesList.Length; i++)
        {
            if (movesList[i].moveName == activeCombatants[currentTurn].movesAvailable[selectAttack])
            {
                // spawn the attack effect where the selected attack target is
                Instantiate(movesList[i].moveEffect, activeCombatants[selectedTarget].transform.position, activeCombatants[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeCombatants[currentTurn].transform.position, activeCombatants[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);
    }

    public void DealDamage(int target, int movePower)
    {
        float attackPower = activeCombatants[currentTurn].strength + activeCombatants[currentTurn].weaponPower;
        float defensePower = activeCombatants[target].defense + activeCombatants[target].armorPower;

        float damageCalculation = (attackPower / defensePower) * movePower * Random.Range(0.9f, 1.1f);
        int damageToGive = Mathf.RoundToInt(damageCalculation);

        Debug.Log(activeCombatants[currentTurn].charName + " is dealing " + damageCalculation + " (" + damageToGive + ") points of damage to " + activeCombatants[target].charName);

        activeCombatants[target].currentHP -= damageToGive;
        if (activeCombatants[target].currentHP < 0)
        {
            activeCombatants[target].currentHP = 0;
        }

        // display the damage being done to the target
        Instantiate(damageNumberDisplay, activeCombatants[target].transform.position, activeCombatants[target].transform.rotation).SetDamage(damageToGive);
        UpdateUIStats();
    }

    public void UpdateUIStats()
    {
        for (int i = 0; i < playerName.Length; i++)
        {
            if (activeCombatants.Count > i)
            {
                if (activeCombatants[i].isPlayer)
                {
                    BattleChar playerData = activeCombatants[i];

                    playerName[i].gameObject.SetActive(true);
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
                } else
                {
                    playerName[i].gameObject.SetActive(false);
                }
            } else
            {
                playerName[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(string moveName, int selectedTarget)
    {
        int movePower = 0;
        for (int i = 0; i < movesList.Length; i++)
        {
            if (movesList[i].moveName == moveName)
            {
                // spawn the attack effect where the selected attack target is
                Instantiate(movesList[i].moveEffect, activeCombatants[selectedTarget].transform.position, activeCombatants[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeCombatants[currentTurn].transform.position, activeCombatants[currentTurn].transform.rotation);
        DealDamage(selectedTarget, movePower);

        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);

        NextTurn();
    }

    public void OpenTargetMenu(string moveName)
    {
        targetMenu.SetActive(true);

        List<int> Enemies = new List<int>();
        for (int i = 0; i < activeCombatants.Count; i++)
        {
            if (!activeCombatants[i].isPlayer)
            {
                Enemies.Add(i);
            }
        }

        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (Enemies.Count > i)
            {
                targetButtons[i].gameObject.SetActive(true);
                targetButtons[i].moveName = moveName;
                targetButtons[i].activeCombatantTarget = Enemies[i];
                targetButtons[i].targetName.text = activeCombatants[Enemies[i]].charName;
            } else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenMagicMenu()
    {
        magicMenu.SetActive(true);

        for (int i = 0; i < magicButtons.Length; i++)
        {
            if (activeCombatants[currentTurn].movesAvailable.Length > i)
            {
                magicButtons[i].gameObject.SetActive(true);
                magicButtons[i].spellName = activeCombatants[currentTurn].movesAvailable[i];
                magicButtons[i].nameText.text = magicButtons[i].spellName;
                
                // this is super inefficient... TODO: convert to dictionary lookup
                for (int j = 0; j < movesList.Length; j++)
                {
                    if (movesList[j].moveName == magicButtons[i].spellName)
                    {
                        magicButtons[i].spellCost = movesList[j].moveCost;
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                    }
                }
            } else
            {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
