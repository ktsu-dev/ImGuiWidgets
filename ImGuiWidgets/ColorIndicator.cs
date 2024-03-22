#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace ktsu.io.ImGuiWidgets;

using ImGuiNET;
using System.Numerics;


public static class ColorIndicator
{
	public static Vector4 Red => new(1, 0, 0, 1);
	public static Vector4 Green => new(0, 1, 0, 1);
	public static Vector4 Blue => new(0, 0, 1, 1);
	public static Vector4 Yellow => new(1, 1, 0, 1);
	public static Vector4 Cyan => new(0, 1, 1, 1);
	public static Vector4 Magenta => new(1, 0, 1, 1);
	public static Vector4 White => new(1, 1, 1, 1);
	public static Vector4 Black => new(0, 0, 0, 1);

	private static void PushCheckColor(Vector4 color)
	{
		ImGui.PushStyleColor(ImGuiCol.FrameBg, color);
		ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, color);
		ImGui.PushStyleColor(ImGuiCol.FrameBgActive, color);
		ImGui.PushStyleColor(ImGuiCol.CheckMark, color);
	}

	private static void PopCheckColor()
	{
		ImGui.PopStyleColor();
		ImGui.PopStyleColor();
		ImGui.PopStyleColor();
		ImGui.PopStyleColor();
	}

	public static void Show(Vector4 color, bool enabled)
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
