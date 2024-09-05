namespace ktsu.io.ImGuiWidgets;
using System.Numerics;
using ImGuiNET;
using ktsu.io.ImGuiStyler;

public static partial class ImGuiWidgets
{
	public static bool Image(uint textureId, Vector2 size) => ImageImpl.Show(textureId, new(size), Vector4.One);
	public static bool Image(uint textureId, Vector2 size, Vector4 color) => ImageImpl.Show(textureId, new(size), color);
	public static bool ImageCentered(uint textureId, Vector2 size) => ImageImpl.Centered(textureId, new(size), Vector4.One);
	public static bool ImageCentered(uint textureId, Vector2 size, Vector4 color) => ImageImpl.Centered(textureId, new(size), color);
	public static bool ImageCenteredWithin(uint textureId, Vector2 size, float width) => ImageImpl.CenteredWithin(textureId, new(size), new(width), Vector4.One);
	public static bool ImageCenteredWithin(uint textureId, Vector2 size, float width, Vector4 color) => ImageImpl.CenteredWithin(textureId, new(size), new(width), color);

	internal static class ImageImpl
	{
		internal static bool Show(uint textureId, ScaledVector2 size) => Show(textureId, size, Vector4.One);
		internal static bool Show(uint textureId, ScaledVector2 size, Vector4 color)
		{
			ImGui.Image((nint)textureId, size.ScaledValue, Vector2.Zero, Vector2.One, color);
			return ImGui.IsItemClicked();
		}

		internal static bool Centered(uint textureId, ScaledVector2 size) => Centered(textureId, size, Vector4.One);
		internal static bool Centered(uint textureId, ScaledVector2 size, Vector4 color)
		{
			Alignment.Center(size.ScaledValue.X);
			return Show(textureId, size, color);
		}

		internal static bool CenteredWithin(uint textureId, ScaledVector2 size, Scaled<float> width) => CenteredWithin(textureId, size, width, Vector4.One);
		internal static bool CenteredWithin(uint textureId, ScaledVector2 size, Scaled<float> width, Vector4 color)
		{
			Alignment.CenterWithin(size.ScaledValue.X, width.ScaledValue);
			return Show(textureId, size, color);
		}
	}
}
