# Stats Converter  [![Github Releases](https://img.shields.io/badge/plugin-HDT-e6af2b.svg)](https://github.com/HearthSim/Hearthstone-Deck-Tracker?maxAge=2592000) [![Github Releases](https://img.shields.io/github/downloads/andburn/hdt-plugin-statsconverter/latest/total.svg?maxAge=604800)](https://github.com/andburn/hdt-plugin-statsconverter/releases/latest) [![GitHub release](https://img.shields.io/github/release/andburn/hdt-plugin-statsconverter.svg?maxAge=604800)](https://github.com/andburn/hdt-plugin-statsconverter/releases/latest)

A plugin to import and export game statistics from [Hearthstone Deck Tracker](https://github.com/HearthSim/Hearthstone-Deck-Tracker).

## Installation
1. Delete any previous releases
- Then unzip the [latest release](https://github.com/andburn/hdt-plugin-statsconverter/releases/latest) into the HDT Plugins directory (this directory can be opened with `options > plugins > plugins folder` button)
- The directory should look like ``Plugins/StatsConverter/[some files]``
- Enable the plugin from ``Options/Plugins`` menu
- Use to the *export*, *import* and *settings* options from the plugins menu as shown below:
![Menu](http://i.imgur.com/HIrkY6T.png)

## Supported Formats
There are limited amount of available formats at the moment:

### Export
- CSV (Column Separated Values) format
 - When *Arena* mode is selected, mark the *Arena Extras* check box to export an additional file containing arena deck and reward data.

   ![Imgur](http://i.imgur.com/VeREf1l.png)

### Import
- (Not Available)

## Development
- To build the plugin the project dependencies need to be added manually.
- Add references (see [plugin-example](https://github.com/andburn/hdt-plugin-example/blob/master/README.md) for details) to the following files in your HDT installation:
 - `HearthstoneDeckTracker.exe`
 - `MahApps.Metro.dll`
 - `Newtonsoft.Json.dll`
- It is also a good idea to set the *Copy Local* property of this references to *False*.
