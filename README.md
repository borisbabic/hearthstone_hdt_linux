## Hearthstone + HDT steps in wine
These instructions assume a clean [wine prefix](https://wiki.winehq.org/FAQ#Wineprefixes), and will use `~/.wine.hearthstone` as the prefix in example commands. If you already have such a prefix please either clean it or use a different one.

## Prerequisites
### Any distro
- wine staging - WARNING: You may need to downgrade to 5.8. HDT won't install if it's not a staging version. I assume you either need the 32bit version or the wow version (which supports both). Tested working on: wine-3.13 (Staging) - 5.0 (Staging)
- winetricks - used for making the setup eaiser. You can manually do the steps without it, but it's easier to use. Tested working on: 20180603 - 20191224
### Arch linux
- wine-staging
- winetricks
- lib32-libldap and lib32-gnutls (without these the login button won't show up on Battle.net)
### Nixos/nix
It's tested working with the following (on the nixos-unstable channel):
- wineWowPackages.staging
- winetricks

## Installation

### 0. Grab HDT oauth from windows

NOTE: If you are running a sufficiently new version of wine (lowest tested was 5.0 staging) you don't need a working windows install and can just login to hsreplay normally without copying the auth over from windows.

If you aren't going to login to hsreplay in HDT you can skip this.

HDT works on linux, however you can't login to hsreplay. However you can grab the login info from a working windows install and then use that on linux. 

First ensure that HDT is not running on windows. Next download the [`hdte.exe`](https://github.com/borisbabic/hearthstone_hdt_linux/raw/master/hdte.exe) file from this repository to your Downloads folder on windows. Then open the command prompt and run the following:

```shell
Downloads\hdte.exe decrypt %AppData%\HearthstoneDeckTracker\hsreplay_oauth Downloads\hsreplay_oauth.decrypted
```

You will have to transfer that `hsreplay_oauth.decrypted` from `Downloads` to Linux (I'll assume it's in `~/Downloads` on linux later on when installing HDT) 

### 1. Setup the prefix
```shell
WINEPREFIX=~/.wine.hearthstone WINEARCH=win32 wine wineboot # Create 32 bit wine prefix
WINEPREFIX=~/.wine.hearthstone winetricks dotnet472 # Install .NET 4.7.2
WINEPREFIX=~/.wine.hearthstone winetricks corefonts # Install fonts for Battle.net, you may not be able to login without this
WINEPREFIX=~/.wine.hearthstone winetricks nocrashdialog # Without this popup warnings appear after running Battle.net
```

There are also two registry keys that need to be edited.  run `WINEPREFIX=~/.wine.hearthstone regedit`

- Navigate to/create `HKEY_CURRENT_USER\Software\Wine\X11 Driver` then create a new entry `UseTakeFocus` with the value `N` (String Value). This solves window focus issues
- Navigate to/create `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Avalon.Graphics` and create `DisableHWAcceleration` with the value `1` (DWORD Value) - allows the installation of newer versions of hdt

You can automatically change these keys using the following script:
```sh
{ cat | sed 's/$/\r/' | iconv -f ascii -t UTF-16 > /tmp/hdt.reg; } <<EOF
Windows Registry Editor Version 5.00

[HKEY_CURRENT_USER\Software\Wine\X11 Driver]
"UseTakeFocus"="N"

[HKEY_CURRENT_USER\Software\Microsoft\Avalon.Graphics]
"DisableHWAcceleration"=dword:00000001
EOF

WINEPREFIX=~/.wine.hearthstone regedit /tmp/hdt.reg
```


### 2. Install HDT

Download HDT from https://hsdecktracker.net/download/ then run

```shell
WINEPREFIX=~/.wine.hearthstone wine ~/Downloads/HDT-Installer.exe # or wherever you downloaded it to
```
NOTE: the following is not necessary for newer versions of wine, you can just login normally. The lowest tested where it wasn't necessary was 5.0 staging

If you want to be logged in to hsreplay then download the [`hdte.exe`](https://github.com/borisbabic/hearthstone_hdt_linux/raw/master/hdte.exe) file from this repository to `~/Downloads` alongside the `hsreplay_oauth.decrypted` file from above, close HDT if it is open, and run the following

```shell
WINEPREFIX=~/.wine.hearthstone wine ~/Downloads/hdte.exe encrypt ~/Downloads/hsreplay_oauth.decrypted ~/.wine.hearthstone/drive_c/users/$USER/Application Data/HearthstoneDeckTracker/hsreplay_oauth # change ~/.wine.hearthstone to your wine prefix if you've changed it 
```

### 3. Install Hearthstone
Download hearthstone https://eu.battle.net/account/download

Run the following to install it:

```shell
WINEPREFIX=~/.wine.hearthstone wine ~/Downloads/Hearthstone-Setup.exe 
```

When you get to the login screen, go to settings>advanced and uncheck "Use browser hardware acceleration when available" (without this you may get a black screen instead of the launcher)

Before running Hearthstone select Hearthstone in Battle.net, go to Options->Game Settings, enable Additional command line arguments, and add the argument `-force-d3d9` (without this you may get a black screen on launch).

## Troubleshooting
If the below doesn't help you can create an issue or ask for help in my [discord](https://discord.gg/uNE6z8u) 
### HDT runs but the overlay doesn't appear in game
Try running the game in windowed mode. In at least KDE the overlay doesn't show above the game in fullscreen. If that doesn't work then check the overlay options in the main HDT window, in particular the "Hide in menu"" and "Hide (completely)". Try toggling the "Hide (completely)" option.

The overlay is a bit weird window wise, so it may be possible your window manager doesn't handle it the best. Please try a different window manager/desktop environment before reporting an issue if this is your problem. The following is tested as working:

- kwin/KDE - works in windowed mode, overlay isn't shown in fullscreen
- Xfwm4/Xfce  - works in windowed mode
- awesomewm4 - works (see below)
- xmonad - works

#### awsomewm4 rules

Use the following rule to ensure the overlay is always on top:
```lua
awful.rules.rules = {
  -- ...
  -- Always show on top
  { rule_any = {
      name = {
        "HearthstoneOverlay",
      },
    },
    properties = {
      ontop = true,
    }
  },
}
```

### Performance issues
There is a known performance drop with the overlay. Some possible workarounds are to disable some stuff, like flavor tooltips and card animations. It's also a good idea to add a shortcut to toggle the overlay for more intensive turns Options -> check Advanced Options -> Hotkeys -> Toggle overlay

If you test this in something else please add to the above list
## Development

Compile using mono: `mcs hdte.cs -reference:System.Security`
