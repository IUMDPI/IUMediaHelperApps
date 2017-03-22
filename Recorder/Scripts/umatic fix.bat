@echo off

set ffmpeg="c:\dependencies\ffmpeg\ffmpeg.exe"

set targetFile=%1
set outDir="%~p1%~n1"
set outputPath="%~p1%~n1\%~n1%~x1"
set workDir="%~p1%~n1\work"

set originalVideoHashPath="%~p1%~n1\work\original_video.md5"
set outputVideoHashPath="%~p1%~n1\work\output_video.md5"

set originalAudioLeftPath="%~p1\%~n1\work\original_audio_left.wav"
set originalAudioRightPath="%~p1\%~n1\work\original_audio_right.wav"
set outputAudioLeftPath="%~p1\%~n1\work\output_audio_left.wav"
set outputAudioRightPath="%~p1\%~n1\work\output_audio_right.wav"

set originalAudioLeftHashPath="%~p1\%~n1\work\original_audio_left.md5"
set originalAudioRightHashPath="%~p1\%~n1\work\original_audio_right.md5"
set outputAudioLeftHashPath="%~p1\%~n1\work\output_audio_left.md5"
set outputAudioRightHashPath="%~p1\%~n1\work\output_audio_right.md5"


echo %outDir%
echo %workDir%
echo.
echo Creating Directories

if not exist %outDir% mkdir %outDir%
if not exist %workDir% mkdir %workDir%

echo Hashing original video content

%ffmpeg% -y -loglevel error -i %targetFile% -map 0:v -f md5 %originalVideoHashPath%

echo Extracting and hashing orginal audio channels

%ffmpeg% -y -loglevel error -i %targetFile% -codec:a copy -map_channel 0.1.0 %originalAudioLeftPath% -map_channel 0.1.1 %originalAudioRightPath%
%ffmpeg% -y -loglevel error -i %originalAudioLeftPath% -map 0:a -f md5 %originalAudioLeftHashPath%
%ffmpeg% -y -loglevel error -i %originalAudioRightPath% -map 0:a -f md5 %originalAudioRightHashPath%

echo Fixing %~n1

%ffmpeg% -y -loglevel error -i %1 -y -codec:a pcm_s24le -codec:v copy -filter_complex "[0:a]pan=mono|c0=c0[a0];[0:a]pan=mono|c0=c1[a1]" -map 0:v -map "[a0]" -map "[a1]" %outputPath%

echo Hashing output video content

%ffmpeg% -y -loglevel error -i %outputPath% -map 0:v -f md5 %outputVideoHashPath%

echo Extracting and hashing output audio content

%ffmpeg% -y -loglevel error -i %outputPath% -map 0:1 -codec:a copy %outputAudioLeftPath%
%ffmpeg% -y -loglevel error -i %outputAudioLeftPath% -map 0:a -f md5 %outputAudioLeftHashPath%

%ffmpeg% -y -loglevel error -i %outputPath% -map 0:2 -codec:a copy %outputAudioRightPath%
%ffmpeg% -y -loglevel error -i %outputAudioRightPath% -map 0:a -f md5 %outputAudioRightHashPath%

pause