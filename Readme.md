# Utils

This repository contains various utility classes and functions that I've used in my projects and decided to make public. If you find them useful, you're welcome to use them in your projects.

**Note**:
- The utilities aren't organized in any particular order.
- Primarily, these were created for my personal use, but feel free to utilize or adapt them as needed.

## DOTween Setup

To use extensions relying on DOTween:

1. **Add Compilation Symbol**:
   - Go to **Edit** > **Project Settings** > **Player** > **Other Settings**.
   - Add `USING_DOTWEEN` to **Scripting Define Symbols**.
   
2. **Use Extensions**:
   - With the symbol set, you can use the DOTween-dependent extensions.
   - If you remove DOTween, delete the `USING_DOTWEEN` symbol to avoid compile errors.
