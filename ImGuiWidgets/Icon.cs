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

		/// <summary>
		/// Returns the tooltip for the Icon
		/// </summary>
		public Func<string>? OnGetTooltip { get; init; }
	}

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="color">The color of the icon.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, float imageSize, Vector4 color) => Icon(label, textureId, new Vector2(imageSize), color, IconAlignment.Horizontal, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, float imageSize, Vector4 color, IconAlignment iconAlignment) => Icon(label, textureId, new Vector2(imageSize), color, iconAlignment, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconDelegates">The delegates for icon events.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, float imageSize, Vector4 color, IconDelegates iconDelegates) => Icon(label, textureId, new Vector2(imageSize), color, IconAlignment.Horizontal, iconDelegates);

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <param name="iconDelegates">The delegates for icon events.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, float imageSize, Vector4 color, IconAlignment iconAlignment, IconDelegates iconDelegates) => Icon(label, textureId, new Vector2(imageSize), color, iconAlignment, iconDelegates);

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="color">The color of the icon.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, Vector2 imageSize, Vector4 color) => Icon(label, textureId, imageSize, color, IconAlignment.Horizontal, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, Vector2 imageSize, Vector4 color, IconAlignment iconAlignment) => Icon(label, textureId, imageSize, color, iconAlignment, new());

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconDelegates">The delegates for icon events.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, Vector2 imageSize, Vector4 color, IconDelegates iconDelegates) => Icon(label, textureId, imageSize, color, IconAlignment.Horizontal, iconDelegates);

	/// <summary>
	/// Renders an icon with the specified parameters.
	/// </summary>
	/// <param name="label">The label of the icon.</param>
	/// <param name="textureId">The texture ID of the icon.</param>
	/// <param name="imageSize">The size of the image.</param>
	/// <param name="color">The color of the icon.</param>
	/// <param name="iconAlignment">The alignment of the icon.</param>
	/// <param name="iconDelegates">The delegates for icon events.</param>
	/// <returns>True if the icon was clicked; otherwise, false.</returns>
	public static bool Icon(string label, uint textureId, Vector2 imageSize, Vector4 color, IconAlignment iconAlignment, IconDelegates iconDelegates)
	{
		ArgumentNullException.ThrowIfNull(label);
		ArgumentNullException.ThrowIfNull(iconDelegates);

		return IconImpl.Show(label, textureId, imageSize, color, iconAlignment, iconDelegates);
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
		/// <summary>
		/// Shows the icon with the specified parameters.
		/// </summary>
		/// <param name="label">The label of the icon.</param>
		/// <param name="textureId">The texture ID of the icon.</param>
		/// <param name="imageSize">The size of the image.</param>
		/// <param name="color">The color of the icon.</param>
		/// <param name="iconAlignment">The alignment of the icon.</param>
		/// <param name="iconDelegates">The delegates for icon events.</param>
		/// <returns>True if the icon was clicked; otherwise, false.</returns>
		public static bool Show(string label, uint textureId, Vector2 imageSize, Vector4 color, IconAlignment iconAlignment, IconDelegates iconDelegates)
		{
			bool wasClicked = false;

			var style = ImGui.GetStyle();
			var framePadding = style.FramePadding;
			var itemSpacing = style.ItemSpacing;

			ImGui.PushID(label);

			var cursorStartPos = ImGui.GetCursorScreenPos();
			var labelSize = ImGui.CalcTextSize(label);// TODO, maybe pass this to an internal overload of CalcIconSize to save recalculating
			var boundingBoxSize = CalcIconSize(label, imageSize, iconAlignment);

			ImGui.SetCursorScreenPos(cursorStartPos + framePadding);

			switch (iconAlignment)
			{
				case IconAlignment.Horizontal:
					HorizontalLayout(label, textureId, imageSize, labelSize, boundingBoxSize, itemSpacing, color, cursorStartPos);
					break;
				case IconAlignment.Vertical:
					VerticalLayout(label, textureId, imageSize, labelSize, boundingBoxSize, itemSpacing, color, cursorStartPos);
					break;
				default:
					throw new NotImplementedException();
			}

			ImGui.SetCursorScreenPos(cursorStartPos);
			ImGui.Dummy(boundingBoxSize);
			bool isHovered = ImGui.IsItemHovered();
			if (iconDelegates.OnGetTooltip is not null)
			{
				ImGui.SetItemTooltip(iconDelegates.OnGetTooltip());
			}
			if (isHovered || EnableIconDebugDraw)
			{
				uint borderColor = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
				var drawList = ImGui.GetWindowDrawList();
				drawList.AddRect(cursorStartPos, cursorStartPos + boundingBoxSize, ImGui.GetColorU32(borderColor));
			}

			if (isHovered)
			{
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
