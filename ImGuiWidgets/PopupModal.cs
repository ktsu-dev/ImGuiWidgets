namespace ktsu.io.ImGuiWidgets;

using ImGuiNET;
using ktsu.io.CaseConverter;

/// <summary>
/// Base class for a modal popup window.
/// </summary>
public abstract class PopupModal
{
	private string Title { get; set; } = string.Empty;

	/// <summary>
	/// Returns false if this is the first frame teh popup is shown, true on subsequent frames.
	/// </summary>
	protected bool WasOpen { get; set; }

	/// <summary>
	/// Gets the id of the popup window.
	/// </summary>
	/// <returns>The id of the popup window.</returns>
	protected string PopupName => $"{Title}###PopupInput_{Title.ToSnakeCase()}";

	/// <summary>
	/// Open the popup and set the title.
	/// </summary>
	/// <param name="title">The title of the popup window.</param>
	public virtual void Open(string title)
	{
		Title = title;
		ImGui.OpenPopup(PopupName);
	}

	/// <summary>
	/// Show the content of the popup.
	/// </summary>
	protected abstract void ShowContent();

	/// <summary>
	/// Show the popup if it is open.
	/// </summary>
	/// <returns>True if the popup is open.</returns>
	public bool ShowIfOpen()
	{
		bool result = ImGui.IsPopupOpen(PopupName);
		if (ImGui.BeginPopupModal(PopupName, ref result, ImGuiWindowFlags.AlwaysAutoResize))
		{
			ShowContent();

			if (ImGui.IsKeyPressed(ImGuiKey.Escape))
			{
				ImGui.CloseCurrentPopup();
			}

			ImGui.EndPopup();
		}

		WasOpen = result;
		return result;
	}
}
