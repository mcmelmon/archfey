using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool is_hidden;
    public int spot_challenge_rating;

    public bool is_locked;
    public int unlock_challenge_rating;
    
    // properties

    public bool IsSpotted { get; set; }
    public bool IsUnlocked { get; set; }
}
