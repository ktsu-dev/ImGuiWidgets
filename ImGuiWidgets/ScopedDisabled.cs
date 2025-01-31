namespace ktsu.ImGuiWidgets;

using ImGuiNET;
using ktsu.ScopedAction;

/// <summary>
/// Represents a scoped disabled action which will set Dear ImGui elements as functionally and visually disabled until
/// the class is disposed.
/// </summary>
public class ScopedDisabled : ScopedAction
{
	/// <summary>
	/// Note as per the Dear ImGui documentation: "Those can be nested but it cannot
	/// be used to enable an already disabled section (a single BeginDisabled(true)
	/// in the stack is enough to keep everything disabled)"
	/// </summary>
	/// <param name="enabled">Should the elements within the scope be disabled</param>
	public ScopedDisabled(bool enabled)
	{
		ImGui.BeginDisabled(enabled);
		OnClose = ImGui.EndDisabled;
	}
}
