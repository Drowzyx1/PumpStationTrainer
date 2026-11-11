using UnityEngine;

[CreateAssetMenu(menuName = "Trainer/Steps/Part In Socket")]
public class PartInSocketStep : TrainingStep
{
    public string requiredPartId;

    public override bool IsComplete(ProcedureContext ctx)
    {
        return ctx.FilterSocket.CurrentPartId == requiredPartId;
    }
}