using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class FileModelExtensionTests
    {
        private readonly ObjectFileModel _objectFileModel = new ObjectFileModel("test.txt");
        private readonly UnknownFileModel _unknownFileModel = new UnknownFileModel();
        private readonly XmlFileModel _xmlFileModel = new XmlFileModel();

      
        [Test]
        public void IsObjectModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_objectFileModel.IsObjectModel(), Is.True);
            Assert.That(_xmlFileModel.IsObjectModel(), Is.False);
            Assert.That(_unknownFileModel.IsObjectModel(), Is.False);
        }

        [Test]
        public void IsXmlModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_objectFileModel.IsXmlModel(), Is.False);
            Assert.That(_xmlFileModel.IsXmlModel(), Is.True);
            Assert.That(_unknownFileModel.IsXmlModel(), Is.False);
        }

        [Test]
        public void IsUnknownModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_objectFileModel.IsUnknownModel(), Is.False);
            Assert.That(_xmlFileModel.IsUnknownModel(), Is.False);
            Assert.That(_unknownFileModel.IsUnknownModel(), Is.True);
        }

        [Test]
        public void EnumGetPreservationOrIntermediateModelReturnIntermediateIfPresent()
        {
            var model1 = new ObjectFileModel("MDPI_1111_01_pres.wav");
            var model2 = new ObjectFileModel("MDPI_1111_01_pres-int.wav");
            var model3 = new ObjectFileModel("MDPI_1111_01_prod.wav");

            var list = new List<ObjectFileModel> {model1, model2, model3};
            Assert.That(list.GetPreservationOrIntermediateModel(), Is.EqualTo(model2));
        }


        [Test]
        public void EnumGetPreservationOrIntermediateModelReturnPresIfPresIntNotPresent()
        {
            var model1 = new ObjectFileModel("MDPI_1111_01_prod.wav");
            var model2 = new ObjectFileModel("MDPI_1111_01_pres.wav");
            

            var list = new List<ObjectFileModel> { model1, model2};
            Assert.That(list.GetPreservationOrIntermediateModel(), Is.EqualTo(model2));
        }


        [Test]
        public void EnumGetPreservationOrIntermediateModelReturnNullIfNeitherPresent()
        {
            var model1 = new ObjectFileModel("MDPI_1111_01_prod.wav");
            var model2 = new ObjectFileModel("MDPI_1111_01_access.wav");


            var list = new List<ObjectFileModel> { model1, model2 };
            Assert.That(list.GetPreservationOrIntermediateModel(), Is.EqualTo(null));
        }

        [Test]
        public void GroupingGetPreservationOrIntermediateModelReturnIntermediateIfPresent()
        {
            var model1 = new ObjectFileModel("MDPI_1111_01_pres.wav");
            var model2 = new ObjectFileModel("MDPI_1111_01_pres-int.wav");
            var model3 = new ObjectFileModel("MDPI_1111_01_prod.wav");

            var grouping = new List<ObjectFileModel> { model1, model2, model3 }
                .GroupBy(m=>m.BarCode).ToList().Single();

            Assert.That(grouping.GetPreservationOrIntermediateModel(), Is.EqualTo(model2));
        }


        [Test]
        public void GroupingGetPreservationOrIntermediateModelReturnPresIfPresIntNotPresent()
        {
            var model1 = new ObjectFileModel("MDPI_1111_01_prod.wav");
            var model2 = new ObjectFileModel("MDPI_1111_01_pres.wav");

            var grouping = new List<ObjectFileModel> { model1, model2 }
                 .GroupBy(m => m.BarCode).ToList().Single();
            Assert.That(grouping.GetPreservationOrIntermediateModel(), Is.EqualTo(model2));
        }

        [Test]
        public void GroupingGetPreservationOrIntermediateModelReturnNullIfNeitherPresent()
        {
            var model1 = new ObjectFileModel("MDPI_1111_01_prod.wav");
            var model2 = new ObjectFileModel("MDPI_1111_01_access.wav");

            var grouping = new List<ObjectFileModel> { model1, model2 }
                 .GroupBy(m => m.BarCode).ToList().Single();
            Assert.That(grouping.GetPreservationOrIntermediateModel(), Is.EqualTo(null));
        }


        [Test]
        public void RemoveDuplicatesShouldWorkCorrectly()
        {
            var model1 = new ObjectFileModel("MDPI_1111_01_pres.wav");
            var model2 = new ObjectFileModel("MDPI_1111_01_pres.wav");
            var model3 = new ObjectFileModel("MDPI_2222_01_pres.wav");

            var models = new List<ObjectFileModel> {model1, model2, model3};

            var result = models.RemoveDuplicates();
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Count(m=>m.ToFileName().Equals("MDPI_1111_01_pres.wav")), Is.EqualTo(1));
            Assert.That(result.Count(m => m.ToFileName().Equals("MDPI_2222_01_pres.wav")), Is.EqualTo(1));
        }
    }
}