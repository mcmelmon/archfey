using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Armors : MonoBehaviour
{
    public enum ArmorName { Padded, Leather, Studded_Leather, Hide, Chain_Shirt, Scale_Mail, Breastplate, Half_Plate, Ring_Mail, Chain_Mail, Splint, Plate, Shield, None };
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
