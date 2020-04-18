@echo off

if not exist "ScadaServer"      mkdir "ScadaServer"
if not exist "ScadaServer\Mod"  mkdir "ScadaServer\Mod"
if not exist "ScadaServer\Lang" mkdir "ScadaServer\Lang"

xcopy "ModAlarm\bin\Release\ModAlarm.dll"  "ScadaServer\Mod" /F /Y
xcopy "ModAlarm\bin\Release\NAudio.dll"    "ScadaServer\Mod" /F /Y
xcopy "ModAlarm\ModAlarm.en-Gb.xml"        "ScadaServer\Lang" /F /Y

7z a "ModAlarm 1.2.5.zip" ".\ScadaServer\*"
