namespace ktsu.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;
using ktsu.ImGuiStyler;

/// <summary>
/// Provides custom ImGui widgets.
/// </summary>
public static partial class ImGuiWidgets
{
	/// <summary>
	/// Specifies the alignment of the icon.
	/// </summary>
	public enum IconAlignment
	{
		/// <summary>
		/// Aligns the icon horizontally.
		/// </summary>
		Horizontal,

		/// <summary>
		/// Aligns the icon vertically.
		/// </summary>
		Vertical,
	}

	/// <summary>
	/// Contains delegate actions for icon events.
	/// </summary>
	public class IconDelegates
	{
		/// <summary>
		/// Gets or sets the action to be performed on click.
		/// </summary>
		public Action? OnClick { get; init; }

		/// <summary>
		/// Gets or sets the action to be performed on double click.
		/// </summary>
		public Action? OnDoubleClick { get; init; }

		/// <summary>
		/// Gets or sets the action to be performed on right click.
		/// </summary>
		public Action? OnRightClick { get; init; }

		/// <summary>
		/// Gets or sets the action to be performed on context menu.
		/// </summary>
		public Action? OnContextMenu { get; init; }
	}

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="color">The color of the icon.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, float size, Vector4 color) => Icon(label, textureId, new Vector2(size), color, IconAlignment.Horizontal, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, float size, Vector4 color, IconAlignment iconAlignment) => Icon(label, textureId, new Vector2(size), color, iconAlignment, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconDelegates">The delegates for icon events.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, float size, Vector4 color, IconDelegates iconDelegates) => Icon(label, textureId, new Vector2(size), color, IconAlignment.Horizontal, iconDelegates);

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <param name="iconDelegates">The delegates for icon events.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, float size, Vector4 color, IconAlignment iconAlignment, IconDelegates iconDelegates) => Icon(label, textureId, new Vector2(size), color, iconAlignment, iconDelegates);

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="color">The color of the icon.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, Vector2 size, Vector4 color) => Icon(label, textureId, size, color, IconAlignment.Horizontal, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, Vector2 size, Vector4 color, IconAlignment iconAlignment) => Icon(label, textureId, size, color, iconAlignment, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconDelegates">The delegates for icon events.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, Vector2 size, Vector4 color, IconDelegates iconDelegates) => Icon(label, textureId, size, color, IconAlignment.Horizontal, iconDelegates);

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <param name="iconDelegates">The delegates for icon events.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, Vector2 size, Vector4 color, IconAlignment iconAlignment, IconDelegates iconDelegates) =>
		IconImpl.Show(label, textureId, size, color, iconAlignment, iconDelegates);

	/// <summary>
	/// Calculates the size of the icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <returns>The calculated size of the icon.</returns>
	public static Vector2 CalcIconSize(string label, float size) => CalcIconSize(label, new Vector2(size), IconAlignment.Horizontal);

	/// <summary>
	/// Calculates the size of the icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <returns>The calculated size of the icon.</returns>
	public static Vector2 CalcIconSize(string label, float size, IconAlignment iconAlignment) => CalcIconSize(label, new Vector2(size), iconAlignment);

	/// <summary>
	/// Calculates the size of the icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <returns>The calculated size of the icon.</returns>
	public static Vector2 CalcIconSize(string label, Vector2 size) => CalcIconSize(label, size, IconAlignment.Horizontal);

	/// <summary>
	/// Calculates the size of the icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="size">The size of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <returns>The calculated size of the icon.</returns>
	public static Vector2 CalcIconSize(string label, Vector2 size, IconAlignment iconAlignment)
	{
		var style = ImGui.GetStyle();
		var framePadding = style.FramePadding;
		var itemSpacing = style.ItemSpacing;
		var labelSize = ImGui.CalcTextSize(label);
		if (iconAlignment == IconAlignment.Horizontal)
		{
			var sizeWithLabel = size + new Vector2(labelSize.X + itemSpacing.X, 0);
			sizeWithLabel.Y = Math.Max(sizeWithLabel.Y, labelSize.Y);
			return sizeWithLabel + (framePadding * 2);
		}
		else if (iconAlignment == IconAlignment.Vertical)
		{
			var sizeWithLabel = size + new Vector2(0, labelSize.Y + itemSpacing.Y);
			sizeWithLabel.X = Math.Max(sizeWithLabel.X, labelSize.X);
			return sizeWithLabel + (framePadding * 2);
		}

		return size;
	}

	/// <summary>
	/// Contains the implementation details for rendering icons.
	/// </summary>
	internal static class IconImpl
	{
		/// <summary>
		/// Shows the icon with the specified parameters.
		/// </summary>
		/// <param name="label">The label of the icon.</param>
		/// <param name="textureId">The texture ID of the icon.</param>
		/// <param name="size">The size of the icon.</param>
		/// <param name="color">The color of the icon.</param>
		/// <param name="iconAlignment">The alignment of the icon.</param>
		/// <param name="iconDelegates">The delegates for icon events.</param>
		/// <returns>True if the icon was clicked; otherwise, false.</returns>
		public static bool Show(string label, uint textureId, Vector2 size, Vector4 color, IconAlignment iconAlignment, IconDelegates iconDelegates)
		{
			bool wasClicked = false;

			var style = ImGui.GetStyle();
			var framePadding = style.FramePadding;
			var itemSpacing = style.ItemSpacing;

			ImGui.PushID(label);

			var cursorStartPos = ImGui.GetCursorScreenPos();
			var sizeWithLabel = CalcIconSize(label, size, iconAlignment);
			ImGui.SetCursorScreenPos(cursorStartPos + framePadding);

			switch (iconAlignment)
			{
				case IconAlignment.Horizontal:
					HorizontalLayout(label, textureId, size, itemSpacing, color, cursorStartPos);
					break;
				case IconAlignment.Vertical:
					VerticalLayout(label, textureId, size, itemSpacing, color, cursorStartPos);
					break;
				default:
					throw new NotImplementedException();
			}

			ImGui.SetCursorScreenPos(cursorStartPos);
			ImGui.Dummy(sizeWithLabel);
			bool isHovered = ImGui.IsItemHovered();
			if (isHovered)
			{
				uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
				var drawList = ImGui.GetWindowDrawList();
				drawList.AddRect(cursorStartPos, cursorStartPos + sizeWithLabel, ImGui.GetColorU32(borderColor));

				if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
				{
					iconDelegates.OnClick?.Invoke();
					wasClicked = true;
				}
				if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
				{
					iconDelegates.OnDoubleClick?.Invoke();
				}
				if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
				{
					iconDelegates.OnRightClick?.Invoke();
				}
				if (ImGui.IsMouseReleased(ImGuiMouseButton.Right) && iconDelegates.OnContextMenu is not null)
				{
					ImGui.OpenPopup($"{label}_Context");
				}
			}

			if (ImGui.BeginPopup($"{label}_Context"))
			{
				iconDelegates.OnContextMenu?.Invoke();
				ImGui.EndPopup();
			}

			ImGui.PopID();

			return wasClicked;
		}

		/// <summary>
		/// Arranges the icon and label in a vertical layout.
		/// </summary>
		/// <param name="label">The label of the icon.</param>
		/// <param name="textureId">The texture ID of the icon.</param>
		/// <param name="size">The size of the icon.</param>
		/// <param name="itemSpacing">The spacing between items.</param>
		/// <param name="color">The color of the icon.</param>
		/// <param name="cursorStartPos">The starting position of the cursor.</param>
		private static void VerticalLayout(string label, uint textureId, Vector2 size, Vector2 itemSpacing, Vector4 color, Vector2 cursorStartPos)
		{
			var sizeWithLabel = CalcIconSize(label, size, IconAlignment.Vertical);
			var imageTopLeft = cursorStartPos + new Vector2((sizeWithLabel.X - size.X) / 2, 0);
			ImGui.SetCursorScreenPos(imageTopLeft);
			ImGui.Image((nint)textureId, size, Vector2.Zero, Vector2.One, color);
			var labelSize = ImGui.CalcTextSize(label);
			var labelTopLeft = cursorStartPos + new Vector2((sizeWithLabel.X - labelSize.X) / 2, size.Y + itemSpacing.Y);
			ImGui.SetCursorScreenPos(labelTopLeft);
			ImGui.TextUnformatted(label);
		}

		/// <summary>
		/// Arranges the icon and label in a horizontal layout.
		/// </summary>
		/// <param name="label">The label of the icon.</param>
		/// <param name="textureId">The texture ID of the icon.</param>
		/// <param name="size">The size of the icon.</param>
		/// <param name="itemSpacing">The spacing between items.</param>
		/// <param name="color">The color of the icon.</param>
		/// <param name="cursorStartPos">The starting position of the cursor.</param>
		private static void HorizontalLayout(string label, uint textureId, Vector2 size, Vector2 itemSpacing, Vector4 color, Vector2 cursorStartPos)
		{
			ImGui.Image((nint)textureId, size, Vector2.Zero, Vector2.One, color);

			var labelSize = ImGui.CalcTextSize(label);
			var widgetSize = CalcIconSize(label, size, IconAlignment.Horizontal);
			var leftAlign = new Vector2(0, widgetSize.Y);
			ImGui.SetCursorScreenPos(cursorStartPos + new Vector2(size.X + itemSpacing.X, 0));
			Alignment.CenterWithin(labelSize, leftAlign);
			ImGui.TextUnformatted(label);
		}
	}
}
