using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RelicLoadoutUI : MonoBehaviour
{
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject relicSlotPrefab;

    private List<RelicDefinition> owned => RelicManager.Instance.ownedRelics;
    private List<RelicDefinition> equipped => RelicManager.Instance.equippedRelics;

    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        foreach (var relic in owned)
        {
            var slot = Instantiate(relicSlotPrefab, slotParent);
            slot.GetComponent<Image>().sprite = relic.icon;

            Button btn = slot.GetComponent<Button>();
            btn.onClick.AddListener(() => ToggleEquip(relic));
        }
    }

    private void ToggleEquip(RelicDefinition relic)
    {
        if (RelicManager.Instance.relicsLocked)
            return;

        if (equipped.Contains(relic))
            equipped.Remove(relic);
        else
            equipped.Add(relic);

        // Refresh UI if needed
    }
}
