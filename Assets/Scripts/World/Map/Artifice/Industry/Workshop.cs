using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Workshop : MonoBehaviour
{
    // Inspector settings
    [SerializeField] List<Proficiencies.Tool> shop_tools = new List<Proficiencies.Tool>();

    // properties

    public Structure Structure { get; set; }

    // Unity

    private void Awake()
    {
        SetComponents();
    }

    // public

    public bool CraftByArtisan(Actor _artisan)
    {
        // For cases where we are inside of a settlement and want to simulate the movement/production of artisans

        if (Products.Instance.AvailableProducts.Count == 0 || Industry.Instance.CurrentlyCrafting.Contains(_artisan)) return false;
        Product product = null;

        foreach (var tool in _artisan.Stats.Tools) {
            // TODO: pick "most valuable" product/tool available to artisan
            product = Products.Instance.AvailableProducts.First(cw => cw.RequiredTool == tool);
            if (product != null) break;
        }

        if (product == null) return false;

        if (product.SufficientMaterialsInStorage(Structure)) {
            Industry.Instance.CurrentlyCrafting.Add(_artisan);
            StartCoroutine(Craft(product, _artisan));
            return true;
        }

        return false;
    }

    public bool CraftByShop(Product _product)
    {
        // For when we want to "just make stuff" as the resources are added to storage associated with the workshop
        // Used primarily by the Industry singleton

        if (_product.SufficientMaterialsInStorage(Structure)) {
            StartCoroutine(Craft(_product));
            return true;
        }

        return false;
    }

    public bool UsefulTo(Actor _artisan)
    {
        foreach (var tool in shop_tools) {
            if (_artisan.Stats.Tools.Contains(tool)) {
                return true;
            }
        }

        return false;
    }

    // private

    private IEnumerator Craft(Product _product, Actor _artisan = null)
    {
        foreach (Product.Components component in _product.Recipe) {
            Structure.Inventory.RemoveFromInventory(component.item, component.quantity);
        }

        int turn = 0;
        float time_to_finish = 1 + _product.GetComponent<Item>().base_cost_cp / 100f;


        while (turn < time_to_finish) {
            turn++;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }

        if (_artisan != null) Industry.Instance.CurrentlyCrafting.Remove(_artisan);
        Structure.Inventory.AddToInventory(_product.gameObject);
    }

    private void SetComponents()
    {
        Structure = GetComponent<Structure>();
    }
}
