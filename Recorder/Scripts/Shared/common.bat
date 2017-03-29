@echo off

set ffmpeg="c:\dependencies\ffmpeg\ffmpeg.exe"
set fixFile=%2

set targetFile=%1
set outDir=%~p3
set outputPath=%3
set workDir=%~p3work

set originalVideoHashPath="%workDir%\original_video.md5"
set outputVideoHashPath="%workDir%\output_video.md5"

set originalAudioLeftPath="%workDir%\original_audio_left.wav"
set originalAudioRightPath="%workDir%\original_audio_right.wav"
set outputAudioLeftPath="%workDir%\output_audio_left.wav"
set outputAudioRightPath="%workDir%\output_audio_right.wav"

set originalAudioLeftHashPath="%workDir%\original_audio_left.md5"
set originalAudioRightHashPath="%workDir%\original_audio_right.md5"
set outputAudioLeftHashPath="%workDir%\output_audio_left.md5"
set outputAudioRightHashPath="%workDir%\output_audio_right.md5"

if exist "%outDir%" rmdir /s "%outDir%" /q
if ERRORLEVEL 1 goto Err:

mkdir "%outDir%"
if ERRORLEVEL 1 goto Err:
mkdir "%workDir%"
if ERRORLEVEL 1 goto Err:

echo Hashing original video content
echo.
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

call %fixFile% %ffmpeg% %1 %outputPath%
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

if ERRORLEVEL 1 (EXIT /B 1)

EXIT /B 0
