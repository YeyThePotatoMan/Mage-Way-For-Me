using UnityEngine;

class VectorHelper
{
    public static Vector2 RotateVector(Vector2 a, float angle)
    {
        /*
        Matrix rotasi
        |x'| = |cos -sin||x|
        |y'|   |sin cos ||y|
        */
        // Ubah jadi radian
        angle *= Mathf.Deg2Rad;
        float x = a.x * Mathf.Cos(angle) - a.y * Mathf.Sin(angle);
        float y = a.x * Mathf.Sin(angle) + a.y * Mathf.Cos(angle);
        return new Vector2(x, y);
    }
    public static Vector2 VectorProjection(Vector2 a, Vector2 b)
    {
        /*
        Misalkan c adalah proyeksi vektor a pada b
        c = a•b / |b|² × b
        c = a•b / |b| × b.normalized
        */
        return Vector2.Dot(a, b) / b.magnitude * b.normalized;
    }
    // Mengembalikan sudut rotasi yang dibutuhkan untuk merotasi a menjadi searah dengan b
    // Kembalian berupa angka pada interval (-180, 180)
    public static float AngleBetweenTwoVector(Vector2 a, Vector2 b)
    {
        return ((Mathf.Atan2(b.y, b.x) - Mathf.Atan2(a.y, a.x)) * Mathf.Rad2Deg + 540) % 360 - 180;
    }
}