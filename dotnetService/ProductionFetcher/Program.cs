using Microsoft.Extensions.Hosting;
using ProductionQueryLib;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using TransferClient;

namespace ProductionService {
    public class TestProgram
    {
        public static async Task Main(string[] args) 
        {
            //ProductionFetcherClass prod=new ProductionFetcherClass(args[0]);
            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options=>{
                    options.ServiceName= "Hourly ProductionWatcher Service";
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ProductionFetcherService>();
                    //services.AddHttpClient<WatcherService>();
                    services.AddTransient<ProductionFetcherClass>(sp=> new ProductionFetcherClass("dbData.txt"));
                    //services.AddTransient<httpSender>();
                })
                .Build();
                await host.RunAsync();
        }

    }

    public class ProductionFetcherService:BackgroundService {

        private readonly ProductionFetcherClass _productiondb;

        public ProductionFetcherService(ProductionFetcherClass productiondb)
        {
            _productiondb = productiondb;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                //_productiondb.getProduction();
                //_productiondb.PrintProduction();
                string result = _productiondb.getProductionCountString();
                string URL = "http://localhost:8000/productionData";
                SimpleClient client = new SimpleClient(URL);
                await client.postJson(result); 
                System.Console.WriteLine($"Production Snapshot: {result}");
                }catch (System.Exception e){
                    System.Console.WriteLine("DbError:"+e.Message);
                }
                //_logger.writeNotification(response);
                //System.Console.WriteLine(response);
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(System.TimeSpan.FromMinutes(59), stoppingToken);
            }
        }
     }
}
