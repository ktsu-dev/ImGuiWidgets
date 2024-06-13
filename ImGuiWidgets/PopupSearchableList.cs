#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

namespace ktsu.io.ImGuiWidgets;

using System;
using ImGuiNET;
using ktsu.io.FuzzySearch;

/// <summary>
/// A popup window to allow the user to search and select an item from a list
/// </summary>
/// <typeparam name="TItem">The type of the list elements</typeparam>
public class PopupSearchableList<TItem> : PopupModal where TItem : class
{
	private TItem? cachedValue;
	private TItem? selectedItem;
	private string searchTerm = string.Empty;
	private Action<TItem> OnConfirm { get; set; } = null!;
	private Func<TItem, string>? GetText { get; set; }
	private string Label { get; set; } = string.Empty;
	private IEnumerable<TItem> Items { get; set; } = [];

	/// <summary>
	/// Open the popup and set the title, label, and default value.
	/// </summary>
	/// <param name="title">The title of the popup window.</param>
	/// <param name="label">The label of the input field.</param>
	/// /// <param name="items">The items to select from.</param>
	/// <param name="defaultItem">The default value of the input field.</param>
	/// <param name="getText">A delegate to get the text representation of an item.</param>
	/// <param name="onConfirm">A callback to handle the new input value.</param>
	public void Open(string title, string label, IEnumerable<TItem> items, TItem? defaultItem, Func<TItem, string>? getText, Action<TItem> onConfirm)
	{
		searchTerm = string.Empty;
		Label = label;
		OnConfirm = onConfirm;
		GetText = getText;
		cachedValue = defaultItem;
		Items = items;
		base.Open(title);
	}

	/// <summary>
	/// Open the popup and set the title, label, and default value.
	/// </summary>
	/// <param name="title">The title of the popup window.</param>
	/// <param name="label">The label of the input field.</param>
	/// <param name="items">The items to select from.</param>
	/// <param name="onConfirm">A callback to handle the new input value.</param>
	public void Open(string title, string label, IEnumerable<TItem> items, Action<TItem> onConfirm) => Open(title, label, items, null, null, onConfirm);

	/// <summary>
	/// Open the popup and set the title, label, and default value.
	/// </summary>
	/// <param name="title">The title of the popup window.</param>
	/// <param name="label">The label of the input field.</param>
	/// <param name="items">The items to select from.</param>
	/// <param name="getText">A delegate to get the text representation of an item.</param>
	/// <param name="onConfirm">A callback to handle the new input value.</param>
	public void Open(string title, string label, IEnumerable<TItem> items, Func<TItem, string> getText, Action<TItem> onConfirm) => Open(title, label, items, null, getText, onConfirm);

	/// <summary>
	/// Dont use this method, use the other Open method
	/// </summary>
	[Obsolete("Use the other Open method.")]
	public override void Open(string title) => throw new InvalidOperationException("Use the other Open method.");

	/// <summary>
	/// Show the content of the popup.
	/// </summary>
	protected override void ShowContent()
	{
		ImGui.TextUnformatted(Label);
		ImGui.NewLine();
		if (!WasOpen && !ImGui.IsItemFocused())
		{
			ImGui.SetKeyboardFocusHere();
		}

		if (ImGui.InputText("##Search", ref searchTerm, 255, ImGuiInputTextFlags.EnterReturnsTrue))
		{
			var confirmedItem = cachedValue ?? selectedItem;
			if (confirmedItem is not null)
			{
				OnConfirm(confirmedItem);
				ImGui.CloseCurrentPopup();
			}
		}

		var sortedItems = Items.OrderByDescending(x =>
		{
			string itemString = x.ToString() ?? string.Empty;
			Fuzzy.Contains(itemString, searchTerm, out int score);
			return score;
		});

		ImGui.BeginListBox("##List");
		selectedItem = null;
		foreach (var item in sortedItems)
		{
			//if nothing has been explicitly selected, select the first item which will be the best match
			if (selectedItem is null && cachedValue is null)
			{
				selectedItem = item;
			}

			string displayText = GetText?.Invoke(item) ?? item.ToString() ?? string.Empty;

			if (ImGui.Selectable(displayText, item == (cachedValue ?? selectedItem)))
			{
				cachedValue = item;
			}
		}
		ImGui.EndListBox();

		if (ImGui.Button($"OK###{PopupName}_OK"))
		{
			var confirmedItem = cachedValue ?? selectedItem;
			if (confirmedItem is not null)
			{
				OnConfirm(confirmedItem);
				ImGui.CloseCurrentPopup();
			}
		}

		ImGui.SameLine();
		if (ImGui.Button($"Cancel###{PopupName}_Cancel"))
		{
			ImGui.CloseCurrentPopup();
		}
	}
}
