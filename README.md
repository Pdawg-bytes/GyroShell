# GyroShell
A shell for Windows 11 (and maybe 10) that aims to create a much nicer and more functional shell. Fully written in C# and WASDK.

# Building
To build GyroShell, you're gonna need a few things. First, you need to have Visual Studio 2022 with WASDK C# installed. Use git to clone this repository, then open the solution in Visual Studio. You're not quite ready yet, you will need to make sure all NuGet packages restored properly. Next, right click the project file, and click "Add a project reference". This is so you can add the Windows UDK for Cs/WinRT projections. Go to the browse tab, and find the folder where you cloned this repo. Next, go into the "WinMD" sub folder, and select "windowsudk.winmd". After that, you're ready to build!

# Usage of the UDK
Why am I using undocumented APIs, you may ask? Well, this is simply to make interacting with the main Windows shell much easier, and it allows the program to open the start menu, action center, etc.

# Installing
To install GyroShell, just download the app package from releases and install the certificate to "Local Machine > Trusted People". This is because my app package is not signed for the store, and needs to be trusted by the certificate. After that, just launch GyroShell and it should take place of your current shell!

# Tools used for this app
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white) ![Visual Studio Code](https://img.shields.io/badge/Visual%20Studio%20Code-0078d7.svg?style=for-the-badge&logo=visual-studio-code&logoColor=white)
