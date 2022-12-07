using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using System.Formats.Asn1;

namespace CSVReader
{
    class Program
    {
        static void Main()
        {
            var inputrecords = new List<input>();
            var outputrecords = new List<output>();

            // reading input csv file
            using (var reader = new StreamReader("Input.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                inputrecords = csv.GetRecords<input>().ToList();

            }


            foreach (var input in inputrecords)
            {
                // For Sorting Date based on Year
                var year = input.DATE.Year;
                var yearList = inputrecords.Where(x => x.DATE.Year == year).ToList();

                // For Finding Year Mean 
                var mean = yearList.Average(x => x.Production);

                // For Finding Year Median
                var median = calculatemedian(yearList);


                // For Comparing Production and Mean 
                var production = input.Production;
                var Greater = production > mean ? "yes" : "no";

                // Rounding off decimal place and show trailing zeros
                string Production = production.ToString("0." + new string('0', 2));
                string Ymean = mean.ToString("0." + new string('0', 2));
                string Ymedian = median.ToString("0." + new string('0', 2));

                // Recording all Values into corresponding Headers
                var Record = new output
                {
                    Date = input.DATE.ToString("yyyy/MM/dd"),
                    Manager = input.Manager,
                    Production = Production,
                    YearMean = Ymean,
                    YearMedian = Ymedian,
                    IsProductionGreaterThanYearMean = Greater
                };
                outputrecords.Add(Record);
            }
            // For Quote the required fields
            var indexes = new[] { 1, 5 };
            var config = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                ShouldQuote = args => args.Row.HeaderRecord != null && indexes.Contains(args.Row.Index),
            };

            // Writing all datas to Output csv file
            using (var writer = new StreamWriter("Output.csv", false, encoding: System.Text.Encoding.UTF8))

            using (var csv = new CsvHelper.CsvWriter(writer, config))
            {

                csv.WriteRecords(outputrecords);

            }


        }





        // For Calculating Median

        public static double calculatemedian(List<input> inputrecords)
        {
            if (inputrecords.Count % 2 == 0)
            {
                var nth = inputrecords.OrderBy(x => x.Production).ToList()[inputrecords.Count / 2].Production;
                var nth1 = inputrecords.OrderBy(x => x.Production).ToList()[(inputrecords.Count / 2) - 1].Production;
                return (nth + nth1) / 2;
            }
            else
            {
                var nth = inputrecords.OrderBy(x => x.DATE).ToList()[inputrecords.Count / 2].Production;
                return nth;
            }
        }




        // Input Class
        public class input
        {
            public DateOnly DATE { get; set; }
            public string Manager { get; set; }
            public double Production { get; set; }

        }

        // Output Class
        public class output
        {
            public string Date { get; set; }
            public string Manager { get; set; }
            public string Production { get; set; }
            public string YearMean { get; set; }
            public string YearMedian { get; set; }
            public string IsProductionGreaterThanYearMean { get; set; }
        }


    }


}
