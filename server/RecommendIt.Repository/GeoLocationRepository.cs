using GeoTagMap.Models.Common;
using GeoTagMap.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTagMap.Repository.Common;

namespace GeoTagMap.Repository
{
    public class GeoLocationRepository : IGeoLoctionRepositrory
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();

        public async Task<List<IGeoLocationModel>> GetAllGeoLocationsAsync()
        {
            Dictionary<Guid, IGeoLocationModel> geoLocations = new Dictionary<Guid, IGeoLocationModel>();
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT g.\"Id\", g.\"Latitude\", g.\"Longitude\", g.\"LocationId\" AS GeoLocationLocationId, g.\"IsActive\", ");
                    query.Append("l.\"Id\" AS LocationId, l.\"Country\", l.\"City\", l.\"Village\", l.\"Address\", l.\"NameOfPlace\" ");
                    query.Append("FROM \"GeoLocation\" g ");
                    query.Append("LEFT JOIN \"Location\" l ON g.\"LocationId\" = l.\"Id\" ");
                    query.Append("WHERE g.\"IsActive\" = true");

                    cmd.CommandText = query.ToString();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            IGeoLocationModel currentGeoLocation;
                            Guid currentGeoLocationId = reader.GetGuid(reader.GetOrdinal("Id"));

                            if (!geoLocations.TryGetValue(currentGeoLocationId, out currentGeoLocation))
                            {
                                currentGeoLocation = MapGeoLocation(reader);
                                geoLocations.Add(currentGeoLocationId, currentGeoLocation);
                            }
                            if (reader["LocationId"] != DBNull.Value)
                            {
                                var location = MapLocation(reader);
                                currentGeoLocation.Location = location;
                            }
                        }
                    }
                }
            }
            return geoLocations.Values.ToList();
        }

        public async Task<IGeoLocationModel> GetGeoLocationAsync(Guid id)
        {
            IGeoLocationModel geoLocation = null;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT g.\"Id\", g.\"Latitude\", g.\"Longitude\", g.\"LocationId\" AS GeoLocationLocationId, g.\"IsActive\", ");
                    query.Append("l.\"Id\" AS LocationId, l.\"Country\", l.\"City\", l.\"Village\", l.\"Address\", l.\"NameOfPlace\" ");
                    query.Append("FROM \"GeoLocation\" g ");
                    query.Append("LEFT JOIN \"Location\" l ON g.\"LocationId\" = l.\"Id\" ");
                    query.Append("WHERE g.\"Id\" = @geoLocationId ");
                    query.Append("AND  g.\"IsActive\" = true");

                    cmd.Parameters.AddWithValue("@geoLocationId", id);
                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (geoLocation == null)
                            {
                                geoLocation = MapGeoLocation(reader);
                            }
                            if (reader["LocationId"] != DBNull.Value)
                            {
                                var location = MapLocation(reader);
                                geoLocation.Location = location;
                            }
                        }
                    }
                }
            }
            return geoLocation;
        }

        public async Task AddGeoLocationAsync(IGeoLocationModel geoLocationModel)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO \"GeoLocation\" (\"Id\", \"Latitude\", \"Longitude\", \"LocationId\", \"DateCreated\", \"DateUpdated\", \"CreatedBy\", \"UpdatedBy\", \"IsActive\") ");
                    query.Append("VALUES (@id, @latitude, @longitude, @locationId, @dateCreated, @dateUpdated, @createdBy, @updatedBy, @isActive)");

                    cmd.CommandText = query.ToString();

                    cmd.Parameters.AddWithValue("@id", geoLocationModel.Id);
                    cmd.Parameters.AddWithValue("@latitude", geoLocationModel.Latitude);
                    cmd.Parameters.AddWithValue("@longitude", geoLocationModel.Longitude);
                    cmd.Parameters.AddWithValue("@locationId", geoLocationModel.LocationId);
                    cmd.Parameters.AddWithValue("@dateCreated", geoLocationModel.DateCreated);
                    cmd.Parameters.AddWithValue("@dateUpdated", geoLocationModel.DateUpdated);
                    cmd.Parameters.AddWithValue("@createdBy", geoLocationModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@updatedBy", geoLocationModel.UpdatedBy);
                    cmd.Parameters.AddWithValue("@isActive", geoLocationModel.IsActive);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateGeoLocationAsync(Guid id, IGeoLocationModel geoLocationData)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE \"GeoLocation\" SET ");

                    if (geoLocationData.Latitude != 0)
                    {
                        query.Append("\"Latitude\" = @latitude, ");
                        cmd.Parameters.AddWithValue("@latitude", geoLocationData.Latitude);
                    }

                    if (geoLocationData.Longitude != 0)
                    {
                        query.Append("\"Longitude\" = @longitude, ");
                        cmd.Parameters.AddWithValue("@longitude", geoLocationData.Longitude);
                    }
                    if (geoLocationData.UpdatedBy != Guid.Empty)
                    {
                        query.Append("\"UpdatedBy\" = @updatedBy, ");
                        cmd.Parameters.AddWithValue("@updatedBy", geoLocationData.UpdatedBy);
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

        private IGeoLocationModel MapGeoLocation(NpgsqlDataReader reader)
        {
            return new GeoLocation
            {
                Id = (Guid)reader["Id"],
                Latitude = Convert.ToDouble(reader["Latitude"]),
                Longitude = Convert.ToDouble(reader["Longitude"]),
                LocationId = (Guid)reader["GeoLocationLocationId"],
                IsActive = reader["IsActive"] as bool?,


                Location = Convert.IsDBNull(reader["LocationId"]) ? null : new LocationModel()
            };
        }

        private ILocationModel MapLocation(NpgsqlDataReader reader)
        {
            return new LocationModel
            {
                Id = (Guid)reader["LocationId"],
                Country = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Country"])) ? Convert.ToString(reader["Country"]) : null,
                City = !string.IsNullOrWhiteSpace(Convert.ToString(reader["City"])) ? Convert.ToString(reader["City"]) : null,
                Village = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Village"])) ? Convert.ToString(reader["Village"]) : null,
                Address = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Address"])) ? Convert.ToString(reader["Address"]) : null,
                NameOfPlace = !string.IsNullOrWhiteSpace(Convert.ToString(reader["NameOfPlace"])) ? Convert.ToString(reader["NameOfPlace"]) : null
            };
        }
    }
}
