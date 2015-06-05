using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Models;
using RestSharp;

namespace Packager.Providers
{
    public interface IPodMetadataProvider
    {
        Task<PodMetadata> Get(string barcode);
    }

    class PodMetadataProvider : IPodMetadataProvider
    {
        private IProgramSettings ProgramSettings { get; set; }

        public PodMetadataProvider(IProgramSettings programSettings)
        {
            ProgramSettings = programSettings;
        }

        public async Task<PodMetadata> Get(string barcode)
        {
            var client = new RestClient(string.Format(ProgramSettings.BaseWebServiceUrlFormat, barcode))
            {
                Authenticator =
                    new HttpBasicAuthenticator(ProgramSettings.PodAuth.UserName, ProgramSettings.PodAuth.Password)
            };

            var request = new RestRequest();

            var response = await client.ExecuteGetTaskAsync<PodMetadata>(request);

            return response.Data;
        }
    }
}
