using UnityEngine;

[CreateAssetMenu(menuName = "Trainer/Steps/Valve State")]
public class ValveStateStep : TrainingStep
{
    public string valveId;
    public bool mustBeOpen;

    public override bool IsComplete(ProcedureContext ctx)
    {
        return ctx.GetValve(valveId).IsOpen == mustBeOpen;
    }
}
