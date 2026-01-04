using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static reference so other scripts can easily access GameManager
    public static GameManager Instance;

    // Currency and progression
    public int gold = 0;
    public int reputation = 0;

    // Inventory: ore name → quantity
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    // Recipes unlocked
    public List<string> unlockedRecipes = new List<string>();

    // Awake runs before Start
    void Awake()
    {
        // If an Instance already exists, destroy duplicates
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // makes GameManager persistent
        }
        else
        {
            Destroy(gameObject); // prevents multiple GameManagers
        }
    }

    // Add ore to inventory
    public void AddOre(string oreName, int amount)
    {
        if (inventory.ContainsKey(oreName))
        {
            inventory[oreName] += amount;
        }
        else
        {
            inventory[oreName] = amount;
        }
    }

    // Spend gold
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    // Unlock a new recipe
    public void UnlockRecipe(string recipeName)
    {
        if (!unlockedRecipes.Contains(recipeName))
        {
            unlockedRecipes.Add(recipeName);
        }
    }
}
