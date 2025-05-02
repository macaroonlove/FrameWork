using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Rarity", fileName = "Rarity", order = 0)]
    public class RarityTemplate : ScriptableObject
    {
        [SerializeField] private int _order;
        [SerializeField] private string _displayName;
        [SerializeField] private ERarity _rarity;
        [SerializeField] private Sprite _sprite;

        #region 프로퍼티
        public int order => _order;
        public string displayName => _displayName;
        public ERarity rarity => _rarity;
        public Sprite sprite => _sprite;
        #endregion
    }
}
