using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : MonoBehaviour {

    public Ruin ruin_prefab;
    public List<Ruin> ruins;
    readonly Dictionary<string, Circle> ruin_circles = new Dictionary<string, Circle>();
    Geography geography;

    // Unity


    private void Awake()
    {
        geography = GetComponentInParent<Map>().GetComponentInChildren<Geography>();
    }


    private void Start () 
    {
        Place();
	}
	

	private void Update () 
    {
		
	}


    // public


    public Dictionary<string, Circle> GetOrCreateRuinCircles()
    {
        if (ruin_circles.Count == 0) Place();
        return ruin_circles;
    }


    public List<Vector3> GetRuinPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var ruin in ruins)
        {
            positions.Add(ruin.transform.position);
        }

        return positions;
    }


    public void Place()
    {
        Locate();
        Construct();
    }


    // private


    void Construct()
    {
        foreach (KeyValuePair<string, Circle> keyValue in ruin_circles)
        {
            switch (keyValue.Key) {
                case "primary":
                    for (int i = 0; i < 9; i++)
                    {
                        Vector3 position = keyValue.Value.RandomVertex();
                        if (!NearRuin(position, Ruin.minimum_ruin_proximity))
                            InstantiateRuin(position, this);
                    }
                    break;
                case "secondary":
                    for (int i = 0; i < 5; i++)
                    {
                        Vector3 position = keyValue.Value.RandomVertex();
                        if (!NearRuin(position, Ruin.minimum_ruin_proximity))
                            InstantiateRuin(position, this);
                    }
                    break;
                case "tertiary":
                    for (int i = 0; i < 2; i++)
                    {
                        Vector3 position = keyValue.Value.RandomVertex();
                        if (!NearRuin(position, Ruin.minimum_ruin_proximity))
                            InstantiateRuin(position, this);
                    }
                    break;
            }
        }
    }


    void InstantiateRuin(Vector3 point, Ruins _ruins)
    {
        Ruin _ruin = Instantiate(ruin_prefab, point, transform.rotation, _ruins.transform);
        _ruin.transform.localScale += new Vector3(4, 16, 4);
        _ruin.transform.position += new Vector3(0, _ruin.transform.localScale.y / 2, 0);
        if (_ruin != null) ruins.Add(_ruin);
    }


    private void Locate()
    {
        if (geography == null) geography = GetComponentInParent<Map>().GetComponentInChildren<Geography>();
        LocatePrimaryRuinComplex();
        LocateSecondaryRuinComplex();
        LocateTertiaryRuinComplex();
    }


    private void LocatePrimaryRuinComplex()
    {
        Circle spawn_circle = new Circle();
        float distance_from_edge = 100f;
        Vector3 circle_center = geography.RandomLocation(distance_from_edge);

        ruin_circles["primary"] = spawn_circle.Inscribe(circle_center, 40f);
    }


    private void LocateSecondaryRuinComplex()
    {
        Circle spawn_circle = new Circle();
        float distance_from_edge = 80f;
        Vector3 circle_center = geography.RandomLocation(distance_from_edge);

        ruin_circles["secondary"] = spawn_circle.Inscribe(circle_center, 20f);
    }


    private void LocateTertiaryRuinComplex()
    {
        Circle spawn_circle = new Circle();
        float distance_from_edge = 80f;
        Vector3 circle_center = geography.RandomLocation(distance_from_edge);

        ruin_circles["tertiary"] = spawn_circle.Inscribe(circle_center, 12f);
    }


    private bool NearRuin(Vector3 position, float how_close)
    {
        foreach (var ruin in GetRuinPositions())
        {
            float distance = Vector3.Distance(position, ruin);
            if (distance < how_close) return true;
        }

        return false;
    }


    private bool NearRuinCircle(Vector3 position, float how_close)
    {
        foreach (KeyValuePair<string, Circle> keyValue in ruin_circles)
        {
            float distance = Vector3.Distance(position, keyValue.Value.center);
            if (distance < how_close) return true;
        }

        return false;
    }
}