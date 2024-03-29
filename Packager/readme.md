# IU Media Helper Apps - Packager #

The IU Media Helper Packager is a post-processor that normalizes and embeds metadata into files generated by the audio and video engineers.

## Development Instructions ##

1. Download and install [Visual Studio 2022 from Microsoft's download site](https://visualstudio.microsoft.com/downloads/). The free, Community Edition, is fine. Be sure to include the ".Net Desktop Development" workload in your installation.
2. Download or clone this repository from Github. The Packager has dependencies on libraries that are shared with other projects in the repository, so you will need to clone the entire repository, not just the Recorder project folder.
3. Open the solution file, "MediaHelper.sln" in Visual Studio 2022
4. Open the Visual Studio Build menu and select "Build Solution." The solution should build.
5. Open the Visual Studio Test menu and select "Run all tests." All tests should pass.
6. In the Solution Explorer pane, right-click the Packager project and select "Set as Startup Project"
7. From the Debug menu, select "Start" (or press F5) to run the project. The Packager should run, though it may complain that its externel dependencies are missing. Close the Packager.
8. By default, the Packager expects the following binaries in the following locations on your development machine:
    1. `c:\dependencies\pod-Auth.xml` - path to XML file containing credentials to access the POD API. Do not commit this file to the source repository.
	2. `c:\dependencies\bwf-metaedit\bwfmetaedit.exe` - path to [BWF Metaedit](https://mediaarea.net/BWFMetaEdit) binary
	3. `c:\dependencies\ffmpeg\ffmpeg.exe` - path to [FFMPEG](https://www.ffmpeg.org/) binary
	4. `C:\Dependencies\ffmpeg\ffprobe.exe` - path to [FFProbe](https://ffmpeg.org/ffprobe.html) binary
	
	Generally, it is best to use the same versions of these binaries as the engineers are using, as they may be using a custom FFMPEG build, etc. It's also possible to modify the default locations for these binaries by updating the appropriate settings in the Packager's `App.config` file. See "Configuring the Packager" below.

9. By default, the Packager requires that the following input and output directories be present on your development machine:
	1. "Input directory" (`c:\work\mdpi`) - directory for files to be processed
	2. "Processing directory" (`c:\work\processing`) - directory for files being processed
	3. "Success directory" (`c:\work\success`) - directory for successfully processed files.
	4. "Error directory" (`c:\work\error`) - directory for files that could not be processed successfully.
	5. "Log director" (`c:\work\logs`) - directory for packager log files.
	6. "Drop Box Directory" (`c:\work\dropbox`) - post-Packager automated workflow directory for successfully processed files. On engineers' machines, this folder is typically a network share. Files in this directory are uploded to the server, etc. by external automated workflows. On Development machines, this folder is typically not a network share, as we don't want development versions of the packager to trigger external, automated workflows.
	7. "Images directory" (`c:\work\images`) - directory for images related to the media being processed (album artwork, etc.)
	
    As with the external binary paths, these directory paths can be customized in the Packager's `App.config` file. See "Configuring the Packager" below. 	


## Release Build Instructions ##

1. Download and install [Visual Studio 2022 from Microsoft's download site](https://visualstudio.microsoft.com/downloads/). The free, Community Edition, is fine. Be sure to include the ".Net Desktop Development" workload in your installation.
2. Download or clone this repository from Github. The Packager has dependencies on libraries that are shared with other projects in the repository, so you will need to clone the entire repository, not just the Recorder project folder.
3. Open the solution file, "MediaHelper.sln" in Visual Studio 2022
4. From the Visual Studio "Build" menu, select and open "Configuration Manager..."
5. On the Configuration Manager dialog, set "Active solution configuration" to "Release." Leave "Active solution platform" set to "Any CPU."
6. Click "Close" to exit the Configuration Manager.
7. From the Visual Studio Solution Explorer, highlight the Packager project.
8. From the Visual Studio "Build" menu, select "Build Packager." Alternatively, you can select "Build Solution" to build the entire solution.
9. When the build completes, it will create a "Release" and a "Deploy" folder in the .\IUMediaHelperApps\Packager\bin. 
    1. Both folders contain the Packager binaries, but the deploy folder contains only the files that need to be deployed to the engineer's computer. 
    2. Additionally, the "Packager.exe.config" file is renamed to "Packager.exe.config.dev" in the Deploy folder. This is to prevent accidently overwriting an engineer's production configuration file when updating the Recorder binaries.

## Configuring the Packager ##

* The Packager requires that BWF Metaedit, FFMPEG, and FFPROBE be installed on the engineer's system. You can download these from [BWF Metaedit](https://mediaarea.net/BWFMetaEdit) and [ffbinaries.com](https://ffbinaries.com/). 
* The Packager's configuration settings are stored in its "Packager.exe.config" file. This file should be located in the same directory as the Packager.exe binary.
* If you are installing the Packager for the first time, you may need to rename or copy "Packager.exe.config.dev" to "Packager.exe.config."
* The following keys should be present and configured under the appSettings node in the "Packager.exe.config" file:
    - `ProjectCode` - this key controls the "Project Code" element of video filenames. In the IU environment, it should be set to `mdpi`.
	- `DigitizingEntity` - the key specifies the Digitizing Entity value used in embedded metadata. In the IU environment, it should be set to `IU Media Digitization Studios`.
	- `WhereStaffWorkDirectoryName` - this key specifies the path to the input directory for files to be processed. In the IU environment, it should be set to `c:\work\mdpi`.
	- `ProcessingDirectoryName` - this key specifies the path to the directory for files being processed. In the IU environment, it should be set to `c:\work\processing`.
	- `SuccessDirectoryName` - this key specifies the path to the directory for successfully processed files. In the IU environment, it should be set to `c:\work\success`.
	- `ErrorDirectoryName` - this key specifies the directory for files that could not be processed successfully. In the IU environment, it should be set to `c:\work\error`.
	- `LogDirectoryName` - this key specifies the directory for packager log files. In the IU environment, it should be set to `c:\work\logs`.
	- `ImageDirectory` - this key specifies the directory for images related to the media being processed (album artwork, etc.). On development machines, it should be set to `c:\work\images`. Consult engineers for production value.
	- `DropBoxDirectoryName` - this key specifies the post-Packager automated workflow directory for successfully processed files. On development machines, it should be set to `c:\work\dropbox`. Consult engineers for production value.
	- `PathToMetaEdit` - this key specifies the path to the BWF Metaedit binary. In the IU environment, it should be set to `c:\dependencies\bwf-metaedit\bwfmetaedit.exe`.
    - `PathToFFMPEG` - this key specifies the path to the FFMPEG binary. In the IU environment, it should be set to `C:\Dependencies\ffmpeg\ffmpeg.exe`.
    - `PathToFFProbe` - this key specifies the path to the FFPROBE binary. In the IU environment, it should be set to `C:\Dependencies\ffmpeg\ffprobe.exe`.
    - `ffmpegAudioProductionArguments` - this key specifies the base FFMPEG arguments used for creating audio Production Master files. Consult engineers for best settings here.
	- `ffmpegAudioAccessArguments` - this key specifies the base FFMPEG arguments used for creating audio Access Master files. Consult engineers for best settings here.
	- `ffmpegVideoMezzanineArguments` - this key specifies the base FFMPEG arguments used for creating video Mezzanine Master files. Consult engineers for best settings here.
	- `ffmpegVideoAccessArguments` - this key specifies the base FFMPEG arguments used for creating video Access Master files. Consult engineers for best settings here.
	- `ffprobeVideoQualityControlArguments` - this key specifies the base arguments to pass to FFPROBE for quality-control-verification operations. Consult engineers for best settings here.
	- `PodAuthorizationFile` - this key specifies the path to the XML file containing credentials to be used to access the POD API. In the IU environment, it should be set to `c:\dependencies\pod-Auth.xml`.
	- `WebServiceUrl` - this key specifies the POD API url. In the IU environment, it should be to to `https://pod.mdpi.iu.edu/`.
	- `UnitPrefix` - this key specifies the name of the unit using the packager. Might not be currently used. In the IU environment, it should be set to `Indiana University-Bloomington`.
	- `SmtpServer` - this key specifies the SMTP server to use when sending packager notifications. Optional.
	- `FromEmailAddress` - this key specifies the "From" email address to use for packager notifications. Optional.
	- `IssueNotifyEmailAddresses` - this key specifies email adddresses to use when sending issue notifications. Separate addresses with a comma. Optional.
	- `SuccessNotifyEmailAddresses` - this key specifies email addresses to use when sending success notifications. Separate addresses with a comma. Optional.
	- `DeferredNotifyEmailAddresses` - this key specifies email addresses to use when sending processing-deferred notifications. Separate email adddresses with a comma. Optional.
	- `DeleteProcessedAfterInDays` - interval (in days) to use when removing processed files from the success directory. Set to 0 to disable.
