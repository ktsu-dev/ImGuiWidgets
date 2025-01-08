namespace ktsu.ImGuiWidgets;

using ImGuiNET;

/// <summary>
/// Provides custom ImGui widgets.
/// </summary>
public static partial class ImGuiWidgets
{
	/// <summary>
	/// Displays a color indicator checkbox widget.
	/// </summary>
	/// <param name="id">Unique ID to differentiate widgets.</param>
	/// <param name="color">The color to be displayed in the checkbox when it is enabled.</param>
	/// <param name="enabled">A boolean indicating whether the checkbox is enabled.</param>
	public static void ColorIndicator(string id, ImColor color, ref bool enabled) => ColorIndicatorImpl.Show(id, color, ref enabled);

	internal static class ColorIndicatorImpl
	{
		private static uint GetColorToRender(bool isEnabled, bool isHovered, ImColor enabledColor)
		{
			if (!isEnabled && !isHovered)
			{
				return ImGui.GetColorU32(ImGuiCol.FrameBg);
			}
			else if (!isEnabled && isHovered)
			{
				return ImGui.GetColorU32(ImGuiCol.FrameBgHovered);
			}
			else if (isEnabled && !isHovered)
			{
				return ImGui.GetColorU32(enabledColor.Value);
			}
			else if (isEnabled && isHovered)
			{
				// TODO: Change this slightly?
				return ImGui.GetColorU32(enabledColor.Value);
			}

			return ImGui.GetColorU32(ImGuiCol.FrameBg);
		}

		public static void Show(string id, ImColor color, ref bool enabled)
		{
			using (new ScopedId(id))
			{
				float frameHeight = ImGui.GetFrameHeight();

				ImGui.Dummy(new System.Numerics.Vector2(frameHeight, frameHeight));
				bool isHovered = ImGui.IsItemHovered();
				bool isClicked = ImGui.IsItemClicked();
				var dummyRectMin = ImGui.GetItemRectMin();
				var dummyRectMax = ImGui.GetItemRectMax();
				var drawList = ImGui.GetWindowDrawList();
				uint showColor = enabled ? ImGui.GetColorU32(color.Value) : ImGui.GetColorU32(ImGuiCol.FrameBg);
				drawList.AddRectFilled(dummyRectMin, dummyRectMax, GetColorToRender(enabled, isHovered, color));

				if (isClicked)
				{
					enabled = !enabled;
				}
			}
		}
	}
}
