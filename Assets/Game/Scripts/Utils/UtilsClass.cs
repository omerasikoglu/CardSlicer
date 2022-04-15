using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// My Library
/// </summary>
public static class UtilsClass
{
    private static Camera MainCamera => Camera.main;

    #region Mouse or Object Position
    public static Vector3 GetScreenToViewportPoint(Vector3? objectPosition = null, float posZ = 0f, Camera cam = null)
    {
        cam ??= MainCamera;

        Vector3 position = objectPosition == null ?
            MainCamera.ScreenToViewportPoint(Input.mousePosition) : MainCamera.ScreenToViewportPoint((Vector3)objectPosition);

        position.z = posZ;
        return position;
    }
    public static Vector3 GetScreenToWorldPoint(Vector3? objectPosition = null, float worldPosZ = 0f, Camera cam = null)
    {
        cam ??= MainCamera;

        Vector3 position = objectPosition == null ?
            cam.ScreenToWorldPoint(Input.mousePosition) : cam.ScreenToWorldPoint((Vector3)objectPosition);

        position.z = worldPosZ;
        return position;
    }
    public static Vector3 GetWorldToScreenPoint(Vector3? objectPosition = null, float screenPosZ = 0f, Camera cam = null)
    {
        cam ??= MainCamera;

        Vector3 position = objectPosition == null ?
            cam.WorldToScreenPoint(Input.mousePosition) : cam.WorldToScreenPoint((Vector3)objectPosition);



        return position;
    }

    public static Ray GetScreenPointToRay(Vector3 position, Camera cam = null)
    {
        cam ??= MainCamera;

        return cam.ScreenPointToRay(position);

    }

    #endregion

    #region Mouse ile obje arasındaki yön vektörü
    public static Vector3 GetMouseDirection(Transform bulletSource, float posZ = 0f)
    {
        //ok atarken gitceði yön vektörünü bulmak için
        Vector3 mouseVector = GetScreenToWorldPoint() - bulletSource.position;
        mouseVector.z = posZ;
        return mouseVector;
    }
    public static Vector3 GetNormalizeMouseDirection(Transform bulletSource, float posZ = 0f)
    {
        // ok atarken gideceği yön vektörünü bulmak için
        Vector3 mouseVector = GetMouseDirection(bulletSource, posZ);
        mouseVector.Normalize();
        return mouseVector;
    }
    #endregion

    #region Random Directions
    //Random Directions
    public static Vector3 GetRandomDirection2D()
    {
        // random x ve y değerleri, z=0f
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    public static Vector3 GetRandomUpDirection2D()
    {
        // nesneyi havaya fırlatma
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), 1f).normalized;
    }
    #endregion

    #region Açılar, diferansiyeller mmm..
    public static float GetAngleFromVector(Vector3 vector)
    {
        // okun yönü düşmana göre dönsün diye
        float radians = Mathf.Atan2(vector.y, vector.x);
        float degrees = radians * Mathf.Rad2Deg;
        return degrees;
    }
    #endregion

    #region Colors

    // Get Hex Color FF00FF
    public static string GetStringFromColor(Color color)
    {
        string red = Dec01_to_Hex(color.r);
        string green = Dec01_to_Hex(color.g);
        string blue = Dec01_to_Hex(color.b);
        return red + green + blue;
    }
    // Returns a hex string based on a number between 0->1
    public static string Dec01_to_Hex(float value)
    {
        return Dec_to_Hex((int)Mathf.Round(value * 255f));
    }
    // Returns 00-FF, value 0->255
    public static string Dec_to_Hex(int value)
    {
        return value.ToString("X2");
    }

    // Get Color from Hex string FF00FFAA
    public static Color GetColorFromString(string color)
    {
        float red = Hex_to_Dec01(color.Substring(0, 2));
        float green = Hex_to_Dec01(color.Substring(2, 2));
        float blue = Hex_to_Dec01(color.Substring(4, 2));
        float alpha = 1f;
        if (color.Length >= 8)
        {
            // Color string contains alpha
            alpha = Hex_to_Dec01(color.Substring(6, 2));
        }
        return new Color(red, green, blue, alpha);
    }

    // Returns a float between 0->1
    public static float Hex_to_Dec01(string hex)
    {
        return Hex_to_Dec(hex) / 255f;
    }

    // Returns 0-255
    public static int Hex_to_Dec(string hex)
    {
        return Convert.ToInt32(hex, 16);
    }
    #endregion

    public static IEnumerator Wait(Action action, float secs)
    {
        yield return new WaitForSeconds(secs);
        action();
    }

    internal static string Wait(Action p) {
        throw new NotImplementedException();
    }
}
