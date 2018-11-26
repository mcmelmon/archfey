using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Installations : MonoBehaviour {

    public Installation installation_template;
    public List<Installation> listing;
    public Map map;
    public int target_number_of_installations;

    // Unity

    void Start () 
    {
        PlaceInstallations();
	}
	

	void Update () 
    {
		
	}


    // public


    public void PlaceInstallations()
    {
        int bail_safe = map.terrain.width * map.terrain.height * target_number_of_installations;
        int bail_count = 0;

        while (listing.Count < target_number_of_installations)
        {
            if (bail_count >= bail_safe) break;
            Tile _tile = map.terrain.PickRandomInteriorTile();
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
            Installation _installation = Instantiate(installation_template, tile.transform.position + new Vector3(0,5,0), tile.transform.rotation, installation_template.transform);
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