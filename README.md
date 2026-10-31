# Please Be Happy Access

Please Be Happy Access is a mod that makes Please Be Happy, the visual novel by Studio Élan, playable with a screen reader. It reads the story, choices and every menu, and makes the menus work properly from the keyboard. It describes every background and illustration, describes each character the first time you meet them, and waits for the voice actors to finish before describing a scene. Everything it says is available in all five of the game's languages.

Speech and braille go through [Prism](https://github.com/ethindp/prism), which talks to NVDA, JAWS and other Windows screen readers, and falls back to Windows' built-in voices if no screen reader is running. Everything the mod says is also sent to your braille display. You play with the keyboard.

- Join the Discord to chat, get help or report problems: <https://discord.gg/KNDXhztGGK>
- Support shaysters on Ko-fi: <https://ko-fi.com/shaysters>

Status: version 0.9.0, the first public release. Nearly complete; see "Known limitations" for what is still to come. See [CHANGELOG.md](CHANGELOG.md) for what has changed.

## Requirements

- Please Be Happy for Windows (version 2.0), from any of these stores:
  - Steam: <https://store.steampowered.com/app/844670/>
  - itch.io, either downloaded from the website or installed with the itch app: <https://vnstudioelan.itch.io/please-be-happy>
  - GOG: <https://www.gog.com/game/please_be_happy>. Not tested yet. GOG's copy has been behind the other stores' version 2.0; if the mod doesn't work with it, please let us know.
- The mod needs the 64-bit Windows version of the game. On itch.io, download the file named "Windows", not "Windows 32-bit"; the installer stops with a message if it finds the 32-bit version. The Mac and Linux versions from itch.io can't run the mod.
- A screen reader such as NVDA or JAWS (optional; without one, Prism uses Windows' built-in speech). Prism comes with the mod; there's nothing extra to install.

The mod runs on BepInEx 5 (64-bit), a mod loader. The installer sets it up for you.

## Installing

1. Close Please Be Happy if it's running.
2. Download **PleaseBeHappyAccessInstaller.exe** from the latest release: <https://github.com/shaysters/please-be-happy-access/releases/latest>
3. Run it. It installs BepInEx 5.4.23.5 and the mod, and checks every download against a known checksum.
   - Steam: it finds the game by itself.
   - itch.io and GOG: it asks you to choose the game folder, the one that contains `PleaseBeHappy.exe`. The itch app installs games in `%APPDATA%\itch\apps` by default; the GOG Galaxy app in `C:\Program Files (x86)\GOG Galaxy\Games`. If you downloaded the itch.io zip, choose the folder inside it named like "Please Be Happy 2.0.6 - Windows", wherever you unpacked it. Keep that folder's path fairly short (for example `C:\Games`): very deep folders, over Windows' 260-character path limit, stop the mod loader from starting.
4. Start Please Be Happy as usual, from Steam, the itch app, GOG Galaxy or the game folder. You'll hear "Please Be Happy Access loaded" as the game starts.

Windows SmartScreen may warn about an unrecognized app the first time; choose "More info", then "Run anyway". The installer is built on [Loadstone](https://github.com/bradjrenshaw/loadstone) by [Brad Renshaw](https://github.com/bradjrenshaw).

### The demo

The mod works with the free demo too, from Steam or itch.io. The installer doesn't find the demo by itself: in the "Please Be Happy directory" box, choose Browse... and pick the demo's folder. On Steam that's:

```
C:\Program Files (x86)\Steam\steamapps\common\Please Be Happy Demo
```

**Moving from the demo to the full game:** once you have the full game, run the installer again. It lists the demo copy (with the mod installed) and the full game separately; choose the full game and install. Then you can choose the demo copy and Uninstall, or keep it. The demo and the full game share their saves and settings, including the mod's settings, so nothing needs setting up again.

**Adding the demo after the full game:** the installer can only add a folder it finds by itself or one you choose before installing anywhere, so once the mod is in the full game it can't add the demo. Either install the mod into the demo by hand (see "Installing by hand"), or uninstall from the full game, install into the demo with Browse..., then run the installer again for the full game.

### Updating

When the game starts, the mod checks once whether a newer version is out. If there is one, it says so after the title screen appears, for example "Please Be Happy Access 1.0.0 is available. To update, close the game and run Please Be Happy Access Installer." Run the installer again and choose Update; it replaces the old files and removes any that the new version no longer uses. You can turn the check off in the mod's settings. The check only downloads the release information from GitHub; it sends nothing about you or your game.

The installer can also repair the mod (reinstall the same version) and uninstall it.

### Installing by hand

1. Close Please Be Happy if it's running.
2. Install BepInEx 5.4.23.5 (64-bit) into the game folder, if it isn't there already. Download **BepInEx_win_x64_5.4.23.5.zip** from <https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.5/BepInEx_win_x64_5.4.23.5.zip> (release page: <https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5>). This is the version the installer uses; take the "win_x64" file, not "win_x86" or a Unix one. Extract the zip so that `winhttp.dll`, `doorstop_config.ini` and the `BepInEx` folder sit next to `PleaseBeHappy.exe`. On a default Steam install that's the folder below; for itch.io and GOG, see step 3 of the installer instructions above.

   ```
   C:\Program Files (x86)\Steam\steamapps\common\Please Be Happy
   ```

3. Extract **PleaseBeHappyAccess.zip** from the release into the game folder. It puts the mod in `BepInEx\plugins\PleaseBeHappyAccess` (`PleaseBeHappyAccess.dll`, `prism.dll` and the `descriptions` folder).
4. Start Please Be Happy as usual.

### Uninstalling

Run the installer and choose Uninstall. By hand: delete the `BepInEx\plugins\PleaseBeHappyAccess` folder. To remove BepInEx too, delete `winhttp.dll`, `doorstop_config.ini`, `changelog.txt` and the `BepInEx` folder from the game folder. The mod doesn't change any of the game's own files.

## Keys

Letter keys are written in lower case because you press them on their own: none of the game's or the mod's keys need Shift.

The game's own keys:

| Key | Where | What it does |
|---|---|---|
| Enter or Space | Story | Continue to the next line. |
| Escape | Story | Open the pause menu: Return, Log, Save, Load, Options, Help, Title. |
| l or Page Up | Story | Open the backlog. |
| Backspace | Story | Show or hide the quick menu. |
| k | Story | Turn auto mode on or off. |
| Tab | Story | Turn skipping on or off. Hold Ctrl to skip only while held. |
| h | Story | Hide the game's interface. |
| Up and Down | Menus, choices, map | Move between options. |
| Left and Right | Menus | Change sliders, and move along rows such as the Options tabs. |
| Enter | Menus, choices, map | Select. |
| Escape | Menus | Go back. |

The mod's keys:

| Key | Where | What it does |
|---|---|---|
| m | Everywhere | Open the mod's settings (also the Accessibility button on the title menu). |
| v | Everywhere | Turn reading of voiced lines on or off. |
| r | Story | Repeat the last line. |
| r | Menus | Read all the text on the screen, such as help tips and descriptions. |
| d | Story | Describe the scene: the background or illustration, and who is on screen with their expression. |

These keys don't work while Ctrl or Alt is held.

Notes:

- When a menu opens, the mod moves the keyboard to its first option and reads it. Up and Down stay inside the menu that's open.
- The pause menu starts by saying where you are in the story, for example "Chapter 1 - Day 1. Fox of Fable."
- Save and load slots are read with their number, and filled slots with their chapter, scene and date, for example "Slot 1, CHAPTER 1 - DAY 1, On The Streets, 2026-10-30 20:24".
- Yes/no questions, such as returning to the title, read the question first, then the Yes and No buttons.
- Tabs that are mostly text, such as Help's QUICK TIPS and CONTROLS, read their text when they open. In Extras, locked entries are counted, for example "??? Locked! (46 of these)".
- In the backlog, the mod starts on the newest line. Up goes back through earlier lines; Enter on a line goes back to that point in the story.
- On the location map, each place is read by name. Places you can't go to are read as unavailable.
- Options opens on the open tab's first setting and says which tab it is. Up goes to the tab row (BASIC, TEXT, SOUND, VOICE); Enter on a tab opens it and moves to its first setting.
- Groups of choices, such as Music Cues, say the row, the choice and what's currently chosen.
- In an open drop-down list, Escape closes just the list.
- Lines with voice acting are read too, unless you turn that off with v or in the mod's settings.
- A new background or illustration is read as "Scene description:" followed by the description; the next line then starts with "Story text:", so you can tell them apart. With scene descriptions turned off, neither is said.
- Lines that are only "..." are read as "Ellipsis" (screen readers would otherwise say nothing). "… …" is read as "Ellipsis Ellipsis".
- Characters coming and going aren't announced; d says who is on screen.
- Descriptions never talk over the voice actors: on a voiced line, the description waits until the voice clip has finished.
- At start-up the mod announces the Studio Élan logo, then describes the title screen and reads "Press any button to start".
- When the mod has nothing to describe for an image (a plain black or white screen, or a visual effect), it stays quiet.

## Settings

Choose **Accessibility** on the title menu (after Help), or press m anywhere, to open the mod's settings. Use Up and Down to move, and Enter, Space, Left or Right to change a setting. Escape or m closes it. While it's open, the game ignores your keys. Changes are saved immediately, inside the game; there's no file to edit.

- **Read voiced lines** (on by default): read lines that have voice acting. Turn it off to hear only the voice actor on those lines.
- **Describe backgrounds and illustrations as they appear** (on by default). The d key still describes the scene when this is off.
- **Describe characters the first time you meet them** (on by default): describes what each character looks like the first time they're on screen. Like the game, it keeps names secret until the story introduces someone, so the description waits until you know their name. The d key still says who is on screen when this is off.
- **Read the name of each song as it starts playing** (on by default): "The Fox of Fable. Now playing."
- **Read pop-ups** (on by default): read the game's other pop-ups: achievements, "Game saved!", and the time of day when a new scene starts.
- **Check for updates when the game starts** (on by default): say when a newer version of the mod is out.

The mod's own messages and all its image descriptions follow the game's language: English, Japanese, Korean, Simplified Chinese or Traditional Chinese. The descriptions use the game's own names for characters and places in each language. The story, menus and choices are read in whatever language the game is set to.

## Known limitations

These are known and planned; reports about anything else are very welcome.

- The quick menu (Backspace) isn't read yet. Its actions have keys of their own (k for auto, Tab or Ctrl to skip), and saving and loading are in the pause menu.
- Viewing an unlocked picture in the Extras gallery doesn't describe it yet.
- On the location map, the icons beside each place (main story, time cost, who you might meet) aren't read yet.
- Minseo has no description yet.
- The translated descriptions were written by Claude and haven't been reviewed by native speakers yet.

## Troubleshooting

The mod writes to BepInEx's log in the game folder, `BepInEx\LogOutput.log`. On a default Steam install that's:

```
C:\Program Files (x86)\Steam\steamapps\common\Please Be Happy\BepInEx\LogOutput.log
```

It says which speech output Prism found (for example "Prism ready, backend: NVDA"), and every line the mod spoke starts with `SAY:`. If something goes wrong, include the log when you report it.

- **No speech at all, and no LogOutput.log:** BepInEx isn't loading. Check that `winhttp.dll` and the `BepInEx` folder are next to `PleaseBeHappy.exe`, not in a subfolder.
- **The log exists but doesn't mention Please Be Happy Access:** the mod's folder must be `BepInEx\plugins\PleaseBeHappyAccess`, with `PleaseBeHappyAccess.dll` inside it.
- **The log says it could not load prism.dll:** `prism.dll` must be in the same folder as `PleaseBeHappyAccess.dll`.

## For developers

The mod is a BepInEx 5 plugin built against the game's own assemblies (Unity 2021.3, Mono, Naninovel). Build with:

```
dotnet build src/PleaseBeHappyAccess -c Release
```

The build copies the mod into the game's `BepInEx\plugins\PleaseBeHappyAccess` folder. Add `/p:SkipDeploy=true` to skip that (for example while the game is running), or `/p:GameDir=<path>` for a different install.

| Path | What it is |
|---|---|
| `src/PleaseBeHappyAccess/Plugin.cs` | Entry point, per-frame runner, mod keys |
| `Speech.cs` | Prism speech and braille output |
| `DialoguePatches.cs` | Reads each printed line (hook on Naninovel's `TextPrinterManager.PrintTextAsync`) and notes voiced lines |
| `FocusReader.cs` | Reads whatever menu option has keyboard focus |
| `ScreenFocus.cs` | Gives menus focus when they open, keeps Up and Down inside them, tab rows, Escape in drop-downs, screen intros, r screen reading |
| `SplashPatches.cs` | The Studio Élan logo and the title screen |
| `PopupPatches.cs` | Pop-ups: Naninovel's `ToastUI.Show` (songs, achievements, notices) and the time-of-day card (`z04TimeDatePopup`, `z04NavigationAssetManager`) |
| `SceneWatcher.cs`, `Descriptions.cs` | Background, illustration and character announcements (from the script's own image names, via Naninovel's `OnCommandExecutionStart`), waiting for voice clips; the d key |
| `ModMenu.cs`, `Settings.cs`, `TitleButton.cs` | Settings menu (m, or the Accessibility button the mod adds to the title menu); settings saved with Unity PlayerPrefs |
| `UpdateCheck.cs` | Start-up check of the latest GitHub release |
| `loadstone-packages/`, `installer/profile.json` | Loadstone packages: the game, BepInEx 5 (pinned to 5.4.23.5 with its SHA-256) and the mod; the installer's profile |
| `scripts/package-release.ps1` | Builds `dist/PleaseBeHappyAccess.zip` |
| `.github/workflows/release.yml` | Builds the branded installer and `loadstone-release.json` and publishes a release |
| `Strings.cs` | The mod's own messages in all five game languages |
| `descriptions/<language>.tsv` | Image descriptions, one `key<TAB>text` per line (en, ja, ko, zh-CN, zh-TW) |

## Credits

The mod's code was written by Claude, Anthropic's AI model, in Claude Code. The image descriptions and their translations were written by Claude too, from screenshots of every image taken in the game.

Speech is provided by [Prism](https://github.com/ethindp/prism) by Ethin Probst, under the MPL-2.0 license. Its license files are in the `lib/prism_licenses` folder.

The installer is built on [Loadstone](https://github.com/bradjrenshaw/loadstone) by [Brad Renshaw](https://github.com/bradjrenshaw).

The mod is loaded by [BepInEx](https://github.com/BepInEx/BepInEx), under the LGPL-2.1 license.

Please Be Happy is made by Studio Élan. This mod isn't affiliated with or endorsed by them.
