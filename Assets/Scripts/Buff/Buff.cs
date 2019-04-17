using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Buff
{
    void Apply(Component charUnit);
    void UnApply();
    string BuffName { get; set; }
    string Description { get; set; }
    bool FinishTurn { get; set; }
    int RecentTurn { get; set; }
}