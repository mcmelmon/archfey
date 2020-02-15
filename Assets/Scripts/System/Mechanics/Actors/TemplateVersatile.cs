using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateVersatile : TemplateMelee
{

    public override void OnFriendsInNeed()
    {
        Me.Actions.KeepEnemiesAtRange();
        Me.Actions.Attack();
    }


    public override void OnHostileActorsSighted()
    {
        Me.Actions.KeepEnemiesAtRange();
        Me.Actions.Attack();
    }


    public override void OnInCombat()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.KeepEnemiesAtRange();
        Me.Actions.Attack();
    }


    public override void OnUnderAttack()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.KeepEnemiesAtRange();
        Me.Actions.Attack();
        Me.RestCounter = 0;
    }


    public override void OnWatch()
    {
        Me.Actions.KeepEnemiesAtRange();
        Me.Actions.Attack();
    }
}
