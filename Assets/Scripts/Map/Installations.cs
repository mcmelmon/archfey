using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Installations : MonoBehaviour {

    public Installation installation_prefab;
    public List<Installation> listing;
    public int target_number_of_installations;

    Map map;


    // Unity


    private void Awake()
    {
        map = transform.parent.transform.parent.GetComponent<Map>();
    }


    private void Start () 
    {
        PlaceInstallations();
	}
	

	private void Update () 
    {
		
	}


    // public


    public void PlaceInstallations()
    {
        int bail_safe = map.GetTerrain().width * map.GetTerrain().height * target_number_of_installations;
        int bail_count = 0;

        while (listing.Count < target_number_of_installations)
        {
            if (bail_count >= bail_safe) break;
            Tile _tile = map.GetTerrain().PickRandomInteriorTile();
            if (InstantiateInstallation(_tile))
            {
                listing.Add(_tile.installation);
            }
            bail_count++;
            continue;
        }
    }

    // private

    bool InstantiateInstallation(Tile tile)
    {
        if (tile.installation == null)
        {
            Installation _installation = Instantiate(installation_prefab, tile.transform.position + new Vector3(0,5,0), tile.transform.rotation, installation_prefab.transform);
            if (_installation != null)
            {
                tile.installation = _installation;
                _installation.transform.parent = transform;
                return true;
            }
        }

        return false;
    }
}