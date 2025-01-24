using DataStructureProject;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataStructureProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your C++ code (end input with an empty line):");

            // Read C++ code
            string inputCode = ReadUserInput();

            LexicalAnalyzer analyzer = new LexicalAnalyzer();
            List<Token> tokens = analyzer.Analyze(inputCode);

            Console.WriteLine("\nTokens:");
            foreach (var token in tokens)
            {
                Console.WriteLine($"[{token.Type}, {token.Value}]");
            }

            // Bonus: Search for the first declaration
            Console.WriteLine("\nBonus: Searching for first identifier declaration...");
            string firstDeclaration = analyzer.FindFirstDeclaration(tokens);
            Console.WriteLine(firstDeclaration != null
                ? $"First declaration: {firstDeclaration}"
                : "No declarations found.");

            // Bonus: Error handling
            Console.WriteLine("\nBonus: Error Handling...");
            List<string> errors = analyzer.ValidateTokens(tokens);
            if (errors.Count > 0)
            {
                Console.WriteLine("Errors found:");
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                Console.WriteLine("No errors found.");
            }

            //---------------------------------------------------------------------------------

            // Create Token Table
            Console.WriteLine("\nToken Table:");

            var tokenTable = analyzer.CreateTokenTable();

            // Print the table
            PrintTokenTable(tokenTable);


            //--------------------------------------------------------------------------------

            var parser = new CFGParser();

            parser.ComputeFirstSets();
            parser.ComputeFollowSets();
            parser.BuildParseTable();

            parser.PrintFirstSets();
            parser.PrintFollowSets();
            parser.PrintParseTable();

            Console.ReadKey();
        }

        private static string ReadUserInput()
        {
            string line;
            string inputCode = "";

            while ((line = Console.ReadLine()) != null && line.Trim() != "")
            {
                inputCode += line + "\n";
            }

            return inputCode;
        }

        private static void PrintTokenTable(List<(string TokenType, string TokenValue, int HashValue)> tokenTable)
        {
            // Print table headers
            Console.WriteLine("+-----------------+---------------------------+--------------+");
            Console.WriteLine("| Token Type      | Token Value               | Hash Value   |");
            Console.WriteLine("+-----------------+---------------------------+--------------+");

            // Print table rows
            foreach (var entry in tokenTable)
            {
                Console.WriteLine($"| {entry.TokenType,-15} | {entry.TokenValue,-25} | {entry.HashValue,-12} |");
            }

            Console.WriteLine("+-----------------+---------------------------+--------------+");
        }
    }
}