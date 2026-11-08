using System;
using System.Collections.Generic;

List<Component> parts = new List<Component>
{
    new Pump("Main Pump", 1200),
    new Valve("Supply Valve", 300),
    new Filter("Line Filter", 250),
};

Valve supply = (Valve)parts[1];
PressureSystem pressure = new PressureSystem(supply);
ValveLogger logger = new ValveLogger(supply);

while (true)
{
    Console.WriteLine("Enter Option:\n1) Inspect All\n2) Toggle valve\n3) Report\n4) Debug Valve\n5) Repair Valve\n0) Exit");
    string input = Console.ReadLine();

    if (input == "0")
    {
        break;
    }

    switch (input)
    {
        case "1":
            foreach (Component c in parts)
            {
                Console.WriteLine($"{c.Describe()} | Faulty: {c.IsFaulty()}");

                if (c is IInspectable inspectable)
                {
                    Console.WriteLine($"    Inspect: {inspectable.Inspect()}");
                }
            }
            break;
        case "2":
            supply.Toggle();
            break;
        case "3":
            Reports.PrintSummary(parts);
            break;
        case "4":
            Console.WriteLine($"Debug Report: {DebugReport.Describe(supply)}");
            break;
        case "5":
            if (supply is IRepairable repairable)
            {
                Console.WriteLine($"Repair Status: {repairable.Repair()}");
            }
            break;
        default:
            Console.WriteLine("Invalid option");
            break;
    }
}