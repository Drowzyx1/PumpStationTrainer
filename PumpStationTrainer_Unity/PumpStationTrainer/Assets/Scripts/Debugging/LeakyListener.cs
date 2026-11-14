using UnityEngine;

public class LeakyListener : MonoBehaviour
{
    private readonly byte[] _payload = new byte[1_000_000];

    void OnEnable() => PressureSystem.OnAnyPressureChanged += Log;

    void OnDisable() => PressureSystem.OnAnyPressureChanged -= Log;

    private void Log(float psi) => Debug.Log($"{name} heard {psi}");
}