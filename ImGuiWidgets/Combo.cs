namespace ktsu.ImGuiWidgets;

using System.Collections.ObjectModel;
using ImGuiNET;
using ktsu.StrongStrings;

public static partial class ImGuiWidgets
{
	/// <summary>
	/// An ImGui.Combo implementation that works with Enums.
	/// </summary>
	/// <typeparam name="TEnum">Type of the Enum.</typeparam>
	/// <param name="label">Label for display and id.</param>
	/// <param name="selectedValue">The currently selected value.</param>
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

	/// <summary>
	/// An ImGui.Combo implementation that works with IStrings.
	/// </summary>
	/// <typeparam name="TString">Type of the StrongString</typeparam>
	/// <param name="label">Label for display and id.</param>
	/// <param name="selectedValue">The currently selected value.</param>
	/// <param name="possibleValues">The collection of possible values.</param>
	/// <returns>If a combo value was selected.</returns>
	public static bool Combo<TString>(string label, ref TString selectedValue, Collection<TString> possibleValues) where TString : IString
	{
		ArgumentNullException.ThrowIfNull(possibleValues);

		int currentIndex = possibleValues.IndexOf(selectedValue);
		string[] possibleValuesNames = possibleValues.Select(e => e.ToString()).ToArray();
		if (ImGui.Combo(label, ref currentIndex, possibleValuesNames, possibleValues.Count))
		{
			selectedValue = possibleValues[currentIndex];
			return true;
		}
		return false;
	}

	/// <summary>
	/// An ImGui.Combo implementation that works with strings.
	/// </summary>
	/// <param name="label">Label for display and id.</param>
	/// <param name="selectedValue">The currently selected value.</param>
	/// <param name="possibleValues">The collection of possible values.</param>
	/// <returns>If a combo value was selected.</returns>
	public static bool Combo(string label, ref string selectedValue, Collection<string> possibleValues)
	{
		ArgumentNullException.ThrowIfNull(possibleValues);

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
