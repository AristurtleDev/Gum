# TextBox

The TextBox control allows users to enter a string. It supports highlighting, copy/paste, selection with mouse and keyboard, and CTRL key for performing operations on entire words.

## Code Example: Creating a TextBox

The following code creates a TextBox.

```csharp
var textBox = new TextBox();
this.Root.Children.Add(textBox.Visual);
textBox.X = 50;
textBox.Y = 50;
textBox.Width = 200;
textBox.Height = 34;
textBox.Placeholder = "Placeholder Text...";

var textBox2 = new TextBox();
this.Root.Children.Add(textBox2.Visual);
textBox2.X = 50;
textBox2.Y = 90;
textBox2.Width = 200;
textBox2.Height = 34;
textBox2.Placeholder = "Placeholder Text...";
```

<figure><img src="../../../../.gitbook/assets/24_07 22 19.gif" alt=""><figcaption><p>Interacting with TextBoxes</p></figcaption></figure>

## Typing

TextBox supports reading characters from the keyboard. It supports:

* Regular character typing
* Backspace
* Delete
* Enter (if multi-line)
* Keyboard repeat rate
* CTRL X, C, and V for cut, copy, and paste

The TextBox respects the OS-level repat rate. For example, the following animation shows the TextBox responding to the Windows Key repeat rate.

<figure><img src="../../../../.gitbook/assets/03_08 07 14.gif" alt=""><figcaption><p>Key repeat rate adjusted in Windows</p></figcaption></figure>

## Selection

Selection can be performed programmatically or by the user using the cursor.

The `SelectionLength` property can be used to determine if any text is selected. The following code shows how to output the selected characters:

```csharp
if(textBox.SelectionLength > 0)
{
    var selectedText = textBox.Text.Substring(
        textBox.SelectionStart, 
        textBox.SelectionLength);
    System.Diagnostics.Debug.WriteLine("Selected text: " + selectedText");
}
```

The `SelectionStart` and `SelectionLength` properties can be modified to change the visual selection. For example, the following selects the first 5 letters:

```csharp
textBox.SelectionStart = 0;
textBox.SelectionLength = 5;
```

The entire text can be selected as shown in the following code:

```csharp
textBox.SelectionStart = 0;
textBox.SelectionLength = textBox.Text?.Length ?? 0; // in case text is null
```

Selection can also be performed by the user. Double-clicking the text box selects all text. Also, pressing CTRL+A selects all text.

<figure><img src="../../../../.gitbook/assets/16_11 18 38.gif" alt=""><figcaption><p>Double-click selects all text</p></figcaption></figure>

A push+drag with the mouse selects the text between the start and the current location of the drag.

<figure><img src="../../../../.gitbook/assets/16_11 20 19.gif" alt=""><figcaption><p>Push+drag to select text</p></figcaption></figure>

Holding down the shift key and pressing the arrow keys adjusts the selection.

<figure><img src="../../../../.gitbook/assets/16_11 22 37.gif" alt=""><figcaption><p>Arrow keys + shift to select</p></figcaption></figure>

## TextWrapping

The `TextWrapping` property can be used to set whether the TextBox wraps text. By default this value is set to `TextWrapping.NoWrap` which means the text does not wrap, but instead extends horizontally.

<figure><img src="../../../../.gitbook/assets/16_11 32 07.gif" alt=""><figcaption><p>TextWrapping.NoWrap causes text to scroll</p></figcaption></figure>

If `TextWrapping` is set to \`TextWrapping.Wrap, then text wraps to multiple lines. Note that usually this is combined with a taller text box so that multiple lines display properly.

```csharp
wrappedTextBox.TextWrapping = TextWrapping.Wrap;
// If you have set up your TextBox in code, you may need to make it taller:
wrappedTextBox.Height = 140;
```

<figure><img src="../../../../.gitbook/assets/16_11 39 19.gif" alt=""><figcaption><p>TextWrapping.Wrap causes text to wrap</p></figcaption></figure>

## Extended Character Sets and Keyboards

The TextBox supports entering characters respecting the current keyboard language settings. This includes typing characters with accents, pasting text, and entering alt codes ([https://www.alt-codes.net/](https://www.alt-codes.net/) ).

<figure><img src="../../../../.gitbook/assets/image (167).png" alt=""><figcaption><p>TextBox displaying the é character</p></figcaption></figure>

Characters must be available in the current font to support being written in TextBoxes. If you would like to support more characters, you can explicitly create a font (.fnt) including the desired characters, or change the default character set in Gum.

For more information on creating fonts, see the [Font](../../../../gum-tool/gum-elements/text/font.md) and [Use Custom Font](../../../../gum-tool/gum-elements/text/use-custom-font.md) pages. For more information on specifying the default character set in Gum, see the [Project Property Font Ranges](../../../../gum-tool/menu/project-properties.md#font-ranges) page.



