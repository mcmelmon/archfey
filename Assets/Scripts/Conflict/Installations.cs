using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Installations : MonoBehaviour {

    public Installation installation_prefab;
    public List<Installation> listing;
    public int target_number_of_installations;

    Map map;
    Terrain terrain;
    TerrainData terrain_data;


    // Unity


    private void Awake()
    {
        map = transform.GetComponentInParent<Map>();
        terrain = map.GetComponentInChildren<Terrain>();
        terrain_data = terrain.terrainData;
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
        for (int i = 0; i <= target_number_of_installations; i++)
        {
            int _w = Random.Range(0, terrain_data.heightmapResolution);
            int _d = Random.Range(0, terrain_data.heightmapResolution);
            float _h = terrain.SampleHeight(new Vector3(_d, 0, _w));
            InstantiateInstallation(_w, _h, _d, this);
        }
    }

    // private

    void InstantiateInstallation(int _w, float _h, int _d, Installations _installations)
    {
        if (NoInstallationWithinExclusionZone(_w, _h, _d))
        {
            Installation _installation = Instantiate(installation_prefab, new Vector3(_w, _h + 2, _d), transform.rotation, _installations.transform);
            _installation.transform.localScale += new Vector3(4, 36, 4);
            if (_installation != null) listing.Add(_installation);
        }
    }


    bool NoInstallationWithinExclusionZone(int _w, float _h, int _d)
    {
        return true;
    }
}