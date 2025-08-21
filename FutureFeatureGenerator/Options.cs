namespace FutureFeatureGenerator;

internal class Options
{
    public bool UseExtensions { get; set; } = false;
    public bool UseRealCondition { get; set; } = false;
    public bool DisableAddDependencies { get; set; } = false;
    public void ExecuteChange(ReadOnlySpan<char> line)
    {
        bool result;
        var tuples = line.Trim().Split(Utils.SpaceSeparator);
        if (tuples.Count < 2)
        {
            return;
        }
        var settingName = line.Slice(tuples[0]);
        if (settingName.Equals(nameof(UseExtensions).AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            if (bool.TryParse(line.Slice(tuples[1]).ToString(), out result))
            {
                UseExtensions = result;
            }
        }
        else if (settingName.Equals(nameof(UseRealCondition).AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            if (bool.TryParse(line.Slice(tuples[1]).ToString(), out result))
            {
                UseRealCondition = result;
            }
        }
    }
}
