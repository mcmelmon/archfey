using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Product : MonoBehaviour
{
    // Inspector settings

    [SerializeField] Products.Category category;
    [SerializeField] List<Components> recipe;
    [SerializeField] Proficiencies.Tool tool; // simplify process to use the "main" tool

    [Serializable]
    public struct Components {
        public GameObject item;
        public int quantity;
    }

    // properties

    public Item Item { get; set; }
    public List<GameObject> Materials { get; set; }
    public string Name { get; set; }
    public List<Components> Recipe { get; set; }
    public Proficiencies.Tool RequiredTool { get; set; }

    // Unity

    private void Awake()
    {
        SetComponents();
    }

    // public

    public bool SufficientMaterialsInStorage(Structure storage)
    {
        if (storage == null || storage.Inventory.StorageCount() <= 0) return false;

        foreach (var component in recipe) {
            if (storage.Inventory.StorageCount(component.item) < component.quantity) return false;
        }

        return true;
    }

    // private

    private void SetComponents()
    {
        Item = GetComponent<Item>();
        Materials = recipe.Select(r => r.item).ToList();
        Recipe = recipe;
        Name = this.name;
        RequiredTool = tool;
    }
}
