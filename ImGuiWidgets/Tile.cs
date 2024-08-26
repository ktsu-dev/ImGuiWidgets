namespace ktsu.io.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;

public static class Tile
{
	public class ResponseDelegates
	{
		public Action? OnClick { get; init; }
		public Action? OnDoubleClick { get; init; }
		public Action? OnRightClick { get; init; }
		public Action? OnContextMenu { get; init; }
	}

	public static bool Show(string id, float width, Action? onShow, ResponseDelegates responseDelegates)
	{
		bool wasClicked = false;

		if (ImGui.BeginChild(id, new(width, 0), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
		{
			var cursorPos = ImGui.GetCursorPos();
			var cursorScreenPos = ImGui.GetCursorScreenPos();

			onShow?.Invoke();

			var endCursorPos = ImGui.GetCursorPos();
			float height = endCursorPos.Y - cursorPos.Y;
			var frameSize = new Vector2(width, height);
			ImGui.SetCursorPos(cursorPos + Vector2.Zero);
			ImGui.Dummy(frameSize);
			bool isHovered = ImGui.IsItemHovered();

			if (isHovered)
			{
				uint color = ImGui.GetColorU32(ImGui.GetStyle().Colors[(int)ImGuiCol.Border]);
				ImGui.GetWindowDrawList().AddRect(cursorScreenPos, cursorScreenPos + frameSize, color);
				if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
				{
					responseDelegates.OnClick?.Invoke();
					wasClicked = true;
				}
				if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
				{
					responseDelegates.OnDoubleClick?.Invoke();
				}
				if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
				{
					responseDelegates.OnRightClick?.Invoke();
				}
				if (ImGui.IsMouseReleased(ImGuiMouseButton.Right))
				{
					ImGui.OpenPopup($"{id}_Context");
				}
			}

			if (ImGui.BeginPopup($"{id}_Context"))
			{
				responseDelegates.OnContextMenu?.Invoke();
				ImGui.EndPopup();
			}
		}
		ImGui.EndChild();

		return wasClicked;
	}

	public static bool Show(string id, float width, Action? onShow) => Show(id, width, onShow, new());
}
