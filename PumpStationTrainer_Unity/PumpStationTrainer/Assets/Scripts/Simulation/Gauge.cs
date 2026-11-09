using UnityEngine;

public class Gauge : MonoBehaviour
{
    [SerializeField] private PressureSystem system;
    [SerializeField] private Transform needle;
    [SerializeField] private int maxPsi = 3000;
    [SerializeField] private int sweepDegrees = 270;

    void OnEnable()
    {
        system.OnPressureChanged += UpdateNeedle;
    }

    void Start()
    {
        UpdateNeedle(system.Psi);
    }

    void OnDisable()
    {
        system.OnPressureChanged -= UpdateNeedle;
    }

    private void UpdateNeedle(float psi)
    {
        float t = Mathf.Clamp01(psi / maxPsi);
        float angle = Mathf.Lerp(0f, sweepDegrees, t);
        needle.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }
}
