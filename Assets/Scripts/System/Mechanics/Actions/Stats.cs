using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public Actor Actor { get; set; }
    public int CharismaProficiency { get; set; }
    public int DexterityProficiency { get; set; }
    public int ConstitutionProficiency { get; set; }
    public int IntelligenceProficiency { get; set; }
    public int StrengthProficiency { get; set; }
    public int WisdomProficiency { get; set; }

    private void OnValidate()
    {
        if (CharismaProficiency > 10) CharismaProficiency = 10;
        if (CharismaProficiency < 0) CharismaProficiency = 0;

        if (DexterityProficiency > 10) DexterityProficiency = 10;
        if (DexterityProficiency < 0) DexterityProficiency = 0;

        if (ConstitutionProficiency > 10) ConstitutionProficiency = 10;
        if (ConstitutionProficiency < 0) ConstitutionProficiency = 0;

        if (IntelligenceProficiency > 10) IntelligenceProficiency = 10;
        if (IntelligenceProficiency < 0) IntelligenceProficiency = 0;

        if (StrengthProficiency > 10) StrengthProficiency = 10;
        if (StrengthProficiency < 0) StrengthProficiency = 0;

        if (WisdomProficiency > 10) WisdomProficiency = 10;
        if (WisdomProficiency < 0) WisdomProficiency = 0;
    }
}
