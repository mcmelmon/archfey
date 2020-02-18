using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Product : MonoBehaviour
{
    // Inspector settings

    [SerializeField] Products.Category category;
    [SerializeField] List<ComponentMaterial> recipe;
    [SerializeField] Proficiencies.Tool tool; // simplify process to use the "main" tool

    public struct ComponentMaterial {
        public Resource material;
        public int quantity;
    }

    // properties

    public Item Item { get; set; }
    public List<Resource> Materials { get; set; }
    public string Name { get; set; }
    public List<ComponentMaterial> Recipe { get; set; }
    public Proficiencies.Tool RequiredTool { get; set; }

    // Unity

    private void Awake()
    {
        SetComponents();
    }

    // public

    public bool SufficientMaterialsInStorage(Structure storage)
    {
        foreach (var component in recipe) {
            if (storage.Inventory.StorageCount(component.material.gameObject) < component.quantity) return false;
        }

        return true;
    }

    // private

    private void SetComponents()
    {
        Item = GetComponent<Item>();
        Materials = recipe.Select(r => r.material).ToList();
        Recipe = recipe;
        Name = this.name;
        RequiredTool = tool;
    }
}
