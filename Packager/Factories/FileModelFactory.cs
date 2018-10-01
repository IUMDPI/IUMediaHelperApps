using System;
using System.Collections.Generic;
using Common.Models;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Factories
{
    public static class FileModelFactory
    { 
        private static readonly Dictionary<ResolverKey, Func<AbstractFile, AbstractFile>> Resolvers =
            new Dictionary<ResolverKey, Func<AbstractFile, AbstractFile>>
            {
                {AudioPresKey, GetAudioPresModel},
                {AudioPresIntKey, GetAudioPresIntModel},
                {AudioPresToneRefKey, GetAudioPresToneRefModel},
                {AudioPresIntToneRefKey, GetAudioPresIntToneRefModel},
                {AudioProdKey, GetProdModel},
                {VideoPresKey, GetVideoPresModel},
                {VideoPresIntKey, GetVideoPresIntModel},
                {VideoMezzKey, GetMezzModel},
                {AccessKey, GetAccessModel},
                {TiffImageKey, GetTiffImageModel },
                {CuePresKey,GetCueModel },
                {CuePresIntKey, GetCueModel},
                {TxtKey, GetTxtModel }
            };

        private static ResolverKey AudioPresKey => new ResolverKey(".wav", FileUsages.PreservationMaster);
        private static ResolverKey AudioPresIntKey => new ResolverKey(".wav", FileUsages.PreservationIntermediateMaster);
        private static ResolverKey AudioPresToneRefKey => new ResolverKey(".wav", FileUsages.PreservationToneReference);
        private static ResolverKey AudioPresIntToneRefKey => new ResolverKey(".wav", FileUsages.PreservationIntermediateToneReference);
        private static ResolverKey VideoPresKey => new ResolverKey(".mkv", FileUsages.PreservationMaster);
        private static ResolverKey VideoPresIntKey => new ResolverKey(".mkv", FileUsages.PreservationIntermediateMaster);
        private static ResolverKey AudioProdKey => new ResolverKey(".wav", FileUsages.ProductionMaster);
        private static ResolverKey VideoMezzKey => new ResolverKey(".mov", FileUsages.MezzanineFile);
        private static ResolverKey AccessKey => new ResolverKey(".mp4", FileUsages.AccessFile);
        private static ResolverKey TiffImageKey => new ResolverKey(".tif", FileUsages.LabelImageFile);
        private static ResolverKey CuePresKey => new ResolverKey(".cue", FileUsages.PreservationMaster);
        private static ResolverKey CuePresIntKey => new ResolverKey(".cue", FileUsages.PreservationIntermediateMaster);
        private static ResolverKey TxtKey => new ResolverKey(".txt", FileUsages.PreservationMaster);

        private static AbstractFile GetMezzModel(AbstractFile arg)
        {
            return new MezzanineFile(arg);
        }

        private static AbstractFile GetVideoPresIntModel(AbstractFile arg)
        {
            return new VideoPreservationIntermediateFile(arg);
        }

        private static AbstractFile GetVideoPresModel(AbstractFile arg)
        {
            return new VideoPreservationFile(arg);
        }

        private static AbstractFile GetProdModel(AbstractFile arg)
        {
            return new ProductionFile(arg);
        }

        private static AbstractFile GetAudioPresIntModel(AbstractFile arg)
        {
            return new AudioPreservationIntermediateFile(arg);
        }

        private static AbstractFile GetAudioPresModel(AbstractFile arg)
        {
            return new AudioPreservationFile(arg);
        }
        
        private static AbstractFile GetAudioPresIntToneRefModel(AbstractFile model)
        {
            return new AudioPreservationIntermediateToneReferenceFile(model);
        }

        private static AbstractFile GetAudioPresToneRefModel(AbstractFile model)
        {
            return new AudioPreservationToneReferenceFile(model);
        }

        private static AbstractFile GetAccessModel(AbstractFile arg)
        {
            return new AccessFile(arg);
        }

        private static AbstractFile GetTiffImageModel(AbstractFile arg)
        {
            return new TiffImageFile(arg);
        }

        private static AbstractFile GetTxtModel(AbstractFile arg)
        {
            return new TextFile(arg);
        }

        private static AbstractFile GetCueModel(AbstractFile arg)
        {
            return new CueFile(arg);
        }

        public static AbstractFile GetModel(string path)
        {
            var rawModel = new UnknownFile(path);
            var key = new ResolverKey(rawModel);


            Resolvers.TryGetValue(key, out var resolver);
            if (resolver != null)
            {
                return resolver(rawModel);
            }
            else
            {
                return rawModel;
            }
        }

      
        private struct ResolverKey
        {
            private string Extension { get; }
            private IFileUsage FileUsage { get; }

            public ResolverKey(string extension, IFileUsage fileUsage)
            {
                Extension = extension.ToDefaultIfEmpty().ToLowerInvariant();
                FileUsage = fileUsage;
            }

            public ResolverKey(AbstractFile model)
            {
                Extension = model.Extension.ToDefaultIfEmpty().ToLowerInvariant();
                FileUsage = model.FileUsage;
            }

            public override bool Equals(object obj)
            {
                return obj is ResolverKey && Equals((ResolverKey)obj);
            }

            private bool Equals(ResolverKey other)
            {
                return string.Equals(Extension, other.Extension)
                    && FileUsage == other.FileUsage;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Extension?.GetHashCode() ?? 0) * 397) ^ (FileUsage?.GetHashCode() ?? 0);
                }
            }
        }
    }
}