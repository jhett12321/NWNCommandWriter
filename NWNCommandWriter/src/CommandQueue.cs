namespace NWNCommandWriter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using NWNCommandWriter.Config;

    public class CommandQueue
    {
        public readonly List<Command> Commands = new List<Command>();

        public CommandQueue(AppConfig config, string text)
        {
            text = ReplaceChars(config.ReplaceRules, text);
            List<string> lines = GetStringLines(text).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach (string line in lines)
            {
                Commands.Add(new Command(CommandType.NewLine));
                
                if (line.Length + config.WriteCommand.Length < config.ChatCommandLimit)
                {
                    stringBuilder.Append(config.WriteCommand);
                    stringBuilder.Append(line);
                }
                else
                {
                    stringBuilder.Append(config.WriteCommand);
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (stringBuilder.Length == config.ChatCommandLimit - 1)
                        {
                            Commands.Add(new Command(CommandType.Text, stringBuilder.ToString()));
                            Commands.Add(new Command(CommandType.NewLine));
                            Commands.Add(new Command(CommandType.NewLine));
                            stringBuilder.Clear();
                            stringBuilder.Append(config.WriteCommand);
                        }
                        
                        stringBuilder.Append(line[i]);
                    }
                }
                
                Commands.Add(new Command(CommandType.Text, stringBuilder.ToString()));
                Commands.Add(new Command(CommandType.NewLine));
                Commands.Add(new Command(CommandType.NewLine));
                Commands.Add(new Command(CommandType.Text, config.NewLineCommand));
                Commands.Add(new Command(CommandType.NewLine));
                stringBuilder.Clear();
            }
        }

        private static string ReplaceChars(IEnumerable<AppConfig.ReplaceRule> rules, string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text);
            foreach (AppConfig.ReplaceRule rule in rules)
            {
                stringBuilder.Replace(rule.Match, rule.Replace);
            }

            return stringBuilder.ToString();
        }

        private static IEnumerable<string> GetStringLines(string text)
        {
            using StringReader stringReader = new StringReader(text);
            string line;
            
            while((line = stringReader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}