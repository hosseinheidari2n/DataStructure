using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataStructureProject
{
    public class LexicalAnalyzer
    {
        public List<Token> Tokens { get; private set; } = new List<Token>();

        private readonly List<(string Type, string Pattern)> tokenDefinitions = new List<(string, string)>
        {
            ("reservedword", @"\b(int|float|void|return|if|include|while|cin|cout|using|namespace|std|main)\b"),
            //("include", @"\b#include\b"),
            ("identifier", @"\b[a-zA-Z_][a-zA-Z0-9_]*\b"),
            ("number", @"\b\d+\b"),
            ("symbol", @"[{}();,<>!=+\-*/]"),
            ("string", "\".*?\""),
            ("whitespace", @"\s+"),
            ("unknown", @".")
        };


        public List<Token> Analyze(string input)
        {
            Tokens.Clear();
            int position = 0;

            while (position < input.Length)
            {
                Token token = MatchToken(input, ref position);
                if (token.Type != "whitespace")
                {
                    Tokens.Add(token);
                }
            }

            return Tokens;
        }

        private Token MatchToken(string input, ref int position)
        {
            foreach (var (type, pattern) in tokenDefinitions)
            {
                Regex regex = new Regex($"^({pattern})", RegexOptions.Compiled);
                Match match = regex.Match(input.Substring(position));

                if (match.Success)
                {
                    string value = match.Value;
                    position += value.Length;
                    return new Token(type, value);
                }
            }

            throw new Exception($"Unexpected token at position {position}");
        }

        // Bonus: Find the first declaration
        public string FindFirstDeclaration(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i].Type == "reservedword" &&
                    (tokens[i].Value == "int" || tokens[i].Value == "float") &&
                    tokens[i + 1].Type == "identifier")
                {
                    return tokens[i + 1].Value;
                }
            }
            return null;
        }

        // Bonus: Validate tokens for common errors
        public List<string> ValidateTokens(List<Token> tokens)
        {
            List<string> errors = new List<string>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == "unknown")
                {
                    errors.Add($"Unknown token: {tokens[i].Value}");
                }

                // Check for missing semicolon
                if (tokens[i].Type == "reservedword" &&
                    tokens[i].Value != "while" && tokens[i].Value != "if" &&
                    i + 1 < tokens.Count && tokens[i + 1].Value != ";")
                {
                    errors.Add($"Missing semicolon after {tokens[i].Value}");
                }

                // Check for invalid assignment
                if (tokens[i].Type == "identifier" &&
                    i + 2 < tokens.Count && tokens[i + 1].Value == "=" &&
                    tokens[i + 2].Type != "number" && tokens[i + 2].Type != "identifier")
                {
                    errors.Add($"Invalid assignment to {tokens[i].Value}");
                }
            }

            return errors;
        }

        public List<(string TokenType, string TokenValue, int HashValue)> CreateTokenTable()
        {
            return Tokens
                .OrderBy(t => t.Hash)
                .ThenBy(t => t.Type)
                .Select(t => (t.Type, t.Value, t.Hash))
                .ToList();
        }
    }
}
