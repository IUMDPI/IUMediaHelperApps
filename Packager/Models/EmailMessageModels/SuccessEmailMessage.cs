using System.Linq;
using System.Text;
using Packager.Extensions;

namespace Packager.Models.EmailMessageModels
{
    public class SuccessEmailMessage:AbstractEmailMessage
    {
        private string[] Barcodes { get; }
        private string MachineName { get; }
        private const string HeaderFormat = "<html><body>";
        private const string FooterFormat = "</body></html>";
        private const string Para1Format = "<p>The packager has successfully processed {0} on workstation {1}:</p>";
        
        public SuccessEmailMessage(string[] barcodes, string[] toAddresses, string fromAddress, string machineName, string log) : base(toAddresses, fromAddress, new [] {log})
        {
            Barcodes = barcodes;
            MachineName = machineName;
        }

        public override string Title=> $"{Barcodes.ToSingularOrPlural("object", "objects")} processed successfully on workstation {MachineName}";

        public override string Body
        {
            get
            {
                var builder = new StringBuilder();
                builder.AppendLine(HeaderFormat);
                builder.AppendFormat(Para1Format, Barcodes.ToSingularOrPlural("object", "objects"), MachineName);
                builder.AppendLine();
                builder.AppendLine("<ul>");
                foreach (var barcode in Barcodes)
                {
                    builder.AppendLine($"<li>{barcode}</li>");
                }
                builder.AppendLine("</ul>");

                if (Attachments.Any())
                {
                    builder.AppendLine(
                        $"<p>See the attached log file for more details.</p>");
                }

                builder.AppendLine(FooterFormat);



                return builder.ToString();
            } 
        }
    }
}
