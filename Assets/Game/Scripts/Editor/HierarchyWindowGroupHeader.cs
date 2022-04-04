using UnityEditor;
using UnityEngine;

/// <summary>
/// Colorful Hierarchy Window Group Header
/// Author: https://github.com/omerasikoglu
/// Thanks for concept of idea : http://diegogiacomelli.com.br/unitytips-hierarchy-window-group-header
/// Samples: "#9856CC HEADER1" , "#magenta header2" , "# HEADER3"
/// </summary>
[InitializeOnLoad]
public static class HierarchyWindowGroupHeader
{
    static HierarchyWindowGroupHeader()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject == null) return;
        if (!gameObject.name.StartsWith("#", System.StringComparison.Ordinal)) return;

        // format is #color and #FF0012
        string colorName = gameObject.name.Substring(1).Split(' ')[0];

        // convert to color
        Color color = IsHexCode() ? GetColorFromString(colorName) :
            (ColorUtility.TryParseHtmlString(colorName.ToLower(), out var _color) ? _color : Color.black);

        EditorGUI.DrawRect(selectionRect, color);
        EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace($"#{colorName}", string.Empty).ToUpperInvariant());

        bool IsHexCode()
        {
            if (colorName.Length < 6) return false;

            for (int i = 0; i < 6; i++)
            {
                if (char.IsDigit(colorName, i)) continue;

                if (char.ToUpper(colorName[i]).Equals('A') || char.ToUpper(colorName[i]).Equals('B') ||
                    char.ToUpper(colorName[i]).Equals('C') || char.ToUpper(colorName[i]).Equals('D') ||
                    char.ToUpper(colorName[i]).Equals('E') || char.ToUpper(colorName[i]).Equals('F'))
                    continue;

                return false;
            }
            return true;
        }

        Color GetColorFromString(string color)
        {
            float red = System.Convert.ToInt32(color.Substring(0, 2), 16) / 255f;
            float green = System.Convert.ToInt32(color.Substring(2, 2), 16) / 255f;
            float blue = System.Convert.ToInt32(color.Substring(4, 2), 16) / 255f;

            return new Color(red, green, blue);
        }
    }

}