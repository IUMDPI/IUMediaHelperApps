using System.Data;
using System.Data.OleDb;
using System.IO;
using Excel;

namespace Packager.Models
{
    public class InputData
    {
        private readonly string _targetPath;

        public InputData(string targetPath)
        {
            _targetPath = targetPath;
            ImportData();
        }


        private void ImportData()
        {
            // read from ....
            if (!File.Exists(_targetPath))
            {
                throw new FileNotFoundException(_targetPath);
            }

            using (var stream = new FileStream(_targetPath, FileMode.Open, FileAccess.Read))
            {
                var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //...
                //3. DataSet - The result of each spreadsheet will be created in the result.Tables
                excelReader.IsFirstRowAsColumnNames = true;
                var result = excelReader.AsDataSet();
                var test = "blah";
                //...
                /*//4. DataSet - Create column names from first row
                excelReader.IsFirstRowAsColumnNames = true;
                DataSet result = excelReader.AsDataSet();*/
            }
            {
                
            }
        }


    }
}
