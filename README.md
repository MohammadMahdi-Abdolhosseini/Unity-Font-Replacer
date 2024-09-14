# Font Replacer for Unity

The **Font Replacer** is an intuitive Unity Editor tool designed to simplify the process of replacing fonts across multiple scenes and prefabs. Whether you're working with **RTL TextMeshPro** (Right-to-Left) components or standard **TextMeshPro** components, this tool streamlines font replacement with a user-friendly interface.

## Features

- **Batch Processing:** Replace fonts across multiple scenes and prefabs in one go.
- **Flexible Font Selection:** Option to replace a specific base font or all fonts in the selected components.
- **RTL and Standard Support:** Works seamlessly with both **RTLTextMeshPro** and **TextMeshProUGUI** components.
- **Detailed Summary:** View the total number of font replacements after the operation.
- **Simple UI:** Easy-to-use editor window for quick font management.

## Requirements

- **TextMeshPro** package (included with Unity)
- **RTLTextMeshPro** plugin (for Right-to-Left text support)

## Installation

1. Download the `FontReplacer` script and place it in your Unity project's `Assets > Editor` folder.
2. Open Unity and navigate to `Tools > Font Replacer` in the toolbar to launch the tool.

## Usage

1. **Open the Tool:** Go to `Tools > Font Replacer` in the Unity Editor.
2. **Select Fonts:**
    - **Base Font:** Choose the font you want to replace (leave it empty to replace all fonts in the selected components).
    - **New Font:** Select the new font to assign.
3. **Choose Component Type:**
    - Select whether you want to replace fonts in **RTLTextMeshPro** or **TextMeshPro** components.
4. **Assign Scenes and Prefabs:**
    - Drag and drop the scenes and prefabs where you want the font replacement to occur.
5. **Replace Fonts:** Click the `Replace Fonts` button to initiate the replacement process. A dialog will appear with the total number of fonts replaced.
6. **Save Changes:** Press `Ctrl + S` (or `Cmd + S` on Mac) to save all changes after replacement.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
