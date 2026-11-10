using UnityEngine;

[CreateAssetMenu(menuName = "Trainer/Steps/Door Open")]
public class DoorOpenStep : TrainingStep
{
    public bool mustBeOpen = true;

    public override bool IsComplete(ProcedureContext ctx)
    {
        return ctx.AccessPanel.IsOpen == mustBeOpen;
    }
}
