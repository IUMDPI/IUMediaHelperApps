using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Packager.Deserializers;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.PodMetadataModels;
using RestSharp;

namespace Packager.Providers
{
    public interface IPodMetadataProvider
    {
        Task<ConsolidatedPodMetadata> Get(string barcode);
    }

    internal class PodMetadataProvider : IPodMetadataProvider
    {
        public PodMetadataProvider(IProgramSettings programSettings, ILookupsProvider lookupsProvider)
        {
            ProgramSettings = programSettings;
            LookupsProvider = lookupsProvider;
        }

        private IProgramSettings ProgramSettings { get; set; }
        private ILookupsProvider LookupsProvider { get; set; }

        public async Task<ConsolidatedPodMetadata> Get(string barcode)
        {
            var client = new RestClient(string.Format(ProgramSettings.BaseWebServiceUrlFormat, barcode))
            {
                Authenticator =
                    new HttpBasicAuthenticator(ProgramSettings.PodAuth.UserName, ProgramSettings.PodAuth.Password),
               };

            var request = new RestRequest {DateFormat = "yyyy-MM-ddTHH:mm:sszzz"};
            var response = await client.ExecuteGetTaskAsync<PodMetadata>(request);

            VerifyResponse(response);

            return ConsolidateMetadata(response.Data);
        }


        private ConsolidatedPodMetadata ConsolidateMetadata(PodMetadata metadata)
        {
            var result = new ConsolidatedPodMetadata
            {
                Identifier = metadata.Data.Object.Details.Id,
                Format = metadata.Data.Object.Details.Format,
                CallNumber = metadata.Data.Object.Details.CallNumber,
                Title = metadata.Data.Object.Details.Title,
                Unit = LookupsProvider.LookupValue(LookupTables.Units, metadata.Data.Object.Assignment.Unit),
                Barcode = metadata.Data.Object.Details.MdpiBarcode,
                Brand = metadata.Data.Object.TechnicalMetadata.TapeStockBrand,
                DirectionsRecorded = metadata.Data.Object.TechnicalMetadata.DirectionsRecorded,
                CleaningDate = metadata.Data.Object.DigitalProvenance.CleaningDate,
                CleaningComment = metadata.Data.Object.DigitalProvenance.CleaningComment,
                BakingDate = metadata.Data.Object.DigitalProvenance.Baking,
                Repaired = ToYesNo(metadata.Data.Object.DigitalProvenance.Repaired),
                DigitizingEntity = metadata.Data.Object.DigitalProvenance.DigitizingEntity,
                PlaybackSpeed = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.PlaybackSpeed),
                TrackConfiguration = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.TrackConfiguration),
                SoundField = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.SoundField),
                TapeThickness = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.TapeThickness),
                FileProvenances = metadata.Data.Object.DigitalProvenance.DigitalFileProvenances,


            };

            return result;
        }

        private static string GetBoolValuesAsList(object instance)
        {
            if (instance == null)
            {
                return string.Empty;
            }

            var properties = instance.GetType().GetProperties().Where(p => p.PropertyType == typeof(bool));
            var results = (properties.Where(property => (bool)property.GetValue(instance))
                .Select(GetNameOrDescription)).Distinct().ToList();
            return string.Join(", ", results);
        }

        private static string GetNameOrDescription(MemberInfo propertyInfo)
        {
            var description = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
            return description == null ? propertyInfo.Name : description.Description;
        }

        private static string ToYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        private static void VerifyResponse(IRestResponse<PodMetadata> response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new PodMetadataException("Could not retrieve metadata from Pod", response.ErrorException);  
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new PodMetadataException("Could not retrieve metadata from Pod", new Exception(response.ErrorMessage));
            }

            if (response.Data == null)
            {
                throw new PodMetadataException("Could not retrieve metadata from Pod");
            }

            if (response.Data.Success == false)
            {
                throw new PodMetadataException(
                    "Could not retrieve metadata: {0}",
                    response.Data.Message.ToDefaultIfEmpty("[no error message present]"));
            }
        }
    }
}