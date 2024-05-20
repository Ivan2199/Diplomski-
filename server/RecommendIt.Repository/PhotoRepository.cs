using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();

        public async Task<List<IPhotoModel>> GetPhotosAsync()
        {
            Dictionary<Guid, IPhotoModel> photos = new Dictionary<Guid, IPhotoModel>();
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT p.\"Id\", p.\"ImagePrefix\", p.\"ImageSuffix\", p.\"TouristSiteId\" AS PhotoTouristSiteId, p.\"StoryId\" AS PhotoStoryId, p.\"UserId\" AS PhotoUserId, p.\"DateCreated\", p.\"DateUpdated\", p.\"CreatedBy\", ");
                    query.Append("t.\"Id\" AS TouristSiteId, t.\"SiteName\", t.\"Link\", t.\"LocationId\" AS TouristSiteLocationId, t.\"Fsq_Id\", t.\"Popularity\", t.\"Rating\", t.\"Description\", t.\"WebsiteUrl\", t.\"Email\", t.\"HoursOpen\", t.\"FacebookId\", t.\"Instagram\", t.\"Twitter\", ");
                    query.Append("s.\"Id\" AS StoryId, s.\"StoryText\", s.\"Date\", s.\"LocationId\" AS StoryLocationId, s.\"UserId\" AS StoryUserId, ");
                    query.Append("u.\"Id\" AS UserId, u.\"Username\", u.\"FirstName\", u.\"LastName\", u.\"Email\" AS UserEmail, u.\"Password\", u.\"Image\", u.\"IsActive\", u.\"RoleId\" AS UserRoleId ");
                    query.Append("FROM \"Photo\" p ");
                    query.Append("LEFT JOIN \"TouristSites\" t ON p.\"TouristSiteId\" = t.\"Id\" ");
                    query.Append("LEFT JOIN \"Story\" s ON p.\"StoryId\" = s.\"Id\" ");
                    query.Append("LEFT JOIN \"User\" u ON p.\"UserId\" = u.\"Id\" ");
                    query.Append("WHERE p.\"IsActive\" = true");

                    cmd.CommandText = query.ToString();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            IPhotoModel currentPhoto;
                            Guid currentPhotoId = reader.GetGuid(reader.GetOrdinal("Id"));
                            if (!photos.TryGetValue(currentPhotoId, out currentPhoto))
                            {
                                currentPhoto = MapPhoto(reader);
                                photos.Add(currentPhotoId, currentPhoto);
                            }
                            if (reader["PhotoTouristSiteId"] != DBNull.Value)
                            {
                                var touristSite = MapTouristSite(reader);

                                currentPhoto.TouristSite = touristSite;
                            }
                            if (reader["PhotoStoryId"] != DBNull.Value)
                            {
                                var story = MapStory(reader);

                                currentPhoto.Story = story;
                            }
                            if (reader["PhotoUserId"] != DBNull.Value)
                            {
                                var user = MapUser(reader);

                                currentPhoto.User = user;
                            }
                        }
                    }
                }
            }
            return photos.Values.ToList();
        }
        public async Task<IPhotoModel> GetPhotoAsync(Guid id)
        {
            IPhotoModel photo = null;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT p.\"Id\", p.\"ImagePrefix\", p.\"ImageSuffix\", p.\"TouristSiteId\" AS PhotoTouristSiteId, p.\"StoryId\" AS PhotoStoryId, p.\"UserId\" AS PhotoUserId , p.\"DateCreated\", p.\"DateUpdated\", p.\"CreatedBy\", ");
                    query.Append("t.\"Id\" AS TouristSiteId, t.\"SiteName\", t.\"Link\", t.\"LocationId\" AS TouristSiteLocationId, t.\"Fsq_Id\", t.\"Popularity\", t.\"Rating\", t.\"Description\", t.\"WebsiteUrl\", t.\"Email\", t.\"HoursOpen\", t.\"FacebookId\", t.\"Instagram\", t.\"Twitter\", ");
                    query.Append("s.\"Id\" AS StoryId, s.\"StoryText\", s.\"Date\", s.\"LocationId\" AS StoryLocationId, s.\"UserId\" AS StoryUserId, ");
                    query.Append("u.\"Id\" AS UserId, u.\"Username\", u.\"FirstName\", u.\"LastName\", u.\"Email\" AS UserEmail, u.\"Password\", u.\"Image\", u.\"IsActive\", u.\"RoleId\" AS UserRoleId ");
                    query.Append("FROM \"Photo\" p ");
                    query.Append("LEFT JOIN \"TouristSites\" t ON p.\"TouristSiteId\" = t.\"Id\" ");
                    query.Append("LEFT JOIN \"Story\" s ON p.\"StoryId\" = s.\"Id\" ");
                    query.Append("LEFT JOIN \"User\" u ON p.\"UserId\" = u.\"Id\" ");
                    query.Append("WHERE p.\"Id\" = @photoId ");
                    query.Append("AND p.\"IsActive\" = true");

                    cmd.Parameters.AddWithValue("@photoId", id);
                    cmd.CommandText = query.ToString();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (photo == null)
                            {
                                photo = MapPhoto(reader);
                            }
                            if (reader["PhotoTouristSiteId"] != DBNull.Value)
                            {
                                var touristSite = MapTouristSite(reader);

                                photo.TouristSite = touristSite;
                            }
                            if (reader["PhotoStoryId"] != DBNull.Value)
                            {
                                var story = MapStory(reader);
                                photo.Story = story;
                            }
                            if (reader["PhotoUserId"] != DBNull.Value)
                            {
                                var user = MapUser(reader);

                                photo.User = user;
                            }
                        }
                    }
                }
            }
            return photo;
        }
        public async Task AddPhotoAsync(IPhotoModel photoModel)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO \"Photo\" (\"Id\", \"ImagePrefix\", \"ImageSuffix\", \"TouristSiteId\", \"StoryId\", \"UserId\", \"DateCreated\", \"DateUpdated\", \"CreatedBy\", \"UpdatedBy\", \"IsActive\") ");
                    query.Append("VALUES (@id, @imagePrefix, @imageSuffix, @touristSiteId, @storyId, @userId, @dateCreated, @dateUpdated, @createdBy, @updatedBy, @isActive)");

                    cmd.CommandText = query.ToString();

                    cmd.Parameters.AddWithValue("@id", photoModel.Id);
                    cmd.Parameters.AddWithValue("@imagePrefix", photoModel.ImagePrefix ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@imageSuffix", photoModel.ImageSuffix ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@touristSiteId", photoModel.TouristSiteId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@storyId", photoModel.StoryId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@userId", photoModel.UserId);
                    cmd.Parameters.AddWithValue("@dateCreated", photoModel.DateCreated);
                    cmd.Parameters.AddWithValue("@dateUpdated", photoModel.DateUpdated);
                    cmd.Parameters.AddWithValue("@createdBy", photoModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@updatedBy", photoModel.UpdatedBy);
                    cmd.Parameters.AddWithValue("@isActive", photoModel.IsActive);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task UpdatePhotoAsync(Guid id, IPhotoModel photoData)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE \"Photo\" SET ");

                    if (!string.IsNullOrEmpty(photoData.ImagePrefix))
                    {
                        query.Append("\"ImagePrefix\" = @imagePrefix, ");
                        cmd.Parameters.AddWithValue("@imagePrefix", photoData.ImagePrefix);
                    }

                    if (!string.IsNullOrEmpty(photoData.ImageSuffix))
                    {
                        query.Append("\"ImageSuffix\" = @imageSuffix, ");
                        cmd.Parameters.AddWithValue("@imageSuffix", photoData.ImageSuffix);
                    }

                    if (photoData.UpdatedBy != Guid.Empty)
                    {
                        query.Append("\"UpdatedBy\" = @updatedBy, ");
                        cmd.Parameters.AddWithValue("@updatedBy", photoData.UpdatedBy);
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
        public async Task DeletePhotoAsync(Guid id)
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
                        var queryPhoto = new StringBuilder();
                        queryPhoto.Append("UPDATE \"Photo\" SET \"IsActive\" = false WHERE \"Id\" = @id");
                        cmd.CommandText = queryPhoto.ToString();
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
        private IPhotoModel MapPhoto(NpgsqlDataReader reader)
        {
            return new PhotoModel
            {
                Id = (Guid)reader["Id"],
                ImagePrefix = !string.IsNullOrWhiteSpace(Convert.ToString(reader["ImagePrefix"])) ? Convert.ToString(reader["ImagePrefix"]) : null,
                ImageSuffix = !string.IsNullOrWhiteSpace(Convert.ToString(reader["ImageSuffix"])) ? Convert.ToString(reader["ImageSuffix"]) : null,
                TouristSiteId = reader["PhotoTouristSiteId"] != DBNull.Value ? (Guid)reader["PhotoTouristSiteId"] : null,
                StoryId = reader["PhotoStoryId"] != DBNull.Value ? (Guid)reader["PhotoStoryId"] : null,
                UserId = (Guid)reader["PhotoUserId"],
                DateCreated = (DateTime)reader["DateCreated"],
                DateUpdated = (DateTime)reader["DateUpdated"],
                CreatedBy = (Guid)reader["CreatedBy"],

                TouristSite = Convert.IsDBNull(reader["PhotoTouristSiteId"]) ? null : new TouristSitesModel(),
                Story = Convert.IsDBNull(reader["PhotoStoryId"]) ? null : new StoryModel(),
            };
        }
        private ITouristSitesModel MapTouristSite(NpgsqlDataReader reader)
        {
            return new TouristSitesModel
            {
                Id = (Guid)reader["TouristSiteId"],
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
        private IUserModel MapUser(NpgsqlDataReader reader)
        {
            return new UserModel
            {
                Id = (Guid)reader["PhotoUserId"],
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
