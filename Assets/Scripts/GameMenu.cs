using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static GameMenu instance;
    public GameObject theMenu;
    public GameObject[] windows;

    private CharacterStats[] playerStats;
    public Text[] nameTexts, hpTexts, mpTexts, levelTexts, expTexts;
    public Slider[] expSliders;
    public Image[] charImages;
    public GameObject[] charStatHolder;
    // Used in the "Display Character Status" window
    public GameObject[] statusButtons;
    public Text statusName, statusHP, statusMP, statusStr, statusDef, statusWpnEquipped, statusWpnPower, statusArmorEquipped, statusArmorPower, statusExp;
    public Image statusImage;

    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item activeItem;
    public Text itemName, itemDescription, useButtonText;

    public GameObject itemCharacterChoiceMenu;
    public Text[] itemCharChoiceNames;
    public Text goldText;

    public string mainMenuName;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (theMenu.activeInHierarchy)
            {
                CloseMenu();
            } else
            {
                theMenu.SetActive(true);
                // when the game menu is opened, update the stats to be reflected by the menu
                UpdateMainStats();
                GameManager.instance.gameMenuOpen = true;
            }

            // play a sound when opening/closing our menu
            AudioManager.instance.PlaySFX(5);
        }
    }

    // function to update the characters' stats so it is reflected accurately in the menu
    public void UpdateMainStats()
    {
        playerStats = GameManager.instance.playerStats;

        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                charStatHolder[i].SetActive(true);
                nameTexts[i].text = playerStats[i].charName;
                hpTexts[i].text = "HP: " + playerStats[i].currentHP + " / " + playerStats[i].maxHP;
                mpTexts[i].text = "MP: " + playerStats[i].currentMP + " / " + playerStats[i].maxMP;
                expTexts[i].text = "" + playerStats[i].currentExp + " / " + playerStats[i].expToNextLevel[playerStats[i].charLevel];
                levelTexts[i].text = "Level: " + playerStats[i].charLevel;
                expSliders[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].charLevel];
                expSliders[i].value = playerStats[i].currentExp;
                charImages[i].sprite = playerStats[i].charImage;
            } else
            {
                // if the character is not active (e.g., maybe not met yet, dead, etc.), then disable their stats from showing in the Menu UI
                charStatHolder[i].SetActive(false);
            }
        }

        goldText.text = GameManager.instance.currentGold.ToString() + 'g';
    }

    public void ToggleWindow(int windowNumber)
    {
        UpdateMainStats();

        for (int i = 0; i < windows.Length; i++)
        {
            if (i == windowNumber)
            {
                windows[i].SetActive(!windows[i].activeInHierarchy);
            } else
            {
                windows[i].SetActive(false);
            }
        }
        
        // if we switch windows, then close the item usage on character choice menu
        itemCharacterChoiceMenu.SetActive(false);
    }

    public void CloseMenu()
    {
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(false);
        }

        theMenu.SetActive(false);
        GameManager.instance.gameMenuOpen = false;
        itemCharacterChoiceMenu.SetActive(false);
    }

    public void OpenStatus()
    {
        UpdateMainStats();

        // update the information that is shown
        DisplayCharStat(0);
        for (int i = 0; i < statusButtons.Length; i++)
        {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
            statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].charName;
        }
    }

    public void DisplayCharStat(int selected)
    {
        statusName.text = playerStats[selected].charName;
        statusHP.text = "" + playerStats[selected].currentHP + " / " + playerStats[selected].maxHP; 
        statusMP.text = "" + playerStats[selected].currentMP + " / " + playerStats[selected].maxMP; 
        statusStr.text = playerStats[selected].strength.ToString(); 
        statusDef.text = playerStats[selected].defense.ToString();
        if (playerStats[selected].equippedWeapon != "")
        {
            statusWpnEquipped.text = playerStats[selected].equippedWeapon;
        } else
        {
            statusWpnEquipped.text = "None";
        }
        statusWpnPower.text = playerStats[selected].weaponPower.ToString();
        if (playerStats[selected].equippedArmor != "")
        {
            statusArmorEquipped.text = playerStats[selected].equippedArmor;
        } else
        {
            statusArmorEquipped.text = "None";
        }
        statusArmorPower.text = playerStats[selected].armorPower.ToString();
        statusExp.text = (playerStats[selected].expToNextLevel[playerStats[selected].charLevel] - playerStats[selected].currentExp).ToString();
        statusImage.sprite = playerStats[selected].charImage;
    }

    public void ShowItems()
    {
        GameManager.instance.SortItems();

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].buttonValue = i;

            if (GameManager.instance.itemsHeld[i] != "")
            {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                itemButtons[i].amountText.text = "x" + GameManager.instance.numberOfItems[i];
            } else
            {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].amountText.text = "";
            }
        }
    }

    public void SelectItem(Item selectedItem)
    {
        activeItem = selectedItem;

        if (activeItem.isItem)
        {
            useButtonText.text = "Use";
        }
        else if (activeItem.isWeapon || activeItem.isArmor)
        {
            useButtonText.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;
    }

    public void DiscardItem()
    {
        if (activeItem != null)
        {
            GameManager.instance.RemoveItem(activeItem.itemName);
        }
    }

    public void OpenItemCharacterChoice()
    {
        itemCharacterChoiceMenu.SetActive(true);

        for (int i = 0; i < itemCharChoiceNames.Length; i++)
        {
            itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
            // disable / hide a character button if that character is not available
            itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void CloseItemCharacterChoice()
    {
        itemCharacterChoiceMenu.SetActive(false);
    }

    public void UseItem(int selectedChar)
    {
        activeItem.Use(selectedChar);
        CloseItemCharacterChoice();
    }

    public void SaveGame()
    {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void PlayButtonSound()
    {
        AudioManager.instance.PlaySFX(4);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(mainMenuName);
        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(gameObject);

        Destroy(GameObject.FindGameObjectWithTag("MainCamera"));
    }
}
