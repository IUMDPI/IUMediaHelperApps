using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Validators;
using RestSharp;

namespace Packager.Test
{
    [TestFixture]
    public abstract class PodMetadataProviderTests
    {
        /* [SetUp]
         public virtual void BeforeEach()
         {
             Response = GetRestResponse();
             RestClient = GetRestClient();
             Observers = Substitute.For<IObserverCollection>();
             Validators = Substitute.For<IValidatorCollection>();
             Provider = new PodMetadataProvider(RestClient, Observers, Validators);
         }

         private IRestClient RestClient { get; set; }
         private IObserverCollection Observers { get; set; }
         private IValidatorCollection Validators { get; set; }

         private PodMetadataProvider Provider { get; set; }

         private IRestResponse<PodMetadata> Response { get; set; }

         protected abstract IRestClient GetRestClient();

         protected abstract IRestResponse<PodMetadata> GetRestResponse();

         internal abstract class WhenGettingObjectMetadata : PodMetadataProviderTests
         {
             private PodMetadata GetBaseMetadata()
             {
                 var metadata = new PodMetadata
                 {
                     Success = true,
                     Data = new Data
                     {
                         Object = new DataObject
                         {
                             Basics = new Basics
                             {
                                 Files = 1,
                                 Format = "testing"
                             },
                             Details = new Details
                             {
                                 Format = "tesing",
                                 CallNumber = "testing",
                                 Title = "testing",
                                 MdpiBarcode = "barcode"
                             },
                             Assignment = new Assignment
                             {
                                 Unit = "unit"
                             },
                             TechnicalMetadata = new TechnicalMetadata
                             {
                                 TapeStockBrand = "tape stock brand",
                                 DirectionsRecorded = "2",
                                 PlaybackSpeed = new PlaybackSpeed
                                 {
                                     SevenPoint5Ips = true
                                 },
                                 TrackConfiguration = new TrackConfiguration
                                 {
                                     HalfTrack = true
                                 },
                                 SoundField = new SoundField
                                 {
                                     Stereo = true
                                 },
                                 TapeThickness = new TapeThickness
                                 {
                                     OneMils = true
                                 },
                                 Damage = new Damage(),
                                 PreservationProblems = new PreservationProblems()
                             },
                             DigitalProvenance = new DigitalProvenance
                             {
                                 CleaningDate = new DateTime(2015, 08, 01),
                                 CleaningComment = "cleaning comment",
                                 Baking = new DateTime(2015, 08, 01),
                                 Repaired = false,
                                 DigitizingEntity = "digitizing entity",
                                 DigitalFiles = new List<DigitalFileProvenance>
                                 {
                                     new DigitalFileProvenance
                                     {
                                         DateDigitized = new DateTime(2015, 08, 01),
                                         SignalChain = new List<Device>()
                                     }
                                 }
                             }
                         }
                     }
                 };

                 return metadata;
             }

             private PodMetadata BaseMetadata { get; set; }

             protected override IRestClient GetRestClient()
             {
                 var result = Substitute.For<IRestClient>();
                 result.ExecuteGetTaskAsync<PodMetadata>(Arg.Any<IRestRequest>()).Returns(Task.FromResult(Response));
                 return result;
             }

             private ConsolidatedPodMetadata Result { get; set; }

             public class WhenThingsGoWell : WhenGettingObjectMetadata
             {
                 public override async void BeforeEach()
                 {
                     base.BeforeEach();
                     Result = await Provider.GetObjectMetadata("test");
                 }

                 protected override IRestResponse<PodMetadata> GetRestResponse()
                 {
                     BaseMetadata = GetBaseMetadata();
                     var result = Substitute.For<IRestResponse<PodMetadata>>();
                     result.ResponseStatus.Returns(ResponseStatus.Completed);
                     result.StatusCode.Returns(HttpStatusCode.OK);
                     result.Data.Returns(BaseMetadata);
                     return result;
                 }

                 [Test]
                 public void ItShouldConvertCleaningDateToUtc()
                 {
                     Assert.That(Result.CleaningDate.HasValue, Is.True);
                     Assert.That(Result.CleaningDate.Value, Is.EqualTo(BaseMetadata.Data.Object.DigitalProvenance.CleaningDate.Value.ToUniversalTime()));
                 }

                 [Test]
                 public void ItShouldConvertBakingDateToUtc()
                 {
                     Assert.That(Result.BakingDate.HasValue, Is.True);
                     Assert.That(Result.BakingDate.Value, Is.EqualTo(BaseMetadata.Data.Object.DigitalProvenance.Baking.Value.ToUniversalTime()));
                 }

                 [Test]
                 public void ItShouldConvertDateDigitizedValuesToUtc()
                 {
                     var original = BaseMetadata.Data.Object.DigitalProvenance.DigitalFiles.First();
                     var provenance = Result.FileProvenances.First();

                     Assert.That(original, Is.Not.Null);
                     Assert.That(provenance, Is.Not.Null);

                     Assert.That(original.DateDigitized.HasValue, Is.True);
                     Assert.That(provenance.DateDigitized.HasValue, Is.True);

                     Assert.That(provenance.DateDigitized.Value, Is.EqualTo(original.DateDigitized.Value.ToUniversalTime()));
                 }
             }
         }*/
    }
}