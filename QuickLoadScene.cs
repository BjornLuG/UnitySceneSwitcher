using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using Commons.Extensions;

namespace LevelTools
{
    public class QuickLoadScene : EditorWindow
    {
        private struct Scene
        {
            public string name;
            public string path;

            public Scene(string name, string path)
            {
                this.name = name;
                this.path = path;
            }
        }

        private SearchField searchField;
        private string search;

        private string[] scenes = new string[0];
        private List<Scene> searchScenes = new List<Scene>();

        private bool ignoreShortcuts;

        private KeyCode[] keys = new KeyCode[]
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };

        [MenuItem("Design/Quick/Quick Load Scene #&z")]
        public static void Init()
        {
            QuickLoadScene window = GetWindow<QuickLoadScene>(true, "Quick Load", true);
            window.ShowUtility();
        }

        private void OnEnable()
        {
            scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            searchField = new SearchField();
            searchField.SetFocus();

            Research();
        }

        private void OnGUI()
        {
            EditorHelper.SetupDarkSkin(position);

            EditorGUILayout.HelpBox("1-9 - Select scenes; R - Refresh scenes; X - Close", MessageType.Info);

            if (CheckInputs())
                return;

            EditorGUI.BeginChangeCheck();
            search = searchField.OnGUI(search);
            if (EditorGUI.EndChangeCheck())
                Research();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Results:", EditorStyles.whiteBoldLabel);
            ignoreShortcuts = GUILayout.Toggle(ignoreShortcuts, "Ignore", "Button");
            GUILayout.EndHorizontal();

            for (int i = 0; i < searchScenes.Count; i++)
                GUILayout.Label((i < 9 ? (i + 1).ToString() : "-") + "    " + searchScenes[i].name);
        }

        /// <summary>
        /// True if Closing
        /// </summary>
        private bool CheckInputs()
        {
            if (Event.current.type == EventType.KeyDown && !ignoreShortcuts)
            {
                KeyCode keyCode = Event.current.keyCode;

                if (keyCode == KeyCode.None)
                    return false;

                if (keyCode == KeyCode.R)
                {
                    Refresh();
                    return false;
                }

                if (keyCode == KeyCode.X)
                {
                    Close();
                    return true;
                }

                for (int i = 0; i < keys.Length; i++)
                    if (keyCode == keys[i])
                        if (i < searchScenes.Count)
                        {
                            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                            EditorSceneManager.OpenScene(searchScenes[i].path);
                            Close();
                            return true;
                        }
            }

            return false;
        }

        private void Research()
        {
            searchScenes.Clear();

            for (int i = 0; i < scenes.Length; i++)
            {
                int start = scenes[i].LastIndexOf('/') + 1;
                int end = scenes[i].LastIndexOf('.');
                string sceneName = scenes[i].Substring(start, end - start);
                if (!string.IsNullOrEmpty(search))
                {
                    if (sceneName.ToLower().Contains(search.ToLower()))
                        searchScenes.Add(new Scene(sceneName, scenes[i]));
                }
                else
                    searchScenes.Add(new Scene(sceneName, scenes[i]));
            }
        }

        private void Refresh()
        {
            scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            Research();
            Repaint();
        }
    }
}