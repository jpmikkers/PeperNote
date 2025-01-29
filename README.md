# PeperNote
PeperNote is an MIT licensed open source desktop stickynote replacement.

I chose the name "PeperNote" because it sounds like the dutch word for a type of small gingerbread [cookie](https://en.wikipedia.org/wiki/Kruidnoten#/media/File:Stapeltje_kruidnoten.jpg).

## Screenshot

![sticky note in edit state](https://github.com/jpmikkers/PeperNote/blob/main/Screenshots/pepernote_v1_1_0.png)

## How to install

- download the zipped msi installer via [this link](https://github.com/jpmikkers/PeperNote/releases/download/v1.2.0/PeperNote_v1.2.0.zip) .
- extract the file **PeperNoteSetup.msi**, double-click to install.
- if you get a popup message "the app you're trying to install isn't a Microsoft-verified app", you may have to unblock the installer by right-clicking **PeperNoteSetup.msi**, select properties (general tab), then checkmark 'unblock'.
- After installation, PeperNote will automatically start after the next reboot / login. If you don't want to wait for that you can start PeperNote from the start menu.
- PeperNote has a yellow system trayicon. Double click it to create a new note, right click to show the menu.

## Technical details

- The installation prerequisite is .NET 9
- The UI uses the WPF library (Windows Presentation Framework). 
- The notes you create will be stored in **%localappdata%\PeperNote** , which typically expands to **c:\users\username\AppData\Local\PeperNote**
- To build the project and installer you need Visual Studio 2022
- Required visual studio extension: Microsoft Visual Studio Installer Projects 2022 v2.0.1
