using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class FileModelExtensionTests
    {
        private readonly AbstractFile _objectFileModel = FileModelFactory.GetModel("mdpi_4890764553278906_01_pres.wav");
        private readonly UnknownFile _unknownFileModel = new UnknownFile("test.text");
        private readonly XmlFile _xmlFileModel = new XmlFile("mdpi", "111111111");
        
        [Test]
        public void IsXmlModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_objectFileModel.IsXmlModel(), Is.False);
            Assert.That(_xmlFileModel.IsXmlModel(), Is.True);
            Assert.That(_unknownFileModel.IsXmlModel(), Is.False);
        }
        

        [Test]
        public void EnumGetPreservationOrIntermediateModelReturnIntermediateIfPresent()
        {
            var model1 = FileModelFactory.GetModel("MDPI_1111_01_pres.wav");
            var model2 = FileModelFactory.GetModel("MDPI_1111_01_presInt.wav");
            var model3 = FileModelFactory.GetModel("MDPI_1111_01_prod.wav");

            var list = new List<AbstractFile> {model1, model2, model3};
            Assert.That(list.GetPreservationOrIntermediateModel(), Is.EqualTo(model2));
        }


        [Test]
        public void EnumGetPreservationOrIntermediateModelReturnPresIfPresIntNotPresent()
        {
            var model1 = FileModelFactory.GetModel("MDPI_1111_01_prod.wav");
            var model2 = FileModelFactory.GetModel("MDPI_1111_01_pres.wav");
            

            var list = new List<AbstractFile> { model1, model2};
            Assert.That(list.GetPreservationOrIntermediateModel(), Is.EqualTo(model2));
        }


        [Test]
        public void EnumGetPreservationOrIntermediateModelReturnNullIfNeitherPresent()
        {
            var model1 = FileModelFactory.GetModel("MDPI_1111_01_prod.wav");
            var model2 = FileModelFactory.GetModel("MDPI_1111_01_access.wav");


            var list = new List<AbstractFile> { model1, model2 };
            Assert.That(list.GetPreservationOrIntermediateModel(), Is.EqualTo(null));
        }

        [Test]
        public void GroupingGetPreservationOrIntermediateModelReturnIntermediateIfPresent()
        {
            var model1 = FileModelFactory.GetModel("MDPI_1111_01_pres.wav");
            var model2 = FileModelFactory.GetModel("MDPI_1111_01_presInt.wav");
            var model3 = FileModelFactory.GetModel("MDPI_1111_01_prod.wav");

            var grouping = new List<AbstractFile> { model1, model2, model3 }
                .GroupBy(m=>m.BarCode).ToList().Single();

            Assert.That(grouping.GetPreservationOrIntermediateModel(), Is.EqualTo(model2));
        }


        [Test]
        public void GroupingGetPreservationOrIntermediateModelReturnPresIfPresIntNotPresent()
        {
            var model1 = FileModelFactory.GetModel("MDPI_1111_01_prod.wav");
            var model2 = FileModelFactory.GetModel("MDPI_1111_01_pres.wav");

            var grouping = new List<AbstractFile> { model1, model2 }
                 .GroupBy(m => m.BarCode).ToList().Single();
            Assert.That(grouping.GetPreservationOrIntermediateModel(), Is.EqualTo(model2));
        }

        [Test]
        public void GroupingGetPreservationOrIntermediateModelReturnNullIfNeitherPresent()
        {
            var model1 = FileModelFactory.GetModel("MDPI_1111_01_prod.wav");
            var model2 = FileModelFactory.GetModel("MDPI_1111_01_access.wav");

            var grouping = new List<AbstractFile> { model1, model2 }
                 .GroupBy(m => m.BarCode).ToList().Single();
            Assert.That(grouping.GetPreservationOrIntermediateModel(), Is.EqualTo(null));
        }


        [Test]
        public void RemoveDuplicatesShouldWorkCorrectly()
        {
            var model1 = FileModelFactory.GetModel("MDPI_1111_01_pres.wav");
            var model2 = FileModelFactory.GetModel("MDPI_1111_01_pres.wav");
            var model3 = FileModelFactory.GetModel("MDPI_2222_01_pres.wav");

            var models = new List<AbstractFile> {model1, model2, model3};

            var result = models.RemoveDuplicates();
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Count(m=>m.ToFileName().Equals("MDPI_1111_01_pres.wav")), Is.EqualTo(1));
            Assert.That(result.Count(m => m.ToFileName().Equals("MDPI_2222_01_pres.wav")), Is.EqualTo(1));
        }
    }
}