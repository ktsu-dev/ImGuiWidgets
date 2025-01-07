namespace ktsu.ImGuiWidgets;

using System.Collections.ObjectModel;
using ImGuiNET;

public static partial class ImGuiWidgets
{
	/// <summary>
	/// An ImGui.Combo implementation that accepts an enum.
	/// </summary>
	/// <typeparam name="TEnum">Type of the enum.</typeparam>
	/// <param name="label">Label for display/id purposes.</param>
	/// <param name="selectedValue">The currently selected enum value.</param>
	/// <returns>If a combo value was selected.</returns>
	public static bool Combo<TEnum>(string label, ref TEnum selectedValue) where TEnum : Enum
	{
		var possibleValues = new Collection<TEnum>((TEnum[])Enum.GetValues(typeof(TEnum)));
		int currentIndex = possibleValues.IndexOf(selectedValue);
		string[] possibleValuesNames = possibleValues.Select(e => e.ToString()).ToArray();
		if (ImGui.Combo(label, ref currentIndex, possibleValuesNames, possibleValues.Count))
		{
			selectedValue = possibleValues[currentIndex];
			return true;
		}
		return false;
	}
}
