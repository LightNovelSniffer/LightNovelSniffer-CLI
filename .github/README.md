# LightNovelSniffer-CLI
Command Line Interface for LightNoverSniffer library

## Setup
Download or clone this repository on your computer. Then create a new VisualStudio solution or open an existing one, and add the .csproj to it.
You also need [LightNovelSniffer core](https://github.com/LightNovelSniffer/LightNovelSniffer). Clone or download it on your computer too (in the same folder than LightNovelSniffer-CLI), and add it to the solution.

Build the LightNovelSniffer project. Then you can build and run the LightNovelSniffer-CLI project.

## Configuration
Based of Config.xml, override the parameters you want in Config_user.xml. Do not change any value in Config.xml, and do not push any content in Config_user.xml (will be rejected in PR)

e.g, you can change the output directory, or publisher name, etc...

## Light novels list
LightNovels.xml contains a list of pre-configured LN.
LightNovels_user.xml can either override this list, or complete it, depending on the way it is imported.

By default, it will override it (this mean that if LightNovels_user.xml is empty, no LN will be processed).

To change this behaviour to "adding" mode, change `true` to `false` in the following line in Program.cs : `ConfigTools.InitLightNovels("LightNovels_user.xml", true);`