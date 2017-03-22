c:\dependencies\ffmpeg\ffmpeg.exe -i %1 -f lavfi -i anullsrc=r=48000:cl=mono -f lavfi -i anullsrc=r=48000:cl=mono -codec:a pcm_s24le -codec:v copy -filter_complex "[0:a]pan=mono|c0=c0[a0];[0:a]pan=mono|c0=c1[a1]" -map 0:v -map "[a0]" -map "[a1]" -map 1:0 -map 2:0 -shortest %~n1_fixed_betacam%~x1

pause