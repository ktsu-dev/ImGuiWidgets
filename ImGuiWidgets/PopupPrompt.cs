namespace ktsu.io.ImGuiWidgets;

using System;
using System.Collections.Generic;
using ImGuiNET;

/// <summary>
/// A class for displaying a prompt popup window.
/// </summary>
public class PopupPrompt : PopupModal
{
	private string Label { get; set; } = string.Empty;
	private Dictionary<string, Action?> Buttons { get; set; } = new();

	/// <summary>
	/// Open the popup and set the title, label, and button definitions.
	/// </summary>
	/// <param name="title">The title of the popup window.</param>
	/// <param name="label">The label of the input field.</param>
	/// <param name="buttons">The names and actions of the buttons.</param>
	public void Open(string title, string label, Dictionary<string, Action?> buttons)
	{
		Label = label;
		Buttons = buttons;
		base.Open(title);
	}

	/// <summary>
	/// Dont use this method, use the other Open method
	/// </summary>
	public override void Open(string title) => throw new InvalidOperationException("Use the other Open method.");

	/// <summary>
	/// Show the content of the popup.
	/// </summary>
	protected override void ShowContent()
	{
		ImGui.TextUnformatted(Label);
		ImGui.NewLine();

		foreach (var (text, action) in Buttons)
		{
			if (ImGui.Button(text))
			{
				action?.Invoke();
				ImGui.CloseCurrentPopup();
			}
			ImGui.SameLine();
		}
	}
}
