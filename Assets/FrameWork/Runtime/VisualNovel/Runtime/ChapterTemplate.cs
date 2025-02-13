using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    public class ChapterTemplate : ScriptableObject
    {
        [HideInInspector, SerializeField] private string _title;
        [HideInInspector, SerializeField] private string _sheetID;
        [HideInInspector, SerializeField] private string _gid;
        [HideInInspector, SerializeField] private string _range = "A2:C";

        [HideInInspector] public List<Episode> episodes = new List<Episode>();

        public string title => _title;

        public void SetTitle(string title)
        {
            _title = title;
        }
    }
}

#if UNITY_EDITOR
namespace FrameWork.VisualNovel.Editor
{
    using System;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine.Networking;

    [CustomEditor(typeof(ChapterTemplate)), CanEditMultipleObjects]
    public class ChapterTemplateEditor : Editor
    {
        private ChapterTemplate _target;

        private SerializedProperty _title;
        private SerializedProperty _sheetID;
        private SerializedProperty _gid;
        private SerializedProperty _range;

        private ReorderableList _reorderableList;

        private void OnEnable()
        {
            _target = target as ChapterTemplate;

            _title = serializedObject.FindProperty("_title");
            _sheetID = serializedObject.FindProperty("_sheetID");
            _gid = serializedObject.FindProperty("_gid");
            _range = serializedObject.FindProperty("_range");

            SetEpisodeList();
        }

