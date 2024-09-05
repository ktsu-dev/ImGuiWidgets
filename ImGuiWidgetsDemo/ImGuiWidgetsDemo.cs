namespace ktsu.io.ImGuiWidgetsDemo;

using System.Numerics;
using ImGuiNET;
using ktsu.io.ImGuiApp;
using ktsu.io.ImGuiStyler;
using ktsu.io.ImGuiWidgets;
using ktsu.io.StrongPaths;

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
	private ImGuiWidgets.DividerContainer DividerContainer { get; } = new("DemoDividerContainer");

	internal static readonly string[] Friends = ["James", "Cameron", "Matt", "Troy", "Hali"];

	private void OnStart()
	{
		DividerContainer.Add(new("Left", 0.25f, ShowLeftPanel));
		DividerContainer.Add(new("Right", 0.75f, ShowRightPanel));
	}

	private void OnTick(float dt) => DividerContainer.Tick(dt);

	private void OnMenu()
	{
		// Method intentionally left empty.
	}

	private void OnWindowResized()
	{
		// Method intentionally left empty.
	}

	private void ShowLeftPanel(float size)
	{
		ImGui.Text("Left Divider Zone");

		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Wiper) + "Test Pascal Case", ref value, 0, 1, 0, null, ImGuiKnobVariant.Wiper);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.WiperOnly), ref value, 0, 1, 0, null, ImGuiKnobVariant.WiperOnly);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.WiperDot), ref value, 0, 1, 0, null, ImGuiKnobVariant.WiperDot);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Tick), ref value, 0, 1, 0, null, ImGuiKnobVariant.Tick);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Stepped), ref value, 0, 1, 0, null, ImGuiKnobVariant.Stepped);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Space), ref value, 0, 1, 0, null, ImGuiKnobVariant.Space);
		ImGuiWidgets.Knob("Throttle Position", ref value, 0, 1, 0, null, ImGuiKnobVariant.Space);

		ImGuiWidgets.ColorIndicator(Color.Red, true);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Red, false);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Green, true);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Green, false);

		ImGui.Button("Hello, Tree!");
		using (var tree = new ImGuiWidgets.Tree())
		{
			for (int i = 0; i < 5; i++)
			{
				using (tree.Child)
				{
					ImGui.Button($"Hello, Child {i}!");
					using (var subtree = new ImGuiWidgets.Tree())
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

		var ktsuIconPath = (AbsoluteDirectoryPath)Environment.CurrentDirectory / (FileName)"ktsu.png";
		var ktsuTexture = ImGuiApp.GetOrLoadTexture(ktsuIconPath);

		if (ImGuiWidgets.Image(ktsuTexture.TextureId, new(128, 128)))
		{
			ShouldOpenOKPopup = true;
			OKPopupTitle = "Click";
			OKPopupMessage = $"You chose the image";
		}

		float tileWidthEms = 10;
		float iconWidthEms = 7.5f;
		float tilePaddingEms = 0.5f;
		float tileWidthPx = ImGuiApp.EmsToPx(tileWidthEms);
		float iconWidthPx = ImGuiApp.EmsToPx(iconWidthEms);
		float tilePaddingPx = ImGuiApp.EmsToPx(tilePaddingEms);

		var iconSize = new Vector2(iconWidthPx, iconWidthPx);


		if (ImGuiWidgets.Tile("Tile1", tileWidthPx, tilePaddingPx, () =>
		{
			ImGuiWidgets.ImageCenteredWithin(ktsuTexture.TextureId, iconSize, tileWidthPx);
			ImGuiWidgets.TextCenteredWithin("Click me", tileWidthPx, clip: true);
		}))
		{
			ShouldOpenOKPopup = true;
			OKPopupTitle = "Click";
			OKPopupMessage = $"Yippee!";
		}

		ImGui.SameLine(0, 0);
		_ = ImGuiWidgets.Tile("Tile2", tileWidthPx, tilePaddingPx, () =>
		{
			ImGuiWidgets.ImageCenteredWithin(ktsuTexture.TextureId, iconSize, tileWidthPx);
			ImGuiWidgets.TextCenteredWithin("Double Click Me", tileWidthPx, clip: true);
		},
		new()
		{
			OnDoubleClick = () =>
			{
				ShouldOpenOKPopup = true;
				OKPopupTitle = "Double Click";
				OKPopupMessage = $"Yippee!";
			},
		});

		ImGui.SameLine(0, 0);
		_ = ImGuiWidgets.Tile("Tile3", tileWidthPx, tilePaddingPx, () =>
		{
			ImGuiWidgets.ImageCenteredWithin(ktsuTexture.TextureId, iconSize, tileWidthPx);
			ImGuiWidgets.TextCenteredWithin("Right Click Me", tileWidthPx, clip: true);
		},
		new()
		{
			OnContextMenu = () =>
			{
				ImGui.MenuItem("Context Menu Item 1");
				ImGui.MenuItem("Context Menu Item 2");
				ImGui.MenuItem("Context Menu Item 3");
			},
		});

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
