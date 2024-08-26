namespace ktsu.io.ImGuiWidgets;
using System.Numerics;
using ImGuiNET;
using ktsu.io.ImGuiStyler;

public static class Image
{
	public static bool Show(uint textureId, Vector2 size)
	{
		ImGui.Image((nint)textureId, size);
		return ImGui.IsItemClicked();
	}

	public static bool Centered(uint textureId, Vector2 size)
	{
		Alignment.Center(size.X);
		return Show(textureId, size);
	}
}
