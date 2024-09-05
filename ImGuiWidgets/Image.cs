namespace ktsu.io.ImGuiWidgets;
using System.Numerics;
using ImGuiNET;
using ktsu.io.ImGuiStyler;

public static partial class ImGuiWidgets
{
	public static bool Image(uint textureId, Vector2 size) => ImageImpl.Show(textureId, size, Vector4.One);
	public static bool Image(uint textureId, Vector2 size, Vector4 color) => ImageImpl.Show(textureId, size, color);
	public static bool ImageCentered(uint textureId, Vector2 size) => ImageImpl.Centered(textureId, size, Vector4.One);
	public static bool ImageCentered(uint textureId, Vector2 size, Vector4 color) => ImageImpl.Centered(textureId, size, color);
	public static bool ImageCenteredWithin(uint textureId, Vector2 size, float width) => ImageImpl.CenteredWithin(textureId, size, width, Vector4.One);
	public static bool ImageCenteredWithin(uint textureId, Vector2 size, float width, Vector4 color) => ImageImpl.CenteredWithin(textureId, size, width, color);

	internal static class ImageImpl
	{
		internal static bool Show(uint textureId, Vector2 size) => Show(textureId, size, Vector4.One);
		internal static bool Show(uint textureId, Vector2 size, Vector4 color)
		{
			ImGui.Image((nint)textureId, size, Vector2.Zero, Vector2.One, color);
			return ImGui.IsItemClicked();
		}

		internal static bool Centered(uint textureId, Vector2 size) => Centered(textureId, size, Vector4.One);
		internal static bool Centered(uint textureId, Vector2 size, Vector4 color)
		{
			Alignment.Center(size.X);
			return Show(textureId, size, color);
		}

		internal static bool CenteredWithin(uint textureId, Vector2 size, float width) => CenteredWithin(textureId, size, width, Vector4.One);
		internal static bool CenteredWithin(uint textureId, Vector2 size, float width, Vector4 color)
		{
			Alignment.CenterWithin(size.X, width);
			return Show(textureId, size, color);
		}
	}
}
