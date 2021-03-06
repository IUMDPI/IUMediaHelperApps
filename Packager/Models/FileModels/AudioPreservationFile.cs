﻿namespace Packager.Models.FileModels
{
    public class AudioPreservationFile : AbstractPreservationFile
    {
        public AudioPreservationFile(AbstractFile original) : base(original, original.Extension)
        {
        }

        public override bool ShouldNormalize => Extension.Equals(".wav");
    }
}