using ProductionQueryLib;

namespace ProductionService {
        public class MainProgram{
                
        public static void Main(string[] args) 
        {
            ProductionFetcher prod=new ProductionFetcher(args[0]);
            try
            {
                System.Console.WriteLine("testing connectivity");
                prod.testConnection();
                System.Console.WriteLine("connectivity test done");
            }
            catch (System.Exception e)
            {
               System.Console.WriteLine($"error:{e.Message}");
               return ;
            }
            try
            {
                System.Console.WriteLine("testing productiondata with StoreProceduretext");
                prod.getProduction();
                prod.PrintProduction();
                System.Console.WriteLine("production count test done");
            }
            catch (System.Exception e)
            {
               System.Console.WriteLine($"error:{e.Message}");
               return ;
            }

        }
    }
}
