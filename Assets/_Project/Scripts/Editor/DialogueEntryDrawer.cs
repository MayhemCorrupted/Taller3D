using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DialogueLineData))]
public class DialogueEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0;
        SerializedProperty typeProp = property.FindPropertyRelative("dialogueType");
        bool is3D = typeProp.enumValueIndex == (int)DialogueType.World_3D;

        SerializedProperty prop = property.Copy();
        SerializedProperty endProp = prop.GetEndProperty();

        prop.NextVisible(true);
        while (!SerializedProperty.EqualContents(prop, endProp))
        {
            if (!is3D && prop.name == "targetToLookAt")
            {
                prop.NextVisible(false);
                continue;
            }
            height += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
            prop.NextVisible(false);
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty typeProp = property.FindPropertyRelative("dialogueType");
        bool is3D = typeProp.enumValueIndex == (int)DialogueType.World_3D;

        Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        SerializedProperty prop = property.Copy();
        SerializedProperty endProp = prop.GetEndProperty();

        prop.NextVisible(true);
        while (!SerializedProperty.EqualContents(prop, endProp))
        {
            if (!is3D && prop.name == "targetToLookAt")
            {
                prop.NextVisible(false);
                continue;
            }

            float propHeight = EditorGUI.GetPropertyHeight(prop, true);
            rect.height = propHeight;

            EditorGUI.PropertyField(rect, prop, true);

            rect.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
            prop.NextVisible(false);
        }
        EditorGUI.EndProperty();
    }
}
