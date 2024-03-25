#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

namespace ktsu.io.ImGuiWidgets;

using System;
using System.Collections.Generic;

/// <summary>
/// A class for displaying a prompt popup window.
/// </summary>
public class PopupMessageOK : PopupPrompt
{
	/// <summary>
	/// Open the popup and set the title and message.
	/// </summary>
	/// <param name="title">The title of the popup window.</param>
	/// <param name="message">The message to show.</param>
	public void Open(string title, string message) => base.Open(title, message, new() { { "OK", null } });

	/// <summary>
	/// Dont use this method, use the other Open method
	/// </summary>
	[Obsolete("Use the other Open method.")]
	public override void Open(string title) => throw new InvalidOperationException("Use the other Open method.");

	/// <summary>
	/// Dont use this method, use the other Open method
	/// </summary>
	[Obsolete("Use the other Open method.")]
	public override void Open(string title, string label, Dictionary<string, Action?> buttons) => throw new InvalidOperationException("Use the other Open method.");
}
