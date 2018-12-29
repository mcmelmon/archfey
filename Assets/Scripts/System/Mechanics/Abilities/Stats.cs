using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public Actor Actor { get; set; }
    public int AgilityRating { get; set; }
    public int ConstitutionRating { get; set; }
    public int IntellectRating { get; set; }
    public int StrengthRating { get; set; }
    public int WillRating { get; set; }

    private void OnValidate()
    {
        if (AgilityRating > 10) AgilityRating = 10;
        if (AgilityRating < 0) AgilityRating = 0;

        if (ConstitutionRating > 10) ConstitutionRating = 10;
        if (ConstitutionRating < 0) ConstitutionRating = 0;

        if (IntellectRating > 10) IntellectRating = 10;
        if (IntellectRating < 0) IntellectRating = 0;

        if (StrengthRating > 10) StrengthRating = 10;
        if (StrengthRating < 0) StrengthRating = 0;

        if (WillRating > 10) WillRating = 10;
        if (WillRating < 0) WillRating = 0;
    }
}
