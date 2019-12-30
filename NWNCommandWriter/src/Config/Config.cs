namespace NWNCommandWriter.Config
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public abstract class Config
    {
        [JsonIgnore]
        public abstract string FileName { get; }
        
        [JsonIgnore]
        public abstract bool WriteMissing { get; }

        public bool Deserialize()
        {
            Reset();
            
            // Write defaults if config does not exist.
            if (!File.Exists(FileName))
            {
                if (WriteMissing)
                {
                    GenerateDefaults();
                    Serialize();
                }

                return false;
            }

            string json = File.ReadAllText(FileName);
            try
            {
                JsonConvert.PopulateObject(json, this);
            }
            catch (Exception e)
            {
                Logger.LogExceptionError($"An error occurred while loading {FileName}. Ensure the file is valid JSON.", e);
                return false;
            }

            // Write any missing settings.
            if (WriteMissing)
            {
                Serialize();
            }
            
            return true;
        }

        public virtual void Reset() {}
        public virtual void GenerateDefaults() {}

        public void Serialize()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }
    }
}