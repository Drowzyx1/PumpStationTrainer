using UnityEngine;

[CreateAssetMenu(menuName = "Trainer/Steps/Pressure Below")]
public class PressureBelowStep : TrainingStep
{
    public float maxPsi = 1f;

    public override bool IsComplete(ProcedureContext ctx)
    {
        return ctx.Pressure.Psi <= maxPsi;
    }
}
