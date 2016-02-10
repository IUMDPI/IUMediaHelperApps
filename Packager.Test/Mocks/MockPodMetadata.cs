/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Packager.Test.Mocks
{
    public class MockPodMetadata:XElement
    {
        public const string IdValue = "100000";
        public const string CallNumberValue = "Test-01";
        public const string TitleValue = "Test title";
        public const string UnitValue = "Unit";
        public const string BarcodeValue = "10000000337941";
        public const string CleaningDateValue = "2015-11-22T19:00:00-0500";
        public const string CleaningCommentValue = "cleaning comment";
        public const string BakingDateValue = "2015-11-22T19:00:00-0500";
        public const string DigitizingEntityValue = "Digitizing entity";
        
        public MockPodMetadata(string format) : base("pod", 
            new XElement("success") {Value = "true"},
            new XElement("data", 
                new XElement("object", new XElement("details",
                    new XElement("id") {Value = IdValue},
                    new XElement("format") {Value = format},
                    new XElement("call_number") {Value = CallNumberValue},
                    new XElement("title") { Value = TitleValue},
                    new XElement("unit") { Value = UnitValue},
                    new XElement("barcode") { Value = BarcodeValue }),
                new XElement("digital_provenance", 
                    new XElement("cleaning_date") {Value = CleaningDateValue}),
                    new XElement("cleaning comment") { Value = CleaningCommentValue},
                    new XElement("baking_date") { Value = BakingDateValue},
                    new XElement("repaired") { Value = "true"},

                    )))
        {
            
        }


        private static XElement GetPodElement(string success, string message = "")
        {
            var element = new XElement("pod",
                new XElement("success") {Value = "success"});

            if (!string.IsNullOrWhiteSpace(message))
            {
                element.Add(new XElement("message") {Value = message});
            }

            return element;
        }

        private static XElement GetLacquerDiscElement(bool success, string message = "", bool repaired=false)
        {
            var podElement = GetPodElement(success.ToString().ToLowerInvariant(), message);
            var objectElement = GetObjectElement();

            objectElement.Add(
                GetDetailsElement("Lacquer Disc"),
                GetDigitalProvenanceElement(repaired));

            podElement.Add(objectElement);

            return podElement;
        }

        private static XElement GetObjectElement()
        {
            return new XElement("data", new XElement("object"));
        }

        private static XElement GetDetailsElement(string format)
        {
            return new XElement("details",
                new XElement("id") {Value = IdValue},
                new XElement("format") {Value = format},
                new XElement("call_number") {Value = CallNumberValue},
                new XElement("title") {Value = TitleValue},
                new XElement("unit") {Value = UnitValue},
                new XElement("barcode") {Value = BarcodeValue});
        }

        private static XElement GetDigitalProvenanceElement(bool repaired)
        {
            return new XElement("digital_provenance",
                new XElement("cleaning_date") {Value = CleaningDateValue},
                new XElement("cleaning comment") {Value = CleaningCommentValue},
                new XElement("baking_date") {Value = BakingDateValue},
                new XElement("repaired") {Value = repaired.ToString().ToLowerInvariant()});
        }
    }
}
*/
