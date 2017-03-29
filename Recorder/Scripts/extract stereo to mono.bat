c:\dependencies\ffmpeg\ffmpeg.exe -i %1 -y -map_channel 0.1.0 %~n1_left.wav -map_channel 0.1.1 %~n1_right.wav
pause