using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace moysklad
{
    class Program
    {
        const string LOGIN = "";
        const string PASSWORD = "";

        static void Main(string[] args)
        {
//            DownloadAssortmentToCsv();
//            DownloadAgentsToCsv();

            /*Console.Write("Введите год: ");
            var year = Console.ReadLine();

            Console.Write("Введите месяц: ");
            var month = Console.ReadLine();
            var listDemandsForYearAndMonth = ReadCsvRealization(year, month);*/

            var listDemandsForYearAndMonth = ReadCsvRealization(new DateTime(2017, 06, 16), new DateTime(2017, 06, 16));
            HandingDemands(listDemandsForYearAndMonth);
        }
        private static List<RowProduct> ReadCsvAgents()
        {
            var lines = File.ReadAllLines(@"D:\user\sklad\contr.csv", Encoding.GetEncoding("Windows-1251"));
            var list = new List<RowProduct>();

//            using (var reader = new StreamReader(@"D:\user\sklad\contr.csv", Encoding.UTF8))
            {
                foreach (var line in lines)
                {
                    var cells = line.Split(';');
                    var row = new RowProduct
                    {
                        id = cells[0],
                        name = cells[1]
                    };
                    list.Add(row);

                    //                    Console.WriteLine(row.id + " " + row.name);
                    //                    Console.WriteLine(lines.Count());
                }
                lines.ToList();
            }
            return list;
        } 

        private static List<RowProduct> ReadCsvProducts()
        {
            var lines = File.ReadAllLines(@"D:\user\sklad\products.csv", Encoding.GetEncoding("Windows-1251"));
            var list = new List<RowProduct>();

//            using (var reader = new StreamReader(@"D:\user\sklad\products.csv", Encoding.UTF8))
            {
                foreach (var line in lines)
                {
                    var cells = line.Split(';');
                    var row = new RowProduct
                        {
                            id = cells[0],
                            name = cells[1]
                        };
                    list.Add(row);
                    
//                    Console.WriteLine(row.id + " " + row.name);
//                    Console.WriteLine(lines.Count());
                }
                lines.ToList();
            }
            return list;
        }


       /* private static IEnumerable<DemandsFromCSV> ReadCsvRealization(string year, string month)
        {
//            using (var reader = new StreamReader(@"D:\user\sklad\all realization.csv", Encoding.UTF8))
            {
                var list = new List<DemandsFromCSV>();
                var lines = File.ReadAllLines(@"D:\user\sklad\all realization.csv");
                foreach (var line in lines/*.Take(2)#1#)
                {
                    var cells = line.Split(';');
                    var item = new DemandsFromCSV()
                    {
                        RealizationDate = cells[1],
                        NumberDoc = cells[2],
                        ProductNameReal = cells[3],
                        CustomerName = cells[4],
                        VolumeReal = cells[5],
                        SellPrice = cells[6]
                    };
                    
                    list.Add(item);
                    Console.WriteLine(item.RealizationDate + " " + item.NumberDoc + " " + item.ProductNameReal + " " + item.CustomerName + " " + item.VolumeReal + " " + item.SellPrice);
                }

                Console.WriteLine(list.Count);

                var listDemandsForYearAndMonth = list.Where(csv => csv.RealizationDate.Contains(year + "-" + month));

                Console.WriteLine(listDemandsForYearAndMonth.Count());

                /*using (StreamWriter fs = new StreamWriter(new FileStream(@"D:\user\MS\ms.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
                {
                    //list.Where(csv => csv.RealizationDate.Contains("2017-06"));
                    foreach (var item in listMonth)
                    {
                        fs.WriteLine(item.RealizationDate + " " + item.NumberDoc + " " + item.ProductNameReal + " " + item.CustomerName + " " + item.VolumeReal + " " + item.SellPrice);
                    }
                    fs.Close();
                }#1#

                return listDemandsForYearAndMonth;
            }
        }*/

        private static IEnumerable<DemandsFromCSV> ReadCsvRealization(DateTime fromTime, DateTime toTime)
        {
            var resultList = new List<DemandsFromCSV>();
//            var lines = File.ReadAllLines(@"D:\user\sklad\all realization.csv", Encoding.GetEncoding("Windows-1251"));
            var lines = File.ReadAllLines(@"D:\user\sklad\all realization.csv", Encoding.Default);
            foreach (var line in lines)
            {
                var cells = line.Split(';');

                var realizationDate = DateTime.Parse(cells[1]);

                if (realizationDate < fromTime
                    || realizationDate > toTime)
                    continue;

                var item = new DemandsFromCSV()
                {
                    RealizationDate = cells[1],
                    NumberDoc = cells[2],
                    ProductNameReal = cells[3],
                    CustomerName = cells[4],
                    VolumeReal = cells[5],
                    SellPrice = cells[6]
                };

                resultList.Add(item);
                Console.WriteLine(item.RealizationDate + " " + item.NumberDoc + " " + item.ProductNameReal + " " + item.CustomerName + " " + item.VolumeReal + " " + item.SellPrice);
            }

            Console.WriteLine(resultList.Count);

            return resultList;
        }

        private static void HandingDemands(IEnumerable<DemandsFromCSV> demandsList)
        {
            //список контрагентов
            var listAgents = ReadCsvAgents();
            //список продуктов
            var productsList = ReadCsvProducts();

            using (var webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(LOGIN, PASSWORD);
                webClient.Encoding = Encoding.UTF8;

                foreach (var item in demandsList)
                {
                    //поиск отгрузки в moysklad
//                    var existingDemandId = GetDemandIdFromSklad(webClient, item.NumberDoc, item.RealizationDate);
                    var existingDemandId = GetDemandIdFromSklad(webClient, item);

                    if (existingDemandId == null)
                    {
                        //поиск контрагента по имени
                        var agentId = FindAgentIdByName(listAgents, item.CustomerName);

                        //создаем отгрузку
//                        var newDemandId = CreateDemand(webClient, item.NumberDoc, agentId);
                        var newDemandId = CreateDemand(webClient, item, agentId);

                        //берем id отгрузки
//                        string demandIdFromHtmlResult = GetDemandIdFromHtmlResult(createHtmlResult);

                        //поиск id продукта по имени
                        var productId = FindProductIdByName(productsList, item.ProductNameReal);
                        Console.WriteLine(productId);

                        //добавление позиции в сборку
                        AddPosition(webClient, newDemandId, productId, item);
                    }
                    else
                    {
                        //поиск id продукта по имени
                        var productId = FindProductIdByName(productsList, item.ProductNameReal);
                        Console.WriteLine(productId);

                        //добавление позиции в сборку
                        AddPosition(webClient, existingDemandId, productId, item);
                    }
                }
            }
        }

        private static string GetDemandIdFromSklad(WebClient webClient, DemandsFromCSV item)
        {
            Console.WriteLine("Поиск отгрузки по имени и дате...");

            var offset = 0;
            while (true)
            {

                string url = "https://online.moysklad.ru/api/remap/1.1/entity/demand?limit=100&offset=" + offset;

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var HtmlResult = webClient.DownloadString(url);

                var jsonString = JsonConvert.DeserializeObject<Demands>(HtmlResult);

                if (jsonString.rows.Count == 0)
                    break;

                var demand = jsonString.rows.FirstOrDefault(
                    rowDemand => rowDemand.name.Contains(item.NumberDoc) &&
                                 rowDemand.moment.Contains(item.RealizationDate.Substring(0, 9)));
                
                if (demand != null)
                {
                    var demandId = demand.id;
                    Console.WriteLine("Отгрузка найдена. id: " + demandId);
                    return demandId;
                }

                offset = offset + 100;
            }

            Console.WriteLine("!Отгрузка не найдена! name: " + item.NumberDoc + ", date: " + item.RealizationDate);
            return null;
        }

       /* private static string GetDemandIdFromHtmlResult(string HtmlResult)
        {
            Console.WriteLine("Получаем id отгрузки");
            var jsonString = JsonConvert.DeserializeObject<JsonDemand>(HtmlResult);
            var id = jsonString.id;
            Console.WriteLine("id найденной отгрузки: " + id);
            return id;
        }*/

        /*private static Product GetProducts(WebClient webClient)
        {
            string url = "https://online.moysklad.ru/api/remap/1.1/entity/product";

            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            var HtmlResult = webClient.DownloadString(url);

            var jsonString = JsonConvert.DeserializeObject<Product>(HtmlResult);
            /*foreach (var row in jsonString.rows)
            {
                Console.WriteLine("id = " + row.id + ", name = " + row.name);
            }#1#
            return jsonString;
        }*/

        private static string FindAgentIdByName(List<RowProduct> rowAgents, string agentName)
        {
            var agent = rowAgents.FirstOrDefault(row => row.name.Contains(agentName));
            if (agent != null)
            {
                var agentId = agent.id;
                Console.WriteLine("Контрагент найден. id: " + agentId);
                return agentId;
            }
            Console.WriteLine("!Контрагент не найден! name = " + agentName);
            return null;
        }

        private static string FindProductIdByName(List<RowProduct> rowProducts, string productName)
        {
            var product = rowProducts.FirstOrDefault(row => row.name.Contains(productName));
            if (product != null)
            {
                var productId = product.id;
                Console.WriteLine("Продукт найден. id: " + productId);
                return productId;
            }
            Console.WriteLine("!Товар не найден! name = " + productName);
            return null;
        }

        private static void AddPosition(WebClient webClient, string demandId, string productId, DemandsFromCSV item)
        {
            Console.WriteLine("Добавляем позицию...");
            Console.WriteLine("demandId: " + demandId);
            Console.WriteLine("productId: " + productId);
            string url = "https://online.moysklad.ru/api/remap/1.1/entity/demand/" + demandId + "/positions";

            var position = new Position
            {
                quantity = decimal.Parse(item.VolumeReal)*100,
                price = decimal.Parse(item.SellPrice)*100,
                assortment = new Assortment()
                {
                    meta = new Meta()
                    {
                        href = "https://online.moysklad.ru/api/remap/1.1/entity/product/" + productId,
                        type = "product",
                        mediaType = "application/json"
                    }
                }
            };

            try
            {
                var myParameters = JsonConvert.SerializeObject(position);
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                webClient.UploadString(url, "POST", myParameters);
                Console.WriteLine("Позиция добавлена");
            }
            catch (Exception)
            {
                Console.WriteLine("!Позиция не добавлена!");
                throw new Exception();
            }
        }

        private static string CreateDemand(WebClient webClient, DemandsFromCSV item, string agentId)
        {
            string url = "https://online.moysklad.ru/api/remap/1.1/entity/demand";

            var organizationId = "ad167a55-6df5-11e7-7a31-d0fd0005b555";
            var storeId = "ad1729de-6df5-11e7-7a31-d0fd0005b557";

            var demand = new Demand()
            {
                name = item.NumberDoc,
                moment = item.RealizationDate + " 15:00:00",
                organization = new Organization
                {
                    meta = new Meta()
                    {
                        href =
                            "https://online.moysklad.ru/api/remap/1.1/entity/organization/" + organizationId,
                        type = "organization",
                        mediaType = "application/json"
                    }
                },
                agent = new Agent
                {
                    meta = new Meta()
                    {
                        href =
                            "https://online.moysklad.ru/api/remap/1.1/entity/counterparty/" + agentId,
                        type = "counterparty",
                        mediaType = "application/json"
                    }
                },

                store = new Store
                {
                    meta = new Meta
                    {
                        href =
                            "https://online.moysklad.ru/api/remap/1.1/entity/store/" + storeId,
                        type = "store",
                        mediaType = "application/json"
                    }
                }
            };

            try
            {
                var myParameters = JsonConvert.SerializeObject(demand);
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string htmlResult = webClient.UploadString(url, "POST", myParameters);
                
                var jsonString = JsonConvert.DeserializeObject<JsonDemand>(htmlResult);
                var id = jsonString.id;
                Console.WriteLine(" Отгрузка создана id = " + id);
                return id;
            }
            catch (Exception e)
            {
                Console.WriteLine(" Отгрузка не создана!!!");
                throw new Exception();
            }
        }

        # region DownloadCSV
        private static void DownloadAssortmentToCsv()
        {
            int countProducts = 0;
            for (int i = 0; i < 500; i+=100)
            {
                string url = "https://online.moysklad.ru/api/remap/1.1/entity/assortment?limit=100&offset=" + i;

                using (var webClient = new WebClient())
                {
                    webClient.Credentials = new NetworkCredential(LOGIN, PASSWORD);
                    webClient.Encoding = Encoding.UTF8;
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string HtmlResult = webClient.DownloadString(url);

                    var jsonString = JsonConvert.DeserializeObject<Product>(HtmlResult);
                    foreach (var row in jsonString.rows)
                    {
                        Console.WriteLine(row.name);
                        using (var sw = new StreamWriter(@"D:\user\sklad\products.csv", true, Encoding.Default))
                        {
                            sw.WriteLine(row.id + ";" + row.name);
                            sw.Close();
                        }
                    }
                    Console.WriteLine("i = " + i + ", count = " + jsonString.rows.Count);
                    countProducts += jsonString.rows.Count;
                }
            }
            Console.WriteLine("all = " + countProducts);
            
        }

        private static void DownloadAgentsToCsv()
        {
            int countAgents = 0;
            Product jsonString = null;

            for (int i = 0; i < 500; i += 100)
            {
                string url = "https://online.moysklad.ru/api/remap/1.1/entity/counterparty?limit=100&offset=" + i;

                using (var webClient = new WebClient())
                {
                    webClient.Credentials = new NetworkCredential("admin@dilatte", "i20n9d1box");
//                    webClient.Credentials = new NetworkCredential("admin@tigratius927", "ffc0e73e0d");
                    webClient.Encoding = Encoding.UTF8;
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                    string HtmlResult = webClient.DownloadString(url);

                    jsonString = JsonConvert.DeserializeObject<Product>(HtmlResult);
                    foreach (var row in jsonString.rows)
                    {
                        Console.WriteLine(row.name);
                        using (var sw = new StreamWriter(@"D:\user\sklad\contr.csv", true, Encoding.Default))
                        {
                            sw.WriteLine(row.id + ";" + row.name);
                            sw.Close();
                        }
                    }
                    Console.WriteLine("i = " + i + ", count = " + jsonString.rows.Count);
                    countAgents += jsonString.rows.Count;
                    
                }
            }
            Console.WriteLine("all = " + countAgents);
//            AddInfoToCsv(jsonString.rows);
        }
        # endregion
    }
}

