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
	/// Gets or sets a value indicating whether to enable debug drawing for icons.
	/// </summary>
	public static bool EnableIconDebugDraw { get; set; }

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
	/// Additional options to modify Icon behavior.
	/// </summary>
	public class IconOptions
	{
		/// <summary>
		/// The color of the icon.
		/// </summary>
		public Vector4 Color { get; set; } = ktsu.ImGuiStyler.Color.White.Value;
		/// <summary>
		/// The tooltip to display.
		/// </summary>
		public string Tooltip { get; set; } = string.Empty;
		/// <summary>
		/// The alignment of the icon.
		/// </summary>
		public IconAlignment IconAlignment { get; set; } = IconAlignment.Horizontal;
		/// <summary>
		/// Actions to be performed.
		/// </summary>
		public IconDelegates IconDelegates { get; set; } = new();
	}

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <returns>Was the icon bounds clicked</returns>
	public static bool Icon(string label, uint textureId, float imageSize) => IconImpl.Show(label, textureId, new(imageSize, imageSize), new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <returns>Was the icon bounds clicked</returns>
	public static bool Icon(string label, uint textureId, Vector2 imageSize) => IconImpl.Show(label, textureId, imageSize, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="options">Additional options</param>
	/// <returns>Was the icon bounds clicked</returns>
	public static bool Icon(string label, uint textureId, float imageSize, IconOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);
		return IconImpl.Show(label, textureId, new(imageSize, imageSize), options);
	}

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="options">Additional options</param>
	/// <returns>Was the icon bounds clicked</returns>
	public static bool Icon(string label, uint textureId, Vector2 imageSize, IconOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);
		return IconImpl.Show(label, textureId, imageSize, options);
	}

	/// <summary>
	/// Calculates the size of the icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <returns>The calculated size of the icon.</returns>
	public static Vector2 CalcIconSize(string label, float imageSize) => CalcIconSize(label, new Vector2(imageSize), IconAlignment.Horizontal);

	/// <summary>
	/// Calculates the size of the icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <returns>The calculated size of the icon.</returns>
	public static Vector2 CalcIconSize(string label, float imageSize, IconAlignment iconAlignment) => CalcIconSize(label, new Vector2(imageSize), iconAlignment);

	/// <summary>
	/// Calculates the size of the icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <returns>The calculated size of the icon.</returns>
	public static Vector2 CalcIconSize(string label, Vector2 imageSize) => CalcIconSize(label, imageSize, IconAlignment.Horizontal);

	/// <summary>
	/// Calculates the size of the icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="iconAlignment">The alignment of the image and label with respect to each other.</param>
	/// <returns>The calculated size of the widget.</returns>
	public static Vector2 CalcIconSize(string label, Vector2 imageSize, IconAlignment iconAlignment)
	{
		var style = ImGui.GetStyle();
		var framePadding = style.FramePadding;
		var itemSpacing = style.ItemSpacing;
		var labelSize = ImGui.CalcTextSize(label);
		if (iconAlignment == IconAlignment.Horizontal)
		{
			var boundingBoxSize = imageSize + new Vector2(labelSize.X + itemSpacing.X, 0);
			boundingBoxSize.Y = Math.Max(boundingBoxSize.Y, labelSize.Y);
			return boundingBoxSize + (framePadding * 2);
		}
		else if (iconAlignment == IconAlignment.Vertical)
		{
			var boundingBoxSize = imageSize + new Vector2(0, labelSize.Y + itemSpacing.Y);
			boundingBoxSize.X = Math.Max(boundingBoxSize.X, labelSize.X);
			return boundingBoxSize + (framePadding * 2);
		}

		return imageSize;
	}

	/// <summary>
	/// Contains the implementation details for rendering icons.
	/// </summary>
	internal static class IconImpl
	{
		internal static bool Show(string label, uint textureId, Vector2 imageSize, IconOptions options)
		{
			ArgumentException.ThrowIfNullOrEmpty(label);

			bool wasClicked = false;

			var style = ImGui.GetStyle();
			var framePadding = style.FramePadding;
			var itemSpacing = style.ItemSpacing;

			ImGui.PushID(label);

			var cursorStartPos = ImGui.GetCursorScreenPos();
			var labelSize = ImGui.CalcTextSize(label);// TODO, maybe pass this to an internal overload of CalcIconSize to save recalculating
			var boundingBoxSize = CalcIconSize(label, imageSize, options.IconAlignment);

			ImGui.SetCursorScreenPos(cursorStartPos + framePadding);

			switch (options.IconAlignment)
			{
				case IconAlignment.Horizontal:
					HorizontalLayout(label, textureId, imageSize, labelSize, boundingBoxSize, itemSpacing, options.Color, cursorStartPos);
					break;
				case IconAlignment.Vertical:
					VerticalLayout(label, textureId, imageSize, labelSize, boundingBoxSize, itemSpacing, options.Color, cursorStartPos);
					break;
				default:
					throw new NotImplementedException();
			}

			ImGui.SetCursorScreenPos(cursorStartPos);
			ImGui.Dummy(boundingBoxSize);
			bool isHovered = ImGui.IsItemHovered();
			bool isMouseClicked = ImGui.IsMouseClicked(ImGuiMouseButton.Left);
			bool isMouseDoubleClicked = ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);
			bool isRightMouseClicked = ImGui.IsMouseClicked(ImGuiMouseButton.Right);
			bool isRightMouseReleased = ImGui.IsMouseReleased(ImGuiMouseButton.Right);

			if (!string.IsNullOrEmpty(options.Tooltip))
			{
				ImGui.SetItemTooltip(options.Tooltip);
			}
			if (isHovered || EnableIconDebugDraw)
			{
				uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
				var drawList = ImGui.GetWindowDrawList();
				drawList.AddRect(cursorStartPos, cursorStartPos + boundingBoxSize, ImGui.GetColorU32(borderColor));
			}

			if (isHovered)
			{
				if (isMouseClicked)
				{
					options.IconDelegates.OnClick?.Invoke();
					wasClicked = true;
				}
				if (isMouseDoubleClicked)
				{
					options.IconDelegates.OnDoubleClick?.Invoke();
				}
				if (isRightMouseClicked)
				{
					options.IconDelegates.OnRightClick?.Invoke();
				}
				if (isRightMouseReleased && options.IconDelegates.OnContextMenu is not null)
				{
					ImGui.OpenPopup($"{label}_Context");
				}
			}

			if (ImGui.BeginPopup($"{label}_Context"))
			{
				options.IconDelegates.OnContextMenu?.Invoke();
				ImGui.EndPopup();
			}

			ImGui.PopID();

			return wasClicked;
		}

		private static void VerticalLayout(string label, uint textureId, Vector2 imageSize, Vector2 labelSize, Vector2 boundingBoxSize, Vector2 itemSpacing, Vector4 color, Vector2 cursorStartPos)
		{
			var imageTopLeft = cursorStartPos + new Vector2((boundingBoxSize.X - imageSize.X) / 2, 0);
			ImGui.SetCursorScreenPos(imageTopLeft);
			ImGui.Image((nint)textureId, imageSize, Vector2.Zero, Vector2.One, color);

			var labelTopLeft = cursorStartPos + new Vector2((boundingBoxSize.X - labelSize.X) / 2, imageSize.Y + itemSpacing.Y);
			ImGui.SetCursorScreenPos(labelTopLeft);
			ImGui.TextUnformatted(label);
		}

		private static void HorizontalLayout(string label, uint textureId, Vector2 imageSize, Vector2 labelSize, Vector2 boundingBoxSize, Vector2 itemSpacing, Vector4 color, Vector2 cursorStartPos)
		{
			ImGui.Image((nint)textureId, imageSize, Vector2.Zero, Vector2.One, color);
			var leftAlign = new Vector2(labelSize.X, boundingBoxSize.Y);
			ImGui.SetCursorScreenPos(cursorStartPos + new Vector2(imageSize.X + itemSpacing.X, 0));
			using (new Alignment.CenterWithin(labelSize, leftAlign))
			{
				ImGui.TextUnformatted(label);
			}
		}
	}
}
