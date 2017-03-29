@echo off

echo.
echo ***********************************************************
echo *                                                         *
echo * Betacam Fix                                             *    
echo *                                                         *
echo * Converts a video file (.mkv) with a single, stereo      *
echo * audio stream to one with 4 mono audio streams, with the *
echo * the first two streams of the new file containing the    *
echo * content of the original's stereo channals.              *
echo *                                                         *
echo ***********************************************************
echo.

set baseScriptPath=%~dp0
set commonCodePath="%baseScriptPath%\Shared\common.bat"
set internalFixCode="%baseScriptPath%\Internal\betacam_fix_internal.bat"
set outputPath="%~p1%~n1_betacam_fix\%~n1%~x1"

set workDir="%~p1%~n1_betacam_fix\work"
set ffmpeg="c:\dependencies\ffmpeg\ffmpeg.exe"
set outputAudioStream3WavPath="%~p1%~n1_betacam_fix\work\output_audio_stream_3.wav"
set outputAudioStream4WavPath="%~p1%~n1_betacam_fix\work\output_audio_stream_4.wav"
set outputAudioStream3Md5Path="%~p1%~n1_betacam_fix\work\output_audio_stream_3.md5"
set outputAudioStream4Md5Path="%~p1%~n1_betacam_fix\work\output_audio_stream_4.md5"

call %commonCodePath% %1 %internalFixCode% %outputPath%
If ERRORLEVEL 1 GOTO Err

echo.
echo Examining empty streams
echo.

%ffmpeg% -y -loglevel error -i %outputPath% -codec:a copy -map 0:3 %outputAudioStream3WavPath%
if ERRORLEVEL 1 goto Err:
%ffmpeg% -y -loglevel error -i %outputAudioStream3WavPath% -f md5 %outputAudioStream3Md5Path%
if ERRORLEVEL 1 goto Err:

%ffmpeg% -y -loglevel error -i %outputPath% -codec:a copy -map 0:4 %outputAudioStream4WavPath%
if ERRORLEVEL 1 goto Err:
%ffmpeg% -y -loglevel error -i %outputAudioStream4WavPath% -f md5 %outputAudioStream4Md5Path%
if ERRORLEVEL 1 goto Err:

set /p audioStream3Hash=<%outputAudioStream3Md5Path%
set /p audioStream4Hash=<%outputAudioStream4Md5Path%
echo audio stream 3 hash:               %audioStream3Hash%
echo audio stream 4 hash:               %audioStream4Hash%

set match=Hashes match
set noMatch=Hashes don't match
set additionalStreamsResult=%noMatch%

if "%audioStream3Hash%"=="%audioStream4Hash%" (
	set additionalStreamsResult=%match%
)

echo Empty streams:                     %additionalStreamsResult%


:Err
echo.

pause

exit /B