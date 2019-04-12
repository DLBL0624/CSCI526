using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Skill
{
    void Apply(Component charUnit);
    void UnApply();
    void Spell(Component targetUnit);

    //Skill Description
    string SkillName { get; set; }
    string Description { get; set; }

    //CoolDown Check
    int StartTurn { get; set; }
    int RecentTurn { get; set; }
    int CoolDown { get; set; }
    bool Spellable { get; set; }

    //Valid Condition
    int TargetTeam { get; set; }
    behaviorStatus TargetBehaviour { get; set; }
    bool TargetItself { get; set; }
}
