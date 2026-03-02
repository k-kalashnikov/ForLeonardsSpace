using System.Text;

namespace Masofa.Common.Models.Satellite.Auxiliary;

public class ValuesList
{
    public List<string> Values { get; set; }

    public override string ToString()
    {
        return string.Join(";", Values?
            .Select(v => v?.Replace(";", @"\;") ?? string.Empty) ?? new List<string>());
    }

    public static ValuesList FromString(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return new ValuesList();

        var values = new List<string>();
        var current = new StringBuilder();
        bool escape = false;

        foreach (var ch in str)
        {
            if (escape)
            {
                if (ch == ';')
                    current.Append(';');
                else
                    current.Append('\\').Append(ch);
                escape = false;
            }
            else if (ch == '\\')
            {
                escape = true;
            }
            else if (ch == ';')
            {
                values.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(ch);
            }
        }

        values.Add(current.ToString());
        return new ValuesList { Values = values };
    }
}

