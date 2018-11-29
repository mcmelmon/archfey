using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : MonoBehaviour {

    public Ruin ruin_prefab;
    public List<Ruin> ruins;

    Map map;
    Geography geography;
    Terrain terrain;
    TerrainData terrain_data;

    Dictionary<string, Circle> ruin_circles = new Dictionary<string, Circle>();


    // Unity


    private void Awake()
    {
        map = GetComponentInParent<Map>();
        geography = GetComponentInParent<Map>().GetComponentInChildren<Geography>();
        terrain = GetComponentInParent<Map>().GetComponentInChildren<Terrain>();
        terrain_data = terrain.terrainData;
    }


    private void Start () 
    {
        PlaceRuins();
	}
	

	private void Update () 
    {
		
	}


    // public


    public void PlaceRuins()
    {
        LocatePrimaryRuinComplex();
        LocateSecondaryRuinComplex();
        LocateTertiaryRuinComplex();
        ConstructRuins();
    }


    // private


    void ConstructRuins()
    {
        foreach (KeyValuePair<string, Circle> keyValue in ruin_circles)
        {
            switch (keyValue.Key) {
                case "primary":
                    for (int i = 0; i < 9; i++)
                    {
                        InstantiateRuin(keyValue.Value.RandomContainedPoint(), this);
                    }
                    break;
                case "secondary":
                    for (int i = 0; i < 5; i++)
                    {
                        InstantiateRuin(keyValue.Value.RandomContainedPoint(), this);
                    }
                    break;
                case "tertiary":
                    for (int i = 0; i < 2; i++)
                    {
                        InstantiateRuin(keyValue.Value.RandomContainedPoint(), this);
                    }
                    break;
            }
        }
    }


    void InstantiateRuin(Vector3 point, Ruins _ruins)
    {
        Ruin _ruin = Instantiate(ruin_prefab, point, transform.rotation, _ruins.transform);
        _ruin.transform.localScale += new Vector3(4, 36, 4);
        _ruin.transform.position += new Vector3(0, _ruin.transform.localScale.y / 2, 0);
        if (_ruin != null) ruins.Add(_ruin);
    }


    public void LocatePrimaryRuinComplex()
    {
        Circle spawn_circle = new Circle();
        Vector3 edge_point = geography.RandomBorderLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), .5f, true);

        ruin_circles["primary"] = spawn_circle.Inscribe(circle_center, 40f);
    }


    public void LocateSecondaryRuinComplex()
    {
        Circle spawn_circle = new Circle();
        Vector3 edge_point = geography.RandomBorderLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), .3f, true);

        ruin_circles["secondary"] = spawn_circle.Inscribe(circle_center, 20f);
    }


    public void LocateTertiaryRuinComplex()
    {
        Circle spawn_circle = new Circle();
        Vector3 edge_point = geography.RandomBorderLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), .1f, true);

        ruin_circles["tertiary"] = spawn_circle.Inscribe(circle_center, 12f);
    }
}