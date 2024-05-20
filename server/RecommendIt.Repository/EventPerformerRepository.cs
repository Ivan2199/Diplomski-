using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository
{
    public class EventPerformerRepository : IEventPerformerRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();


        public async Task AddEventPerformerAsync(IEventPerformerModel eventPerformer)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();

                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO \"PerformerEvent\" (\"Id\", \"PerformerId\", \"EventId\", \"DateCreated\", \"DateUpdated\", \"CreatedBy\", \"UpdatedBy\", \"IsActive\") ");
                    query.Append("VALUES (@id, @performerId, @eventId, @dateCreated, @dateUpdated, @createdBy, @updatedBy, @isActive)");

                    cmd.CommandText = query.ToString();

                    cmd.Parameters.AddWithValue("@id", eventPerformer.Id);
                    cmd.Parameters.AddWithValue("@performerId", eventPerformer.PerformerId);
                    cmd.Parameters.AddWithValue("@EventId", eventPerformer.EventId);
                    cmd.Parameters.AddWithValue("@dateCreated", eventPerformer.DateCreated);
                    cmd.Parameters.AddWithValue("@dateUpdated", eventPerformer.DateUpdated);
                    cmd.Parameters.AddWithValue("@createdBy", eventPerformer.CreatedBy);
                    cmd.Parameters.AddWithValue("@updatedBy", eventPerformer.UpdatedBy);
                    cmd.Parameters.AddWithValue("@isActive", eventPerformer.IsActive);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
