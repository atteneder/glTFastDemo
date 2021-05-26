# Custom Test Files

To add custom test glTf files:

- Place test glTF file in this folder
- Add the relative path to it into `Assets/StreamingAssets/local-test-files.txt`

Example:

For file `Assets/StreamingAssets/local/subfolder/file.glb` -> Add the line `subfolder/file.glb`

When you run the scene `TestLoadScene`, select the `local` test set. Your new entries should appear in the list.
