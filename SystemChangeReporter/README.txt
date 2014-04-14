System Change Reporter
David Maxson
scnerd@gmail.com
4/13/14


==============================
INTRODUCTION
==============================

System Change Reporter (SCR) is a small utility to help track changes to the Windows file system and registry. It allows live tracking of the file system and on-demand polling of the registry, and displays changes to both since you last checked. You may also define filters of changes to ignore. Note that due to the registry's naming conventions, you may not define wildcards (* or ?) for registry filters. For further details, see the USAGE section below.


==============================
INSTALLATION
==============================

To get this SCR running properly, you should only need .NET 4.0 installed. If you do not have it installed already, you can install it from here:

http://www.microsoft.com/en-us/download/details.aspx?id=17851


==============================
LAUNCHING
==============================

Right-click on SystemChangeReporter.exe and click "Run As Administrator".

If you do not have administrator privileges, you can also just run SystemChangeReporter.exe, but much of the registry may be unviewable.

Note that if you have the entire source code, this can be found in SystemChangeReporter\SystemChangeReporter\bin\Release\


==============================
USAGE
==============================

--- Launching ---
When you first start up, SCR must retrieve an initial copy of the registry. Please be patient, as this may take 10-30 seconds, depending on your computer and hard drive speeds. Any keys that you do not have access to will be ignored, and filters for those keys will be added to your list of filters.

--- Understanding Change ---
Changes to the file system are shown in real-time, and changes to the registry are shown when you poll for them. Note that you can set up SCR to automatically poll every few seconds (see the Montoring section below). All changes are logged in the scrolling log boxes (middle-lower in the UI), and are displayed in a much friendlier tree format above that. Changes are color-coded in the following way:

Black - Unchanged, or unknown change
Blue  - Altered content
Green - Newly created
Red   - Deleted
Light Blue - Renamed

--- Main Controls ---
In the main UI, you are presented with two buttons, Reset and Freeze. Use the reset button to re-initialize the program's status, deleting all logged data and fetching a new copy of the registry. Use the freeze (and unfreeze) button to pause and resume logging and automatic polling.

--- Drives ---
You can select which drive you want to monitor in the upper-left hand corner. Just select a drive, and that drive will automatically begin watching for changes.

--- Filters ---
You may add filters to automatically ignore uninteresting changes, such as regular Windows operations or registry keys you aren't concerned about.

The easiest way to work with filters is to use the drop-down lists for Drive and Registry filters, respectively. The drop-down list will be automatically populated with all filters currently in use. Note that any displayed filter can be altered by simply clicking on it and changing it. A blank box will always be at the top of this list, so you can add a new filter with that.

Also, filters will be automatically added for registry keys that you don't have access to. You'll likely want to use the "Save Filters" button to save these for later, as it will make the registry initialization faster and cleaner next time around. Note that filters are automatically loaded from the "filters.txt" file at launch time (if the file exists), and can be saved back to that file using the "Save Filters" button. The "Load Filters" button is provided if you change the filters file during run time, and will overwrite the existing filters with those from the filters.txt file. This file can be directly edited using the "Edit Filters File" button.

-- Filters File --
The filters.txt file follows the following format:

Any line of the format
D'[stuff]'
will add [stuff] to your drive filters.

Any line of the format
R'[stuff]'
will add [stuff] to your registry filters.

Any other line will be ignored

-- Filter Rules --
All filters are completely case insensitive.

Drive filters may use standard wildcards. That is, you may use "?" to represent a single unknown character, or "*" to represent zero or more unknown characters. This is particularly useful to specify filters for any drive "?:\my\drive", or filters for entire directory trees "C:\users\me\stuff_not_to_scan\*"

Registry filters require that you specify exactly which key to ignore, such as "HKEY_CURRENT_USER\Environment" will ignore the Environment key, along with all of its subkeys. Note that any misspelling, like "HEKY_...." will cause the filter to do nothing.

--- Monitoring ---
The "Toggle Monitors" menu provides the ability to enable/disable drive monitoring and automatic registry polling. Note that by default, drives are automatically monitored, and the registry is not automatically polled. The registry can be manually polled using the "Manually Poll" button. If you do want automatic poll, simply click the "Toggle Monitors -> Registry Monitor" button to enable auto-polling. You can also change the number of seconds between polls in the "Toggle Monitors -> Registry Poll Time" menu. Note that this is the number of seconds after one poll finishes before the next starts, thus actual changes from the registry may be retrieved several seconds later than this poll time.

--- Help ---
Hitting the "Help" button will bring up this README. For further information, you may contact the author at scnerd@gmail.com
