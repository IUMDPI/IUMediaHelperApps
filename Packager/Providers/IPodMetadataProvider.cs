using System;
using System.Net;
using System.Threading.Tasks;
using Packager.Deserializers;
using Packager.Exceptions;
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


            // use custom deserializer
            var serializer = new PodMetadataDeserializer(LookupsProvider);
            client.AddHandler("text/xml", serializer);
            client.AddHandler("application/xml", serializer);

            var request = new RestRequest {DateFormat = "yyyy-MM-ddTHH:mm:sszzz"};

            var response = await client.ExecuteGetTaskAsync<ConsolidatedPodMetadata>(request);

            VerifyResponse(response);

            return response.Data;
        }

        private static void VerifyResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new PodMetadataException("Could not retrieve metadata from Pod", response.ErrorException);  
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new PodMetadataException("Could not retrieve metadata from Pod", new Exception(response.ErrorMessage));
            }
        }
    }
}