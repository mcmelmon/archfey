using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{

    Queue<GameObject> defenders = new Queue<GameObject>();
    readonly List<GameObject> deployed = new List<GameObject>();
    Dictionary<Ruins.Category, Circle> ruin_circles = new Dictionary<Ruins.Category, Circle>();
    List<GameObject> scouts = new List<GameObject>();
    List<GameObject> strikers = new List<GameObject>();
    List<GameObject> heavies = new List<GameObject>();


    // Unity


    private void Awake()
    {

    }


    private void Start()
    {

    }

    private void Update()
    {

    }


    // public


    public void Defend(Queue<GameObject> _defenders)
    {
        GameObject defense_parent = new GameObject();
        defense_parent.name = "Defend";
        defense_parent.AddComponent<Defend>();
        defense_parent.transform.parent = transform;
        defenders = _defenders;
        ruin_circles = GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetOrCreateRuinCircles();

        Deploy(defense_parent);
        FormUp();
    }


    public Dictionary<Ruins.Category, Circle> GetRuinCircles()
    {
        return ruin_circles;
    }
                

    // private


    private void Deploy(GameObject parent)
    {
        foreach (KeyValuePair<Ruins.Category, Circle> keyValue in ruin_circles)
        {
            switch (keyValue.Key)
            {
                case Ruins.Category.Primary:
                    for (int i = 0; i < 12; i++)
                    {
                        heavies.Add(Spawn(keyValue.Value.RandomContainedPoint(), parent.transform));
                    }
                    break;
                case Ruins.Category.Secondary:
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject _striker = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _striker.AddComponent<Striker>();
                        strikers.Add(_striker);
                    }
                    break;
                case Ruins.Category.Tertiary:
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject _scout = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _scout.AddComponent<Scout>();
                        scouts.Add(_scout);
                    }
                    break;
            }
        }
    }


    private void FormUp()
    {
        Formation strike_formation = Formation.CreateFormation(ruin_circles[Ruins.Category.Secondary].center, 10f, Formation.Profile.Square);
        foreach (var striker in strikers)
        {
            strike_formation.JoinFormation(striker);
        }
    }


    private GameObject Spawn(Vector3 point, Transform defense_parent)
    {
        GameObject _defender = defenders.Dequeue();
        _defender.transform.position = point;
        _defender.AddComponent<Defend>();
        _defender.transform.parent = defense_parent;
        _defender.SetActive(true);
        deployed.Add(_defender);
        return _defender;
    }
}