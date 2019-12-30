namespace NWNCommandWriter
{
    public sealed class Command
    {
        public CommandType CommandType;
        public string Text;

        public Command(CommandType commandType, string text = null)
        {
            this.CommandType = commandType;
            this.Text = text;
        }
    }
}