#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ktsu.io.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;
using ktsu.io.ScopedAction;

public static class Color
{
	public static ImColor FromRGB(byte r, byte g, byte b) => new()
	{
		Value = new Vector4(r / 255f, g / 255f, b / 255f, 1f)
	};

	public static ImColor FromRGBA(byte r, byte g, byte b, byte a) => new()
	{
		Value = new Vector4(r / 255f, g / 255f, b / 255f, a / 255f)
	};

	public static ImColor FromRGB(float r, float g, float b) => new()
	{
		Value = new Vector4(r, g, b, 1f)
	};

	public static ImColor FromRGBA(float r, float g, float b, float a) => new()
	{
		Value = new Vector4(r, g, b, a)
	};

	public static ImColor FromVector(Vector3 vector) => new()
	{
		Value = new Vector4(vector.X, vector.Y, vector.Z, 1f)
	};

	public static ImColor FromVector(Vector4 vector) => new()
	{
		Value = vector
	};

	public static ImColor Red => FromRGB(255, 0, 0);
	public static ImColor Green => FromRGB(0, 255, 0);
	public static ImColor Blue => FromRGB(0, 0, 255);
	public static ImColor Yellow => FromRGB(255, 255, 0);
	public static ImColor Cyan => FromRGB(0, 255, 255);
	public static ImColor Magenta => FromRGB(255, 0, 255);
	public static ImColor White => FromRGB(255, 255, 255);
	public static ImColor Black => FromRGB(0, 0, 0);
	public static ImColor Gray => FromRGB(128, 128, 128);
	public static ImColor LightGray => FromRGB(192, 192, 192);
	public static ImColor DarkGray => FromRGB(64, 64, 64);
	public static ImColor Transparent => FromRGBA(0, 0, 0, 0);
}

public class ScopedColor : ScopedAction
{
	public ScopedColor(ImGuiCol target, ImColor color) : base(
		onOpen: () => ImGui.PushStyleColor(target, color.Value),
		onClose: ImGui.PopStyleColor)
	{
	}
}
