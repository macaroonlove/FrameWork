using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Temporary.Core
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Transform _projectileHitPoint;
        protected int _id;

        internal Transform projectileHitPoint => _projectileHitPoint;
        internal int id => _id;

        private Dictionary<Type, AlwaysAbility> _alwaysAbilities = new Dictionary<Type, AlwaysAbility>();
        private Dictionary<Type, ConditionAbility> _conditionAbilities = new Dictionary<Type, ConditionAbility>();

        internal ConditionAbility currentAbility { get; private set; }

        internal void Initialize(Unit unit)
        {
            var alwaysAbilities = GetComponents<AlwaysAbility>();
            var conditionAbilities = GetComponents<ConditionAbility>();

            foreach (var ability in alwaysAbilities)
            {
                ability.Initialize(unit);
                _alwaysAbilities[ability.GetType()] = ability;
            }

            foreach (var ability in conditionAbilities)
            {
                ability.Initialize(unit);
                _conditionAbilities[ability.GetType()] = ability;
            }
        }

        internal void Deinitialize()
        {
            foreach (var ability in _alwaysAbilities.Values)
            {
                ability.Deinitialize();
            }
            foreach (var ability in _conditionAbilities.Values)
            {
                ability.Deinitialize();
            }
        }

        private void Update()
        {
            foreach (var ability in _alwaysAbilities.Values)
            {
                if (ability.useUpdate)
                {
                    ability.UpdateAbility();
                }
            }

            CheckAbilityState();

            if (currentAbility != null)
            {
                currentAbility.UpdateAbility();
            }
        }

        /// <summary>
        /// 현재 능력 상태 확인
        /// </summary>
        private void CheckAbilityState()
        {
            ConditionAbility newAbility = currentAbility;

            foreach (var ability in _conditionAbilities.Values)
            {
                if (ability == currentAbility) continue;

                // 해당 능력이 실행 가능한 상황이라면
                if (ability.IsExecute())
                {
                    // 기존 능력의 우선순위보다, 실행 가능한 능력의 우선순위가 높다면
                    if (newAbility == null || ability.priorty > newAbility.priorty)
                    {
                        newAbility = ability;
                    }
                }
            }

            if (newAbility != currentAbility)
            {
                if (currentAbility != null)
                {
                    currentAbility.StopAbility();
                }

                currentAbility = newAbility;

                currentAbility.StartAbility();
            }
        }

        internal void ReleaseCurrentAbility()
        {
            currentAbility = null;
        }

        public T GetAbility<T>() where T : Ability
        {
            if (_alwaysAbilities.TryGetValue(typeof(T), out var alwaysAbility))
            {
                return alwaysAbility as T;
            }

            if (_conditionAbilities.TryGetValue(typeof(T), out var conditionAbility))
            {
                return conditionAbility as T;
            }

            return null;
        }
    }
}

