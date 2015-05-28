using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Excel;
using Packager.Attributes;
using Packager.Utilities;

namespace Packager.Models
{
    [Serializable]
    public class CarrierData
    {
        [ExcelField("Carrier Type", true)]
        [XmlAttribute("type")]
        public string CarrierType { get; set; }

        [ExcelField("Identifier", true)]
        [XmlElementAttribute(Order = 1)] 
        public string Identifier { get; set; }

        [ExcelField("Barcode", true)]
        [XmlElementAttribute(Order = 2)] 
        public string Barcode { get; set; }

        [XmlElementAttribute(Order = 4)] 
        [ExcelField("Brand", true)]
        public string Brand { get; set; }

        [ExcelField("Thickness", true)]
        [XmlElementAttribute(Order = 5)] 
        public string Thickness { get; set; }

        [ExcelField("DirectionsRecorded", true)]
        [XmlElementAttribute(Order = 6)] 
        public string DirectionsRecorded { get; set; }

        [XmlElementAttribute(Order = 8)] 
        [ExcelField("Repaired", true)]
        public string Repaired { get; set; }

        [ExcelObject]
        [XmlElementAttribute(Order = 3)] 
        public ConfigurationData Configuration { get; set; }

        [ExcelObject]
        [XmlElementAttribute(Order = 7)] 
        public PhysicalConditionData PhysicalCondition { get; set; }

        [ExcelObject]
        [XmlElementAttribute(Order = 9)] 
        public PreviewData Preview { get; set; }

        [ExcelObject]
        [XmlElementAttribute(Order = 10)] 
        public CleaningData Cleaning { get; set; }

        [ExcelObject]
        [XmlElementAttribute(Order = 11)] 
        public BakingData Baking { get; set; }

        [ExcelObject]
        [XmlElementAttribute(Order = 12)] 
        public PartsData Parts { get; set; }

        public static CarrierData ImportFromExcel(string targetPath)
        {
            if (!File.Exists(targetPath))
            {
                throw new FileNotFoundException(targetPath);
            }

            IExcelDataReader excelReader = null;
            try
            {
                using (var stream = new FileStream(targetPath, FileMode.Open, FileAccess.Read))
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    excelReader.IsFirstRowAsColumnNames = true;
                    var dataSet = excelReader.AsDataSet();

                    var row = dataSet.Tables["InputData"].Rows[0];

                    var result = (CarrierData) new ExcelImporter<CarrierData>().Import(row);
                    result.Parts.Sides = new[]
                    {
                        ImportSideData(row, 1), 
                        ImportSideData(row, 2)
                    };
                    
                    return result;
                }
            }
            finally
            {
                if (excelReader != null)
                {
                    excelReader.Close();
                }
            }
        }

        private static SideData ImportSideData(DataRow row,int sideValue)
        {
            return new SideData
            {
                Side = sideValue.ToString(CultureInfo.InvariantCulture),
                Files = new List<FileData>(),
                ManualCheck = row[string.Format("Part-Side-{0}-ManualCheck", sideValue)].ToString(),
                Ingest = new IngestData
                {
                    SpeedUsed = row[string.Format("Part-Side-{0}-Speed_used",sideValue)].ToString(),
                    Comments = row[string.Format("Part-Side-{0}-Comments", sideValue)].ToString()
                }
            };
        }
    }
}