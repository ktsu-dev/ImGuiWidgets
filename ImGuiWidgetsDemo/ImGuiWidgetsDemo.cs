namespace ktsu.io.ImGuiWidgetsDemo;

using ImGuiNET;
using ktsu.io.ImGuiApp;
using ktsu.io.ImGuiWidgets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
internal class ImGuiWidgetsDemo
{
	private static void Main(string[] args)
	{
		ImGuiWidgetsDemo imGuiWidgetsDemo = new();
		ImGuiApp.Start(nameof(ImGuiWidgetsDemo), new ImGuiAppWindowState(), imGuiWidgetsDemo.Tick, imGuiWidgetsDemo.ShowMenu, imGuiWidgetsDemo.WindowResized);
	}

	private static float value = 0.5f;

	private readonly PopupInputString popupInputString = new();
	private string inputString = "String Input Popup";
	private void Tick(float dt)
	{
		float ms = dt * 1000;
		Knob.Draw("DT", ref ms, 0, 10, 150f);
		ImGui.SameLine();
		Knob.Draw("Value", ref value, 0f, 1f, 150f);
		if (ImGui.Button(inputString))
		{
			popupInputString.Open("Enter a string", "Enter", "Yeet", (string result) =>
			{
				inputString = result;
			});
		}

		popupInputString.ShowIfOpen();
	}

	private void ShowMenu()
	{
	}

	private void WindowResized()
	{
	}
}
