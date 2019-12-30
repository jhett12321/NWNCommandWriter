﻿namespace NWNCommandWriter.Config
{
    public sealed class CommandLineConfig
    {
        // Valid Arguments - NWN
        private const string DIRECTORY_ARG = "-userDirectory";
        private const string CONNECT_ARG = "+connect";

        // Custom Home Path for NWN
        public readonly string UserDirectory;

        // Server to connect to on startup. Used for server info rich presence.
        public readonly string Connect;

        public CommandLineConfig(string[] commandLineArgs)
        {
            for (int i = 0; i < commandLineArgs.Length; i++)
            {
                switch (commandLineArgs[i])
                {
                    case DIRECTORY_ARG:
                        if (i + 1 < commandLineArgs.Length)
                        {
                            UserDirectory = commandLineArgs[i + 1];
                        }
                        break;
                    case CONNECT_ARG:
                        if (i + 1 < commandLineArgs.Length)
                        {
                            Connect = commandLineArgs[i + 1];
                        }
                        break;
                }
            }
        }
    }
}