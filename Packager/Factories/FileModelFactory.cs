using Packager.Models.FileModels;

namespace Packager.Factories
{
    public static class FileModelFactory
    {
        public static AbstractFile GetModel(string path)
        {
            var rawModel = new UnknownFile(path);

            switch (rawModel.Extension.ToLowerInvariant())
            {
                case ".wav":
                    return ConvertToSpecificAudioFileModel(rawModel);
                case ".mkv":
                    return ConvertToSpecifiedVideoFileModel(rawModel);
                case ".mov":
                    return ConvertToSpecifiedVideoFileModel(rawModel);
                default:
                    return new UnknownFile(path);
            }
        }

        private static AbstractFile ConvertToSpecificAudioFileModel(AbstractFile unknownModel)
        {
            switch (unknownModel.FileUse.ToLowerInvariant())
            {
                case "pres":
                    return new AudioPreservationFile(unknownModel);
                case "presint":
                    return new AudioPreservationIntermediateFile(unknownModel);
                case "prod":
                    return new ProductionFile(unknownModel);
                default:
                    return unknownModel;
            }
        }

        private static AbstractFile ConvertToSpecifiedVideoFileModel(AbstractFile unknownModel)
        {
            switch (unknownModel.FileUse.ToLowerInvariant())
            {
                case "pres":
                    return new VideoPreservationFile(unknownModel);
                case "pres-int":
                    return new VideoPreservationIntermediateFile(unknownModel);
                case "prod":
                    return new MezzanineFile(unknownModel);
                default:
                    return unknownModel;
            }
        }
    }
}
