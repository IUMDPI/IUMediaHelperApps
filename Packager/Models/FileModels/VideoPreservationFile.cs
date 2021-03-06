﻿namespace Packager.Models.FileModels
{
    public class VideoPreservationFile : AbstractPreservationFile
    {
        private const string ExtensionValue = ".mkv";

        public VideoPreservationFile(AbstractFile original) : base(original, ExtensionValue)
        {
        }

        public override bool ShouldNormalize => true;
    }
}