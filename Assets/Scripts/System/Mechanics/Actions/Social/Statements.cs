using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statements : MonoBehaviour
{
    // Inspector

    [SerializeField] List<Statement> statements;

    // properties

    public Actor Me { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public Statement GiveStatementTo(Actor _listener)
    {
        Statement statement = null;

        return statement;
    }


    // private

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
    }
}
