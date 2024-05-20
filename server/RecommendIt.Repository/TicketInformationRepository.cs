using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository
{
    public class TicketInformationRepository : ITicketInformationRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();

        public async Task<List<ITicketInformationModel>> GetTicketInformationsAsync()
        {
            Dictionary<Guid, ITicketInformationModel> ticketInformations = new Dictionary<Guid, ITicketInformationModel>();
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT t.\"Id\", t.\"Price\", t.\"PriceCurrency\", t.\"SellerName\", t.\"Url\", ");
                    query.Append("e.\"Id\" AS EventId, e.\"Name\", e.\"Url\", e.\"IsActive\", e.\"EventStatus\", e.\"Image\", e.\"StartDate\", e.\"EndDate\", e.\"IsAccessibleForFree\", e.\"Type\", e.\"TicketInformationId\" AS EventTicketInformationId, e.\"LocationId\" AS EventLocationId ");
                    query.Append("FROM \"TicketInformation\" t ");
                    query.Append("LEFT JOIN \"Event\" e ON t.\"Id\" = e.\"TicketInformationId\" ");
                    query.Append("WHERE t.\"IsActive\" = true");



                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ITicketInformationModel currentTicketInformation;
                            Guid ticketInformationId = reader.GetGuid(reader.GetOrdinal("Id"));

                            if(!ticketInformations.TryGetValue(ticketInformationId, out currentTicketInformation))
                            {
                                currentTicketInformation = MapTicketInformation(reader);
                                ticketInformations.Add(ticketInformationId, currentTicketInformation);
                            }
                            if (reader["EventId"] != DBNull.Value)
                            {
                                IEventModel eventModel = MapEvent(reader);
                                currentTicketInformation.Event = eventModel;
                            }
                        }
                    }
                }
            }
            return ticketInformations.Values.ToList();
        }

        public async Task<ITicketInformationModel> GetTicketInformationAsync(Guid id)
        {
            ITicketInformationModel ticketInformation = null;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT t.\"Id\", t.\"Price\", t.\"PriceCurrency\", t.\"SellerName\", t.\"Url\", ");
                    query.Append("e.\"Id\" AS EventId, e.\"Name\", e.\"Url\", e.\"IsActive\", e.\"EventStatus\", e.\"Image\", e.\"StartDate\", e.\"EndDate\", e.\"IsAccessibleForFree\", e.\"Type\", e.\"TicketInformationId\" AS EventTicketInformationId, e.\"LocationId\" AS EventLocationId ");
                    query.Append("FROM \"TicketInformation\" t ");
                    query.Append("LEFT JOIN \"Event\" e ON t.\"Id\" = e.\"TicketInformationId\" ");
                    query.Append("WHERE t.\"Id\" = @ticketInformationId ");
                    query.Append("AND t.\"IsActive\" = true");


                    cmd.Parameters.AddWithValue("@ticketInformationId", id);
                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (ticketInformation == null)
                            {
                                ticketInformation = MapTicketInformation(reader);
                            }
                            if (reader["EventId"] != DBNull.Value)
                            {
                                IEventModel eventModel = MapEvent(reader);
                                ticketInformation.Event = eventModel;
                            }
                        }
                    }
                }
            }
            return ticketInformation;
        }

        public async Task AddTicketInformationAsync(ITicketInformationModel ticketInformation)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO \"TicketInformation\" (\"Id\", \"Price\", \"PriceCurrency\", \"SellerName\", \"DateCreated\", \"DateUpdated\", \"CreatedBy\", \"UpdatedBy\", \"IsActive\", \"Url\") ");
                    query.Append("VALUES (@id, @price, @priceCurrency, @seller, @dateCreated, @dateUpdated, @createdBy, @updatedBy, @isActive, @url)");

                    cmd.CommandText = query.ToString();

                    cmd.Parameters.AddWithValue("@id", ticketInformation.Id);
                    cmd.Parameters.AddWithValue("@price", ticketInformation.Price ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@priceCurrency", ticketInformation.PriceCurrency ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@seller", ticketInformation.Seller ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@dateCreated", ticketInformation.DateCreated);
                    cmd.Parameters.AddWithValue("@dateUpdated", ticketInformation.DateUpdated);
                    cmd.Parameters.AddWithValue("@createdBy", ticketInformation.CreatedBy);
                    cmd.Parameters.AddWithValue("@updatedBy", ticketInformation.UpdatedBy);
                    cmd.Parameters.AddWithValue("@isActive", ticketInformation.IsActive);
                    cmd.Parameters.AddWithValue("@url", ticketInformation.Url ?? (object)DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateTicketInformationAsync(Guid id, ITicketInformationModel ticketInformationData)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE \"TicketInformation\" SET ");

                    if (ticketInformationData.Price != null)
                    {
                        query.Append("\"Price\" = @price, ");
                        cmd.Parameters.AddWithValue("@price", ticketInformationData.Price);
                    }

                    if (!string.IsNullOrEmpty(ticketInformationData.PriceCurrency))
                    {
                        query.Append("\"PriceCurrency\" = @priceCurrency, ");
                        cmd.Parameters.AddWithValue("@priceCurrency", ticketInformationData.PriceCurrency);
                    }

                    if (!string.IsNullOrEmpty(ticketInformationData.Seller))
                    {
                        query.Append("\"SellerName\" = @seller, ");
                        cmd.Parameters.AddWithValue("@seller", ticketInformationData.Seller);
                    }

                    if (ticketInformationData.UpdatedBy != Guid.Empty)
                    {
                        query.Append("\"UpdatedBy\" = @updatedBy, ");
                        cmd.Parameters.AddWithValue("@updatedBy", ticketInformationData.UpdatedBy);
                    }


                    query.Append("\"DateUpdated\" = @dateUpdated, ");
                    cmd.Parameters.AddWithValue("@dateUpdated", DateTime.Now);

                    query.Length -= 2;
                    query.Append(" WHERE \"Id\" = @id");

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = query.ToString();

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private ITicketInformationModel MapTicketInformation(NpgsqlDataReader reader)
        {
            return new TicketInformationModel
            {
                Id = (Guid)reader["Id"],
                Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : null,
                PriceCurrency = !string.IsNullOrWhiteSpace(Convert.ToString(reader["PriceCurrency"])) ? Convert.ToString(reader["PriceCurrency"]) : null,
                Seller = !string.IsNullOrWhiteSpace(Convert.ToString(reader["SellerName"])) ? Convert.ToString(reader["SellerName"]) : null,
                Url = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Url"])) ? Convert.ToString(reader["Url"]) : null,
                IsActive = reader["IsActive"] as bool?,

                Event = Convert.IsDBNull(reader["EventId"]) ? null : new EventModel()
            };
        }
        private IEventModel MapEvent(NpgsqlDataReader reader)
        {
            return new EventModel
            {
                Id = (Guid)reader["EventId"],
                Name = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Name"])) ? Convert.ToString(reader["Name"]) : null,
                Url = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Url"])) ? Convert.ToString(reader["Url"]) : null,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                EventStatus = !string.IsNullOrWhiteSpace(Convert.ToString(reader["EventStatus"])) ? Convert.ToString(reader["EventStatus"]) : null,
                Image = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Image"])) ? Convert.ToString(reader["Image"]) : null,
                StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null,
                IsAccessibleForFree = reader["IsAccessibleForFree"] != DBNull.Value && Convert.ToBoolean(reader["IsAccessibleForFree"]),
                Type = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Type"])) ? Convert.ToString(reader["Type"]) : null,
                TicketInformationId = Convert.IsDBNull(reader["EventTicketInformationId"]) ? null : (Guid)reader["EventTicketInformationId"],
                LocationId = Convert.IsDBNull(reader["EventLocationId"]) ? null : (Guid)reader["EventLocationId"]
            };
        }
    }
}
