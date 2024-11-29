#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace ktsu.ImGuiWidgets;

using ImGuiNET;
using ktsu.ScopedAction;

public class ScopedId : ScopedAction
{
	public ScopedId(string id)
	{
		ImGui.PushID(id);
		OnClose = ImGui.PopID;
	}

	public ScopedId(int id)
	{
		ImGui.PushID(id);
		OnClose = ImGui.PopID;
	}
}
