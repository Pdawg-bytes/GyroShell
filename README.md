# GyroShell
A shell for Windows 11 (and possibly 10) that aims to create a more pleasant and functional shell. Fully written in C# and WASDK.

# Building
To build GyroShell, you'll need a few things. First, you'll need to have Visual Studio 2022 with WASDK C# installed. Use git to clone this repository, then open the solution in Visual Studio. You're not quite ready yet, you'll need to make sure all NuGet packages are restored properly. Next, right-click the project file, and click "Add a project reference." This is so you can add the Windows UDK for Cs/WinRT projections. Go to the browse tab, and find the folder where you cloned this repo. Next, go into the "WinMD" subfolder, and select "windowsudk.winmd." After that, you're ready to build!

# Usage of the UDK
You may ask, "Why are you using undocumented APIs?" Well, this is simply to make interacting with the main Windows shell much easier and allows the program to open the start menu, action center, etc.

# Installing
To install GyroShell, download the app package from releases and install the certificate to "Local Machine > Trusted People." This is because my app package is not signed for the store, and needs to be trusted by the certificate. Then, launch GyroShell and it should take the place of your current shell.

# Tools used for this app
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white) ![Visual Studio Code](https://img.shields.io/badge/Visual%20Studio%20Code-0078d7.svg?style=for-the-badge&logo=visual-studio-code&logoColor=white)

# Screenshots (***subject to change***)
![Screenshot_20221225_101323](https://user-images.githubusercontent.com/83825746/209495446-49c0e040-2537-4f92-b47b-924777dce877.png)
![Screenshot_20221225_101526](https://user-images.githubusercontent.com/83825746/209495456-f8b4243c-ebab-4640-862e-1b2f51f8d823.png)
