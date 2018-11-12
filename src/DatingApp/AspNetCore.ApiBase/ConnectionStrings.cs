using System.Collections.Generic;

namespace AspNetCore.ApiBase
{
    public static class ConnectionStrings
    {
        private static Dictionary<string, string> _connectionStrings = new Dictionary<string, string>();

        public static void AddConnectionString(string name, string connectionString)
        {
            if (_connectionStrings.ContainsKey(name))
            {
                _connectionStrings[name] = connectionString;
            }
            else
            {
                _connectionStrings.Add(name, connectionString);
            }
        }

        public static void AddConnectionStringIfNotExists(string name, string connectionString)
        {
            if (!_connectionStrings.ContainsKey(name))
            {
                _connectionStrings[name] = connectionString;
            }
        }

        public static string GetConnectionString(string name)
        {
            return _connectionStrings[name];
        }
    }
}