#if UNITY_EDITOR
namespace Temporary.Editor
{
    using Temporary.Core;
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Unit))]
    public class UnitEditor : Editor
    {
        private Unit _unit = null;
        private int _currentAbilityIndex = -1;

        // ConditionAbility와 AlwaysAbility 리스트 추가
        private List<ConditionAbility> _conditionAbilities = new List<ConditionAbility>();
        private List<AlwaysAbility> _alwaysAbilities = new List<AlwaysAbility>();
        private List<string> _abilityNames = new List<string>();

        private void OnEnable()
        {
            RenewalAbility();
            HideAbilities();

            _unit = target as Unit;

            _currentAbilityIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_currentAbilityIndex >= _abilityNames.Count)
            {
                _currentAbilityIndex = 0;
            }

            EditorGUILayout.Space();

            if (_unit != null && _unit.currentAbility != null)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.LabelField("현재 적용된 능력", _unit.currentAbility.GetType().Name);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();

            _currentAbilityIndex = EditorGUILayout.Popup("수정할 능력", _currentAbilityIndex, _abilityNames.ToArray());

            if (GUILayout.Button("Script", GUILayout.Width(50)))
            {
                OpenScript();
            }
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
            {
                AddAbilityMenu();
            }
            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                RemoveAbilityMenu();
            }

            EditorGUILayout.EndHorizontal();

            // 선택된 능력의 Inspector 표시
            if (_currentAbilityIndex != -1 && _currentAbilityIndex < _conditionAbilities.Count + _alwaysAbilities.Count)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                Ability currentAbility = _currentAbilityIndex < _conditionAbilities.Count
                    ? _conditionAbilities[_currentAbilityIndex]
                    : _alwaysAbilities[_currentAbilityIndex - _conditionAbilities.Count] as Ability;

                var editor = CreateEditor(currentAbility);
                editor.CreateInspectorGUI();
                editor.OnInspectorGUI();
                editor.serializedObject.ApplyModifiedProperties();

                EditorGUILayout.EndVertical();
            }
        }

        #region 스크립트 열기
        private void OpenScript()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(_currentAbilityIndex < _conditionAbilities.Count
                ? _conditionAbilities[_currentAbilityIndex]
                : _alwaysAbilities[_currentAbilityIndex - _conditionAbilities.Count]));

            if (!string.IsNullOrEmpty(scriptPath))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath));
            }
        }
        #endregion

        #region 능력 추가
        protected virtual void AddAbilityMenu()
        {
            GenericMenu menu = new GenericMenu();

            // 여기에 Ability 종류별 추가 가능성 제공 (예: ConditionAbility, AlwaysAbility)
            menu.AddItem(new GUIContent("ConditionAbility"), false, AddAbility, typeof(ConditionAbility));
            menu.AddItem(new GUIContent("AlwaysAbility"), false, AddAbility, typeof(AlwaysAbility));

            menu.ShowAsContext();
        }

        protected void AddAbility(object targetAbility)
        {
            // 능력 컴포넌트로 추가
            var ability = Undo.AddComponent((serializedObject.targetObject as MonoBehaviour).gameObject, targetAbility as Type) as Ability;

            // 추가한 능력 인스펙터에서 숨기기
            ability.hideFlags = HideFlags.HideInInspector;

            RenewalAbility();

            _currentAbilityIndex = _conditionAbilities.Contains(ability as ConditionAbility)
                ? _conditionAbilities.FindIndex(n => n == ability as ConditionAbility)
                : _conditionAbilities.Count + _alwaysAbilities.FindIndex(n => n == ability as AlwaysAbility);

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 능력 삭제
        private void RemoveAbilityMenu()
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < _conditionAbilities.Count; i++)
            {
                menu.AddItem(new GUIContent(_conditionAbilities[i].GetType().Name), false, RemoveAbility, _conditionAbilities[i]);
            }

            for (int i = 0; i < _alwaysAbilities.Count; i++)
            {
                menu.AddItem(new GUIContent(_alwaysAbilities[i].GetType().Name), false, RemoveAbility, _alwaysAbilities[i]);
            }

            menu.ShowAsContext();
        }

        private void RemoveAbility(object targetAbility)
        {
            var ability = targetAbility as Ability;
            Undo.DestroyObjectImmediate(ability);

            RenewalAbility();

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 보조 메서드들
        /// <summary>
        /// 능력 최신화
        /// </summary>
        private void RenewalAbility()
        {
            // 능력 비우기
            _conditionAbilities.Clear();
            _alwaysAbilities.Clear();
            _abilityNames.Clear();

            // 컴포넌트로 달려있는 모든 교환 가능한 능력들 추가
            var abilities = (serializedObject.targetObject as MonoBehaviour).GetComponents<Ability>();

            // 능력을 Condition과 Always로 구분해서 리스트에 추가
            foreach (var ability in abilities)
            {
                if (ability is ConditionAbility conditionAbility)
                {
                    _conditionAbilities.Add(conditionAbility);
                }
                else if (ability is AlwaysAbility alwaysAbility)
                {
                    _alwaysAbilities.Add(alwaysAbility);
                }
            }

            // 정렬 및 이름 목록 갱신
            _conditionAbilities.Sort((x, y) => x.priorty.CompareTo(y.priorty));

            _abilityNames.AddRange(_conditionAbilities.Select(element => element.GetType().Name));
            _abilityNames.AddRange(_alwaysAbilities.Select(element => element.GetType().Name));
        }

        /// <summary>
        /// 능력들 인스펙터에 보이게 하기
        /// </summary>
        private void ShowAbilities()
        {
            foreach (var ability in _conditionAbilities.Concat<Ability>(_alwaysAbilities))
            {
                if (ability != null)
                {
                    ability.hideFlags = HideFlags.None;
                }
            }
        }

        /// <summary>
        /// 능력들 인스펙터에서 숨기기
        /// </summary>
        private void HideAbilities()
        {
            foreach (var ability in _conditionAbilities.Concat<Ability>(_alwaysAbilities))
            {
                if (ability != null)
                {
                    ability.hideFlags = HideFlags.HideInInspector;
                }
            }
        }

        private void OnValidate()
        {
            RenewalAbility();
            HideAbilities();
        }

        private void OnDestroy()
        {
            ShowAbilities();
        }
        #endregion
    }
}
#endif