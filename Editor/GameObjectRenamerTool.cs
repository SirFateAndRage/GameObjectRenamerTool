using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Utilities.GameObjectRenamerTool
{
    public class GameObjectRenamerTool : EditorWindow
    {
        private static string _gameobjectName;

        private static GameObject _selectedGameobject;

        private static GameObject[] _gameObjectSelection;

        bool initializedPosition = false;

        [Shortcut("Renamer", null, KeyCode.R, ShortcutModifiers.Shift)]
        public static void ShowWindow()
        {
            _gameObjectSelection = Selection.gameObjects;
            if (null == _gameObjectSelection)
                return;

            _gameobjectName = _gameObjectSelection[0].name;


            GetWindowWithRect(typeof(GameObjectRenamerTool), new Rect(Vector2.zero, new Vector2(300, 50)), false, "Renamer");
        }

        [Shortcut("RemoveFinalNumber", null, KeyCode.T, ShortcutModifiers.Shift)]
        public static void RemoveFinalNumber()
        {
            GameObject[] selectedGameobjects = Selection.gameObjects;

            foreach (var gameObject in selectedGameobjects)
            {
                gameObject.name = CorrectName(gameObject.name);
            }
        }

        [Shortcut("RenameWithTheScriptName", null, KeyCode.E, ShortcutModifiers.Action)]
        public static void RenameWithTheScriptName()
        {
            GameObject[] selectedGameobjects = Selection.gameObjects;

            foreach (var gameObject in selectedGameobjects)
            {
                MonoBehaviour monoBehaviour = gameObject.GetComponent<MonoBehaviour>();

                if (null != monoBehaviour)
                    gameObject.name = monoBehaviour.GetType().Name;
            }
        }

        private void OnGUI()
        {
            if (!initializedPosition)
            {
                Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                position = new Rect(mousePos.x - 150, mousePos.y - 25, position.width, position.height);
                initializedPosition = true;

                _gameobjectName = CorrectName(_gameobjectName);
            }

            _gameobjectName = GUILayout.TextField(_gameobjectName);

            if (GUILayout.Button("Rename"))
                SaveAllNames();

            Event e = Event.current;
            if (EventType.KeyDown == e.type && KeyCode.Return == e.keyCode)
            {
                SaveName();
            }
        }

        private void SaveName()
        {
            _selectedGameobject.name = _gameobjectName;
            this.Close();
        }

        private void SaveAllNames()
        {
            int count = 1;

            foreach (var item in _gameObjectSelection)
            {
                item.name = _gameobjectName + "_" + count.ToString();
                count++;
            }

            this.Close();
        }

        #region Names

        /// <summary>
        /// If the name contains index (ex: (1), (33)) remove them
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        private static string CorrectName(string _name)
        {
            char finalChar = _name[_name.Length - 1];
            string correctName = _name;

            if (finalChar == ')')
                correctName = ParenthesisSyntax(_name);
            else if (char.IsDigit(finalChar))
                correctName = DigitSyntax(_name);

            return correctName;
        }

        private static string ParenthesisSyntax(string _name)
        {
            int numbers = NumberQuant(_name.TrimEnd(')')) + 2;

            if (_name[_name.Length - numbers] == '(')
            {
                string newName = "";

                for (int i = 0; i < _name.Length - (numbers + 1); i++)
                {
                    newName += _name[i];
                }
                return newName;
            }

            return _name;
        }

        private static string DigitSyntax(string _name)
        {
            string newName = "";
            int numbers = NumberQuant(_name);

            for (int i = 0; i < _name.Length - numbers; i++)
            {
                newName += _name[i];
            }

            return newName;

        }

        private static int NumberQuant(string _name)
        {
            int numbers = 0;
            for (int i = _name.Length - 1; i > 0; i--)
            {
                if (char.IsDigit(_name[i]))
                    numbers++;

                else
                    break;
            }

            return numbers;
        }

        #endregion
    }
}
