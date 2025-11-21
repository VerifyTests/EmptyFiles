How it works:

 1. Inputs="$(MSBuildThisFileFile)" - Tracks the package file timestamp
 2. Outputs="$(EmptyFilesMarker)" - Tracks a marker file in the output directory
 3. MSBuild automatically skips the target if all outputs are newer than all inputs
 4. SkipUnchangedFiles="true" - Additional optimization to skip individual unchanged files
 5. Touch - Updates the marker file timestamp after successful copy

This will only copy files when:

 1. The targets is newer than the marker file, OR
 2. The marker file doesn't exist (first build/clean build)

Benefits:

 * Dramatically reduces IO on incremental builds
 * Works with dotnet clean (removes marker file)
 * Still respects individual file changes via SkipUnchangedFiles
