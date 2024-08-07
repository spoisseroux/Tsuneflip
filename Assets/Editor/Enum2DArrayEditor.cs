using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Enum2DArray))]
public class Enum2DArrayEditor : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty rows = property.FindPropertyRelative("rows");
        return rows.intValue * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty rows = property.FindPropertyRelative("rows");
        SerializedProperty columns = property.FindPropertyRelative("columns");
        SerializedProperty array = property.FindPropertyRelative("array");

        if (rows.intValue > 0 && columns.intValue > 0)
        {
            float width = position.width / columns.intValue;
            for (int i = 0; i < rows.intValue; i++)
            {
                for (int j = 0; j < columns.intValue; j++)
                {
                    Rect elementRect = new Rect(position.x + j * width, position.y + i * EditorGUIUtility.singleLineHeight, width, EditorGUIUtility.singleLineHeight);
                    int index = i * columns.intValue + j;
                    SerializedProperty element = array.GetArrayElementAtIndex(index);
                    element.enumValueIndex = (int)(FlipCode)EditorGUI.EnumPopup(elementRect, (FlipCode)element.enumValueIndex);
                }
            }
        }

        EditorGUI.EndProperty();
    }
}