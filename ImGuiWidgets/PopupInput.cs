namespace ktsu.io.ImGuiWidgets;

using ImGuiNET;

/// <summary>
/// Base class for a popup input window.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TDerived">The type of the derived class for CRTP.</typeparam>
public abstract class PopupInput<TInput, TDerived> where TDerived : PopupInput<TInput, TDerived>, new()
{
	public static bool Show(string title, string label, ref TInput input)
	{
		bool result = false;
		if (ImGui.IsPopupOpen(title))
		{
			ImGui.OpenPopup(title);
		}

		if (ImGui.BeginPopupModal(title, ref result, ImGuiWindowFlags.AlwaysAutoResize))
		{
			ImGui.Text(label);
			ImGui.NewLine();
			if (new TDerived().ShowEdit(ref input))
			{
				ImGui.CloseCurrentPopup();
			}
			ImGui.SameLine();
			if (ImGui.Button("OK"))
			{
				ImGui.CloseCurrentPopup();
			}
			ImGui.EndPopup();
		}
		return result;
	}

	protected abstract bool ShowEdit(ref TInput input);
}

public class PopupInputString : PopupInput<string, PopupInputString>
{
	protected override bool ShowEdit(ref string input) => ImGui.InputText("##Input", ref input, 100);
}
