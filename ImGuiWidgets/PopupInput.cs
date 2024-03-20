namespace ktsu.io.ImGuiWidgets;

using ImGuiNET;

/// <summary>
/// Base class for a popup input window.
/// </summary>
/// <typeparam name="TInput">The type of the input value.</typeparam>
/// <typeparam name="TDerived">The type of the derived class for CRTP.</typeparam>
public abstract class PopupInput<TInput, TDerived> : PopupModal where TDerived : PopupInput<TInput, TDerived>, new()
{
	private TInput? cachedValue;
	private Action<TInput> OnConfirm { get; set; } = null!;
	private string Label { get; set; } = string.Empty;

	/// <summary>
	/// Open the popup and set the title, label, and default value.
	/// </summary>
	/// <param name="title">The title of the popup window.</param>
	/// <param name="label">The label of the input field.</param>
	/// <param name="defaultValue">The default value of the input field.</param>
	/// <param name="onConfirm">A callback to handle the new input value.</param>
	public void Open(string title, string label, TInput defaultValue, Action<TInput> onConfirm)
	{
		Label = label;
		OnConfirm = onConfirm;
		cachedValue = defaultValue;
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
		if (cachedValue is not null)
		{
			ImGui.TextUnformatted(Label);
			ImGui.NewLine();

			if (!WasOpen && !ImGui.IsItemFocused())
			{
				ImGui.SetKeyboardFocusHere();
			}

			if (ShowEdit(ref cachedValue))
			{
				OnConfirm(cachedValue);
				ImGui.CloseCurrentPopup();
			}

			ImGui.SameLine();
			if (ImGui.Button($"OK###{PopupName}_OK"))
			{
				OnConfirm(cachedValue);
				ImGui.CloseCurrentPopup();
			}
		}
	}

	/// <summary>
	/// Show the input field for the derived class.
	/// </summary>
	/// <param name="value">The input value.</param>
	/// <returns>True if the input field is changed.</returns>
	protected abstract bool ShowEdit(ref TInput value);
}

/// <summary>
/// A popup input window for strings.
/// </summary>
public class PopupInputString : PopupInput<string, PopupInputString>
{
	/// <summary>
	/// Show the input field for strings.
	/// </summary>
	/// <param name="value">The input value.</param>
	/// <returns>True if Enter is pressed.</returns>
	protected override bool ShowEdit(ref string value) => ImGui.InputText($"###{PopupName}_INPUT", ref value, 100, ImGuiInputTextFlags.EnterReturnsTrue);
}

/// <summary>
/// A popup input window for integers.
/// </summary>
public class PopupInputInt : PopupInput<int, PopupInputInt>
{
	/// <summary>
	/// Show the input field for integers.
	/// </summary>
	/// <param name="value">The input value.</param>
	/// <returns>False</returns>
	protected override bool ShowEdit(ref int value)
	{
		ImGui.InputInt("##Input", ref value);
		return false;
	}
}

/// <summary>
/// A popup input window for floats.
/// </summary>
public class PopupInputFloat : PopupInput<float, PopupInputFloat>
{
	/// <summary>
	/// Show the input field for floats.
	/// </summary>
	/// <param name="value">The input value.</param>
	/// <returns>False</returns>
	protected override bool ShowEdit(ref float value)
	{
		ImGui.InputFloat("##Input", ref value);
		return false;
	}
}
