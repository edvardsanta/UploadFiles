using Microsoft.Extensions.Hosting;
using RankText;
using static RankText.Configuration;
//TrainModel.Train();
IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogging)
                .Build();

host.Run();