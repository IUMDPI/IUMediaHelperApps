using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Excel;
using Packager.Attributes;
using Packager.Utilities;

namespace Packager.Models
{
    public class CarrierData
    {
        [ExcelField("Carrier Type", true)]
        public string CarrierType { get; set; }

        [ExcelField("Identifier", true)]
        public string Identifier { get; set; }

        [ExcelField("Barcode", true)]
        public string Barcode { get; set; }

        [ExcelField("Brand", true)]
        public string Brand { get; set; }

        [ExcelField("Thickness", true)]
        public string Thickness { get; set; }

        [ExcelField("DirectionsRecorded", true)]
        public string DirectionsRecorded { get; set; }

        [ExcelField("Repaired", true)]
        public string Repaired { get; set; }

        [ExcelObject]
        public ConfigurationData Configuration { get; set; }

        [ExcelObject]
        public PhysicalConditionData PhysicalCondition { get; set; }

        [ExcelObject]
        public PreviewData Preview { get; set; }

        [ExcelObject]
        public CleaningData Cleaning { get; set; }

        [ExcelObject]
        public BakingData Baking { get; set; }

        [ExcelObject]
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
                    result.Parts.Side1 = ImportSideData(row, 1);
                    result.Parts.Side2 = ImportSideData(row, 2);
                    
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