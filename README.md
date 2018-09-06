

## Hearthstone + HDT steps in wine
These instructions assume a clean [wine prefix](https://wiki.winehq.org/FAQ#Wineprefixes), and will use `~/.wine.hearthstone` as the prefix in example commands. If you already have such a prefix please either clean it or use a different one.

### Requirements
- wine (duh) - I assume you either need the 32bit version or the wow version (which supports both). Tested working on: wine-3.13 (Staging)
- winetricks - used for making the setup eaiser. You can manually do the steps without it, but it's easier to use. Tested working on: 20180603 - sha256sum: a114ec82c634d87b048cef33f1a0bfe9d26f3795459fd85c5bb064dc0260299c



### 0. Grab HDT oauth from windows
If you aren't going to login to hsreplay in HDT you can skip this.

HDT works on linux, however you can't login to hsreplay. However you can grab the login info from a working windows install and then use that on linux. 

First ensure that HDT is not running on windows. Next download the hdte.exe file from this repository to your Downloads folder on windows. Finally open the command prompt and run the following:

```shell
Downloads\hdte.exe decrypt %AppData%\HearthstoneDeckTracker\hsreplay_oauth Downloads\hsreplay_oauth.decrypted
```


### 1. Setup the prefix
```shell
WINEPREFIX=~/.wine.hearthstone winetricks dotnet45 # you can skip this if you dont want HDT. it installs .NET 4.5 which is required by HDT. just do the next,next,next dance
WINEPREFIX=~/.wine.hearthstone winetricks win7 # sets the windows version to 7
WINEPREFIX=~/.wine.hearthstone winetricks corefonts # battlenet is a bit messed up and it can be hard/impossible to login without this
WINEPREFIX=~/.wine.hearthstone winetricks nocrashdialog # not necessary but stops the popup warnings after running battlenet
```

### 2. Install HDT

Download hdt from https://hsdecktracker.net/download/ then run 

```shell
WINEPREFIX=~/.wine.hearthstone wine ~/Downloads/HDTInstaller.exe # or wherever you downloaded it to
```

If you are going to login to hsreplay then download the `hdte.exe` file from this repository to `~/Downloads` alongside the `hsreplay_oauth.decrypted` file from above, close HDT if it is open, and run the following

```shell
WINEPREFIX=~/.wine.hearthstone wine ~/Downloads/hdte.exe decrypt ~/Downloads/hsreplay_oauth.decrypted ~/.wine.hearthstone/drive_c/users/$USER/Application Data/HearthstoneDeckTracker/hsreplay_oauth # change ~/.wine.hearthstone to your wine prefix if you've changed it
```


### 3. Install Hearthstone
Download hearthstone https://eu.battle.net/account/download


Run the following to install it:

```shell
WINEPREFIX=~/.wine.hearthstone wine ~/Downloads/Hearthstone-Setup.exe 
```

After installing hearthstone before running it on the hearthstone screen in battlenet click options->game settings and set `-force-d3d9` as additional command line argument which solves the black window issue.
