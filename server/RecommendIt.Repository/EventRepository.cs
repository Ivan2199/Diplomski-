using Npgsql;
using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTagMap.Repository.Common;
using NpgsqlTypes;
using System.Data;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Common;

namespace GeoTagMap.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();

        public async Task<PagingInfo<IEventModel>> GetAllEventsAsync(Paging paging, Sorting sort, EventFiltering filtering)
        {
            Dictionary<Guid, IEventModel> events = new Dictionary<Guid, IEventModel>();
            int totalEvents = 0;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT e.\"Id\", e.\"Name\", e.\"Url\", e.\"IsActive\", e.\"EventStatus\", e.\"Image\", e.\"StartDate\", e.\"EndDate\", e.\"IsAccessibleForFree\", e.\"Type\", e.\"TicketInformationId\" AS EventTicketInformationId, e.\"LocationId\" AS EventLocationId, e.\"JambaseIdentifier\", ");
                    query.Append("t.\"Id\" AS InformationId, t.\"Price\", t.\"PriceCurrency\", t.\"SellerName\", t.\"Url\", ");
                    query.Append("p.\"Id\" AS EventPerformerId, p.\"EventId\", p.\"PerformerId\", ");
                    query.Append("l.\"Id\" AS LocationId, l.\"Country\", l.\"City\", l.\"Village\", l.\"Address\", l.\"NameOfPlace\", l.\"IsActive\", l.\"JambaseIdentifier\", ");
                    query.Append("c.\"Id\" AS CommentId, c.\"CommentText\", c.\"IsActive\", c.\"UserId\" AS CommentUserId, c.\"EventId\" AS CommentEventId, c.\"TouristSitesId\" AS CommentTouristSitesId, c.\"StoryId\" AS CommentStoryId ");
                    query.Append("FROM \"Event\" e ");
                    query.Append("LEFT JOIN \"TicketInformation\" t ON e.\"TicketInformationId\" = t.\"Id\" ");
                    query.Append("LEFT JOIN \"PerformerEvent\" p ON e.\"Id\" = p.\"EventId\" ");
                    query.Append("LEFT JOIN \"Location\" l ON e.\"LocationId\" = l.\"Id\" ");
                    query.Append("LEFT JOIN \"Comment\" c ON e.\"Id\" = c.\"EventId\" ");
                    query.Append("WHERE e.\"IsActive\" = true");

                    if(!string.IsNullOrEmpty(filtering.Name))
                    {
                        query.Append($" AND e.\"Name\" = '{FormatString(filtering.Name)}'");
                    }
                    if(!string.IsNullOrEmpty(filtering.EventStatus))
                    {
                        query.Append($" AND e.\"EventStatus\" = '{filtering.EventStatus}'");
                    }
                    if (filtering.StartDate != null)
                    {
                        string formattedStartDate = filtering.StartDate.Value.ToString("yyyy-MM-dd");
                        query.Append($" AND e.\"StartDate\" >= '{formattedStartDate}'");
                    }

                    if (filtering.IsAccessibleForFree != null)
                    {
                        query.Append($" AND e.\"IsAccessibleForFree\" = '{filtering.IsAccessibleForFree}'");
                    }
                    if (!string.IsNullOrEmpty(filtering.Type))
                    {
                        query.Append($" AND e.\"Type\" = '{FormatString(filtering.Type)}'");
                    }
                    if (!string.IsNullOrEmpty(filtering.SearchKeyword))
                    {
                        query.Append($" AND (e.\"Name\" LIKE '%{FormatString(filtering.SearchKeyword)}%')");
                    }

                    using (var countCmd = con.CreateCommand())
                    {
                        countCmd.Connection = con;
                        countCmd.CommandText = "SELECT COUNT(*) FROM \"Event\" e";

                        if (!string.IsNullOrEmpty(filtering.Name))
                        {
                            countCmd.CommandText += $" WHERE e.\"Name\" = '{FormatString(filtering.Name)}'";
                        }
                        if (!string.IsNullOrEmpty(filtering.EventStatus))
                        {
                            if (countCmd.CommandText.Contains("WHERE"))
                            {
                                countCmd.CommandText += $" AND e.\"EventStatus\" = '{filtering.EventStatus}'";
                            } else
                            {
                                countCmd.CommandText += $" WHERE e.\"EventStatus\" = '{filtering.EventStatus}'";
                            }
                        }
                        if (filtering.StartDate != null)
                        {
                            if (countCmd.CommandText.Contains("WHERE"))
                            {
                                string formattedStartDate = filtering.StartDate.Value.ToString("yyyy-MM-dd");
                                countCmd.CommandText += $" AND e.\"StartDate\" >= '{formattedStartDate}'";
                            }
                            else
                            {
                                string formattedStartDate = filtering.StartDate.Value.ToString("yyyy-MM-dd");
                                countCmd.CommandText += $" WHERE e.\"StartDate\" >= '{formattedStartDate}'";
                            }
                        }
                        if (filtering.IsAccessibleForFree != null)
                        {
                            if (countCmd.CommandText.Contains("WHERE"))
                            {
                                countCmd.CommandText += $" AND e.\"IsAccessibleForFree\" = '{filtering.IsAccessibleForFree}'";
                            }
                            else
                            {
                                countCmd.CommandText += $" WHERE e.\"IsAccessibleForFree\" = '{filtering.IsAccessibleForFree}'";
                            }
                        }
                        if (!string.IsNullOrEmpty(filtering.Type))
                        {
                            if (countCmd.CommandText.Contains("WHERE"))
                            {
                                countCmd.CommandText += $" AND e.\"Type\" = '{FormatString(filtering.Type)}'";
                            }
                            else
                            {
                                countCmd.CommandText += $" WHERE e.\"Type\" = '{FormatString(filtering.Type)}'";
                            }
                        }
                        if (!string.IsNullOrEmpty(filtering.SearchKeyword))
                        {
                            if (countCmd.CommandText.Contains("WHERE"))
                            {
                                countCmd.CommandText += $" AND (e.\"Name\" LIKE '%{FormatString(filtering.SearchKeyword)}%')";
                            }
                            else
                            {
                                countCmd.CommandText += $" WHERE (e.\"Name\" LIKE '%{FormatString(filtering.SearchKeyword)}%')";
                            }
                        }

                        totalEvents = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
                    }

                    if(!string.IsNullOrEmpty(sort.OrderBy))
                    {
                        query.Append($" ORDER BY \"{sort.OrderBy}\" {(sort.SortOrder.ToUpper() == "DESC" ? "DESC" : "ASC")}");
                    }
                    query.Append($" OFFSET {paging.RRP * (paging.PageNumber - 1)}");
                    query.Append($" LIMIT {paging.RRP}");

                    cmd.CommandText = query.ToString();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            IEventModel currentEvent;
                            Guid currentEventId = reader.GetGuid(reader.GetOrdinal("Id"));

                            if (!events.TryGetValue(currentEventId, out currentEvent))
                            {
                                currentEvent = MapEvent(reader);
                                events.Add(currentEventId, currentEvent);
                            }
                            if (reader["InformationId"] != DBNull.Value)
                            {
                                var ticketInformation = MapTicketInformation(reader);

                                currentEvent.TicketInformation = ticketInformation;
                            }
                            if (reader["EventPerformerId"] != DBNull.Value)
                            {
                                var eventPerformer = MapEventPerformer(reader);
                                currentEvent.EventPerformers.Add(eventPerformer);
                            }
                            if (reader["LocationId"] != DBNull.Value)
                            {
                                var eventLocation = MapLocation(reader);
                                currentEvent.Location = eventLocation;
                            }
                            if (reader["CommentId"] != DBNull.Value)
                            {
                                var comment = MapComment(reader);
                                currentEvent.Comments.Add(comment);
                            }
                        }
                    }
                }
            }

            PagingInfo<IEventModel> pagingInfo = new PagingInfo<IEventModel>
            {
                List = events.Values.ToList(),
                RRP = paging.RRP,
                PageNumber = paging.PageNumber,
                TotalSize = totalEvents
            };
            return pagingInfo;
        }

        private string FormatString(string inputString)
        {
            string[] words = inputString.ToLower().Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    char[] letters = words[i].ToCharArray();
                    if (char.IsLetter(letters[0]))
                        letters[0] = char.ToUpper(letters[0]);
                    for (int j = 1; j < letters.Length; j++)
                    {
                        letters[j] = char.ToLower(letters[j]);
                    }
                    words[i] = new string(letters);
                }
            }
            return string.Join(" ", words);
        }

        public async Task<IEventModel> GetEventAsync(Guid id)
        {
            IEventModel eventModel = null;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT e.\"Id\", e.\"Name\", e.\"Url\", e.\"IsActive\", e.\"EventStatus\", e.\"Image\", e.\"StartDate\", e.\"EndDate\", e.\"IsAccessibleForFree\", e.\"Type\", e.\"TicketInformationId\" AS EventTicketInformationId, e.\"LocationId\" AS EventLocationId, e.\"JambaseIdentifier\", ");
                    query.Append("t.\"Id\" AS InformationId, t.\"Price\", t.\"PriceCurrency\", t.\"SellerName\", t.\"Url\", ");
                    query.Append("p.\"Id\" AS EventPerformerId, p.\"EventId\", p.\"PerformerId\", ");
                    query.Append("l.\"Id\" AS LocationId, l.\"Country\", l.\"City\", l.\"Village\", l.\"Address\", l.\"NameOfPlace\", l.\"IsActive\", l.\"JambaseIdentifier\", ");
                    query.Append("c.\"Id\" AS CommentId, c.\"CommentText\", c.\"IsActive\", c.\"UserId\" AS CommentUserId, c.\"EventId\" AS CommentEventId, c.\"TouristSitesId\" AS CommentTouristSitesId, c.\"StoryId\" AS CommentStoryId, c.\"DateCreated\" AS CommentDateCreated, c.\"DateUpdated\" AS CommentDateUpdated, c.\"CreatedBy\" AS CommentCreatedBy ");
                    query.Append("FROM \"Event\" e ");
                    query.Append("LEFT JOIN \"TicketInformation\" t ON e.\"TicketInformationId\" = t.\"Id\" ");
                    query.Append("LEFT JOIN \"PerformerEvent\" p ON e.\"Id\" = p.\"EventId\" ");
                    query.Append("LEFT JOIN \"Location\" l ON e.\"LocationId\" = l.\"Id\" ");
                    query.Append("LEFT JOIN \"Comment\" c ON e.\"Id\" = c.\"EventId\" ");
                    query.Append("WHERE e.\"Id\" = @eventId ");
                    query.Append("AND e.\"IsActive\" = true");


                    cmd.Parameters.AddWithValue("@eventId", id);
                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (eventModel == null)
                            {
                                eventModel = MapEvent(reader);
                            }
                            if (reader["InformationId"] != DBNull.Value)
                            {
                                var ticketInformation = MapTicketInformation(reader);

                                eventModel.TicketInformation = ticketInformation;
                            }
                            if (reader["EventPerformerId"] != DBNull.Value)
                            {
                                var eventPerformer = MapEventPerformer(reader);
                                eventModel.EventPerformers.Add(eventPerformer);
                            }
                            if (reader["LocationId"] != DBNull.Value)
                            {
                                var eventLocation = MapLocation(reader);
                                eventModel.Location = eventLocation;
                            }
                            if (reader["CommentId"] != DBNull.Value)
                            {
                                var comment = MapComment(reader);
                                eventModel.Comments.Add(comment);
                            }
                        }
                    }
                }
            }
            return eventModel;
        }

        public async Task<IEventModel> GetEventByJambaseIdentifierAsync(string jambaseIdentifier)
        {
            IEventModel eventModel = null;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT e.\"Id\", e.\"Name\", e.\"Url\", e.\"IsActive\", e.\"EventStatus\", e.\"Image\", e.\"StartDate\", e.\"EndDate\", e.\"IsAccessibleForFree\", e.\"Type\", e.\"TicketInformationId\" AS EventTicketInformationId, e.\"LocationId\" AS EventLocationId, e.\"JambaseIdentifier\", ");
                    query.Append("t.\"Id\" AS InformationId, t.\"Price\", t.\"PriceCurrency\", t.\"SellerName\", t.\"Url\", ");
                    query.Append("p.\"Id\" AS EventPerformerId, p.\"EventId\", p.\"PerformerId\", ");
                    query.Append("l.\"Id\" AS LocationId, l.\"Country\", l.\"City\", l.\"Village\", l.\"Address\", l.\"NameOfPlace\", l.\"IsActive\", l.\"JambaseIdentifier\", ");
                    query.Append("c.\"Id\" AS CommentId, c.\"CommentText\", c.\"IsActive\", c.\"UserId\" AS CommentUserId, c.\"EventId\" AS CommentEventId, c.\"TouristSitesId\" AS CommentTouristSitesId, c.\"StoryId\" AS CommentStoryId, c.\"DateCreated\" AS CommentDateCreated, c.\"DateUpdated\" AS CommentDateUpdated, c.\"CreatedBy\" AS CommentCreatedBy ");
                    query.Append("FROM \"Event\" e ");
                    query.Append("LEFT JOIN \"TicketInformation\" t ON e.\"TicketInformationId\" = t.\"Id\" ");
                    query.Append("LEFT JOIN \"PerformerEvent\" p ON e.\"Id\" = p.\"EventId\" ");
                    query.Append("LEFT JOIN \"Location\" l ON e.\"LocationId\" = l.\"Id\" ");
                    query.Append("LEFT JOIN \"Comment\" c ON e.\"Id\" = c.\"EventId\" ");
                    query.Append("WHERE e.\"JambaseIdentifier\" = @jambaseIdentifier ");
                    query.Append("AND e.\"IsActive\" = true");


                    cmd.Parameters.AddWithValue("@jambaseIdentifier", jambaseIdentifier);
                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (eventModel == null)
                            {
                                eventModel = MapEvent(reader);
                            }
                            if (reader["InformationId"] != DBNull.Value)
                            {
                                var ticketInformation = MapTicketInformation(reader);

                                eventModel.TicketInformation = ticketInformation;
                            }
                            if (reader["EventPerformerId"] != DBNull.Value)
                            {
                                var eventPerformer = MapEventPerformer(reader);
                                eventModel.EventPerformers.Add(eventPerformer);
                            }
                            if (reader["LocationId"] != DBNull.Value)
                            {
                                var eventLocation = MapLocation(reader);
                                eventModel.Location = eventLocation;
                            }
                            if (reader["CommentId"] != DBNull.Value)
                            {
                                var comment = MapComment(reader);
                                eventModel.Comments.Add(comment);
                            }
                        }
                    }
                }
            }
            return eventModel;
        }
        public async Task AddEventAsync(IEventModel eventModel)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO \"Event\" (\"Id\", \"Name\", \"Url\", \"IsActive\", \"EventStatus\", \"TicketInformationId\", \"LocationId\", \"Image\", \"StartDate\", \"EndDate\", \"IsAccessibleForFree\", \"Type\", \"DateCreated\", \"DateUpdated\", \"CreatedBy\", \"UpdatedBy\", \"JambaseIdentifier\") ");
                    query.Append("VALUES (@id, @name, @url, @isActive, @eventStatus, @ticketInformationId, @locationId, @image, @startDate, @endDate, @isAccessibleForFree, @type, @dateCreated, @dateUpdated, @createdBy, @updatedBy, @jambaseIdentifier)");

                    cmd.CommandText = query.ToString();

                    cmd.Parameters.AddWithValue("@id", eventModel.Id);
                    cmd.Parameters.AddWithValue("@name", eventModel.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@url", eventModel.Url ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isActive", eventModel.IsActive);
                    cmd.Parameters.AddWithValue("@eventStatus", eventModel.EventStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@startDate", eventModel.StartDate ?? (object)DBNull.Value); 
                    cmd.Parameters.AddWithValue("@endDate", eventModel.EndDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isAccessibleForFree", eventModel.IsAccessibleForFree ?? (object)DBNull.Value); 
                    cmd.Parameters.AddWithValue("@type", eventModel.Type ?? (object)DBNull.Value); 
                    cmd.Parameters.AddWithValue("@image", eventModel.Image ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ticketInformationId", eventModel.TicketInformationId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@locationId", eventModel.LocationId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@dateCreated", eventModel.DateCreated);
                    cmd.Parameters.AddWithValue("@dateUpdated", eventModel.DateUpdated);
                    cmd.Parameters.AddWithValue("@updatedBy", eventModel.UpdatedBy);
                    cmd.Parameters.AddWithValue("@createdBy", eventModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@jambaseIdentifier", eventModel.JambaseIdentifier ?? (object)DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateEventAsync(Guid id, IEventModel eventData)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE \"Event\" SET ");

                    if (!string.IsNullOrEmpty(eventData.Name))
                    {
                        query.Append("\"Name\" = @name, ");
                        cmd.Parameters.AddWithValue("@name", eventData.Name);
                    }

                    if (!string.IsNullOrEmpty(eventData.Url))
                    {
                        query.Append("\"Url\" = @url, ");
                        cmd.Parameters.AddWithValue("@url", eventData.Url);
                    }

                    if (!string.IsNullOrEmpty(eventData.EventStatus))
                    {
                        query.Append("\"EventStatus\" = @eventStatus, ");
                        cmd.Parameters.AddWithValue("@eventStatus", eventData.EventStatus);
                    }

                    if (eventData.Image != null)
                    {
                        query.Append("\"Image\" = @image, ");
                        cmd.Parameters.AddWithValue("@image", eventData.Image);
                    }

                    if (eventData.StartDate != null)
                    {
                        query.Append("\"StartDate\" = @startDate, ");
                        cmd.Parameters.AddWithValue("@startDate", eventData.StartDate);
                    }

                    if (eventData.EndDate != null)
                    {
                        query.Append("\"EndDate\" = @endDate, ");
                        cmd.Parameters.AddWithValue("@endDate", eventData.EndDate);
                    }

                    if (eventData.IsAccessibleForFree.HasValue)
                    {
                        query.Append("\"IsAccessibleForFree\" = @isAccessibleForFree, ");
                        cmd.Parameters.AddWithValue("@isAccessibleForFree", eventData.IsAccessibleForFree.Value);
                    }

                    if (!string.IsNullOrEmpty(eventData.Type))
                    {
                        query.Append("\"Type\" = @type, ");
                        cmd.Parameters.AddWithValue("@type", eventData.Type);
                    }
                    if (eventData.UpdatedBy != Guid.Empty)
                    {
                        query.Append("\"UpdatedBy\" = @updatedBy, ");
                        cmd.Parameters.AddWithValue("@updatedBy", eventData.UpdatedBy);
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


        public async Task DeleteEventAsync(Guid id)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@id", id);

                await using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryEvent = new StringBuilder();
                        queryEvent.Append("UPDATE \"Event\" SET \"IsActive\" = false WHERE \"Id\" = @id");
                        cmd.CommandText = queryEvent.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        var queryComment = new StringBuilder();
                        queryComment.Append("UPDATE \"Comment\" SET \"IsActive\" = false WHERE \"EventId\" = @id");
                        cmd.CommandText = queryComment.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        var queryTicketInformation = new StringBuilder();
                        queryTicketInformation.Append("UPDATE \"TicketInformation\" SET \"IsActive\" = false WHERE \"Id\" IN (SELECT \"TicketInformationId\" FROM \"Event\" WHERE \"Id\" = @id)");
                        cmd.CommandText = queryTicketInformation.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        var queryEventPerformer = new StringBuilder();
                        queryEventPerformer.Append("UPDATE \"PerformerEvent\" SET \"IsActive\" = false WHERE \"EventId\" = @id");
                        cmd.CommandText = queryEventPerformer.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        var queryLocation = new StringBuilder();
                        queryLocation.Append("UPDATE \"Location\" SET \"IsActive\" = false WHERE \"Id\" IN (SELECT \"LocationId\" FROM \"Event\" WHERE \"Id\" = @id)");
                        cmd.CommandText = queryLocation.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        var queryGeoLocation = new StringBuilder();
                        queryGeoLocation.Append("UPDATE \"GeoLocation\" SET \"IsActive\" = false WHERE \"LocationId\" IN (SELECT \"LocationId\" FROM \"Event\" WHERE \"Id\" = @id)");
                        cmd.CommandText = queryGeoLocation.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw; 
                    }
                }
            }
        }


        private IEventModel MapEvent(NpgsqlDataReader reader)
        {
            return new EventModel
            {
                Id = (Guid)reader["Id"],
                Name = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Name"])) ? Convert.ToString(reader["Name"]) : null,
                Url = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Url"])) ? Convert.ToString(reader["Url"]) : null,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                EventStatus = !string.IsNullOrWhiteSpace(Convert.ToString(reader["EventStatus"])) ? Convert.ToString(reader["EventStatus"]) : null,
                Image = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Image"])) ? Convert.ToString(reader["Image"]) : null,
                StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null,
                IsAccessibleForFree = reader["IsAccessibleForFree"] != DBNull.Value && Convert.ToBoolean(reader["IsAccessibleForFree"]),
                Type = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Type"])) ? Convert.ToString(reader["Type"]) : null,
                TicketInformationId = reader["EventTicketInformationId"] != DBNull.Value ? (Guid)reader["EventTicketInformationId"] : null,
                LocationId = reader["EventLocationId"] != DBNull.Value ? (Guid)reader["EventLocationId"] : null,


                TicketInformation = Convert.IsDBNull(reader["EventTicketInformationId"]) ? null : new TicketInformationModel(),
                EventPerformers = Convert.IsDBNull(reader["EventPerformerId"]) ? null : new List<IEventPerformerModel>(),
                Location = Convert.IsDBNull(reader["EventLocationId"]) ? null : new LocationModel(),
                Comments = Convert.IsDBNull(reader["CommentId"]) ? null : new List<ICommentModel>(),
            };
        }

        private ITicketInformationModel MapTicketInformation(NpgsqlDataReader reader)
        {
            return new TicketInformationModel
            {
                Id = (Guid)reader["InformationId"],
                Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : null,
                PriceCurrency = !string.IsNullOrWhiteSpace(Convert.ToString(reader["PriceCurrency"])) ? Convert.ToString(reader["PriceCurrency"]) : null,
                Seller = !string.IsNullOrWhiteSpace(Convert.ToString(reader["SellerName"])) ? Convert.ToString(reader["SellerName"]) : null,
                Url = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Url"])) ? Convert.ToString(reader["Url"]) : null,
            };
        }

        private IEventPerformerModel MapEventPerformer(NpgsqlDataReader reader)
        {
            return new EventPerformerModel
            {
                Id = (Guid)reader["EventPerformerId"],
                EventId = (Guid)reader["EventId"],
                PerformerId = (Guid)reader["PerformerId"],
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
                NameOfPlace = !string.IsNullOrWhiteSpace(Convert.ToString(reader["NameOfPlace"])) ? Convert.ToString(reader["NameOfPlace"]) : null,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                JambaseIdentifier = !string.IsNullOrWhiteSpace(Convert.ToString(reader["JambaseIdentifier"])) ? Convert.ToString(reader["JambaseIdentifier"]) : null
            };
        }
        private ICommentModel MapComment(NpgsqlDataReader reader)
        {
            return new CommentModel
            {
                Id = (Guid)reader["CommentId"],
                Text = !string.IsNullOrWhiteSpace(Convert.ToString(reader["CommentText"])) ? Convert.ToString(reader["CommentText"]) : null,
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                DateCreated = (DateTime)reader["CommentDateCreated"],
                DateUpdated = (DateTime)reader["CommentDateUpdated"],
                CreatedBy = (Guid)reader["CommentCreatedBy"],
                UserId = (Guid)reader["CommentUserId"],
                EventId = Convert.IsDBNull(reader["CommentEventId"]) ? null : (Guid)reader["CommentEventId"],
                StoryId = Convert.IsDBNull(reader["CommentStoryId"]) ? null : (Guid)reader["CommentStoryId"],
                TouristSiteId = Convert.IsDBNull(reader["CommentTouristSitesId"]) ? null : (Guid)reader["CommentTouristSitesId"]
            };
        }
    }
}
