# Menutee

A set of scripts that make generating and maintaining a simple menu through code straightforward. I've been using this in 2019.3 and higher, but it very well could work on earlier versions.

## Installation

There are two options for installation. One involves manually editing a file, and the other involves adding a URL to package manager.

### Add to Package Manager

Open the package manager (Window -> Package Manager), and hit the plus button (+) in the top right, then "add package from git URL". In that field, enter `https://github.com/khutchins/menutee.git` and click Add.

### Modify manifest.json

Open Packages/manifest.json and add this to the list of dependencies (omitting the comma if it's at the end):

```
"com.khutchins.menutee": "https://github.com/khutchins/menutee.git",
```

## Usage

### Import the sample project

The sample project example main menu and in-game menu scripts that you can use, alongside of prefabs for buttons, sliders, dropdowns, and toggles that should form a solid base for customizing your menu.

To import the sample project, click Window -> Package Manager. In the package manager window, change the `Packages: <Some Text>` in the top to `Package: In Project` and click on Menutee.

Click on the import button next  to Samples -> Menu Generation. You should then see the imported code in `Samples/Menutee/<Some Version Number>/Menu Generation`. You can move this folder and its contents around freely.

### Add Prefabs

Copy the MenuStack prefab and InGameMenu prefab to your own project.

