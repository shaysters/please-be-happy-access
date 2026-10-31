# Changelog

All notable changes to Please Be Happy Access. Newest first.

## 0.9.2 (2026-10-31)

### Fixed
- On chapter 1's map, the single unnamed pin that leads to the next scene was read as "Unknown, button". It is now read as "Next story location, not named yet, button" (reported on Discord).

## 0.9.1 (2026-10-31)

### Fixed
- In an open drop-down list in Options, the arrow keys now stay in the list and stop at its first and last entries. Before, Down on the last entry left the list for the next setting without choosing anything (reported with the Language list, which has only English).
- Each entry in a drop-down list now says its position, such as "1 of 1" or "2 of 3".

## 0.9.0 (2026-10-31)

The first public release. It includes everything from the development versions below.

### Added
- Backgrounds and illustrations (CGs) are described when they appear, before the next line is read.
- The first time you meet a character, the mod describes them.
- The d key describes the scene: the background or illustration, and who is on screen with their expression and pose.
- Image descriptions live in `descriptions/<language>.tsv`, with English as the fallback for anything missing.
- Descriptions of every background and illustration, and of the characters, in English, Japanese, Korean, Simplified Chinese and Traditional Chinese, using the game's own names for characters and places in each language.
- "Describe backgrounds and illustrations" and "Describe characters the first time you meet them" settings, so either can be turned off; the d key still works either way.
- The Studio Élan logo and the title screen's "Press any button to start" are read at start-up.
- Standalone installer (PleaseBeHappyAccessInstaller.exe, built on Loadstone). It finds the game, installs BepInEx 5.4.23.5 and the mod with checksum-verified downloads, and can update, repair and uninstall.
- The game's pop-ups are read: song names, achievements, "Game saved!" and other notices, and the time of day at the start of a scene. "Read pop-ups" in the mod settings turns this off.
- Yes/no questions read the question, then the buttons.
- Tabs that are mostly text (Help's QUICK TIPS and CONTROLS, Extras' Gallery and Episodes) read their text when opened; repeated entries are counted ("??? Locked! (46 of these)"). Extras reads its welcome text.
- Filled save slots are read with chapter, scene and date (checked in game).
- When the game starts, the mod checks whether a newer version is out and says so. "Check for updates when the game starts" in the mod settings turns this off.

### Changed
- The mod's keys moved off the F keys: m opens the settings, v turns voiced lines on or off, r repeats or reads the screen, d describes the scene.
- The title menu has an **Accessibility** button, after Help, that opens the mod's settings.
- Characters coming and going are no longer announced. The setting is now "Describe characters the first time you meet them"; the d key still says who is on screen.
- Lines that are only "..." are read as "Ellipsis" instead of nothing.
- Works with the itch.io and GOG versions of the game as well as Steam. The installer asks for the game folder when the game isn't from Steam, and stops with an explanation if it's pointed at itch.io's 32-bit version, which the mod can't run.
- Works with the free demo (Steam and itch.io), checked in game. The README explains installing into the demo and moving to the full game.
- If one part of the mod fails (for example on a different version of the game), the other parts keep working instead of all stopping.
- Song names ("The Fox of Fable. Now playing.") have their own setting, "Read the name of each song as it starts playing", separate from "Read pop-ups".
- A dialogue line no longer cuts off a scene description; it's read straight after it.
- Scene descriptions start with "Scene description:", and the line after one starts with "Story text:".
- On voiced lines, descriptions wait until the voice actor has finished.
- Options opens on the open tab's first setting (for example "BASIC tab. Language, drop-down, English"); Up reaches the tabs. Choosing a tab says its name and moves to its first setting.
- Tabs are read as tabs; choice groups such as Music Cues say their row and current choice ("Music Cues: Disabled, option, currently Enabled"); drop-down entries say "chosen" or "option"; each voice volume row's button says it plays a sample.

