using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Conflict : MonoBehaviour
{

    public enum Alignment { LawfulGood, NeutralGood, ChaoticGood, LawfulNeutral, Neutral, ChaoticNeutral, LawfulEvil, NeutralEvil, ChaoticEvil, Unaligned };

    // properties

    public static Conflict Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one conflict instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public List<Alignment> AlignmentAntagonisms(Alignment alignment)
    {
        switch(alignment) {
            case Alignment.LawfulGood:
                return new List<Alignment>() { Alignment.LawfulEvil, Alignment.NeutralEvil, Alignment.ChaoticEvil, Alignment.ChaoticNeutral };
            case Alignment.NeutralGood:
                return new List<Alignment>() { Alignment.NeutralEvil, Alignment.ChaoticEvil };
            case Alignment.ChaoticGood:
                return new List<Alignment>() { Alignment.LawfulNeutral, Alignment.LawfulEvil, Alignment.NeutralEvil, Alignment.ChaoticEvil, Alignment.ChaoticNeutral };
            case Alignment.LawfulNeutral:
                return new List<Alignment>() { Alignment.NeutralEvil, Alignment.ChaoticEvil, Alignment.ChaoticNeutral, Alignment.ChaoticGood };
            case Alignment.Neutral:
                return new List<Alignment>();
            case Alignment.ChaoticNeutral:
                return new List<Alignment>() { Alignment.LawfulGood, Alignment.LawfulNeutral, Alignment.LawfulEvil };
            case Alignment.LawfulEvil:
                return new List<Alignment>() { Alignment.NeutralGood, Alignment.ChaoticGood, Alignment.ChaoticNeutral };
            case Alignment.NeutralEvil:
                return new List<Alignment>() { Alignment.LawfulNeutral, Alignment.LawfulGood, Alignment.NeutralGood, Alignment.ChaoticGood, Alignment.ChaoticNeutral };
            case Alignment.ChaoticEvil:
                return new List<Alignment>() { Alignment.LawfulNeutral, Alignment.LawfulGood, Alignment.NeutralGood, Alignment.ChaoticGood, Alignment.ChaoticNeutral, Alignment.Neutral, Alignment.LawfulEvil };
            default:
                return new List<Alignment>() { Alignment.LawfulNeutral, Alignment.LawfulGood, Alignment.LawfulEvil, Alignment.NeutralGood, Alignment.ChaoticGood, Alignment.ChaoticNeutral, Alignment.ChaoticEvil, Alignment.Neutral, Alignment.NeutralEvil, Alignment.Unaligned };
        }
    }


    // private


    private void CreateNavigationMesh()
    {
        GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }
}