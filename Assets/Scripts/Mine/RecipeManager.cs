using UnityEngine;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    private HashSet<RecipeDefinition> unlocked = new HashSet<RecipeDefinition>();

    void Awake() => Instance = this;

    public void UnlockRecipe(RecipeDefinition recipe)
    {
        if (unlocked.Add(recipe))
            Debug.Log("Unlocked recipe: " + recipe.tileName);
    }
}
