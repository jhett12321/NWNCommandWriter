namespace NWNCommandWriter.Config
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    
    public sealed class AppConfig : Config
    {
        public override string FileName => "config.json";
        public override bool WriteMissing => true;
        
        // Serialized properties
        [JsonProperty("process_name")]
        public string ProcessName = "nwnmain";

        [JsonProperty("write_command")]
        public string WriteCommand = "!w ";
        [JsonProperty("new_line_command")]
        public string NewLineCommand = "!e";
        
        [JsonProperty("command_delay_ms")]
        public int CommandDelay = 30;

        [JsonProperty("chat_command_limit")]
        public int ChatCommandLimit = 295;
        [JsonProperty("description_text_limit")]
        public int TextLimit = 8192;

        [JsonProperty("replace_rules")]
        public List<ReplaceRule> ReplaceRules = new List<ReplaceRule>();

        public override void Reset()
        {
            ReplaceRules.Clear();
        }

        public override void GenerateDefaults()
        {
            // Common symbols replaced by MS Word
            ReplaceRules.Add(new ReplaceRule("“", "\""));
            ReplaceRules.Add(new ReplaceRule("”", "\""));
            ReplaceRules.Add(new ReplaceRule("‘", "'"));
            ReplaceRules.Add(new ReplaceRule("’", "'"));
            ReplaceRules.Add(new ReplaceRule("…", "..."));
        }

        public class ReplaceRule
        {
            [JsonProperty("match")]
            public string Match;
            [JsonProperty("replace")]
            public string Replace;

            public ReplaceRule(string match, string replace)
            {
                Match = match;
                Replace = replace;
            }
        }
    }
}