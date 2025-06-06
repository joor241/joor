# Cover Art Manager

This project is a simple WPF application written in C# (.NET 8) that allows
embedding or removing cover art from common audio formats. It uses TagLib# for
the metadata operations and follows a basic MVVM architecture with
CommunityToolkit.Mvvm.

The application provides drag and drop areas for an audio file and an image,
as well as a cropping dialog to trim the image before embedding. The main
window is styled with a dark theme.

To build the project locally you need the .NET 8 SDK and can run:

```bash
cd CoverArtManager
dotnet build
```
