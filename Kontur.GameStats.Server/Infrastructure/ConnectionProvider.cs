using System;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using Model;

namespace Kontur.GameStats.Server.Infrastructure
{
    public class ConnectionProvider
    {
        public Entities GetEntities(string directory = null)
        {
            var connection = GetConnection(directory ?? AppDomain.CurrentDomain.BaseDirectory, "GameStats");
            var entities = new Entities(connection);
            entities.Configuration.AutoDetectChangesEnabled = false;
            entities.Configuration.ValidateOnSaveEnabled = false;
            return entities;
        }

        private static EntityConnection GetConnection(string directory, string dbName)
        {
            var dbPath = Path.Combine(directory, dbName);
            var conn = new EntityConnectionStringBuilder
            {
                Metadata = $@"res://*/{dbName}.csdl|res://*/{dbName}.ssdl|res://*/{dbName}.msl",
                Provider = "System.Data.SQLite.EF6",
                ProviderConnectionString =
                    $@"data source={dbPath}.db;Version=3;"
            };
            return new EntityConnection(conn.ConnectionString);
        }
    }
}
