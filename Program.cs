using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace JsonMerger
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path of the folder containing JSON files:");
            string folderPath = Console.ReadLine();

            // Check if the folder path is valid
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Invalid folder path.");
                return;
            }

            // Get all JSON files from the specified folder
            string[] fileNames = Directory.GetFiles(folderPath, "*.json");

            // Check if any files were found
            if (fileNames.Length == 0)
            {
                Console.WriteLine("No JSON files found in the specified folder.");
                return;
            }

            // Merge the JSON files and export the result
            string mergedFileName = Path.Combine(folderPath, "merged_output.json");
            MergeJsonFiles(fileNames, mergedFileName);

            Console.WriteLine("JSON files merged and exported successfully.");
        }

        static void MergeJsonFiles(string[] fileNames, string outputFilePath)
        {
            using (var mergedStream = new StreamWriter(outputFilePath))
            using (var jsonWriter = new JsonTextWriter(mergedStream))
            {
                jsonWriter.WriteStartArray();

                foreach (var fileName in fileNames)
                {
                    try
                    {
                        using (var fileStream = File.OpenRead(fileName))
                        using (var jsonReader = new JsonTextReader(new StreamReader(fileStream)))
                        {
                            // Move to the first token (usually the start of the array/object)
                            while (jsonReader.Read())
                            {
                                if (jsonReader.TokenType == JsonToken.StartArray || jsonReader.TokenType == JsonToken.StartObject)
                                {
                                    break;
                                }
                            }

                            // Copy the content from the current file to the merged output
                            jsonWriter.WriteToken(jsonReader);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading file '{fileName}': {ex.Message}");
                    }
                }

                jsonWriter.WriteEndArray();
            }
        }
    }
}
