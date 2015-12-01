# Stats Converter (HDT Plugin)

A plugin to import and export game statistics from [Hearthstone Deck Tracker](https://github.com/Epix37/Hearthstone-Deck-Tracker) in different format.

## Installation
1. Delete any previous releases
- Then unzip the [latest release](https://github.com/andburn/hdt-plugin-statsconverter/releases/latest) into the HDT Plugins directory (this directory can be opened with `options > plugins > plugins folder` button)
- The directory should look like ``Plugins/StatsConverter/[some files]``
- Enable the plugin from ``Options/Plugins`` menu
- Use to the *export*, *import* and *settings* options from the plugins menu as shown below:
![Menu](http://i.imgur.com/HIrkY6T.png)

## Supported Formats
There are limited amount of available formats at the moment:

### Exporting
- CSV (Column Seperated Values) format

### Importing
- Hearthstone log files (**only works with the log files generated from tracker v0.11.0 or greater**)

## Known Issues
- Importing of log files essentially replays the games within the file as far as HDT is concerned. This can take a long time for large log files.
- The new way the tracker treats log files means that log importing is only partially working and may need manual updating after the import.

## Development
- Like all HDT plugins, when building from source you first need to add a reference to the `HearthstoneDeckTracker.exe` on your system. For both the plugin and test projects.
- The other external dependencies should be restored by [NuGet](https://docs.nuget.org/Consume/Package-Restore) automatically when you build the project.
