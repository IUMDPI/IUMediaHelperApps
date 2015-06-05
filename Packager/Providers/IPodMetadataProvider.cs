using System.Threading.Tasks;
using Packager.Models;
using Packager.Models.PodMetadataModels;
using RestSharp;

namespace Packager.Providers
{
    public interface IPodMetadataProvider
    {
        Task<PodMetadata> Get(string barcode);
    }

    internal class PodMetadataProvider : IPodMetadataProvider
    {
        public PodMetadataProvider(IProgramSettings programSettings)
        {
            ProgramSettings = programSettings;
        }

        private IProgramSettings ProgramSettings { get; set; }

        public async Task<PodMetadata> Get(string barcode)
        {
            var client = new RestClient(string.Format(ProgramSettings.BaseWebServiceUrlFormat, barcode))
            {
                Authenticator =
                    new HttpBasicAuthenticator(ProgramSettings.PodAuth.UserName, ProgramSettings.PodAuth.Password)
            };

            var request = new RestRequest {DateFormat = "yyyy-MM-ddTHH:mm:sszzz"};

            var response = await client.ExecuteGetTaskAsync<PodMetadata>(request);
           
            return response.Data;
        }
    }
}