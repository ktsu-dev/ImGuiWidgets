namespace ktsu.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;

public static partial class ImGuiWidgets
{
	public enum IconAlignment
	{
		Horizontal,
		Vertical,
	}

	public class IconDelegates
	{
		public Action? OnClick { get; init; }
		public Action? OnDoubleClick { get; init; }
		public Action? OnRightClick { get; init; }
		public Action? OnContextMenu { get; init; }
	}

	public static bool Icon(string label, uint textureId, float size, Vector4 color) => Icon(label, textureId, new Vector2(size), color, IconAlignment.Horizontal, new());
	public static bool Icon(string label, uint textureId, float size, Vector4 color, IconAlignment iconAlignment) => Icon(label, textureId, new Vector2(size), color, iconAlignment, new());
	public static bool Icon(string label, uint textureId, float size, Vector4 color, IconDelegates iconDelegates) => Icon(label, textureId, new Vector2(size), color, IconAlignment.Horizontal, iconDelegates);
	public static bool Icon(string label, uint textureId, float size, Vector4 color, IconAlignment iconAlignment, IconDelegates iconDelegates) => Icon(label, textureId, new Vector2(size), color, iconAlignment, iconDelegates);

	public static bool Icon(string label, uint textureId, Vector2 size, Vector4 color) => Icon(label, textureId, size, color, IconAlignment.Horizontal, new());
	public static bool Icon(string label, uint textureId, Vector2 size, Vector4 color, IconAlignment iconAlignment) => Icon(label, textureId, size, color, iconAlignment, new());
	public static bool Icon(string label, uint textureId, Vector2 size, Vector4 color, IconDelegates iconDelegates) => Icon(label, textureId, size, color, IconAlignment.Horizontal, iconDelegates);
	public static bool Icon(string label, uint textureId, Vector2 size, Vector4 color, IconAlignment iconAlignment, IconDelegates iconDelegates) =>
		IconImpl.Show(label, textureId, size, color, iconAlignment, iconDelegates);

	public static Vector2 CalcIconSize(string label, float size) => CalcIconSize(label, new Vector2(size), IconAlignment.Horizontal);
	public static Vector2 CalcIconSize(string label, float size, IconAlignment iconAlignment) => CalcIconSize(label, new Vector2(size), iconAlignment);
	public static Vector2 CalcIconSize(string label, Vector2 size) => CalcIconSize(label, size, IconAlignment.Horizontal);
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

	internal static class IconImpl
	{
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

		private static void HorizontalLayout(string label, uint textureId, Vector2 size, Vector2 itemSpacing, Vector4 color, Vector2 cursorStartPos)
		{
			ImGui.Image((nint)textureId, size, Vector2.Zero, Vector2.One, color);

			var labelSize = ImGui.CalcTextSize(label);
			ImGui.SetCursorScreenPos(cursorStartPos + new Vector2(size.X + itemSpacing.X, (size.Y - labelSize.Y) / 2));
			ImGui.TextUnformatted(label);
		}
	}
}
