using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Armors : MonoBehaviour
{
    public enum ArmorName { 
        Breastplate,
        Chain_Mail,
        Chain_Shirt,
        Half_Plate,
        Hide,
        Leather,
        None,
        Padded,
        Plate,
        Ring_Mail,
        Scale_Mail,
        Shield,
        Splint,
        Studded_Leather
    };

    public enum ArmorType { Light, Medium, Heavy };

    // Inspector settings

    public List<GameObject> armors;


    // properties

    public static Armors Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one armors instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public Armor GetArmorNamed(ArmorName name)
    {
        return armors.First(armor => armor.GetComponent<Armor>().armor_name == name).GetComponent<Armor>();
    }
}
