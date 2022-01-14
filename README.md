# applbot-CDTracker
ACT Plugin for FFXIV, a simple cooldown timer tracker.

Please put the folder inside your ACT folder!

Download from the release section: https://github.com/applepudding/applbot-CDTracker/releases

![In Game Screenshot](https://github.com/applepudding/applbot-CDTracker/blob/master/screenshot0.png?raw=true)

Edit the config.json to add/adjust skill cooldowns, and put pictures in the imgs folder.
"rawmode_autoreset":0 ---> 0 will use act AfterCombatAction, other values will use OnLogLineRead.
It's recommended to use 0 if the ffxiv main plugin is up to date, otherwise set it to 300(for example)!
