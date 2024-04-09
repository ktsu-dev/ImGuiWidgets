namespace ktsu.io.ImGuiWidgetsDemo;

using ImGuiNET;
using ktsu.io.ImGuiApp;
using ktsu.io.ImGuiWidgets;

internal class ImGuiWidgetsDemo
{
	private static void Main()
	{
		ImGuiWidgetsDemo imGuiWidgetsDemo = new();
		ImGuiApp.Start(nameof(ImGuiWidgetsDemo), new ImGuiAppWindowState(), imGuiWidgetsDemo.OnStart, imGuiWidgetsDemo.OnTick, imGuiWidgetsDemo.OnMenu, imGuiWidgetsDemo.OnWindowResized);
	}

	private static float value = 0.5f;
	private static string inputString = "String Input Popup";

	private bool ShouldOpenOKPopup { get; set; }

	private readonly PopupInputString popupInputString = new();
	private readonly PopupFilesystemBrowser popupFilesystemBrowser = new();
	private readonly PopupMessageOK popupMessageOK = new();
	private readonly PopupSearchableList<string> popupSearchableList = new();
	private string OKPopupMessage { get; set; } = string.Empty;
	private string OKPopupTitle { get; set; } = string.Empty;
	private DividerContainer DividerContainer { get; } = new("DemoDividerContainer");

	internal static readonly string[] Friends = new[] { "James", "Cameron", "Matt", "Troy", "Hali" };

	private void OnStart()
	{
		DividerContainer.Add(new("Left", 0.25f, ShowLeftPanel));
		DividerContainer.Add(new("Right", 0.75f, ShowRightPanel));
	}

	private void OnTick(float dt) => DividerContainer.Tick(dt);

	private void OnMenu()
	{
	}

	private void OnWindowResized()
	{
	}

	private void ShowLeftPanel(float size)
	{
		ImGui.Text("Left Divider Zone");

		Knob.Draw("Value", ref value, 0f, 1f, 150f);
		ColorIndicator.Show(Color.Red, true);
		ImGui.SameLine();
		ColorIndicator.Show(Color.Red, false);
		ImGui.SameLine();
		ColorIndicator.Show(Color.Green, true);
		ImGui.SameLine();
		ColorIndicator.Show(Color.Green, false);

		ImGui.Button("Hello, Tree!");
		using (var tree = new Tree())
		{
			for (int i = 0; i < 5; i++)
			{
				using (tree.Child)
				{
					ImGui.Button($"Hello, Child {i}!");
					using (var subtree = new Tree())
					{
						using (subtree.Child)
						{
							ImGui.Button($"Hello, Grandchild!");
						}
					}
				}
			}
		}
	}

	private void ShowRightPanel(float size)
	{
		ImGui.Text("Right Divider Zone");

		if (ImGui.Button(inputString))
		{
			popupInputString.Open("Enter a string", "Enter", "Yeet", (string result) =>
			{
				inputString = result;
			});
		}

		if (ImGui.Button("Open File"))
		{
			popupFilesystemBrowser.FileOpen("Open File", (f) =>
			{
				ShouldOpenOKPopup = true;
				OKPopupTitle = "File Chosen";
				OKPopupMessage = $"You chose: {f}";
			}, "*.cs");
		}

		if (ImGui.Button("Save File"))
		{
			popupFilesystemBrowser.FileSave("Save File", (f) =>
			{
				ShouldOpenOKPopup = true;
				OKPopupTitle = "File Chosen";
				OKPopupMessage = $"You chose: {f}";
			}, "*.cs");
		}

		if (ImGui.Button("Choose Best Friend"))
		{
			popupSearchableList.Open("Best Friend", "Who is your best friend?", Friends, (string result) =>
			{
				ShouldOpenOKPopup = true;
				OKPopupTitle = "Best Friend Chosen";
				OKPopupMessage = $"You chose: {result}";
			});
		}

		if (ShouldOpenOKPopup)
		{
			popupMessageOK.Open(OKPopupTitle, OKPopupMessage);
			ShouldOpenOKPopup = false;
		}

		popupSearchableList.ShowIfOpen();
		popupMessageOK.ShowIfOpen();
		popupInputString.ShowIfOpen();
		popupFilesystemBrowser.ShowIfOpen();
	}
}
