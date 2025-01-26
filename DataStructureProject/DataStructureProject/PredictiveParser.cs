using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataStructureProject
{
    public class PredictiveParser
    {
        private Dictionary<string, Dictionary<string, string>> parseTable;
        private Stack<string> parseStack;
        private List<Token> tokens;

        public PredictiveParser(Dictionary<string, Dictionary<string, string>> parseTable, List<Token> tokens)
        {
            this.parseTable = parseTable;
            this.tokens = tokens;
            this.parseStack = new Stack<string>();
        }

        public void Parse()
        {
            //parseStack.Push("$"); // End marker
            //parseStack.Push("Start"); // Start symbol (replace with your grammar's start symbol)

            //int tokenIndex = 0;
            //while (parseStack.Count > 0)
            //{
            //    string top = parseStack.Peek();
            //    Token currentToken = tokenIndex < tokens.Count ? tokens[tokenIndex] : new Token("$", "$");

            //    if (top == "$" && currentToken.Type == "$")
            //    {
            //        Console.WriteLine("Parsing successful!");
            //        return;
            //    }

            //    if (IsTerminal(top))
            //    {
            //        if (top == currentToken.Value)
            //        {
            //            parseStack.Pop();
            //            tokenIndex++;
            //        }
            //        else
            //        {
            //            Console.WriteLine($"Syntax error: Expected '{top}', found '{currentToken.Value}'");
            //            return;
            //        }
            //    }
            //    else if (parseTable.ContainsKey(top) && parseTable[top].ContainsKey(currentToken.Value))
            //    {
            //        parseStack.Pop();
            //        string production = parseTable[top][currentToken.Value];

            //        if (production != "ε") // If not epsilon, push production in reverse order
            //        {
            //            foreach (var symbol in production.Split(' ').Reverse())
            //            {
            //                parseStack.Push(symbol);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Syntax error: No rule for '{top}' with input '{currentToken.Value}'");
            //        return;
            //    }
            //}

            //if (tokenIndex < tokens.Count)
            //{
            //    Console.WriteLine("Syntax error: Unexpected tokens remaining.");
            //}


            parseStack = new Stack<string>();
            parseStack.Push("S"); 


            while (parseStack.Count > 0)
            {
                string top = parseStack.Peek();
                string currentToken = tokens[0].ToString();

                if (IsTerminal(top))
                {
                    if (top == currentToken)
                    {
                        Console.WriteLine($"Matched: {top}");
                        parseStack.Pop();
                        tokens.RemoveAt(0);
                    }
                    else
                    {
                        Console.WriteLine($"Error: Expected {top}, but got {currentToken}");
                        break;
                    }
                }
                else
                {
                    if (parseTable.ContainsKey(top) && parseTable[top].ContainsKey(currentToken))
                    {
                        string production = parseTable[top][currentToken];
                        Console.WriteLine($"Applying production: {top} -> {production}");
                        parseStack.Pop();

                        var productionParts = production.Split(' ');
                        foreach (var part in productionParts)
                        {
                            if (part != "epsilon")
                            {
                                parseStack.Push(part);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: No production for {top} with {currentToken}");
                        break;
                    }
                }
            }

            if (tokens.Count == 0)
            {
                Console.WriteLine("Parsing successful!");
            }
            else
            {
                Console.WriteLine("Parsing failed.");
            }
        }

        private bool IsTerminal(string symbol)
        {
            return !parseTable.ContainsKey(symbol);
        }
    }
}

