using System.Runtime.CompilerServices;

namespace FutureFeatureGenerator;

internal class Options
{
    private Dictionary<string, bool> _values = new(StringComparer.OrdinalIgnoreCase);
    public bool UseExtensions
    {
        get => GetValue();
        set => SetValue(value);
    }
    public bool UseRealCondition
    {
        get => GetValue();
        set => SetValue(value);
    }
    public bool DisableAddDependencies
    {
        get => GetValue();
        set => SetValue(value);
    }
    private bool GetValue([CallerMemberName] string memberName = "")
    {
        return _values.TryGetValue(memberName, out var value) ? value : false;
    }
    private void SetValue(bool value, [CallerMemberName] string memberName = "")
    {
        _values[memberName] = value;
    }
    public Options()
    {
        UseExtensions = false;
        UseRealCondition = false;
        DisableAddDependencies = false;
    }
    public void ExecuteChange(ReadOnlySpan<char> line)
    {
        bool result;
        var tuples = line.Trim().Split(Utils.SpaceSeparator);
        if (tuples.Count < 2)
        {
            return;
        }
        var settingName = line.Slice(tuples[0]);
        foreach (var name in _values.Keys)
        {
            if (settingName.Equals(name.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                if (bool.TryParse(line.Slice(tuples[1]).ToString(), out result))
                {
                    SetValue(result, name);
                }
            }
        }
    }
}
