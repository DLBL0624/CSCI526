using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Skill
{
    void Apply(Component charUnit);
    string SkillName { get; set; }
    string Description { get; set; }
    bool StartTurn { get; set; }
    int RecentTurn { get; set; }
    int CoolDown { get; set; }
}
