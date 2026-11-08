using System.Reflection;
using System.Text;

public static class DebugReport
{
    public static string Describe(object obj)
    {
        var sb = new StringBuilder();
        sb.AppendLine(obj.GetType().Name);
        foreach (PropertyInfo prop in obj.GetType().GetProperties())
        {
            if (Attribute.IsDefined(prop, typeof(ShowInDebugAttribute)))
                sb.AppendLine($"  {prop.Name}: {prop.GetValue(obj)}");
        }
        return sb.ToString();
    }
}