using System;
using System.Collections.Generic;
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
                {AudioProdKey, GetProdModel},
                {VideoPresKey, GetVideoPresModel},
                {VideoPresIntKey, GetVideoPresIntModel},
                {VideoMezzKey, GetMezzModel},
                {AccessKey, GetAccessModel},
                {TiffImageKey, GetTiffImageModel }
            };

        private static ResolverKey AudioPresKey => new ResolverKey(".wav", "pres");
        private static ResolverKey AudioPresIntKey => new ResolverKey(".wav", "presint");
        private static ResolverKey VideoPresKey => new ResolverKey(".mkv", "pres");
        private static ResolverKey VideoPresIntKey => new ResolverKey(".mkv", "presint");
        private static ResolverKey AudioProdKey => new ResolverKey(".wav", "prod");
        private static ResolverKey VideoMezzKey => new ResolverKey(".mov", "mezz");
        private static ResolverKey AccessKey => new ResolverKey(".mp4", "access");
        private static ResolverKey TiffImageKey => new ResolverKey(".tif","label");

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

        private static AbstractFile GetAccessModel(AbstractFile arg)
        {
            return new AccessFile(arg);
        }

        private static AbstractFile GetTiffImageModel(AbstractFile arg)
        {
            return new TiffImageFile(arg);
        }

        public static AbstractFile GetModel(string path)
        {
            var rawModel = new UnknownFile(path);
            var key = new ResolverKey(rawModel);

            Func<AbstractFile, AbstractFile> resolver;

            return Resolvers.TryGetValue(key, out resolver)
                ? resolver(rawModel)
                : rawModel;
        }

        private struct ResolverKey
        {
            private string Extension { get; }
            private string FileUse { get; }

            public ResolverKey(string extension, string fileUse)
            {
                Extension = extension.ToDefaultIfEmpty().ToLowerInvariant();
                FileUse = fileUse.ToDefaultIfEmpty().ToLowerInvariant();
            }

            public ResolverKey(AbstractFile model)
            {
                Extension = model.Extension.ToDefaultIfEmpty().ToLowerInvariant();
                FileUse = model.FileUse.ToDefaultIfEmpty().ToLowerInvariant();
            }

            public override bool Equals(object obj)
            {
                return obj is ResolverKey && Equals((ResolverKey) obj);
            }

            private bool Equals(ResolverKey other)
            {
                return string.Equals(Extension, other.Extension) 
                    && string.Equals(FileUse, other.FileUse);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Extension?.GetHashCode() ?? 0)*397) ^ (FileUse?.GetHashCode() ?? 0);
                }
            }
        }
    }
}