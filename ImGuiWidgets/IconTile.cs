namespace ktsu.io.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;
using ktsu.io.ImGuiStyler;

public static class IconTile
{
	public enum BorderStyle
	{
		None,
		Always,
		Hover
	}

	public class Properties
	{
		public uint TextureId { get; set; }
		public string Label { get; set; } = string.Empty;
		public Vector2 IconSize { get; set; } = new(64, 64);
		public Vector2 LabelSize { get; set; } = new(64, 16);
		public int Padding { get; set; } = 8;
		public BorderStyle BorderStyle { get; set; } = BorderStyle.Hover;
	}

	public static bool Show(Properties properties)
	{
		float frameWidth = Math.Max(properties.IconSize.X, properties.LabelSize.X) + (properties.Padding * 2);
		float frameHeight = properties.IconSize.Y + properties.LabelSize.Y + (properties.Padding * 3);
		var frameSize = new Vector2(frameWidth, frameHeight);

		ImGui.BeginChild(properties.Label, frameSize, false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
		var cursorPos = ImGui.GetCursorPos();
		var cursorScreenPos = ImGui.GetCursorScreenPos();

		ImGui.SetCursorPos(cursorPos + new Vector2(properties.Padding, properties.Padding));
		Image.Show(properties.TextureId, properties.IconSize);
		ImGui.SetCursorPos(cursorPos + new Vector2(properties.Padding, properties.IconSize.Y + (properties.Padding * 2)));
		Text.CenteredWithin(properties.Label, properties.LabelSize.X);
		ImGui.SetCursorPos(cursorPos + Vector2.Zero);
		ImGui.Dummy(frameSize);
		bool wasClicked = ImGui.IsItemClicked();
		bool isHovered = ImGui.IsItemHovered();

		bool border = properties.BorderStyle switch
		{
			BorderStyle.None => false,
			BorderStyle.Always => true,
			BorderStyle.Hover => isHovered,
			_ => false
		};

		if (border)
		{
			uint color = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
			ImGui.GetWindowDrawList().AddRect(cursorScreenPos, cursorScreenPos + frameSize, color);
		}
		ImGui.EndChild();

		return wasClicked;
	}
}
