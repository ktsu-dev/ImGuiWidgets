## v1.6.1 (patch)

Changes since v1.6.0:

- Update packages ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.6.0 (minor)

Changes since v1.5.0:

- Add LICENSE template ([@matt-edmondson](https://github.com/matt-edmondson))
- Readd icon to fix LFS ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove icon to fix LFS ([@matt-edmondson](https://github.com/matt-edmondson))
- Update packages ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.5.2 (patch)

Changes since v1.5.1:

- Update packages ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.5.1 (patch)

Changes since v1.5.0:

- Readd icon to fix LFS ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove icon to fix LFS ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.5.0 (minor)

Changes since v1.4.0:

- Adjust grid item slider to allow for testing zero items in the grid ([@matt-edmondson](https://github.com/matt-edmondson))
- Display a dummy and early out if there are no items to show in the grid ([@matt-edmondson](https://github.com/matt-edmondson))
- Dont render a dummy if theres no items and the layout is "FitToContents" ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.4.0 (minor)

Changes since v1.3.0:

- Enhance code style and improve UI widget functionality ([@matt-edmondson](https://github.com/matt-edmondson))
- Update .gitignore to include additional IDE and OS-specific files ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.3.1 (patch)

Changes since v1.3.0:

- Update .gitignore to include additional IDE and OS-specific files ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.3.0 (minor)

Changes since v1.2.0:

- Refactor array initializations and clean up code ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove debugging trace from changelog and version scripts ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.2.1 (patch)

Changes since v1.2.0:

- Remove debugging trace from changelog and version scripts ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.2.0 (minor)

Changes since v1.1.0:

- [minor] Add last non-merge commit message to environment variables output ([@matt-edmondson](https://github.com/matt-edmondson))
- [minor] Fix command to retrieve the last non-merge commit message with a limit to one entry ([@matt-edmondson](https://github.com/matt-edmondson))
- [minor] Fix commit retrieval logic to use date order for last non-merge commit and topo order for subsequent commits ([@matt-edmondson](https://github.com/matt-edmondson))
- [minor] Force minor update ([@matt-edmondson](https://github.com/matt-edmondson))
- [minor] Refactor version increment logic to use Contains method for commit message tags ([@matt-edmondson](https://github.com/matt-edmondson))
- Add a new Combo widget that works with the enum type ([@Damon3000s](https://github.com/Damon3000s))
- Add an IString and string combo implementation ([@Damon3000s](https://github.com/Damon3000s))
- Add another API method ([@Damon3000s](https://github.com/Damon3000s))
- Add conditional checks for release tasks in CI workflow ([@matt-edmondson](https://github.com/matt-edmondson))
- Add GitHub token to workflow for fork detection ([@matt-edmondson](https://github.com/matt-edmondson))
- Add icon back hopefully to LFS ([@matt-edmondson](https://github.com/matt-edmondson))
- Add IS_FORK environment variable to GitHub Actions workflow ([@matt-edmondson](https://github.com/matt-edmondson))
- Add ScopedDisabled to ImGuiWidgets ([@Damon3000s](https://github.com/Damon3000s))
- Add the new combo to the ImGuiWidgets demo project ([@Damon3000s](https://github.com/Damon3000s))
- Add version sorting suffixes for alpha, beta, rc, and pre releases, fixes bug where version increment could be wrong ([@matt-edmondson](https://github.com/matt-edmondson))
- Added OnGetTooltip to Icon Delegates ([@Damon3000s](https://github.com/Damon3000s))
- Change the ColorIndicator implementation based on review feedback ([@Damon3000s](https://github.com/Damon3000s))
- Changed formatting of collection member variable ([@Damon3000s](https://github.com/Damon3000s))
- Cleanup ([@Damon3000s](https://github.com/Damon3000s))
- Cleanup old logic ([@Damon3000s](https://github.com/Damon3000s))
- Convert enum usages of ImGui.Combo to ImGuiWidgets.Combo ([@Damon3000s](https://github.com/Damon3000s))
- Default back to 32 grid items to show ([@Damon3000s](https://github.com/Damon3000s))
- Delete icon to try fix lfs ([@matt-edmondson](https://github.com/matt-edmondson))
- Don't call measureDelegate twice ([@Damon3000s](https://github.com/Damon3000s))
- Enhance versioning logic to increment based on commit message tags ([@matt-edmondson](https://github.com/matt-edmondson))
- Feedback ([@Damon3000s](https://github.com/Damon3000s))
- First pass of adding row alignment and fixing layout issues ([@Damon3000s](https://github.com/Damon3000s))
- First pass of implementing a new Column Major grid ([@Damon3000s](https://github.com/Damon3000s))
- Fix an issue where incrementing from a stable to prerelease version would incorrectly be attributed as a patch ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix ColumnMajor crashing when there isn't enough vertical space ([@Damon3000s](https://github.com/Damon3000s))
- Fix demo ([@Damon3000s](https://github.com/Damon3000s))
- Fix for grid double counting item spacing, yet only using a single during layout ([@Damon3000s](https://github.com/Damon3000s))
- Fix for potential 0 column situation ([@Damon3000s](https://github.com/Damon3000s))
- Fix for potential out of range access in ColumnMajor mode ([@Damon3000s](https://github.com/Damon3000s))
- Fix for SetPos ImGui assert ([@Damon3000s](https://github.com/Damon3000s))
- Fix IDE style warnings ([@Damon3000s](https://github.com/Damon3000s))
- Fix when grid calculates 0 columns ([@Damon3000s](https://github.com/Damon3000s))
- Group code with regions ([@Damon3000s](https://github.com/Damon3000s))
- Improve implementation ([@Damon3000s](https://github.com/Damon3000s))
- Large refactot ([@Damon3000s](https://github.com/Damon3000s))
- Minor formatting fix. ([@Damon3000s](https://github.com/Damon3000s))
- More renames and cleanup ([@Damon3000s](https://github.com/Damon3000s))
- More target type new expressions ([@Damon3000s](https://github.com/Damon3000s))
- Move IconAlignment to a parameter ([@Damon3000s](https://github.com/Damon3000s))
- Move ScopedId to be within ImGuiWidgets class ([@Damon3000s](https://github.com/Damon3000s))
- Re-implement RowMajor ([@Damon3000s](https://github.com/Damon3000s))
- Refactor ColorIndicator ([@Damon3000s](https://github.com/Damon3000s))
- Remove the IconDelegates class ([@Damon3000s](https://github.com/Damon3000s))
- Resolving merge issues ([@Damon3000s](https://github.com/Damon3000s))
- Revert additional files ([@Damon3000s](https://github.com/Damon3000s))
- Review feedback ([@Damon3000s](https://github.com/Damon3000s))
- Review Feedback ([@Damon3000s](https://github.com/Damon3000s))
- Review feedback ([@Damon3000s](https://github.com/Damon3000s))
- Review Feedback and general improvements ([@Damon3000s](https://github.com/Damon3000s))
- Simplify the assert fix ([@Damon3000s](https://github.com/Damon3000s))
- Tweak code layout ([@Damon3000s](https://github.com/Damon3000s))
- Update GitHub Actions workflow to check if the repository is a fork using GitHub CLI ([@matt-edmondson](https://github.com/matt-edmondson))
- Update release condition to exclude forked repositories ([@matt-edmondson](https://github.com/matt-edmondson))
- Update the demo project with the new combos ([@Damon3000s](https://github.com/Damon3000s))
- Updated all packages and fixed errors ([@Damon3000s](https://github.com/Damon3000s))
- Updated comments for ColumnMajor enum ([@Damon3000s](https://github.com/Damon3000s))
- Updated variable names and some comments ([@Damon3000s](https://github.com/Damon3000s))
- Use Collection instead of List ([@Damon3000s](https://github.com/Damon3000s))
- Use target type new ([@Damon3000s](https://github.com/Damon3000s))
- Various cleanups ([@Damon3000s](https://github.com/Damon3000s))

## v1.1.4 (patch)

Changes since v1.1.3:

- [minor] Force minor update ([@matt-edmondson](https://github.com/matt-edmondson))
- [minor] Refactor version increment logic to use Contains method for commit message tags ([@matt-edmondson](https://github.com/matt-edmondson))
- Add GitHub token to workflow for fork detection ([@matt-edmondson](https://github.com/matt-edmondson))
- Add IS_FORK environment variable to GitHub Actions workflow ([@matt-edmondson](https://github.com/matt-edmondson))
- Enhance versioning logic to increment based on commit message tags ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix an issue where incrementing from a stable to prerelease version would incorrectly be attributed as a patch ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.1.3 (patch)

Changes since v1.1.2:

- Update GitHub Actions workflow to check if the repository is a fork using GitHub CLI ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.1.2 (patch)

Changes since v1.1.1:

- Add another API method ([@Damon3000s](https://github.com/Damon3000s))
- Add ScopedDisabled to ImGuiWidgets ([@Damon3000s](https://github.com/Damon3000s))
- Add version sorting suffixes for alpha, beta, rc, and pre releases, fixes bug where version increment could be wrong ([@matt-edmondson](https://github.com/matt-edmondson))
- Added OnGetTooltip to Icon Delegates ([@Damon3000s](https://github.com/Damon3000s))
- Cleanup ([@Damon3000s](https://github.com/Damon3000s))
- Cleanup old logic ([@Damon3000s](https://github.com/Damon3000s))
- Convert enum usages of ImGui.Combo to ImGuiWidgets.Combo ([@Damon3000s](https://github.com/Damon3000s))
- Default back to 32 grid items to show ([@Damon3000s](https://github.com/Damon3000s))
- Don't call measureDelegate twice ([@Damon3000s](https://github.com/Damon3000s))
- Feedback ([@Damon3000s](https://github.com/Damon3000s))
- First pass of adding row alignment and fixing layout issues ([@Damon3000s](https://github.com/Damon3000s))
- First pass of implementing a new Column Major grid ([@Damon3000s](https://github.com/Damon3000s))
- Fix ColumnMajor crashing when there isn't enough vertical space ([@Damon3000s](https://github.com/Damon3000s))
- Fix demo ([@Damon3000s](https://github.com/Damon3000s))
- Fix for grid double counting item spacing, yet only using a single during layout ([@Damon3000s](https://github.com/Damon3000s))
- Fix for potential 0 column situation ([@Damon3000s](https://github.com/Damon3000s))
- Fix for potential out of range access in ColumnMajor mode ([@Damon3000s](https://github.com/Damon3000s))
- Fix IDE style warnings ([@Damon3000s](https://github.com/Damon3000s))
- Fix when grid calculates 0 columns ([@Damon3000s](https://github.com/Damon3000s))
- Group code with regions ([@Damon3000s](https://github.com/Damon3000s))
- Improve implementation ([@Damon3000s](https://github.com/Damon3000s))
- Large refactot ([@Damon3000s](https://github.com/Damon3000s))
- More renames and cleanup ([@Damon3000s](https://github.com/Damon3000s))
- More target type new expressions ([@Damon3000s](https://github.com/Damon3000s))
- Move IconAlignment to a parameter ([@Damon3000s](https://github.com/Damon3000s))
- Move ScopedId to be within ImGuiWidgets class ([@Damon3000s](https://github.com/Damon3000s))
- Re-implement RowMajor ([@Damon3000s](https://github.com/Damon3000s))
- Remove the IconDelegates class ([@Damon3000s](https://github.com/Damon3000s))
- Resolving merge issues ([@Damon3000s](https://github.com/Damon3000s))
- Revert additional files ([@Damon3000s](https://github.com/Damon3000s))
- Review Feedback ([@Damon3000s](https://github.com/Damon3000s))
- Review feedback ([@Damon3000s](https://github.com/Damon3000s))
- Review Feedback ([@Damon3000s](https://github.com/Damon3000s))
- Review Feedback and general improvements ([@Damon3000s](https://github.com/Damon3000s))
- Tweak code layout ([@Damon3000s](https://github.com/Damon3000s))
- Update release condition to exclude forked repositories ([@matt-edmondson](https://github.com/matt-edmondson))
- Updated comments for ColumnMajor enum ([@Damon3000s](https://github.com/Damon3000s))
- Updated variable names and some comments ([@Damon3000s](https://github.com/Damon3000s))
- Use Collection instead of List ([@Damon3000s](https://github.com/Damon3000s))
- Use target type new ([@Damon3000s](https://github.com/Damon3000s))
- Various cleanups ([@Damon3000s](https://github.com/Damon3000s))

## v1.1.1 (patch)

Changes since v1.1.0:

- Add a new Combo widget that works with the enum type ([@Damon3000s](https://github.com/Damon3000s))
- Add an IString and string combo implementation ([@Damon3000s](https://github.com/Damon3000s))
- Add conditional checks for release tasks in CI workflow ([@matt-edmondson](https://github.com/matt-edmondson))
- Add icon back hopefully to LFS ([@matt-edmondson](https://github.com/matt-edmondson))
- Add the new combo to the ImGuiWidgets demo project ([@Damon3000s](https://github.com/Damon3000s))
- Change the ColorIndicator implementation based on review feedback ([@Damon3000s](https://github.com/Damon3000s))
- Changed formatting of collection member variable ([@Damon3000s](https://github.com/Damon3000s))
- Delete icon to try fix lfs ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix for SetPos ImGui assert ([@Damon3000s](https://github.com/Damon3000s))
- Minor formatting fix. ([@Damon3000s](https://github.com/Damon3000s))
- Refactor ColorIndicator ([@Damon3000s](https://github.com/Damon3000s))
- Review feedback ([@Damon3000s](https://github.com/Damon3000s))
- Simplify the assert fix ([@Damon3000s](https://github.com/Damon3000s))
- Update the demo project with the new combos ([@Damon3000s](https://github.com/Damon3000s))
- Updated all packages and fixed errors ([@Damon3000s](https://github.com/Damon3000s))

## v1.1.0 (minor)

Changes since v1.0.0:

- Add API documentation ([@matt-edmondson](https://github.com/matt-edmondson))
- Add automation scripts for metadata management and versioning ([@matt-edmondson](https://github.com/matt-edmondson))
- Renamed metadata files ([@matt-edmondson](https://github.com/matt-edmondson))
- Replace LICENSE file with LICENSE.md and update copyright information ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.0.15 (patch)

Changes since v1.0.15-pre.9:

- Add automation scripts for metadata management and versioning ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.0.12 (patch)

Changes since v1.0.11:

- Replace LICENSE file with LICENSE.md and update copyright information ([@matt-edmondson](https://github.com/matt-edmondson))

## v1.0.0 (major)

Changes since 0.0.0.0:

- Add a comment explaining the new break case ([@Damon3000s](https://github.com/Damon3000s))
- Add a common interface to facilitate scaling of all widgets by a common scale value ([@matt-edmondson](https://github.com/matt-edmondson))
- Add a delegate to get the string to show in the list for each item ([@matt-edmondson](https://github.com/matt-edmondson))
- Add a more general Tile class ([@matt-edmondson](https://github.com/matt-edmondson))
- Add a ScopedId class for convenience ([@Damon3000s](https://github.com/Damon3000s))
- Add basic visual debug options ([@Damon3000s](https://github.com/Damon3000s))
- Add color tinting to images ([@matt-edmondson](https://github.com/matt-edmondson))
- Add ColorIndicator widget ([@matt-edmondson](https://github.com/matt-edmondson))
- Add colors ([@matt-edmondson](https://github.com/matt-edmondson))
- Add DividerContainers and zones from ImGuiApp ([@matt-edmondson](https://github.com/matt-edmondson))
- Add dividers to the demo ([@matt-edmondson](https://github.com/matt-edmondson))
- Add documentation ([@matt-edmondson](https://github.com/matt-edmondson))
- Add documentation comments ([@matt-edmondson](https://github.com/matt-edmondson))
- Add EnableWindowsTargeting to demo project ([@matt-edmondson](https://github.com/matt-edmondson))
- Add github package support ([@matt-edmondson](https://github.com/matt-edmondson))
- Add globbing support #11 ([@matt-edmondson](https://github.com/matt-edmondson))
- Add Grid layout and Icon widget to replace the Tile widget ([@matt-edmondson](https://github.com/matt-edmondson))
- Add GridOrder and fix ColumnMajor layout bug ([@Damon3000s](https://github.com/Damon3000s))
- Add IsPackable=false to the demo project ([@matt-edmondson](https://github.com/matt-edmondson))
- Add modal input popups for strings, ints, and floats ([@matt-edmondson](https://github.com/matt-edmondson))
- Add new Image and IconTile widgets ([@matt-edmondson](https://github.com/matt-edmondson))
- Add nicer knobs from https://github.com/imgui-works/imgui-knobs-dial-gauge-meter ([@matt-edmondson](https://github.com/matt-edmondson))
- Add PopupFilesystemBrowser ([@matt-edmondson](https://github.com/matt-edmondson))
- Add PopupMessageOK ([@matt-edmondson](https://github.com/matt-edmondson))
- Add PopupPrompt to display a simple prompt window with configurable buttons ([@matt-edmondson](https://github.com/matt-edmondson))
- Add searchable list popup ([@matt-edmondson](https://github.com/matt-edmondson))
- Add text input to specify a new filename when saving ([@matt-edmondson](https://github.com/matt-edmondson))
- Add trees ([@matt-edmondson](https://github.com/matt-edmondson))
- Add wrapping to knob titles and ability to put them below the knob ([@matt-edmondson](https://github.com/matt-edmondson))
- Addressing review feedback ([@Damon3000s](https://github.com/Damon3000s))
- Allow zones to be added in the constructor ([@matt-edmondson](https://github.com/matt-edmondson))
- Assign dependabot PRs to matt ([@matt-edmondson](https://github.com/matt-edmondson))
- Avoid double upload of symbols package ([@matt-edmondson](https://github.com/matt-edmondson))
- Bump ImGuiStyler ([@matt-edmondson](https://github.com/matt-edmondson))
- Change icon strings ([@Damon3000s](https://github.com/Damon3000s))
- Create dependabot-merge.yml ([@matt-edmondson](https://github.com/matt-edmondson))
- Create dependabot.yml ([@matt-edmondson](https://github.com/matt-edmondson))
- Create VERSION ([@matt-edmondson](https://github.com/matt-edmondson))
- Disable grid debugging by default ([@Damon3000s](https://github.com/Damon3000s))
- Dont try to push packages when building pull requests ([@matt-edmondson](https://github.com/matt-edmondson))
- dotnet 8 ([@matt-edmondson](https://github.com/matt-edmondson))
- Double clicking a zone handle resets it to its original size ([@matt-edmondson](https://github.com/matt-edmondson))
- Enable sourcelink ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix a crash in the line wrapping ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix a grid wrapping issue ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix a runtime cast failure when dragging divider zones ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix a type in a comment ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix broken formatting ([@Damon3000s](https://github.com/Damon3000s))
- Fix end cursor position in nested trees ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix grid layout ([@Damon3000s](https://github.com/Damon3000s))
- Fix ID stack issues in popups and add escape key to cancel popup ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix incorrect icon calculation for 1 item grid ([@Damon3000s](https://github.com/Damon3000s))
- Fix issue with not invoking the callback if you didnt pick from the list view ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix mismatched ImGui version, add a demo project, use multilevel directory props instead of project.props files ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix some knob title alignment issues ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix Tile alignment issues ([@matt-edmondson](https://github.com/matt-edmondson))
- Fix typos and renames for clarity ([@matt-edmondson](https://github.com/matt-edmondson))
- Hoist popup functionality out into its own base class so that it can be used by other projects wanting modal popups ([@matt-edmondson](https://github.com/matt-edmondson))
- Initial commit ([@matt-edmondson](https://github.com/matt-edmondson))
- Migrate ktsu.io to ktsu namespace ([@matt-edmondson](https://github.com/matt-edmondson))
- Missing changes from previous commit ([@Damon3000s](https://github.com/Damon3000s))
- Move popups into their own library ([@matt-edmondson](https://github.com/matt-edmondson))
- Provide the item dimensions to the draw delegate ([@matt-edmondson](https://github.com/matt-edmondson))
- Push id in tile render to differentiate contents ([@matt-edmondson](https://github.com/matt-edmondson))
- Read from AUTHORS file during build ([@matt-edmondson](https://github.com/matt-edmondson))
- Read from VERSION when building ([@matt-edmondson](https://github.com/matt-edmondson))
- Read PackageDescription from DESCRIPTION file ([@matt-edmondson](https://github.com/matt-edmondson))
- Refactor and enhance class constructors ([@matt-edmondson](https://github.com/matt-edmondson))
- Release v1.0.0 with extensive documentation updates ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove a debug print ([@matt-edmondson](https://github.com/matt-edmondson))
- Remove intrusive scaling ([@matt-edmondson](https://github.com/matt-edmondson))
- Rollback to ImGui.NET 1.89.5 ([@matt-edmondson](https://github.com/matt-edmondson))
- Update build config ([@matt-edmondson](https://github.com/matt-edmondson))
- Update demo to include single item grid ([@Damon3000s](https://github.com/Damon3000s))
- Update Directory.Build.props ([@matt-edmondson](https://github.com/matt-edmondson))
- Update Directory.Build.targets ([@matt-edmondson](https://github.com/matt-edmondson))
- Update dotnet.yml ([@matt-edmondson](https://github.com/matt-edmondson))
- Update ImGui.NET ([@matt-edmondson](https://github.com/matt-edmondson))
- Update ImGuiStyler, ImGuiApp, and ImGuiPopups package references to latest alpha versions ([@matt-edmondson](https://github.com/matt-edmondson))
- Update LICENSE ([@matt-edmondson](https://github.com/matt-edmondson))
- Update nuget.config ([@matt-edmondson](https://github.com/matt-edmondson))
- Update package references for StrongPaths and ImGuiPopups to latest versions ([@matt-edmondson](https://github.com/matt-edmondson))
- Update package references to latest versions ([@matt-edmondson](https://github.com/matt-edmondson))
- Update README.md ([@matt-edmondson](https://github.com/matt-edmondson))
- Update SetSizesFromList to take an ICollection ([@matt-edmondson](https://github.com/matt-edmondson))
- Update to the latest ktsu.Styler version ([@Damon3000s](https://github.com/Damon3000s))
- Update url in gitignore ([@matt-edmondson](https://github.com/matt-edmondson))
- Update VERSION ([@matt-edmondson](https://github.com/matt-edmondson))
- Use the new ImGuiApp.Start signature ([@matt-edmondson](https://github.com/matt-edmondson))
- v1.0.0-alpha.8 ([@matt-edmondson](https://github.com/matt-edmondson))
- Vertically center the horizontal label correctly ([@matt-edmondson](https://github.com/matt-edmondson))
- WIP PopupInput ([@matt-edmondson](https://github.com/matt-edmondson))


