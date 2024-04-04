#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace ktsu.io.ImGuiWidgets;

using ImGuiNET;

public static class ColorIndicator
{
	private static void PushCheckColor(ImColor color)
	{
		ImGui.PushStyleColor(ImGuiCol.FrameBg, color.Value);
		ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, color.Value);
		ImGui.PushStyleColor(ImGuiCol.FrameBgActive, color.Value);
		ImGui.PushStyleColor(ImGuiCol.CheckMark, color.Value);
	}

	private static void PopCheckColor()
	{
		ImGui.PopStyleColor();
		ImGui.PopStyleColor();
		ImGui.PopStyleColor();
		ImGui.PopStyleColor();
	}

	public static void Show(ImColor color, bool enabled)
	{
		if (enabled)
		{
			PushCheckColor(color);
		}
		ImGui.Checkbox("##hidelabel", ref enabled);
		if (enabled)
		{
			PopCheckColor();
		}
	}
}
