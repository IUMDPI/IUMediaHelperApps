
schtasks /create /tn "Media Packager" /tr "%CD%\Packager.exe" /sc weekly /d MON,WED,THU,FRI /st 19:00