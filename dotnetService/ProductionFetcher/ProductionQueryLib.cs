using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;


namespace ProductionQueryLib{
    
    public interface IProductionFetcher{
        public static string connectionStringReader(string configFileName)
        {
            string FileName=configFileName;//"dbData.txt";
            var reader= new StreamReader(FileName);
            string ip=reader.ReadLine().Split("=")[1];
            string userID=reader.ReadLine().Split("=")[1];
            string password=reader.ReadLine().Split("=")[1];
            string db=reader.ReadLine().Split("=")[1];
            reader.Close();
            string connString=$"Data Source=tcp:{ip};Initial Catalog={db};User ID={userID};Password={password}";//check without initial catalog;
            return connString;
        }

        public string getSqlCommand();
        public void testConnection();
        public void getProduction();

        public void countProduction(Dictionary<string,object> line); 

    }
    public class ProductionFetcher:IProductionFetcher
    {
        private string ShopVariable="givenShopName";
        private string platformVariable="platform";
        private string dateVariable="date";

        private string countVariable="productionCount";
        public Dictionary <string, string> PrintShopName = new Dictionary<string, string>(){
            {"LTV_TCF","Q Block"},
            {"MNAL_TCF","P Block"},
            {"U202_TCF","R Block"},
            {"W201_TCF", "S Block"},
            {"Z101_TCF","T Block"}
        };
        public Dictionary <string, string> EligibleModel = new Dictionary<string, string>()
        {
            {"LTV","LTV"},
            {"MPV","MPV"},
            {"HCV","HCV"},
            {"ICV","ICV"},
            {"LCV", "LCV"},
            {"BLR","BLR"},
            {"BLRPICKUP","BLRPICKUP"},
            {"BLRP108","BLRP108"},
            {"S101","S101"},
            {"W601","W601"},
            {"Z101","Z101"},
            {"U301","U301"}
        };
        
        public Dictionary<string,Dictionary<string,int>> countRecord =new Dictionary<string,Dictionary<string,int>>() ;
        public readonly SqlConnection connection;
        public readonly string connectionString;
        public readonly string sqlStatement;
        public ProductionFetcher (string configFileName)
        {
            connectionString=IProductionFetcher.connectionStringReader(configFileName);
            connection= new SqlConnection();
            connection.ConnectionString=connectionString;
            sqlStatement=getSqlCommand();
        }
        
        public string getSqlCommand(){

            string statement=
            "EXEC dbo.ExecuteLinkedDataSetQuery 'System.Sources.Db.Oracle.FTPC_PDS.Queries.PROD_TODAY_SYNC_POC', 'mesftvp8' ";
            return statement;
        }
        
        public void testConnection()
        {
            System.Console.WriteLine($"test string:{connectionString}");
            connection.Open();
            System.Console.WriteLine("State: {0}", connection.State);
            System.Console.WriteLine("ConnectionString: {0}", connectionString);
            connection.Close();
        }

        public void getProduction(){
            connection.Open();
            SqlCommand cmd =new SqlCommand(sqlStatement,connection);
            cmd.CommandType = CommandType.Text; //CommandType.StoredProcedure; //check why StoredProcedure didn't work;
            SqlDataReader reader= cmd.ExecuteReader();
            while (reader.Read()){
                //current column order is> 0:date, 1:shiftname,2:shopname,3:plaform , 4:subgroup,5:Modelcode,6:started,7:BookedOrder; 
                Dictionary<string,object>line = new Dictionary<string,object>();
                line.Add(dateVariable,reader.GetString(0));
                line.Add(ShopVariable,reader.GetString(2));
                line.Add(platformVariable,reader.GetString(3));
                line.Add(countVariable, reader.GetDecimal(7)); //confirm the int used in the read
                System.Console.WriteLine("Transaction: {0} {1} {2} ", reader.GetString(0), reader.GetString(1), reader.GetDecimal(7));
                countProduction(line);
            }
            connection.Close();
        }

        public void countProduction(Dictionary<string,object> line) 
        {
            if(!EligibleTransaction(line))// filter here to pass only relevant shops and platforms counts;
            {
                return ;
            }
            IDictionary<string,object> relevantLine = line; 
            string BlockName = PrintShopName[(string)relevantLine[ShopVariable]];
            string platform = (string)relevantLine[platformVariable];
            int  count = System.Decimal.ToInt32((decimal)relevantLine[countVariable]);
            //append the count 
            if (countRecord.ContainsKey(BlockName))
            {
                Dictionary<string,int> temp = countRecord[BlockName];
                //check whether the shop is present : 
                if (temp.ContainsKey(platform))
                {
                    temp[platform]=temp[platform]+count;
                    countRecord[BlockName]=temp;
                    return ;
                }
                //if the platform has been counted first time in the shop :
                countRecord[BlockName].Add(platform,count);
                return ;
            }
            //addition of new block and its entry 
            countRecord.Add(BlockName, new Dictionary<string,int>{{platform,count}});
            return ;
        }

        public void PrintProduction ()
        {
            foreach (KeyValuePair<string, Dictionary<string, int>> subBlock in countRecord ) 
            {
                System.Console.WriteLine($"Under Block : {subBlock.Key}") ;
                System.Console.WriteLine ("Production is : ");
                foreach(KeyValuePair<string, int> modelCount in countRecord[subBlock.Key]) 
                {
                    System.Console.WriteLine($"{modelCount.Key} model with total:{modelCount.Value}");
                }
                System.Console.WriteLine ("-------------");
            } 
            System.Console.WriteLine ("x---x-x----------------x-x---x-x-x");
        }

        public bool EligibleTransaction(Dictionary<string,object> line)
        {
            if(PrintShopName.ContainsKey((string)line[ShopVariable]))
            {
                if(EligibleModel.ContainsKey((string)line[platformVariable]))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    
    }

}