using System.Runtime.CompilerServices;

namespace FutureFeatureGenerator;

internal class Options
{
    private Dictionary<string, object> _values = new(StringComparer.OrdinalIgnoreCase);
    public bool UseExtensions
    {
        get => (bool)GetValue();
        set => SetValue(value);
    }
    public bool UseRealCondition
    {
        get => (bool)GetValue();
        set => SetValue(value);
    }
    public bool DisableAddDependencies
    {
        get => (bool)GetValue();
        set => SetValue(value);
    }
    public bool AutoAddLangType
    {
        get => (bool)GetValue();
        set => SetValue(value);
    }
    public bool NotWriteGeneratedCodeAttribute
    {
        get => (bool)GetValue();
        set => SetValue(value);
    }
    public string DefaultModifier
    {
        get => (string)GetValue();
        set => SetValue(value);
    }
    static readonly string[] sBooleanKeys = [nameof(UseExtensions), nameof(UseRealCondition), nameof(DisableAddDependencies), nameof(AutoAddLangType), nameof(NotWriteGeneratedCodeAttribute)];
    static readonly string[] sAllowedModifiers = [Modifiers.Internal, Modifiers.Public];
    private object GetValue([CallerMemberName] string memberName = "")
    {
        return _values[memberName];
    }
    private void SetValue(object value, [CallerMemberName] string memberName = "")
    {
        _values[memberName] = value;
    }
    public Options()
    {
        foreach (var key in sBooleanKeys)
        {
            SetValue(false, key);
        }
        DefaultModifier = Modifiers.Internal;
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
        foreach (var name in sBooleanKeys)
        {
            if (settingName.Equals(name.AsSpan(), StringComparison.OrdinalIgnoreCase) && bool.TryParse(line.Slice(tuples[1]).ToString(), out result))
            {
                SetValue(result, name);
                return;
            }
        }
        if(settingName.Equals(nameof(DefaultModifier).AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            var value = line.Slice(tuples[1]).ToString();
            if (Array.IndexOf(sAllowedModifiers, value) != -1)
            {
                SetValue(value, nameof(DefaultModifier));
            }
        }
    }
}
