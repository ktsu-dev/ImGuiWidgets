namespace ktsu.io.ImGuiWidgets;
using System.Numerics;
using ImGuiNET;
using ktsu.io.ImGuiStyler;

public static class Image
{
	public static bool Show(uint textureId, Vector2 size) => Show(textureId, size, Vector4.One);
	public static bool Show(uint textureId, Vector2 size, Vector4 color)
	{
		ImGui.Image((nint)textureId, size, Vector2.Zero, Vector2.One, color);
		return ImGui.IsItemClicked();
	}

	public static bool Centered(uint textureId, Vector2 size) => Centered(textureId, size, Vector4.One);
	public static bool Centered(uint textureId, Vector2 size, Vector4 color)
	{
		Alignment.Center(size.X);
		return Show(textureId, size, color);
	}

	public static bool CenteredWithin(uint textureId, Vector2 size, float width) => CenteredWithin(textureId, size, width, Vector4.One);
	public static bool CenteredWithin(uint textureId, Vector2 size, float width, Vector4 color)
	{
		if (width < size.X)
		{
			size.Y *= width / size.X;
			size.X = width;
		}

		Alignment.CenterWithin(size.X, width);
		return Show(textureId, size, color);
	}
}
