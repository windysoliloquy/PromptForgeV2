# Where To Find The App

If you are new to Visual Studio or GitHub, this is the quickest way to find the runnable Prompt Forge app.

## After Building in Visual Studio

Once the solution builds successfully, the executable is normally created here:

PromptForge.App\bin\Debug\net8.0-windows\PromptForge.App.exe

On this project, that full path will usually be:

C:\Users\windy\OneDrive\Desktop\Prompt Forge\PromptForge.App\bin\Debug\net8.0-windows\PromptForge.App.exe

## How To Build It

1. Open PromptForge.sln in Visual Studio 2022.
2. Wait for the solution to finish loading.
3. In the top toolbar, make sure the startup project is PromptForge.App.
4. Press Ctrl+F5 or click the green Run button.
5. Visual Studio will build the app and create the executable in the folder above.

## If You Cannot Find The EXE

Check these folders in order:

1. PromptForge.App\bin\Debug\net8.0-windows\
2. PromptForge.App\bin\Release\net8.0-windows\ if someone built a Release version instead

## If You Just Want To Share The App With Someone

Do not share the plain `bin\Debug` or `bin\Release` build output for releases.
Use the self-contained publish output instead:

`artifacts\publish\PromptForge-win-x64\`

The installer pipeline packages that publish folder because it includes the required .NET desktop runtime files for machines that do not already have them installed.

The important file inside that folder is:

PromptForge.App.exe

## GitHub Note

GitHub mainly stores the source code. New users often expect the app executable to be sitting at the top of the repository, but for a Visual Studio desktop app it is created after a build and placed in the in folder.
