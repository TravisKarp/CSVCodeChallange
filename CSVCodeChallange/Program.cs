using CSVCodeChallange.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CSVCodeChallange
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Assumptions: 
             * Input file is in the valid format and each field contains valid data. General validation is present to catch likely failures.
             * Example Output presented the expected datatypes (int and string). 
             */

            string argumentExceptionString = "The Record Type did not match the expected format.";
            string inputPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sample.csv");
            string resultOutputPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Results.json");

            Console.WriteLine("Attempting to convert file.");
            try
            {
                using (var reader = new StreamReader(inputPath))
                {
                    var fileRecord = new FileRecord() { Orders = new List<Order>() } ;
                    while (!reader.EndOfStream)
                    {
                        var nextLine = ReadNextLine(reader);
                        // Process File Start
                        if (nextLine[0].Equals("F", StringComparison.InvariantCultureIgnoreCase))
                        {
                            fileRecord.Date = nextLine[1];
                            fileRecord.Type = nextLine[2];
                            
                            nextLine = ReadNextLine(reader);

                            // Process Order Start
                            if (nextLine[0].Equals("O", StringComparison.InvariantCultureIgnoreCase))
                            {
                                Order newOrder = new Order()
                                {
                                    Date = nextLine[1],
                                    Code = nextLine[2],
                                    Number = nextLine[3],
                                    Items = new List<Item>()
                                };
                                nextLine = ReadNextLine(reader);

                                // Process Buyer
                                if (nextLine[0].Equals("B", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    newOrder.Buyer = new Buyer()
                                    {
                                        Name = nextLine[1],
                                        Street = nextLine[2],
                                        Zip = nextLine[3]
                                    };
                                    nextLine = ReadNextLine(reader);
                                }

                                // Recursively Process Items
                                if (nextLine[0].Equals("L", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    nextLine = ParseItem(nextLine, newOrder, reader);
                                }
                                // Process Timings
                                if (nextLine[0].Equals("T", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var timings = new Timing()
                                    {
                                        Start = Int32.Parse(nextLine[1]),
                                        Stop = Int32.Parse(nextLine[2]),
                                        Gap = Int32.Parse(nextLine[3]),
                                        Offset = Int32.Parse(nextLine[4]),
                                        Pause = Int32.Parse(nextLine[5])
                                    };
                                    newOrder.Timings = timings;
                                    nextLine = ReadNextLine(reader);
                                } 
                                // If Record Type does not match expected end input throw exception.
                                else
                                {
                                    throw new ArgumentException(argumentExceptionString);
                                }
                                fileRecord.Orders.Add(newOrder);
                            }
                            // Process End of File
                            if (nextLine[0].Equals("E", StringComparison.InvariantCultureIgnoreCase))
                            {
                                fileRecord.Ender = new Ender()
                                {
                                    Process = Int32.Parse(nextLine[1]),
                                    Paid = Int32.Parse(nextLine[2]),
                                    Created = Int32.Parse(nextLine[3])
                                };
                            }
                            // If Record Type does not match expected end input throw exception.
                            else
                            {
                                throw new ArgumentException(argumentExceptionString);
                            }
                                
                        }
                        // Convert to Json sting using Camel case to match challange example output. 
                        string json = JsonConvert.SerializeObject(fileRecord, new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                         
                        File.WriteAllText(resultOutputPath, json);

                        Console.WriteLine("Conversion Complete.");
                    }
                }
            }

            catch(Exception exception)
            {
                if (exception is IOException)
                {
                    Console.WriteLine(string.Concat("An error occured when opening or writing to the file. Error: ", exception));
                }
                else if (exception is ArgumentException || exception is ArgumentNullException || exception is ArgumentOutOfRangeException || exception is IndexOutOfRangeException)
                {
                    Console.WriteLine(string.Concat("A parsing error occured please check the input and try again Error: ", exception));
                } 
                else
                {
                    Console.WriteLine(string.Concat("An unknown error occured. Error: ", exception));
                }
            }
        }

        private static string[] ReadNextLine(StreamReader reader)
        {
            string rawLine = reader.ReadLine();
            var line = Regex.Replace(rawLine, @"\""", "");

            return line.Split(',');
        }

        private static string[] ParseItem(string[] line, Order order, StreamReader reader)
        {
            var newItem = new Item()
            {
                Sku = line[1],
                Qty = Int32.Parse(line[2])
            };
            order.Items.Add(newItem);
            var nextLine = ReadNextLine(reader);
            if (nextLine[0].Equals("L", StringComparison.InvariantCultureIgnoreCase))
            {
                nextLine = ParseItem(nextLine, order, reader);
            }
            return nextLine;
        }
        
    }
}