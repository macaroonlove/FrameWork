using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Temporary.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Temporary.Editor
{
    public class LoadAgentTemplateEditorWindow : EditorWindow
    {
        private string _sheetID;
        private string _gid;
        private int _startId;
        private int _endId;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("시트 ID", GUILayout.Width(50));
            _sheetID = EditorGUILayout.TextField(_sheetID);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("GID", GUILayout.Width(50));
            _gid = EditorGUILayout.TextField(_gid);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("No.", GUILayout.Width(50));
            _startId = EditorGUILayout.IntField(_startId);
            EditorGUILayout.LabelField("부터", GUILayout.Width(50));
            _endId = EditorGUILayout.IntField(_endId);
            EditorGUILayout.LabelField("까지 불러오기", GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("유닛 불러오기"))
            {
                LoadCSVData();
            }
        }

        #region CSV 데이터 불러오기
        private async void LoadCSVData()
        {
            EditorUtility.DisplayProgressBar("로딩 중", "Google Sheets에서 데이터를 가져오는 중...", 0f);

            try
            {
                string csvData = await LoadCSVFromGoogleSheets();
                if (!string.IsNullOrEmpty(csvData))
                {
                    var csvDic = CSVToDictionary(csvData);
                    if (csvDic.Count > 0)
                    {
                        InitializeRarityTemplates();
                        InitializeJobTemplates();
                        ConvertCSVToAgentTemplate(csvDic);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV 데이터를 불러오는 중 오류 발생: {e.Message}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private Dictionary<string, List<string>> CSVToDictionary(string data)
        {
            var lines = data.Split("\n");
            if (lines.Length < 2) return null;

            var headers = lines[0].Trim().Split(',');
            var csvDict = new Dictionary<string, List<string>>();

            foreach (var header in headers)
            {
                csvDict[header] = new List<string>();
            }

            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Trim().Split(',');

                if (values.Length != headers.Length) continue;

                for (int j = 0; j < headers.Length; j++)
                {
                    csvDict[headers[j]].Add(values[j]);
                }
            }

            return csvDict;
        }

        private async UniTask<string> LoadCSVFromGoogleSheets()
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{_sheetID}/export?format=csv&gid={_gid}&range=A1:AA{_endId + 2}"))
            {
                var operation = request.SendWebRequest().ToUniTask();

                await operation;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    Debug.LogError("스프레드시트 데이터 로드 실패: " + request.error);
                    return null;
                }
            }
        }

        private void ConvertCSVToAgentTemplate(Dictionary<string, List<string>> csvDic)
        {
            Dictionary<int, AgentTemplate> templateDic = new Dictionary<int, AgentTemplate>();
            var guids = AssetDatabase.FindAssets("t:AgentTemplate");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<AgentTemplate>(path);
                templateDic[template.id] = template;
            }

            var idList = csvDic["No."];

            for (int i = 0; i < idList.Count; i++)
            {
                if (int.TryParse(idList[i], out int id) == false) continue;
                if (id < _startId || id > _endId) continue;

                // 템플릿이 존재한다면 수정
                if (templateDic.TryGetValue(id, out var template))
                {
                    // 등급
                    if (_rarityDic.TryGetValue(csvDic["등급"][i], out var rarity))
                    {
                        template.SetRarity(rarity);
                    }

                    // 직군
                    if (_jobDic.TryGetValue(csvDic["직군"][i], out var job))
                    {
                        template.SetJob(job);
                    }

                    // 유닛 이름
                    if (template.displayName != csvDic["유닛 이름"][i])
                    {
                        string newName = csvDic["유닛 이름"][i];
                        template.SetDisplayName(newName);

                        string assetPath = AssetDatabase.GetAssetPath(template);
                        AssetDatabase.RenameAsset(assetPath, $"Agent_{newName}");
                    }

                    // 이동 방식
                    if (Enum.TryParse<EMoveType>(csvDic["이동 방식"][i], out var moveType))
                    {
                        template.SetMoveType(moveType);
                    }

                    // 이동 속도
                    if (float.TryParse(csvDic["이동 속도"][i], out var moveSpeed))
                    {
                        template.SetMoveSpeed(moveSpeed);
                    }

                    // 추적 거리
                    if (float.TryParse(csvDic["추적 거리"][i], out var chaseRange))
                    {
                        template.SetChaseRange(chaseRange);
                    }

                    // 추적 실패 거리
                    if (float.TryParse(csvDic["추적 실패 거리"][i], out var chaseFailRange))
                    {
                        template.SetChaseFailRange(chaseFailRange);
                    }

                    // 공격 방식
                    if (Enum.TryParse<EAttackType>(csvDic["공격 방식"][i], out var attackType))
                    {
                        template.SetAttackType(attackType);
                    }

                    // 데미지 타입
                    if (Enum.TryParse<EDamageType>(csvDic["데미지 타입"][i], out var damageType))
                    {
                        template.SetDamageType(damageType);
                    }

                    // 공격력
                    if (int.TryParse(csvDic["공격력"][i], out var atk))
                    {
                        template.SetATK(atk);
                    }

                    // 공격 간격
                    if (float.TryParse(csvDic["공격 간격"][i], out var attackTerm))
                    {
                        template.SetAttackTerm(attackTerm);
                    }

                    // 공격 사거리
                    if (float.TryParse(csvDic["공격 사거리"][i], out var attackRange))
                    {
                        template.SetAttackRange(attackRange);
                    }

                    // 물리 관통력
                    if (int.TryParse(csvDic["물리 관통력"][i], out var physicalPenetration))
                    {
                        template.SetPhysicalPenetration(physicalPenetration);
                    }

                    // 마법 관통력
                    if (int.TryParse(csvDic["마법 관통력"][i], out var magicPenetration))
                    {
                        template.SetMagicPenetration(magicPenetration);
                    }

                    // 치명타 확률
                    if (float.TryParse(csvDic["치명타 확률"][i], out var criticalHitChance))
                    {
                        template.SetCriticalHitChance(criticalHitChance);
                    }

                    // 치명타 데미지
                    if (float.TryParse(csvDic["치명타 데미지"][i], out var criticalHitDamage))
                    {
                        template.SetCriticalHitDamage(criticalHitDamage);
                    }

                    // 최대 체력
                    if (int.TryParse(csvDic["최대 체력"][i], out var maxHP))
                    {
                        template.SetMaxHP(maxHP);
                    }

                    // 방어력
                    if (int.TryParse(csvDic["방어력"][i], out var physicalResistance))
                    {
                        template.SetPhysicalResistance(physicalResistance);
                    }

                    // 마법 저항력
                    if (int.TryParse(csvDic["마법 저항력"][i], out var magicResistance))
                    {
                        template.SetMagicResistance(magicResistance);
                    }

                    // 초당 체력 회복량
                    if (int.TryParse(csvDic["초당 체력 회복량"][i], out var hpRecoveryPerSec))
                    {
                        template.SetHPRecoveryPerSec(hpRecoveryPerSec);
                    }

                    // 마나 회복 방식
                    if (Enum.TryParse<EManaRecoveryType>(csvDic["마나 회복 방식"][i], out var manaRecoveryType))
                    {
                        template.SetManaRecoveryType(manaRecoveryType);
                    }

                    // 최대 마나
                    if (int.TryParse(csvDic["최대 마나"][i], out var maxMana))
                    {
                        template.SetMaxMana(maxMana);
                    }

                    // 시작 마나
                    if (int.TryParse(csvDic["시작 마나"][i], out var startMana))
                    {
                        template.SetStartMana(startMana);
                    }

                    // 초당 마나 회복량
                    if (int.TryParse(csvDic["초당 마나 회복량"][i], out var manaRecoveryPerSec))
                    {
                        template.SetManaRecoveryPerSec(manaRecoveryPerSec);
                    }

                    EditorUtility.SetDirty(template);
                }
                // 템플릿이 존재하지 않는다면 생성
                else
                {
                    var newTemplate = CreateInstance<AgentTemplate>();

                    // 식별번호
                    newTemplate.SetId(id);

                    // 등급
                    if (_rarityDic.TryGetValue(csvDic["등급"][i], out var rarity))
                    {
                        newTemplate.SetRarity(rarity);
                    }

                    // 직군
                    if (_jobDic.TryGetValue(csvDic["직군"][i], out var job))
                    {
                        newTemplate.SetJob(job);
                    }

                    // 유닛 이름
                    newTemplate.SetDisplayName(csvDic["유닛 이름"][i]);

                    // 이동 방식
                    if (Enum.TryParse<EMoveType>(csvDic["이동 방식"][i], out var moveType))
                    {
                        newTemplate.SetMoveType(moveType);
                    }

                    // 이동 속도
                    if (float.TryParse(csvDic["이동 속도"][i], out var moveSpeed))
                    {
                        newTemplate.SetMoveSpeed(moveSpeed);
                    }

                    // 추적 거리
                    if (float.TryParse(csvDic["추적 거리"][i], out var chaseRange))
                    {
                        newTemplate.SetChaseRange(chaseRange);
                    }

                    // 추적 실패 거리
                    if (float.TryParse(csvDic["추적 실패 거리"][i], out var chaseFailRange))
                    {
                        newTemplate.SetChaseFailRange(chaseFailRange);
                    }

                    // 공격 방식
                    if (Enum.TryParse<EAttackType>(csvDic["공격 방식"][i], out var attackType))
                    {
                        newTemplate.SetAttackType(attackType);
                    }

                    // 데미지 타입
                    if (Enum.TryParse<EDamageType>(csvDic["데미지 타입"][i], out var damageType))
                    {
                        newTemplate.SetDamageType(damageType);
                    }

                    // 공격력
                    if (int.TryParse(csvDic["공격력"][i], out var atk))
                    {
                        newTemplate.SetATK(atk);
                    }

                    // 공격 간격
                    if (float.TryParse(csvDic["공격 간격"][i], out var attackTerm))
                    {
                        newTemplate.SetAttackTerm(attackTerm);
                    }

                    // 공격 사거리
                    if (float.TryParse(csvDic["공격 사거리"][i], out var attackRange))
                    {
                        newTemplate.SetAttackRange(attackRange);
                    }

                    // 물리 관통력
                    if (int.TryParse(csvDic["물리 관통력"][i], out var physicalPenetration))
                    {
                        newTemplate.SetPhysicalPenetration(physicalPenetration);
                    }

                    // 마법 관통력
                    if (int.TryParse(csvDic["마법 관통력"][i], out var magicPenetration))
                    {
                        newTemplate.SetMagicPenetration(magicPenetration);
                    }

                    // 치명타 확률
                    if (float.TryParse(csvDic["치명타 확률"][i], out var criticalHitChance))
                    {
                        newTemplate.SetCriticalHitChance(criticalHitChance);
                    }

                    // 치명타 데미지
                    if (float.TryParse(csvDic["치명타 데미지"][i], out var criticalHitDamage))
                    {
                        newTemplate.SetCriticalHitDamage(criticalHitDamage);
                    }

                    // 최대 체력
                    if (int.TryParse(csvDic["최대 체력"][i], out var maxHP))
                    {
                        newTemplate.SetMaxHP(maxHP);
                    }

                    // 방어력
                    if (int.TryParse(csvDic["방어력"][i], out var physicalResistance))
                    {
                        newTemplate.SetPhysicalResistance(physicalResistance);
                    }

                    // 마법 저항력
                    if (int.TryParse(csvDic["마법 저항력"][i], out var magicResistance))
                    {
                        newTemplate.SetMagicResistance(magicResistance);
                    }

                    // 초당 체력 회복량
                    if (int.TryParse(csvDic["초당 체력 회복량"][i], out var hpRecoveryPerSec))
                    {
                        newTemplate.SetHPRecoveryPerSec(hpRecoveryPerSec);
                    }

                    // 마나 회복 방식
                    if (Enum.TryParse<EManaRecoveryType>(csvDic["마나 회복 방식"][i], out var manaRecoveryType))
                    {
                        newTemplate.SetManaRecoveryType(manaRecoveryType);
                    }

                    // 최대 마나
                    if (int.TryParse(csvDic["최대 마나"][i], out var maxMana))
                    {
                        newTemplate.SetMaxMana(maxMana);
                    }

                    // 시작 마나
                    if (int.TryParse(csvDic["시작 마나"][i], out var startMana))
                    {
                        newTemplate.SetStartMana(startMana);
                    }

                    // 초당 마나 회복량
                    if (int.TryParse(csvDic["초당 마나 회복량"][i], out var manaRecoveryPerSec))
                    {
                        newTemplate.SetManaRecoveryPerSec(manaRecoveryPerSec);
                    }

                    string path = $"Assets/Temporary/GameData/Unit/Agent/Agent_{csvDic["유닛 이름"][i]}.asset";
                    AssetDatabase.CreateAsset(newTemplate, path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion

        #region 등급 템플릿 가져오기
        private Dictionary<string, RarityTemplate> _rarityDic = new Dictionary<string, RarityTemplate>();

        private void InitializeRarityTemplates()
        {
            var guids = AssetDatabase.FindAssets("t:RarityTemplate");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var rarity = AssetDatabase.LoadAssetAtPath<RarityTemplate>(path);
                _rarityDic[rarity.rarity.ToString()] = rarity;
            }
        }
        #endregion

        #region 직업 템플릿 가져오기
        private Dictionary<string, JobTemplate> _jobDic = new Dictionary<string, JobTemplate>();

        private void InitializeJobTemplates()
        {
            var guids = AssetDatabase.FindAssets("t:JobTemplate");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var job = AssetDatabase.LoadAssetAtPath<JobTemplate>(path);
                _jobDic[job.job.ToString()] = job;
            }
        }
        #endregion
    }
}