# Menutee

A set of scripts that make it easy to generate and maintain a simple menu through code. I've been using this in 2019.3 and higher, but it very well could work on earlier versions. 

The name is combining the words Manatee and Menu together. I know it's a reach.

## Overview

[Here's](https://khutchins.itch.io/the-abyss-of-dastroreth) an example of it in action (the main menu and in-game menus all use Menutee).

The menu generation component of this library is meant for getting a menu quickly running for jam games and projects where you just don't want to have to do a menu. It is somewhat flexible, but certain things are not possible with how it is at the moment.

### Capabilities

* Buttons, sliders, dropdowns, and toggles are supported out of the box.
* All menu elements are customizable (my example primarily looks ugly because I lack an eye for art).
* Additional UI element types can be added without modifying the library code.
* Supports using a scriptable object for setting the palette, so you can easily tweak all the colors in one location.

### Limitations

* Does not easily support a combination of horizontal and vertical layouts. If you want to have two buttons next to each other in a vertical menu, it's probably possible if you set the navigation directions directly, but it may be easier to implement the menu directly at that point.
* Doesn't support animations. It's possible (maybe even straightforward) to add this, but I haven't gotten the motivation to do it in a robust way.

## Installation

There are two options for installation. One involves manually editing a file, and the other involves adding a URL to package manager.

NOTE: You should always back up your project before installing a new package.

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


#### Samples

##### InGameMenuSample

This scene shows an example of what an in-game menu could look like. SampleInGameMenu.cs is the script that drives the creation of the menu. The builder, MenuConfig.Builder, allows the menu to be built in a more or less readable way. The first argument to MenuConfig.Builder's constructor is whether the menu is closeable, and the second is whether or not the menu pauses the game. For an in-game menu, likely both of those should be true.

After that, panels (groupings of menu options that are displayed at a given time) are added to the MenuConfig builder object. Each panel has a set of PanelObjects, which are built using various subclasses of PanelObjectConfig, like ButtonConfig or SliderConfig.

Once the MenuConfig object is built, pass it to CreateMenu() and the menu itself will be constructed.

##### MainMenuSample

Main menu sample shows an example configuration for a main menu. In contrast to the in-game menu, the menu is not closeable nor does it pause the game. Instead of using a builder pattern, it takes all of the arguments directly into the MenuConfig constructor. I personally find this less flexible and readable, but it's an option of you prefer it. Note that you have to pass in the key for the default menu explicitly, as opposed to the builder, which allows you to specify it when adding a panel.

The main menu script and in-game menu scripts are different for sample purposes, but you may want to have the contents of the menus be the same except for some minor differences. If that's the case, you can generally share the same builder but with some different flags set based on whether it's for the main or in-game menu.

##### MenuHookSample

This is another version of a main menu, but this example actually leverages some of the functionality of MenuStack—the ability to hook it in to other canvases. In this example, a second canvas, HookedCanvas, contains a MenuHook script that allows the generated menu to display it and to hide the menu when it's done. Since it's designed as a flexible way to handle external menu sources, its goal is to avoid being restrictive.

Often you'll want to use the unity event callbacks on the object itself to call into the canvas you're hooking into's internals. You can display this menu by calling `MenuStack.Shared.PushAndShowMenu()` with the MenuHook (or anything that implements IMenu) as an argument.

If you hook up the Canvas property, it will automatically show and hide the canvas for you (note that this won't disable interaction, so you might run into problems. I'm trying to find a good way to solve this, maybe canvas groups, I need to research that more). Similarly, you can configure it to automatically set the default selected game object when it's pushed to or popped back to.

##### MenuWithCustomBackSample

This example has a menu with buttons that exist outside of the programmatic generation. One instance where this is useful is having a custom panel with a static back button. These custom elements can be set as the default selectable by using `.SetDefaultSelectableCallback` on the PanelConfig builder. Similarly, you can hook up the navigation by using the using `.SetCustomNavigation` method. MenuGenerator has `SetHorizontalNavigation` and `SetVerticalNavigation` convenience methods that automate the UI hookup process. See the `SampleMenuWithCustomBack` script to look at how they're implemented.

### Add & Customize Prefabs

Copy the MenuStack prefab and SampleInGameMenu prefab to your own project. Using SampleInGameMenu.cs as a base, create a replacement (or copy the script out of the Samples folder and edit it directly), and replace the menu options with ones that fit your own game.

Now that they're both in a scene, you should be able to show the menu. By default, the menu toggle button is set to the "Jump" mapping (space bar), since that is there by default on new Unity projects.

Customize the prefabs: UIDropdown, UISlider, UITextButton, UIToggleButton, and UIVerticalPanel to use your own fonts and styling where desired. I recommend moving these out of the Samples directory so your own changes don't get overridden by a new import.

### Palette Config

Palette configs allow you to customize all the different UI element color options by using a single scriptable object. You can swap out the colors of all the elements using it.

Create a new Palette Config by using the right click menu `Create -> Menutee -> Palette Config`, customize the colors, and replace SamplePalette in your menu script with your own.

## Troubleshooting

### Incorrect Time Scale And/Or Mouse Behavior

The MenuStack script (which the in-game menus rely on) will attempt to manage your game's time scale and cursor visibility/lock mode for you. If you're seeing the incorrect behavior initially, go to where the MenuStack script is set (on the MenuStack prefab, if you've been following the instructions), and you can tweak the default cursor visibility and lock mode.

If you don't want MenuStack to manage cursor settings or time settings, you can enable or disable this behavior on the same script. This is likely what you want to do if you have another script that already handles these lock modes.

### Inputs Not Working

If inputs aren't working in the menu, make sure that you both have an EventSystem and StandaloneInputModule in the scene.

### A Pushed Canvas Appears Under the Canvas it Should Be On Top Of

MenuStack currently doesn't handle canvas ordering. You should set the `Sort Order` property on the canvas to reflect the ordering you desire. A higher number indicates that that canvas should appear on top of one with a lower number.