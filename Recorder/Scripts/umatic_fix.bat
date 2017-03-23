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

if exist %outDir% rmdir /s %outDir% /q
if ERRORLEVEL 1 goto Err:

mkdir %outDir%
if ERRORLEVEL 1 goto Err:
mkdir %workDir%
if ERRORLEVEL 1 goto Err:

echo Hashing original video content

%ffmpeg% -y -loglevel error -i %targetFile% -map 0:v -f md5 %originalVideoHashPath%
if ERRORLEVEL 1 goto Err:

set /p originalVideoHash=<%originalVideoHashPath%
echo Original video stream hash:        %originalVideoHash%

%ffmpeg% -y -loglevel error -i %targetFile% -map_channel 0.1.0 %originalAudioLeftPath% -map_channel 0.1.1 %originalAudioRightPath%
if ERRORLEVEL 1 goto Err:
%ffmpeg% -y -loglevel error -i %originalAudioLeftPath% -f md5 %originalAudioLeftHashPath%
if ERRORLEVEL 1 goto Err:
%ffmpeg% -y -loglevel error -i %originalAudioRightPath% -f md5 %originalAudioRightHashPath%
if ERRORLEVEL 1 goto Err:

set /p originalAudioLeftHash=<%originalAudioLeftHashPath%
set /p originalAudioRightHash=<%originalAudioRightHashPath%
echo Original left audio channel hash:  %originalAudioLeftHash%
echo Original right audio channel hash: %originalAudioRightHash%
echo.

echo Correcting %~n1
echo.

%ffmpeg% -y -loglevel error -i %1 -y -codec:a pcm_s24le -codec:v copy -filter_complex "[0:a]pan=mono|c0=c0[a0];[0:a]pan=mono|c0=c1[a1]" -map 0:v -map "[a0]" -map "[a1]" %outputPath%
if ERRORLEVEL 1 goto Err:

echo Hashing output (corrected) video content
echo.

%ffmpeg% -y -loglevel error -i %outputPath% -map 0:v -f md5 %outputVideoHashPath%
if ERRORLEVEL 1 goto Err:

set /p outputVideoHash=<%outputVideoHashPath%
echo Corrected video stream hash:       %outputVideoHash%

%ffmpeg% -y -loglevel error -i %outputPath% -codec:a copy -map 0:1 %outputAudioLeftPath%
if ERRORLEVEL 1 goto Err:
%ffmpeg% -y -loglevel error -i %outputAudioLeftPath% -f md5 %outputAudioLeftHashPath%
if ERRORLEVEL 1 goto Err:
%ffmpeg% -y -loglevel error -i %outputPath% -codec:a copy -map 0:2 %outputAudioRightPath%
if ERRORLEVEL 1 goto Err:
%ffmpeg% -y -loglevel error -i %outputAudioRightPath% -f md5 %outputAudioRightHashPath%
if ERRORLEVEL 1 goto Err:

set /p outputAudioLeftHash=<%outputAudioLeftHashPath%
set /p outputAudioRightHash=<%outputAudioRightHashPath%
echo Corrected left audio stream hash:  %outputAudioLeftHash%
echo Corrected right audio stream hash: %outputAudioRightHash%
echo.

echo Comparing hashes
echo.

set noMatch=Hashes don't match
set match=Hashes match

set videoStreamHashResult=%noMatch%
set audioLeftHashResult=%noMatch%
set audioRightHashResult=%noMatch%

if "%originalVideoHash%"=="%outputVideoHash%" (
	set VideoStreamHashResult=%match%
)

if "%originalAudioLeftHash%"=="%outputAudioLeftHash%" (
	set audioLeftHashResult=%match%
)

if "%originalAudioRightHash%"=="%outputAudioRightHash%" (
	set audioRightHashResult=%match%
)

echo Video stream content:              %videoStreamHashResult%
echo Audio left stream content:         %audioLeftHashResult%
echo Audio right stream content:        %audioRightHashResult%

:Err
echo.

pause

exit /B