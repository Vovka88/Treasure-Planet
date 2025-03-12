using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(ArrayLayout))]
    public class ArrayLa : PropertyDrawer
    {
        private const float CellPadding = 24f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PrefixLabel(position, label);
            var reposition = position;
            reposition.y += CellPadding;
            var data = property.FindPropertyRelative("rows");

            if (data.arraySize != Config.maximal_table_vertical)
                data.arraySize = Config.maximal_table_vertical;

            for (var j = 0; j < Config.maximal_table_vertical; j++)
            {
                var row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
                reposition.height = CellPadding;
                if (row.arraySize != Config.maximal_table_horizontal)
                    row.arraySize = Config.maximal_table_horizontal;
                reposition.width = position.width / Config.maximal_table_horizontal;
                for (var i = 0; i < Config.maximal_table_horizontal; i++)
                {
                    EditorGUI.PropertyField(reposition, row.GetArrayElementAtIndex(i), GUIContent.none);
                    reposition.x += reposition.width;
                }
                reposition.x = position.x;
                reposition.y += CellPadding;

            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CellPadding * (Config.maximal_table_vertical + 1);
        }
    }
}