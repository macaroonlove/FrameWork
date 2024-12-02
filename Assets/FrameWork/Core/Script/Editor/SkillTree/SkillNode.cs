using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SkillNode : Node {
    [Output] public int level;
    [HideInInspector] public int index;
    [Input(dynamicPortList = true)] public List<int> prevSkill;
}