﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorPoint))]
public class ColorPointDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int oldIndentLevel = EditorGUI.indentLevel;
        label = EditorGUI.BeginProperty(position,label,property);
        //GUI 前缀标签
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        if(position.height>16f)
        {
            position.height = 16f;
            EditorGUI.indentLevel += 1;
            contentPosition = EditorGUI.IndentedRect(position);
            contentPosition.y += 18f;
        }
        contentPosition.width *= 0.75f;
        EditorGUI.indentLevel = 0;  //消除自动缩进
        //重写position 属性
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("position"), GUIContent.none);
        contentPosition.x += contentPosition.width;
        contentPosition.width /= 3f;
        //调整标签像素宽度
        EditorGUIUtility.labelWidth = 14f;
        EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("color"),new GUIContent("C"));
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = oldIndentLevel;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return label != GUIContent.none && Screen.width < 333 ? (16f + 18f) : 16f;   //控制自定义盒子的高度 Screen指的是窗口？？？
    }
}
