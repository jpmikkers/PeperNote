# PeperNote
PeperNote is an MIT licensed open source desktop stickynote replacement.

I chose the name "PeperNote" because it sounds like the dutch word for a type of small gingerbread [cookie](https://en.wikipedia.org/wiki/Kruidnoten#/media/File:Stapeltje_kruidnoten.jpg).

## Technical details
- The installation prerequisite is .NET 4.7.2
- The UI uses the WPF library (Windows Presentation Framework). 
- The notes you create will be stored in **%localappdata%\PeperNote** , which typically expands to **c:\users\username\AppData\Local\PeperNote**
- To build the project and installer you need Visual Studio 2019
- The installer project uses [WixSharp](https://github.com/oleg-shilo/wixsharp), the excellent code-first Wix-wrapper created by Oleg Shilo.
- As this was my first WixSharp attempt, the installer is rather simple now: it will install PeperNote for all users.
