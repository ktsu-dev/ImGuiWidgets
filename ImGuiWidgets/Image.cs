namespace ktsu.ImGuiWidgets;
using System.Numerics;
using ImGuiNET;
using ktsu.ImGuiStyler;

public static partial class ImGuiWidgets
{
	public static bool Image(uint textureId, Vector2 size) => ImageImpl.Show(textureId, size, Vector4.One);
	public static bool Image(uint textureId, Vector2 size, Vector4 color) => ImageImpl.Show(textureId, size, color);
	public static bool ImageCentered(uint textureId, Vector2 size) => ImageImpl.Centered(textureId, size, Vector4.One);
	public static bool ImageCentered(uint textureId, Vector2 size, Vector4 color) => ImageImpl.Centered(textureId, size, color);
	public static bool ImageCenteredWithin(uint textureId, Vector2 size, Vector2 containerSize) => ImageImpl.CenteredWithin(textureId, size, containerSize, Vector4.One);
	public static bool ImageCenteredWithin(uint textureId, Vector2 size, Vector2 containerSize, Vector4 color) => ImageImpl.CenteredWithin(textureId, size, containerSize, color);

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
			bool clicked = false;
			using (new Alignment.Center(size))
			{
				clicked = Show(textureId, size, color);
			}
			return clicked;
		}

		internal static bool CenteredWithin(uint textureId, Vector2 size, Vector2 containerSize) => CenteredWithin(textureId, size, containerSize, Vector4.One);
		internal static bool CenteredWithin(uint textureId, Vector2 imageSize, Vector2 containerSize, Vector4 color)
		{
			bool clicked = false;
			using (new Alignment.CenterWithin(imageSize, containerSize))
			{
				clicked = Show(textureId, imageSize, color);
			}
			return clicked;
		}
	}
}