### Fixed
- Escape in an open drop-down list closed the whole Options screen; now it only closes the list.
- On the title screen, the first arrow press could jump back to Start.
- Arrow keys no longer land on scroll bars, which read out a whole list of names.
- Layered backgrounds such as the airport crowd weren't described; they now use the image name the story asks for.
- One broken hook can no longer stop the rest of the mod from loading.
- The title screen's own greeting voice no longer marks the first story line as voiced.
- Labels keep their line breaks as pauses ("1. A Fox's Journey, The main menu theme."), and the literal `\n` and `\t` in some credits are no longer read out.
- Focus no longer lands on the confirmation's invisible close ribbon.
- Characters were announced by name before the story introduced them, giving names away that the game shows as "???". Until a name is revealed the mod now says "Someone appears" (or the game's own label, such as "Mother"), and saves the first-meeting description for when you know who they are.
- Image descriptions corrected after checking them against the pictures and the story: Elijah is Tommy's dad, not a teenager; the Aspen ending illustrations show Miho holding up her copy of Aspen's book (not "a girl with a box"); Lyra, Elijah and Gwen are now named in the illustrations they appear in; in the play's curtain call, Lena is dressed as the prince and Cyrus as the White Swan, and the masked actor isn't Miho; the fisher Mr. Kim helps is called the fisher; the Botanical Garden gates at night have archways, not iron gates. Fixed in all five languages.
- After loading a save, the scene could be described with the background from before the load. Loading now announces the loaded scene and who is in it.
- With scene descriptions turned off, "Story text:" could still be said before lines.
- A hidden title-menu button could get focus during the story and be read out ("Start, button").
- Music room entries ran the song name into its note ("A Fox's Journey The main menu theme"); the game separates them with tabs, which now read as a pause.
- Extras' Patrons tab said "Miho Plushie" over and over (placeholder text the game shows before filling in the names). Tabs are now read a moment after they open, and Patrons reads the patrons' names.
- The r key ran the lines of a list together ("Raistlin Majere Lambda Bledsoe"); each line is now its own item. Short lines starting in lower case ("phenoct") are no longer glued onto the line before.
- The r key in Options read the text-speed preview (the story's first paragraphs) after the settings; it now skips it.
- Backlog entries no longer read a doubled full stop before "Enter to go back to this line".
- With "Read voiced lines" off, the "Story text:" label no longer moves on to the next unvoiced line.
- Opening the mod settings before the game had finished starting could leave the story keys switched off after closing it.

## 0.4.0

### Added
- Story choices are read, and Up and Down move between them.
- The location map reads each place by name; places you can't go to are read as unavailable.
- The pause menu (Escape) says where you are in the story when it opens, for example "Chapter 1 - Day 1. Fox of Fable."
- Save and load slots are read with their number.
- The backlog reads each line's text, starting on the newest line, and tells you Enter goes back to that line.
- F3 repeats the last line during the story, or reads all the text on the open menu.

### Fixed
- After closing a menu, the arrow keys and Enter could act on hidden title-screen buttons during the story. Focus is now cleared when you return to the story.

## 0.3.0

### Added
- Menus get keyboard focus when they open (Options, Load, Extras, Help and others). Before, the arrow keys kept moving through the hidden title menu.
- Up and Down follow each menu's own order and stop at its first and last options instead of escaping into menus behind it.
- F1 opens the mod's settings menu inside the game.

### Changed
- Settings are saved inside the game instead of in a BepInEx config file.
- The open tab in a row of tabs is read as "selected" instead of "unavailable".

### Fixed
- The first arrow press on the title screen no longer jumps back to Start.

## 0.2.0

### Added
- Menu options are read as you move with the arrow keys, with their type and value (button, checked, slider percentage, drop-down choice).
- Buttons that only show an icon are read by their hidden label (Q.SAVE, LOG and so on); drop-downs are read with their row's caption, such as "Language".
- Reading of voiced lines can be turned off; F2 switches it.
- The mod's own messages are translated into Japanese, Korean, Simplified Chinese and Traditional Chinese, following the game's language.

## 0.1.0

### Added
- First version: a BepInEx 5 plugin that speaks and brailles through Prism.
- Dialogue and narration are read as each line appears, with the speaker's name first.
