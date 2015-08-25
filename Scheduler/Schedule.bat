@echo off
:: use schedule.exe
scheduler.exe -days=monday,tuesday,wednesday,thursday,friday -start=19:00

:: To use windows command, comment out following line
:: schtasks /create /tn "Media Packager" /tr "%CD%\Packager.exe" /sc weekly /d MON,WED,THU,FRI /st 19:00