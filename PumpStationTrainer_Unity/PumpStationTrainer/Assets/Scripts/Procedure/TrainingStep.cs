using UnityEngine;

public abstract class TrainingStep : ScriptableObject
{
    [TextArea] public string instruction;

    public abstract bool IsComplete(ProcedureContext ctx);
}
