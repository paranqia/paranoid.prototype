---
description: Create a quick Zip backup of the Assets and ProjectSettings
---

1. Create a Backups directory if it doesn't exist
   - run: `mkdir Backups` (ignore error if exists)

2. Compress Assets and ProjectSettings to a zip file with a timestamp
   - Powershell command to zip `Assets` and `ProjectSettings` folders.
   - run: `Compress-Archive -Path Assets, ProjectSettings -DestinationPath "Backups/Backup_$(Get-Date -Format 'yyyyMMdd_HHmm').zip"`

3. List the new backup
   - run: `ls Backups`
