using UnityEngine;

public interface IAct
{
    void OnBadlyInjured();
    void OnCrafting();
    void OnFriendsInNeed();
    void OnFriendlyActorsSighted();
    void OnFullLoad();
    void OnDamagedFriendlyStructuresSighted();
    void OnHarvesting();
    void OnHasObjective();
    void OnHostileActorsSighted();
    void OnHostileStructuresSighted();
    void OnIdle();
    void OnInCombat();
    void OnMedic();
    void OnMovingToGoal();
    void OnNeedsRest();
    void OnReachedGoal();
    void OnUnderAttack();
    void OnWatch();
}
