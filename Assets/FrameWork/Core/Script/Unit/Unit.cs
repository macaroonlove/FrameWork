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
        /// ���� �ɷ� ���� Ȯ��
        /// </summary>
        private void CheckAbilityState()
        {
            ConditionAbility newAbility = currentAbility;

            foreach (var ability in _conditionAbilities.Values)
            {
                if (ability == currentAbility) continue;

                // �ش� �ɷ��� ���� ������ ��Ȳ�̶��
                if (ability.IsExecute())
                {
                    // ���� �ɷ��� �켱��������, ���� ������ �ɷ��� �켱������ ���ٸ�
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

        // ConditionAbility�� AlwaysAbility ����Ʈ �߰�
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

                EditorGUILayout.LabelField("���� ����� �ɷ�", _unit.currentAbility.GetType().Name);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();

            _currentAbilityIndex = EditorGUILayout.Popup("������ �ɷ�", _currentAbilityIndex, _abilityNames.ToArray());

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

            // ���õ� �ɷ��� Inspector ǥ��
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

        #region ��ũ��Ʈ ����
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

        #region �ɷ� �߰�
        protected virtual void AddAbilityMenu()
        {
            GenericMenu menu = new GenericMenu();

            // ���⿡ Ability ������ �߰� ���ɼ� ���� (��: ConditionAbility, AlwaysAbility)
            menu.AddItem(new GUIContent("ConditionAbility"), false, AddAbility, typeof(ConditionAbility));
            menu.AddItem(new GUIContent("AlwaysAbility"), false, AddAbility, typeof(AlwaysAbility));

            menu.ShowAsContext();
        }

        protected void AddAbility(object targetAbility)
        {
            // �ɷ� ������Ʈ�� �߰�
            var ability = Undo.AddComponent((serializedObject.targetObject as MonoBehaviour).gameObject, targetAbility as Type) as Ability;

            // �߰��� �ɷ� �ν����Ϳ��� �����
            ability.hideFlags = HideFlags.HideInInspector;

            RenewalAbility();

            _currentAbilityIndex = _conditionAbilities.Contains(ability as ConditionAbility)
                ? _conditionAbilities.FindIndex(n => n == ability as ConditionAbility)
                : _conditionAbilities.Count + _alwaysAbilities.FindIndex(n => n == ability as AlwaysAbility);

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region �ɷ� ����
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

        #region ���� �޼����
        /// <summary>
        /// �ɷ� �ֽ�ȭ
        /// </summary>
        private void RenewalAbility()
        {
            // �ɷ� ����
            _conditionAbilities.Clear();
            _alwaysAbilities.Clear();
            _abilityNames.Clear();

            // ������Ʈ�� �޷��ִ� ��� ��ȯ ������ �ɷµ� �߰�
            var abilities = (serializedObject.targetObject as MonoBehaviour).GetComponents<Ability>();

            // �ɷ��� Condition�� Always�� �����ؼ� ����Ʈ�� �߰�
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

            // ���� �� �̸� ��� ����
            _conditionAbilities.Sort((x, y) => x.priorty.CompareTo(y.priorty));

            _abilityNames.AddRange(_conditionAbilities.Select(element => element.GetType().Name));
            _abilityNames.AddRange(_alwaysAbilities.Select(element => element.GetType().Name));
        }

        /// <summary>
        /// �ɷµ� �ν����Ϳ� ���̰� �ϱ�
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
        /// �ɷµ� �ν����Ϳ��� �����
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