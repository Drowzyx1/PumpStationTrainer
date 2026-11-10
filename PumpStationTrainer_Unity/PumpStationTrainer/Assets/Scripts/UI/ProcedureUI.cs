using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class ProcedureUI : MonoBehaviour
{
    [SerializeField] private ProcedureManager procedure;
    // [SerializeField] private MistakeTracker mistakes;      // turn back on in Day 7
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text checklistText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private TMP_Text summaryText;

    void OnEnable()
    {
        procedure.OnStepStarted += ShowStep;
        procedure.OnProcedureComplete += ShowSummary;
        // mistakes.OnMistake += ShowWarning;
    }

    void OnDisable()
    {
        procedure.OnStepStarted -= ShowStep;
        procedure.OnProcedureComplete -= ShowSummary;
        // mistakes.OnMistake -= ShowWarning;
    }

    private void ShowStep(TrainingStep step)
    {
        instructionText.text = step.instruction;
        RefreshChecklist();
    }

    private void ShowWarning(string message) => warningText.text = $"WARNING: {message}";

    private void RefreshChecklist()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < procedure.Steps.Count; i++)
        {
            string box = i < procedure.CurrentIndex ? "[x]" : "[ ]";
            sb.AppendLine($"{box} {procedure.Steps[i].instruction}");
        }
        checklistText.text = sb.ToString();
    }

    private void ShowSummary()
    {
        instructionText.text = "Procedure complete!";
        RefreshChecklist();

        float total = procedure.StepTimes.Sum();
        float slowest = procedure.StepTimes.Max();
        int slowestIndex = procedure.StepTimes.ToList().IndexOf(slowest);

        summaryText.text =
            $"Total time: {total:0.0}s\n" +
            // $"Mistakes: {mistakes.Mistakes.Count}\n" +       // turn back on in Day 7
            $"Slowest step: {procedure.Steps[slowestIndex].instruction} ({slowest:0.0}s)";
    }
}