        #region ReorderableList
        private void SetEpisodeList()
        {
            _reorderableList = new ReorderableList(_target.episodes, typeof(Episode), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "에피소드 목록");
                },
                onAddDropdownCallback = (buttonRect, list) =>
                {
                    EpisodeMenu();
                },
                onRemoveCallback = list =>
                {
                    if (!EditorUtility.DisplayDialog("경고!", "이 항목을 삭제하시겠습니까?", "네", "아니요")) return;

                    _target.episodes.RemoveAt(list.index);

                    EditorUtility.SetDirty(_target);
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var episode = _target.episodes[index];
                    episode.Draw(rect);

                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(_target);
                    }
                },
                elementHeightCallback = (index) =>
                {
                    var episode = _target.episodes[index];
                    return episode.GetHeight() * 20 + 8;
                }
            };
        }

        private void EpisodeMenu()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("대화 시작 명령어"), false, CreateEpisodeCallback, typeof(SpeakStartEpisode));
            menu.AddItem(new GUIContent("대화 종료 명령어"), false, CreateEpisodeCallback, typeof(SpeakEndEpisode));
            menu.AddItem(new GUIContent("캐릭터 및 물체 표시 명령어"), false, CreateEpisodeCallback, typeof(SCGShowEpisode));
            menu.AddItem(new GUIContent("캐릭터 및 물체 숨김 명령어"), false, CreateEpisodeCallback, typeof(SCGHideEpisode));
            menu.AddItem(new GUIContent("캐릭터 및 물체 이동 명령어"), false, CreateEpisodeCallback, typeof(SCGMoveEpisode));
            menu.AddItem(new GUIContent("배경 표시 명령어"), false, CreateEpisodeCallback, typeof(ECGShowEpisode));
            menu.AddItem(new GUIContent("배경 숨김 명령어"), false, CreateEpisodeCallback, typeof(ECGHideEpisode));
            menu.AddItem(new GUIContent("배경음 실행 명령어"), false, CreateEpisodeCallback, typeof(BGMPlayEpisode));
            menu.AddItem(new GUIContent("배경음 정지 명령어"), false, CreateEpisodeCallback, typeof(BGMStopEpisode));
            menu.AddItem(new GUIContent("효과음 실행 명령어"), false, CreateEpisodeCallback, typeof(SFXPlayEpisode));
            menu.AddItem(new GUIContent("효과음 정지 명령어"), false, CreateEpisodeCallback, typeof(SFXStopEpisode));
            menu.AddItem(new GUIContent("명령어 그룹으로 묶기 시작"), false, CreateEpisodeCallback, typeof(CommandGroupStartEpisode));
            menu.AddItem(new GUIContent("명령어 그룹으로 묶기 종료"), false, CreateEpisodeCallback, typeof(CommandGroupEndEpisode));

            menu.ShowAsContext();
        }

        private void CreateEpisodeCallback(object obj)
        {
            var episode = CreateInstance((Type)obj) as Episode;
            if (episode != null)
            {
                episode.hideFlags = HideFlags.HideInHierarchy;
                _target.episodes.Add(episode);

                var template = target as ChapterTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(episode, path);
                EditorUtility.SetDirty(template);
            }
        }
        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            GUILayout.Label("주제", GUILayout.Width(120));
            EditorGUILayout.PropertyField(_title, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sheet ID", GUILayout.Width(120));
            EditorGUILayout.PropertyField(_sheetID, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("GID", GUILayout.Width(120));
            EditorGUILayout.PropertyField(_gid, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("범위", GUILayout.Width(120));
            EditorGUILayout.PropertyField(_range, GUIContent.none);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("불러오기"))
            {
                LoadCSVData(_sheetID.stringValue, _gid.stringValue, _range.stringValue);
            }

            GUILayout.Space(20);

            _reorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #region CSV 데이터 불러오기
        private async void LoadCSVData(string sheetID, string gid, string range)
        {
            string csvData = await LoadCSVFromGoogleSheets(sheetID, gid, range);
            if (!string.IsNullOrEmpty(csvData))
            {
                ConvertCSVToEpisode(csvData);
            }
        }

        private async Task<string> LoadCSVFromGoogleSheets(string sheetID, string gid, string range)
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{sheetID}/export?format=csv&gid={gid}&range={range}"))
            {
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

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

        private void ConvertCSVToEpisode(string data)
        {
            List<Episode> episodes = _target.episodes;
            episodes.Clear();

            var lines = data.Split("\r\n");

            foreach (var line in lines)
            {
                var cell = line.Split(',');

                if (Enum.TryParse(cell[0], out CommandType command))
                {
                    switch (command)
                    {
                        case CommandType.Speak_Start:
                            SpeakStartEpisode speakStartEpisode = new SpeakStartEpisode();
                            speakStartEpisode.Initialize(cell[1], cell[2]);
                            episodes.Add(speakStartEpisode);
                            break;
                        case CommandType.Speak_End:
                            SpeakEndEpisode speakEndEpisode = new SpeakEndEpisode();
                            episodes.Add(speakEndEpisode);
                            break;
                        case CommandType.SCG_Show:
                            SCGShowEpisode scgShowEpisode = new SCGShowEpisode();
                            scgShowEpisode.Initialize(cell[1], cell[2]);
                            episodes.Add(scgShowEpisode);
                            break;
                        case CommandType.SCG_Hide:
                            SCGHideEpisode scgHideEpisode = new SCGHideEpisode();
                            scgHideEpisode.Initialize(cell[1]);
                            episodes.Add(scgHideEpisode);
                            break;
                        case CommandType.SCG_Move:
                            SCGMoveEpisode scgMoveEpisode = new SCGMoveEpisode();
                            scgMoveEpisode.Initialize(cell[1], cell[2]);
                            episodes.Add(scgMoveEpisode);
                            break;
                        case CommandType.ECG_Show:
                            ECGShowEpisode ecgShowEpisode = new ECGShowEpisode();
                            ecgShowEpisode.Initialize(cell[1]);
                            episodes.Add(ecgShowEpisode);
                            break;
                        case CommandType.ECG_Hide:
                            ECGHideEpisode ecgHideEpisode = new ECGHideEpisode();
                            episodes.Add(ecgHideEpisode);
                            break;
                        case CommandType.BGM_Play:
                            BGMPlayEpisode bgmPlayEpisode = new BGMPlayEpisode();
                            bgmPlayEpisode.Initialize(cell[1]);
                            episodes.Add(bgmPlayEpisode);
                            break;
                        case CommandType.BGM_Stop:
                            BGMStopEpisode bgmStopEpisode = new BGMStopEpisode();
                            bgmStopEpisode.Initialize(cell[1]);
                            episodes.Add(bgmStopEpisode);
                            break;
                        case CommandType.SFX_Play:
                            SFXPlayEpisode sfxPlayEpisode = new SFXPlayEpisode();
                            sfxPlayEpisode.Initialize(cell[1]);
                            episodes.Add(sfxPlayEpisode);
                            break;
                        case CommandType.SFX_Stop:
                            SFXStopEpisode sfxStopEpisode = new SFXStopEpisode();
                            sfxStopEpisode.Initialize(cell[1]);
                            episodes.Add(sfxStopEpisode);
                            break;
                        case CommandType.CommandGroup_Start:
                            CommandGroupStartEpisode commandGroupStartEpisode = new CommandGroupStartEpisode();
                            episodes.Add(commandGroupStartEpisode);
                            break;
                        case CommandType.CommandGroup_End:
                            CommandGroupEndEpisode commandGroupEndEpisode = new CommandGroupEndEpisode();
                            episodes.Add(commandGroupEndEpisode);
                            break;
                    }
                }
            }
        }
        #endregion
    }
}
#endif