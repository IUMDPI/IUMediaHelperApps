# IU Media Helper Apps - Recorder #

The IU Media Helper Recorder provides a graphical user shell for FFMPEG, allowing video engineers better control over the video capture and processing pipeline.

For a more detailed description of the recorder, see the Appendix to the white paper, [Indiana University Media Digitization and Preservation Initiative (MDPI)](https://mdpi.iu.edu/doc/MDPIwhitepaper.pdf).

## Build Instructions ##

1. Download and install [Visual Studio 2022 from Microsoft's download site](https://visualstudio.microsoft.com/downloads/). The free, Community Edition, is fine. Be sure to include the ".Net Desktop Development" workload in your installation.
2. Download or clone this repository from Github. The Recorder has dependencies on libraries that are shared with other projects in the repository, so you will need to clone the entire repository, not just the Recorder project folder.
3. Open the solution file, "MediaHelper.sln" in Visual Studio 2022
4. From the Visual Studio "Build" menu, select and open "Configuration Manager..."
5. On the Configuration Manager dialog, set "Active solution configuration" to "Release." Leave "Active solution platform" set to "Any CPU."
6. Click "Close" to exit the Configuration Manager.
7. From the Visual Studio Solution Explorer, highlight the Recorder project.
8. From the Visual Studio "Build" menu, select "Build Recorder." Alternatively, you can select "Build Solution" to build the entire solution.
9. When the build completes, it will create a "Release" and a "Deploy" folder in the .\IUMediaHelperApps\Recorder\bin. 
    1. Both folders contain the Recorder binaries, but the deploy folder contains only the files that need to be deployed to the engineer's computer. 
    2. Additionally, the "Recorder.exe.config" file is renamed to "Recorder.exe.config.dev" in the Deploy folder. This is to prevent accidently overwriting an engineer's production configuration file when updating the Recorder binaries.

## Configuring the Recorder ##

* The Recorder requires that FFMPEG and FFPROBE be installed on the engineer's system. You can download these from [ffbinaries.com](https://ffbinaries.com/). 
* The Recorder's configuration settings are stored in its "Recorder.exe.config" file. This file should be located in the same directory as the Recorder.exe binary.
* If you are installing the Recorder for the first time, you may need to rename or copy "Recorder.exe.config.dev" to "Recorder.exe.config."
* The following keys should be present and configured under the appSettings node in the "Recorder.exe.config" file:
    - `ProjectCode` - this key controls the "Project Code" element of video filenames. In the IU environment, it should be set to `mdpi`.
    - `PathToFFMPEG` - this key specifies the path to the FFMPEG binary. In the IU environment, it should be set to `C:\Dependencies\ffmpeg\ffmpeg.exe`.
    - `PathToFFProbe` - this key specifies the path to the FFPROBE binary. In the IU environment, it should be set to `C:\Dependencies\ffmpeg\ffprobe.exe`.
    - `OutputDirectoryName` - this key specifies the path to the Recorder's output directory. This is the directory to which final, assembled videos will be saved.
    - `WorkingDirectoryName` - this key specifies the path to the Recorder's working directory. This is the directory that contains video files that are still being processed.
    - `FFMPEGArguments` - the arguments string to pass to FFMPEG when capturing videos. In the IU production environment, this value should be `-rtbufsize 1500M -stats -f decklink -i &quot;DeckLink Studio 4K@1&quot; -acodec pcm_s24le -strict -2 -ar 48000 -vcodec ffv1 -level 3 -threads 8 -coder 1 -context 1 -g 1 -slices 24 -slicecrc 1 -pix_fmt yuv422p10le`.
    - `BarcodeScannerIdentifiers` - a comma-delimited list of identifiers associated with the barcode scanners attached to the system.
