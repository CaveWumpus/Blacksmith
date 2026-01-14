using UnityEngine;

public class ItemUI : MonoBehaviour
{
    public string itemName;
    public int requiredLevel;
    public MaterialRequirement[] requiredMaterials;

    [System.Serializable]
    public class MaterialRequirement
    {
        public string materialName;
        public int amount;
    }
}
