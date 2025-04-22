using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Unit/Skin", fileName = "Skin_Battle_", order = 1)]
    public class SkinBattleTemplate : ScriptableObject
    {
        public GameObject prefab;

        [Header("∫∏¿ÃΩ∫")]
        public AudioClip voice_Spawn;
        public AudioClip voice_Death;
    }
}