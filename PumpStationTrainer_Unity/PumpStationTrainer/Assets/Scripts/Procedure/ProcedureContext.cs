using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ProcedureContext : MonoBehaviour
{
    [SerializeField] private PressureSystem pressure;
    [SerializeField] private Door accessPanel;
    [SerializeField] private FilterSocket filterSocket;
    private Dictionary<string, Valve> _valves;
    public PressureSystem Pressure { get => pressure; }
    public Door AccessPanel { get => accessPanel; }
    public FilterSocket FilterSocket { get => filterSocket; }

    public void Awake()
    {
        _valves = new Dictionary<string, Valve>();
        _valves = FindObjectsByType<Valve>(FindObjectsSortMode.None).ToDictionary(v => v.Id);
    }

    public Valve GetValve(string id)
    {
        if (!_valves.ContainsKey(id))
        {
            Debug.LogError($"Valve with id {id} not found");
            return null;
        }
        return _valves[id];
    }
}
