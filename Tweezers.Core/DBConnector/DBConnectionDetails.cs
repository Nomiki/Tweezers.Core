// ReSharper disable InconsistentNaming

namespace Tweezers.DBConnector
{
    public sealed class DBConnectionDetails
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string DBName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DBType { get; set; }
    }
}
