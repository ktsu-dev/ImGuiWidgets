#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ktsu.io.ImGuiWidgets;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using ImGuiNET;
using ktsu.io.Extensions;
using ktsu.io.StrongPaths;
using Microsoft.Extensions.FileSystemGlobbing;

public enum PopupFilesystemBrowserMode
{
	Open,
	Save
}

public enum PopupFilesystemBrowserTarget
{
	File,
	Directory
}

/// <summary>
/// A class for displaying a prompt popup window.
/// </summary>
public class PopupFilesystemBrowser : PopupModal
{
	private PopupFilesystemBrowserMode BrowserMode { get; set; }
	private PopupFilesystemBrowserTarget BrowserTarget { get; set; }
	private Action<AbsoluteFilePath> OnChooseFile { get; set; } = (f) => { };
	private Action<AbsoluteDirectoryPath> OnChooseDirectory { get; set; } = (d) => { };
	[JsonInclude]
	private AbsoluteDirectoryPath CurrentDirectory { get; set; } = (AbsoluteDirectoryPath)Environment.CurrentDirectory;
	private Collection<AnyAbsolutePath> CurrentContents { get; set; } = new();
	private AnyAbsolutePath ChosenItem { get; set; } = new();
	private Collection<string> Drives { get; set; } = new();
	private string Glob { get; set; } = "*";
	private Matcher Matcher { get; set; } = new();

	public void FileOpen(string title, Action<AbsoluteFilePath> onChooseFile, string glob = "*") => File(title, PopupFilesystemBrowserMode.Open, onChooseFile, glob);

	public void FileSave(string title, Action<AbsoluteFilePath> onChooseFile, string glob = "*") => File(title, PopupFilesystemBrowserMode.Save, onChooseFile, glob);

	private void File(string title, PopupFilesystemBrowserMode mode, Action<AbsoluteFilePath> onChooseFile, string glob) => OpenPopup(title, mode, PopupFilesystemBrowserTarget.File, onChooseFile, (d) => { }, glob);

	public void ChooseDirectory(string title, Action<AbsoluteDirectoryPath> onChooseDirectory) => OpenPopup(title, PopupFilesystemBrowserMode.Open, PopupFilesystemBrowserTarget.Directory, (d) => { }, onChooseDirectory, "*");

	private void OpenPopup(string title, PopupFilesystemBrowserMode mode, PopupFilesystemBrowserTarget target, Action<AbsoluteFilePath> onChooseFile, Action<AbsoluteDirectoryPath> onChooseDirectory, string glob)
	{
		BrowserMode = mode;
		BrowserTarget = target;
		OnChooseFile = onChooseFile;
		OnChooseDirectory = onChooseDirectory;
		Glob = glob;
		Matcher = new();
		Matcher.AddInclude(Glob);
		Drives.Clear();
		Environment.GetLogicalDrives().ForEach(Drives.Add);
		RefreshContents();
		base.Open(title);
	}


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
		if (Drives.Count != 0)
		{
			if (ImGui.BeginCombo("##Drives", Drives[0]))
			{
				string currentDrive = CurrentDirectory.Split(Path.VolumeSeparatorChar).First() + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
				foreach (string drive in Drives)
				{
					if (ImGui.Selectable(drive, drive == currentDrive))
					{
						CurrentDirectory = (AbsoluteDirectoryPath)drive;
						RefreshContents();
					}
				}
				ImGui.EndCombo();
			}
		}
		ImGui.TextUnformatted($"{CurrentDirectory}{Path.DirectorySeparatorChar}{Glob}");
		ImGui.BeginChild("FilesystemBrowser", new(500, 400), border: false);
		ImGui.BeginTable(nameof(PopupFilesystemBrowser), 1, ImGuiTableFlags.Borders);
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 40);
		//ImGui.TableSetupColumn("Size", ImGuiTableColumnFlags.None, 3);
		//ImGui.TableSetupColumn("Modified", ImGuiTableColumnFlags.None, 3);
		ImGui.TableHeadersRow();

		var flags = ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick | ImGuiSelectableFlags.DontClosePopups;
		ImGui.TableNextRow();
		ImGui.TableNextColumn();
		if (ImGui.Selectable("..", false, flags))
		{
			if (ImGui.IsMouseDoubleClicked(0))
			{
				string? newPath = Path.GetDirectoryName(CurrentDirectory.WeakString.Trim(Path.DirectorySeparatorChar));
				if (newPath is not null)
				{
					CurrentDirectory = (AbsoluteDirectoryPath)newPath;
					RefreshContents();
				}
			}
		}

		foreach (var path in CurrentContents.OrderBy(p => p is not AbsoluteDirectoryPath).ThenBy(p => p).ToCollection())
		{
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			var directory = path as AbsoluteDirectoryPath;
			var file = path as AbsoluteFilePath;
			string displayPath = path.WeakString;
			displayPath = displayPath.RemovePrefix(CurrentDirectory).Trim(Path.DirectorySeparatorChar);

			if (directory is not null)
			{
				displayPath += Path.DirectorySeparatorChar;
			}

			if (ImGui.Selectable(displayPath, ChosenItem == path, flags))
			{
				if (directory is not null)
				{
					ChosenItem = directory;
					if (ImGui.IsMouseDoubleClicked(0))
					{
						CurrentDirectory = directory;
						RefreshContents();
					}
				}
				else if (file is not null)
				{
					ChosenItem = file;
					if (ImGui.IsMouseDoubleClicked(0))
					{
						ChooseItem();
					}
				}
			}
		}

		ImGui.EndTable();
		ImGui.EndChild();

		string confirmText = BrowserMode switch
		{
			PopupFilesystemBrowserMode.Open => "Open",
			PopupFilesystemBrowserMode.Save => "Save",
			_ => "Choose"
		};
		if (ImGui.Button(confirmText))
		{
			ChooseItem();
		}
		ImGui.SameLine();
		if (ImGui.Button("Cancel"))
		{
			ImGui.CloseCurrentPopup();
		}
	}

	private void ChooseItem()
	{
		if (ChosenItem is AbsoluteFilePath file)
		{
			OnChooseFile(file);
		}
		else if (ChosenItem is AbsoluteDirectoryPath directory)
		{
			OnChooseDirectory(directory);
		}
		ImGui.CloseCurrentPopup();
	}

	private void RefreshContents()
	{
		ChosenItem = new();
		CurrentContents.Clear();
		CurrentDirectory.Contents.ForEach(p =>
		{
			if (BrowserTarget == PopupFilesystemBrowserTarget.File || (BrowserTarget == PopupFilesystemBrowserTarget.Directory && p is AbsoluteDirectoryPath))
			{
				if (p is AbsoluteDirectoryPath || Matcher.Match(Path.GetFileName(p)).HasMatches)
				{
					CurrentContents.Add(p);
				}
			}
		});
	}
}
