using Npgsql;
using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();

        public async Task<List<IRoleModel>> GetAllRolesAsync()
        {
            Dictionary<Guid, IRoleModel> roleDict = new Dictionary<Guid, IRoleModel>();
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT r.\"Id\", r.\"Type\", u.\"Id\" AS UserId, u.\"Username\", u.\"FirstName\", u.\"LastName\", u.\"Email\", u.\"Password\", u.\"SurveyMetaData\", u.\"RoleId\" ");
                    query.Append("FROM \"Role\" r ");
                    query.Append("LEFT JOIN \"User\" u ON r.\"Id\" = u.\"RoleId\"");

                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            IRoleModel currentRole;
                            Guid currentRoleId = reader.GetGuid(reader.GetOrdinal("Id"));

                            if (!roleDict.TryGetValue(currentRoleId, out currentRole))
                            {
                                currentRole = MapRole(reader);
                                roleDict.Add(currentRoleId, currentRole);
                            }

                            if (reader["UserId"] != DBNull.Value)
                            {
                                var user = MapUser(reader);
                                currentRole.Users.Add(user);
                            }
                        }
                    }
                }
            }
            return roleDict.Values.ToList();
        }

        public async Task<IRoleModel> GetRoleByIdAsync(Guid id)
        {
            IRoleModel role = null;
            List<IUserModel> users = new List<IUserModel>();
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    cmd.Parameters.AddWithValue("@id", id);

                    query.Append("SELECT r.\"Id\", r.\"Type\", u.\"Id\" AS UserId, u.\"Username\", u.\"FirstName\", u.\"LastName\", u.\"Email\", u.\"Password\", u.\"SurveyMetaData\", u.\"RoleId\" ");
                    query.Append("FROM \"Role\" r ");
                    query.Append("LEFT JOIN \"User\" u ON r.\"Id\" = u.\"RoleId\"");
                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (role == null)
                            {
                                role = MapRole(reader);
                            }

                            if (!reader.IsDBNull(2))
                            {
                                var user = MapUser(reader);
                                users.Add(user);
                            }
                        }
                    }
                }
            }

            if (role != null)
            {
                role.Users = users;
            }
            return role;
        }
        public async Task<Guid> FindRoleByTypeAsync(string type)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    cmd.Parameters.AddWithValue("@type", type);

                    query.Append("SELECT \"Id\" FROM \"Role\" WHERE \"Type\" = @type");
                    cmd.CommandText = query.ToString();

                    var roleId = await cmd.ExecuteScalarAsync();

                    if (roleId != null && roleId != DBNull.Value)
                    {
                        return (Guid)roleId;
                    }
                    else
                    {
                        return Guid.Empty;
                    }
                }
            }
        }

        public async Task UpdateRoleAsync(Guid id, IRoleModel role)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE \"Role\" SET ");

                    if (!string.IsNullOrEmpty(role.Type))
                    {
                        query.Append("\"Type\" = @type, ");
                        cmd.Parameters.AddWithValue("@type", role.Type);
                    }
                    query.Length -= 2;
                    query.Append(" WHERE \"Id\" = @id");

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = query.ToString();

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private IRoleModel MapRole(NpgsqlDataReader reader)
        {
            return new RoleModel
            {
                Id = (Guid)reader["Id"],
                Type = Convert.ToString(reader["Type"]),
                Users = new List<IUserModel>()
            };
        }

        private IUserModel MapUser(NpgsqlDataReader reader)
        {
            return new UserModel
            {
                Id = (Guid)reader["UserId"],
                Username = Convert.ToString(reader["Username"]),
                FirstName = Convert.ToString(reader["FirstName"]),
                LastName = Convert.ToString(reader["LastName"]),
                Email = Convert.ToString(reader["Email"]),
                Password = Convert.ToString(reader["Password"]),
                RoleId = Convert.IsDBNull(reader["RoleId"]) ? Guid.Empty : (Guid)reader["RoleId"],
            };
        }
    }
}