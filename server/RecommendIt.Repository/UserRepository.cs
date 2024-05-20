using Npgsql;
using NpgsqlTypes;
using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;

namespace GeoTagMap.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();

        public async Task<PagingInfo<IUserModel>> GetAllUsersAsync(Paging paging, Sorting sort, UserFiltering filtering)
        {
            List<IUserModel> users = new List<IUserModel>();
            int totalUsers = 0;

            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT ");
                    query.Append("u.\"Id\", ");
                    query.Append("u.\"Username\", ");
                    query.Append("u.\"FirstName\", ");
                    query.Append("u.\"LastName\", ");
                    query.Append("u.\"Email\", ");
                    query.Append("u.\"Password\", ");
                    query.Append("u.\"Image\", ");
                    query.Append("u.\"IsActive\", ");
                    query.Append("u.\"RoleId\" AS UserRoleId, ");
                    query.Append("r.\"Id\" AS RoleId, ");
                    query.Append("r.\"Type\" ");
                    query.Append("FROM \"User\" u ");
                    query.Append("LEFT JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\"");

                    if (!string.IsNullOrEmpty(filtering.Username))
                    {
                        query.Append($" WHERE u.\"Username\" = '{filtering.Username}'");
                    }
                    if (!string.IsNullOrEmpty(filtering.FirstName))
                    {
                        if (query.ToString().Contains("WHERE"))
                        {
                            query.Append($" AND u.\"FirstName\" = '{filtering.FirstName}'");
                        }
                        else
                        {
                            query.Append($" WHERE u.\"FirstName\" = '{filtering.FirstName}'");
                        }
                    }
                    if (!string.IsNullOrEmpty(filtering.LastName))
                    {
                        if (query.ToString().Contains("WHERE"))
                        {
                            query.Append($" AND u.\"LastName\" = '{filtering.LastName}'");
                        }
                        else
                        {
                            query.Append($" WHERE u.\"LastName\" = '{filtering.LastName}'");
                        }
                    }

                    using (var countCmd = con.CreateCommand())
                    {
                        countCmd.Connection = con;
                        countCmd.CommandText = "SELECT COUNT(*) FROM \"User\" u LEFT JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\"";

                        if (!string.IsNullOrEmpty(filtering.Username))
                        {
                            countCmd.CommandText += $" WHERE u.\"Username\" = '{filtering.Username}'";
                        }
                        if (!string.IsNullOrEmpty(filtering.FirstName))
                        {
                            if (countCmd.CommandText.Contains("WHERE"))
                            {
                                countCmd.CommandText += $" AND u.\"FirstName\" = '{filtering.FirstName}'";
                            }
                            else
                            {
                                countCmd.CommandText += $" WHERE u.\"FirstName\" = '{filtering.FirstName}'";
                            }
                        }
                        if (!string.IsNullOrEmpty(filtering.LastName))
                        {
                            if (countCmd.CommandText.Contains("WHERE"))
                            {
                                countCmd.CommandText += $" AND u.\"LastName\" = '{filtering.LastName}'";
                            }
                            else
                            {
                                countCmd.CommandText += $" WHERE u.\"LastName\" = '{filtering.LastName}'";
                            }
                        }

                        totalUsers = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
                    }

                    if (!string.IsNullOrEmpty(sort.OrderBy))
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
                            IUserModel currentUser = MapUser(reader);
                            if (reader["RoleId"] != DBNull.Value)
                            {
                                var userRole = MapUserRole(reader);
                                currentUser.UserRole = userRole;
                            }
                            users.Add(currentUser);
                        }
                    }
                }
            }

            PagingInfo<IUserModel> pagingInfo = new PagingInfo<IUserModel>
            {
                List = users,
                RRP = paging.RRP,
                PageNumber = paging.PageNumber,
                TotalSize = totalUsers
            };

            return pagingInfo;
        }

        public async Task<List<IUserModel>> GetAllUsersAsync()
        {
            Dictionary<Guid, IUserModel> users = new Dictionary<Guid, IUserModel>();
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT ");
                    query.Append("u.\"Id\", ");
                    query.Append("u.\"Username\", ");
                    query.Append("u.\"FirstName\", ");
                    query.Append("u.\"LastName\", ");
                    query.Append("u.\"Email\", ");
                    query.Append("u.\"Password\", ");
                    query.Append("u.\"Image\", ");
                    query.Append("u.\"IsActive\", ");
                    query.Append("u.\"RoleId\" AS UserRoleId, ");
                    query.Append("r.\"Id\" AS RoleId, ");
                    query.Append("r.\"Type\" ");
                    query.Append("FROM \"User\" u ");
                    query.Append("LEFT JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\"");

                    cmd.CommandText = query.ToString();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            IUserModel currentUser;
                            Guid currentUserId = reader.GetGuid(reader.GetOrdinal("Id"));

                            if (!users.TryGetValue(currentUserId, out currentUser))
                            {
                                currentUser = MapUser(reader);
                                users.Add(currentUserId, currentUser);
                            }

                            if (reader["RoleId"] != DBNull.Value)
                            {
                                var userRole = MapUserRole(reader);

                                currentUser.UserRole = userRole;
                            }
                        }
                    }
                }
            }
            return users.Values.ToList();
        }



        public async Task<IUserModel> GetUserByIdAsync(Guid id)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    cmd.Parameters.AddWithValue("@id", id);

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT ");
                    query.Append("u.\"Id\", ");
                    query.Append("u.\"Username\", ");
                    query.Append("u.\"FirstName\", ");
                    query.Append("u.\"LastName\", ");
                    query.Append("u.\"Email\", ");
                    query.Append("u.\"Password\", ");
                    query.Append("u.\"Image\", ");
                    query.Append("u.\"IsActive\", ");
                    query.Append("u.\"RoleId\" AS UserRoleId, ");
                    query.Append("r.\"Id\" AS RoleId, ");
                    query.Append("r.\"Type\" ");
                    query.Append("FROM \"User\" u ");
                    query.Append("LEFT JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\" ");
                    query.Append("WHERE u.\"Id\" = @id");

                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var user = MapUser(reader);

                            if (reader["RoleId"] != DBNull.Value)
                            {
                                var userRole = MapUserRole(reader);
                                user.UserRole = userRole;
                            }

                            return user;
                        }
                    }
                }
            }

            return null;
        }



        public async Task<IUserModel> FindUserAsync(UserFilter userFilter)
        {
            IUserModel user = null;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();

                    cmd.Parameters.AddWithValue("@username", userFilter.UserName);
                    query.Append("SELECT ");
                    query.Append("u.\"Id\", ");
                    query.Append("u.\"Username\", ");
                    query.Append("u.\"FirstName\", ");
                    query.Append("u.\"LastName\", ");
                    query.Append("u.\"Email\", ");
                    query.Append("u.\"Password\", ");
                    query.Append("u.\"Image\", ");
                    query.Append("u.\"IsActive\", ");
                    query.Append("u.\"RoleId\" AS UserRoleId, ");
                    query.Append("r.\"Id\" AS RoleId, ");
                    query.Append("r.\"Type\" ");
                    query.Append("FROM \"User\" u ");
                    query.Append("LEFT JOIN \"Role\" r ON u.\"RoleId\" = r.\"Id\"");
                    query.Append(" WHERE");
                    if (!string.IsNullOrEmpty(userFilter.UserName))
                    {
                        query.Append(" u.\"Username\" = @username");
                    }
                    cmd.CommandText = query.ToString();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (user == null) { user = MapUser(reader); }

                                if (reader["RoleId"] != DBNull.Value)
                                {
                                    var userRole = MapUserRole(reader);
                                    user.UserRole = userRole;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return user;
        }

        public async Task AddUserAsync(IUserModel user)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO \"User\" (\"Id\", \"Username\", \"FirstName\", \"LastName\", \"Email\", \"Password\", \"RoleId\", \"IsActive\", \"Image\") ");
                    query.Append("VALUES (@id, @username, @firstName, @lastName, @email, @password, @roleId, @isActive, @image)");

                    cmd.CommandText = query.ToString();

                    Guid randid = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("@id", randid);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", user.LastName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@image", user.Image ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isActive", user.IsActive);
                    cmd.Parameters.AddWithValue("@roleId", user.RoleId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateUserAsync(Guid id, IUserModel user)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE \"User\" SET ");

                    if (!string.IsNullOrEmpty(user.Username))
                    {
                        query.Append("\"Username\" = @username, ");
                        cmd.Parameters.AddWithValue("@username", user.Username);
                    }

                    if (!string.IsNullOrEmpty(user.FirstName))
                    {
                        query.Append("\"FirstName\" = @firstName, ");
                        cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                    }

                    if (!string.IsNullOrEmpty(user.LastName))
                    {
                        query.Append("\"LastName\" = @lastName, ");
                        cmd.Parameters.AddWithValue("@lastName", user.LastName);
                    }

                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        query.Append("\"Email\" = @email, ");
                        cmd.Parameters.AddWithValue("@email", user.Email);
                    }
                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        query.Append("\"Password\" = @passwrod, ");
                        cmd.Parameters.AddWithValue("@passwrod", user.Password);
                    }
                    if (user.RoleId != Guid.Empty)
                    {
                        query.Append("\"RoleId\" = @roleId, ");
                        cmd.Parameters.AddWithValue("@roleId", user.RoleId);
                    }
                    if (user.Image != null)
                    {
                        query.Append("\"Image\" = @image, ");
                        cmd.Parameters.AddWithValue("@image", user.Image);
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

        public async Task DeleteUserAsync(Guid id, Guid bannedRoleId)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@roleId", bannedRoleId);

                await using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        StringBuilder queryUser = new();
                        queryUser.Append("UPDATE \"User\" SET \"RoleId\" = @roleId WHERE \"User\".\"Id\" = @id;");
                        cmd.CommandText = queryUser.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        StringBuilder querySong = new();
                        querySong.Append("UPDATE \"Comment\" SET \"IsActive\" = false WHERE \"Comment\".\"CreatedBy\" = @id;");
                        cmd.CommandText = querySong.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        StringBuilder queryComment = new();
                        queryComment.Append("UPDATE \"Story\" SET \"IsActive\" = false WHERE \"Story\".\"CreatedBy\" = @id;");
                        cmd.CommandText = queryComment.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        StringBuilder queryHistory = new();
                        queryHistory.Append("UPDATE \"Location\" SET \"IsActive\" = false WHERE \"Location\".\"CreatedBy\" = @id;");
                        cmd.CommandText = queryHistory.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        StringBuilder queryReview = new();
                        queryReview.Append("UPDATE \"GeoLocation\" SET \"IsActive\" = false WHERE \"GeoLocation\".\"CreatedBy\" = @id");
                        cmd.CommandText = queryReview.ToString();
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

        private IUserModel MapUser(NpgsqlDataReader reader)
        {
            return new UserModel
            {
                Id = (Guid)reader["Id"],
                Username = Convert.ToString(reader["Username"]),
                FirstName = Convert.ToString(reader["FirstName"]),
                LastName = Convert.ToString(reader["LastName"]),
                Email = Convert.ToString(reader["Email"]),
                Password = Convert.ToString(reader["Password"]),
                RoleId = (Guid)reader["UserRoleId"],
                UserRole = new RoleModel(),
                Image = string.IsNullOrEmpty(reader["Image"].ToString()) ? null : Convert.ToString(reader["Image"]),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                
            };
        }

        private IRoleModel MapUserRole(NpgsqlDataReader reader)
        {
            return new RoleModel
            {
                Id = (Guid)(reader["RoleId"]),
                Type = Convert.ToString(reader["Type"])
            };
        }
    }
}