namespace ktsu.io.ImGuiWidgets;
using System.Numerics;
using ImGuiNET;

public static class Image
{
	public static bool Show(uint textureId, Vector2 size)
	{
		ImGui.Image((nint)textureId, size);
		return ImGui.IsItemClicked();
	}
}
