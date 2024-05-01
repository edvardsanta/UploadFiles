using CsvHelper.Configuration;
using CsvHelper;
using MassTransit;
using System.Globalization;
using UploadFiles.Shared.Contracts;
using MyML.Abstracts;
using MyML;

namespace RankText
{
    public class RankTextConsumer : IConsumer<RankTextMessage>
    {
        private NaiveBayesClassifier NaiveBayesClassifier; 
        public RankTextConsumer()
        {
            // TODO Use this filename in appsettings
            var resumeDataList = ReadCsvFile(@"soft_hard_skills.csv");
            NaiveBayesClassifier = new MultinomialNaiveBayesClassifier();
            NaiveBayesClassifier.Train(resumeDataList);
        }

        public async Task Consume(ConsumeContext<RankTextMessage> context)
        {
            string normalizedText = context.Message.NormalizedText;
            var skill = NaiveBayesClassifier.Predict(normalizedText);

            await Task.Delay(500);  
        }


        private static List<ResumeData> ReadCsvFile(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };
            // TODO Find a way to improve performance here
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = new List<ResumeData>();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = csv.GetRecord<ResumeData>();
                    records.Add(record);
                }
                return records;
            }
        }
    }
}
