using GeoTagMap.Models;
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
    public class CommentRepository : ICommentRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();

        public async Task<List<ICommentModel>> GetAllCommentsAsync()
        {
            Dictionary<Guid, ICommentModel> comments = new Dictionary<Guid, ICommentModel>();
            using(var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using(var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT c.\"Id\", c.\"CommentText\", c.\"IsActive\", c.\"UserId\" AS CommentUserId, c.\"EventId\" AS CommentEventId, c.\"TouristSitesId\" AS CommentTouristSitesId, c.\"StoryId\" AS CommentStoryId, c.\"DateCreated\", c.\"DateUpdated\", c.\"CreatedBy\", ");
                    query.Append("e.\"Id\" AS EventId, e.\"Name\", e.\"Url\", e.\"IsActive\", e.\"EventStatus\", e.\"Image\", e.\"StartDate\", e.\"EndDate\", e.\"IsAccessibleForFree\", e.\"Type\", e.\"TicketInformationId\" AS EventTicketInformationId, e.\"LocationId\" AS EventLocationId, ");
                    query.Append("s.\"Id\" AS StoryId, s.\"StoryText\", s.\"Date\", s.\"LocationId\" AS StoryLocationId, s.\"UserId\" AS StoryUserId, ");
                    query.Append("t.\"Id\" AS TouristSitesId, t.\"SiteName\", t.\"Link\", t.\"LocationId\" AS TouristSiteLocationId, t.\"Fsq_Id\", t.\"Popularity\", t.\"Rating\", t.\"Description\", t.\"WebsiteUrl\", t.\"Email\", t.\"HoursOpen\", t.\"FacebookId\", t.\"Instagram\", t.\"Twitter\", ");
                    query.Append("u.\"Id\" AS UserId, u.\"Username\", u.\"FirstName\", u.\"LastName\", u.\"Email\" AS UserEmail, u.\"Password\", u.\"Image\", u.\"IsActive\", u.\"RoleId\" AS UserRoleId ");
                    query.Append("FROM \"Comment\" c ");
                    query.Append("LEFT JOIN \"Event\" e ON c.\"EventId\" = e.\"Id\" ");
                    query.Append("LEFT JOIN \"Story\" s ON c.\"StoryId\" = s.\"Id\" ");
                    query.Append("LEFT JOIN \"TouristSites\" t ON c.\"TouristSitesId\" = t.\"Id\" ");
                    query.Append("LEFT JOIN \"User\" u ON c.\"UserId\" = u.\"Id\" ");
                    query.Append("WHERE c.\"IsActive\" = true");

                    cmd.CommandText = query.ToString();
                    using(var reader = await cmd.ExecuteReaderAsync()) 
                    {
                        while(await reader.ReadAsync())
                        {
                            ICommentModel currentComment;
                            Guid commentId = reader.GetGuid(reader.GetOrdinal("Id"));

                            if (!comments.TryGetValue(commentId, out currentComment))
                            {
                                currentComment = MapComment(reader);
                                comments.Add(commentId, currentComment);
                            }
                            if (reader["EventId"] != DBNull.Value)
                            {
                                IEventModel eventModel = MapEvent(reader);
                                currentComment.Event = eventModel;
                            }
                            if (reader["StoryId"] != DBNull.Value)
                            {
                                IStoryModel story = MapStory(reader);
                                currentComment.Story = story;
                            }
                            if (reader["TouristSitesId"] != DBNull.Value)
                            {
                                ITouristSitesModel touristSite = MapTouristSite(reader);
                                currentComment.Site = touristSite;
                            }
                            if (reader["CommentUserId"] != DBNull.Value)
                            {
                                var user = MapUser(reader);
                                currentComment.User = user;
                            }
                        }
                    }
                }
            }
            return comments.Values.ToList();
        }

        public async Task<ICommentModel> GetCommentAsync(Guid id)
        {
            ICommentModel currentComment = null;
            using(var con =new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using( var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT c.\"Id\", c.\"CommentText\", c.\"IsActive\", c.\"UserId\" AS CommentUserId, c.\"EventId\" AS CommentEventId, c.\"TouristSitesId\" AS CommentTouristSitesId, c.\"StoryId\" AS CommentStoryId, c.\"DateCreated\", c.\"DateUpdated\", c.\"CreatedBy\", ");
                    query.Append("e.\"Id\" AS EventId, e.\"Name\", e.\"Url\", e.\"IsActive\", e.\"EventStatus\", e.\"Image\", e.\"StartDate\", e.\"EndDate\", e.\"IsAccessibleForFree\", e.\"Type\", e.\"TicketInformationId\" AS EventTicketInformationId, e.\"LocationId\" AS EventLocationId, ");
                    query.Append("s.\"Id\" AS StoryId, s.\"StoryText\", s.\"Date\", s.\"LocationId\" AS StoryLocationId, s.\"UserId\" AS StoryUserId, ");
                    query.Append("t.\"Id\" AS TouristSitesId, t.\"SiteName\", t.\"Link\", t.\"LocationId\" AS TouristSiteLocationId, t.\"Fsq_Id\", t.\"Popularity\", t.\"Rating\", t.\"Description\", t.\"WebsiteUrl\", t.\"Email\", t.\"HoursOpen\", t.\"FacebookId\", t.\"Instagram\", t.\"Twitter\", ");
                    query.Append("u.\"Id\" AS UserId, u.\"Username\", u.\"FirstName\", u.\"LastName\", u.\"Email\" AS UserEmail, u.\"Password\", u.\"Image\", u.\"IsActive\", u.\"RoleId\" AS UserRoleId ");
                    query.Append("FROM \"Comment\" c ");
                    query.Append("LEFT JOIN \"Event\" e ON c.\"EventId\" = e.\"Id\" ");
                    query.Append("LEFT JOIN \"Story\" s ON c.\"StoryId\" = s.\"Id\" ");
                    query.Append("LEFT JOIN \"TouristSites\" t ON c.\"TouristSitesId\" = t.\"Id\" ");
                    query.Append("LEFT JOIN \"User\" u ON c.\"UserId\" = u.\"Id\" ");
                    query.Append("WHERE c.\"Id\" = @commentId ");
                    query.Append("AND c.\"IsActive\" = true");

                    cmd.Parameters.AddWithValue("@commentId", id);
                    cmd.CommandText = query.ToString();
                    using(var reader = await cmd.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            if(currentComment == null)
                            {
                                currentComment = MapComment(reader);
                            }
                            if (reader["EventId"] != DBNull.Value)
                            {
                                IEventModel eventModel = MapEvent(reader);
                                currentComment.Event = eventModel;
                            }
                            if (reader["StoryId"] != DBNull.Value)
                            {
                                IStoryModel story = MapStory(reader);
                                currentComment.Story = story;
                            }
                            if (reader["TouristSitesId"] != DBNull.Value)
                            {
                                ITouristSitesModel touristSite = MapTouristSite(reader);
                                currentComment.Site = touristSite;
                            }
                            if (reader["CommentUserId"] != DBNull.Value)
                            {
                                var user = MapUser(reader);
                                currentComment.User = user;
                            }
                        }
                    }
                }
            }
            return currentComment;
        }

        public async Task AddCommentAsync(ICommentModel comment)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO \"Comment\" (\"Id\", \"CommentText\", \"IsActive\", \"UserId\", \"EventId\", \"TouristSitesId\", \"StoryId\", \"DateCreated\", \"DateUpdated\", \"CreatedBy\", \"UpdatedBy\") ");
                    query.Append("VALUES (@id, @text, @isActive, @userId, @eventId, @touristSiteId, @storyId, @dateCreated, @dateUpdated, @createdBy, @updatedBy)");

                    cmd.CommandText = query.ToString();

                    Guid randid = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("@id", randid);
                    cmd.Parameters.AddWithValue("@text", comment.Text ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isActive", comment.IsActive);
                    cmd.Parameters.AddWithValue("@userId", comment.UserId);
                    cmd.Parameters.AddWithValue("@eventId", comment.EventId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@touristSiteId", comment.TouristSiteId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@storyId", comment.StoryId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@dateCreated", comment.DateCreated);
                    cmd.Parameters.AddWithValue("@dateUpdated", comment.DateUpdated);
                    cmd.Parameters.AddWithValue("@updatedBy", comment.UpdatedBy);
                    cmd.Parameters.AddWithValue("@createdBy", comment.CreatedBy);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateCommentAsync(Guid id, ICommentModel comment)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE \"Comment\" SET ");

                    if (!string.IsNullOrEmpty(comment.Text))
                    {
                        query.Append("\"CommentText\" = @text, ");
                        cmd.Parameters.AddWithValue("@text", comment.Text);
                    }
                    if (comment.UpdatedBy != Guid.Empty)
                    {
                        query.Append("\"UpdatedBy\" = @updatedBy, ");
                        cmd.Parameters.AddWithValue("@updatedBy", comment.UpdatedBy);
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

        public async Task DeleteCommentAsync(Guid id)
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
                        StringBuilder queryCategory = new StringBuilder();
                        queryCategory.Append("UPDATE \"Comment\" SET \"IsActive\" = false WHERE \"Id\" = @id");
                        cmd.CommandText = queryCategory.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private ICommentModel MapComment(NpgsqlDataReader reader)
        {
            return new CommentModel
            {
                Id = (Guid)reader["Id"],
                Text = Convert.ToString(reader["CommentText"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                UserId = (Guid)reader["CommentUserId"],
                EventId = Convert.IsDBNull(reader["CommentEventId"]) ? null : (Guid)reader["CommentEventId"],
                StoryId = Convert.IsDBNull(reader["CommentStoryId"]) ? null : (Guid)reader["CommentStoryId"],
                TouristSiteId = Convert.IsDBNull(reader["CommentTouristSitesId"]) ? null : (Guid)reader["CommentTouristSitesId"],
                DateCreated = Convert.ToDateTime(reader["DateCreated"]),
                DateUpdated = Convert.ToDateTime(reader["DateUpdated"]),
                CreatedBy = (Guid)reader["CreatedBy"],

                User = MapUser(reader),
                Event = Convert.IsDBNull(reader["CommentEventId"]) ? null : new EventModel(),
                Story = Convert.IsDBNull(reader["CommentStoryId"]) ? null : new StoryModel(),
                Site = Convert.IsDBNull(reader["CommentTouristSitesId"]) ? null : new TouristSitesModel()
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
                LocationId = Convert.IsDBNull(reader["EventLocationId"]) ? null : (Guid)reader["EventLocationId"],
            };
        }
        private IStoryModel MapStory(NpgsqlDataReader reader)
        {
            return new StoryModel
            {
                Id = (Guid)reader["StoryId"],
                Text = !string.IsNullOrWhiteSpace(Convert.ToString(reader["StoryText"])) ? Convert.ToString(reader["StoryText"]) : null,
                LocationId = (Guid)reader["StoryLocationId"],
                UserId = (Guid)reader["StoryUserId"],
                DateTime = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : null,
            };
        }

        private ITouristSitesModel MapTouristSite(NpgsqlDataReader reader)
        {
            return new TouristSitesModel
            {
                Id = (Guid)reader["TouristSitesId"],
                LocationId = reader["TouristSiteLocationId"] != DBNull.Value ? (Guid)reader["TouristSiteLocationId"] : null,
                Name = !string.IsNullOrWhiteSpace(Convert.ToString(reader["SiteName"])) ? Convert.ToString(reader["SiteName"]) : null,
                Link = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Link"])) ? Convert.ToString(reader["Link"]) : null,
                Fsq_Id = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Fsq_Id"])) ? Convert.ToString(reader["Fsq_Id"]) : null,
                Popularity = reader["Popularity"] != DBNull.Value ? Convert.ToDouble(reader["Popularity"]) : 0.0,
                Rating = reader["Rating"] != DBNull.Value ? Convert.ToDouble(reader["Rating"]) : 0.0,
                Description = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Description"])) ? Convert.ToString(reader["Description"]) : null,
                WebsiteUrl = !string.IsNullOrWhiteSpace(Convert.ToString(reader["WebsiteUrl"])) ? Convert.ToString(reader["WebsiteUrl"]) : null,
                Email = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Email"])) ? Convert.ToString(reader["Email"]) : null,
                HoursOpen = !string.IsNullOrWhiteSpace(Convert.ToString(reader["HoursOpen"])) ? Convert.ToString(reader["HoursOpen"]) : null,
                FacebookId = !string.IsNullOrWhiteSpace(Convert.ToString(reader["FacebookId"])) ? Convert.ToString(reader["FacebookId"]) : null,
                Instagram = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Instagram"])) ? Convert.ToString(reader["Instagram"]) : null,
                Twitter = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Twitter"])) ? Convert.ToString(reader["Twitter"]) : null,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
            };
        }
        private IUserModel MapUser(NpgsqlDataReader reader)
        {
            return new UserModel
            {
                Id = (Guid)reader["CommentUserId"],
                Username = string.IsNullOrEmpty(reader["Username"].ToString()) ? null : Convert.ToString(reader["Username"]),
                FirstName = string.IsNullOrEmpty(reader["FirstName"].ToString()) ? null : Convert.ToString(reader["FirstName"]),
                LastName = string.IsNullOrEmpty(reader["LastName"].ToString()) ? null : Convert.ToString(reader["LastName"]),
                Email = string.IsNullOrEmpty(reader["UserEmail"].ToString()) ? null : Convert.ToString(reader["UserEmail"]),
                Password = Convert.ToString(reader["Password"]),
                RoleId = Convert.IsDBNull(reader["UserRoleId"]) ? Guid.Empty : (Guid)reader["UserRoleId"],
                Image = string.IsNullOrEmpty(reader["Image"].ToString()) ? null : Convert.ToString(reader["Image"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

    }
}
