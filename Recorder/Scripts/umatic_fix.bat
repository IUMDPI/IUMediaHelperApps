@echo off

echo.
echo ***********************************************************
echo *                                                         *
echo * Umatic Fix                                              *    
echo *                                                         *
echo * Converts a video file (.mkv) with one stereo audio      *
echo * stream to one with 2 mono audio streams containing      *
echo * the content of the original's stereo channels.          *
echo *                                                         *
echo ***********************************************************
echo.

set baseScriptPath=%~dp0
set commonCodePath="%baseScriptPath%\Shared\common.bat"
set internalFixCode="%baseScriptPath%\Internal\umatic_fix_internal.bat"
set outputPath="%~p1%~n1_umatic_fix\%~n1%~x1"

call %commonCodePath% %1 %internalFixCode% %outputPath%

:Err
echo.

pause

exit /B