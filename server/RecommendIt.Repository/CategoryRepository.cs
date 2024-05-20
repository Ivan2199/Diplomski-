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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ToString();

        public async Task<List<ICategoryModel>> GetAllCategoriesAsync()
        {
            Dictionary<Guid, ICategoryModel> categories = new Dictionary<Guid, ICategoryModel>();
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT c.\"Id\", c.\"Type\", c.\"Icon\", c.\"DateCreated\" AS CategoryDateCreated, c.\"DateUpdated\" AS CategoryDateUpdated, c.\"CreatedBy\" AS CategoryCreatedBy, c.\"UpdatedBy\" AS CategoryUpdatedBy, c.\"IsActive\" AS CategoryIsActive, c.\"Fsq_CategoryId\", ");
                    query.Append("t.\"Id\" AS TouristSiteCategoryId, t.\"TouristSiteId\", t.\"CategoryId\", t.\"DateCreated\" AS TouristSiteDateCreated, t.\"DateUpdated\" AS TouristSiteDateUpdated, t.\"CreatedBy\" AS TouristSiteCreatedBy, t.\"UpdatedBy\" AS TouristSiteUpdatedBy, t.\"IsActive\" AS TouristSiteIsActive ");
                    query.Append("FROM \"Category\" c ");
                    query.Append("LEFT JOIN \"TouristSiteCategory\" t ON c.\"Id\" = t.\"CategoryId\" ");
                    query.Append("WHERE c.\"IsActive\" = true");

                    cmd.CommandText = query.ToString();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            ICategoryModel currentCategory;
                            Guid categoryId = reader.GetGuid(reader.GetOrdinal("Id"));

                            if (!categories.TryGetValue(categoryId, out currentCategory))
                            {
                                currentCategory = MapCategory(reader);
                                categories.Add(categoryId ,currentCategory);
                            }
                            if (reader["TouristSiteCategoryId"] != DBNull.Value)
                            {
                                ITouristSiteCategoryModel touristSiteCategory = MapTouristSiteCategory(reader);
                                currentCategory.SiteCategories.Add(touristSiteCategory);
                            }
                        }
                    }
                }
            }
            return categories.Values.ToList();
        }

        public async Task<ICategoryModel> GetCategoryAsync(Guid id)
        {
            ICategoryModel categoryModel = null;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT c.\"Id\", c.\"Type\", c.\"Icon\", c.\"DateCreated\" AS CategoryDateCreated, c.\"DateUpdated\" AS CategoryDateUpdated, c.\"CreatedBy\" AS CategoryCreatedBy, c.\"UpdatedBy\" AS CategoryUpdatedBy, c.\"IsActive\" AS CategoryIsActive, c.\"Fsq_CategoryId\", ");
                    query.Append("t.\"Id\" AS TouristSiteCategoryId, t.\"TouristSiteId\", t.\"CategoryId\", t.\"DateCreated\" AS TouristSiteDateCreated, t.\"DateUpdated\" AS TouristSiteDateUpdated, t.\"CreatedBy\" AS TouristSiteCreatedBy, t.\"UpdatedBy\" AS TouristSiteUpdatedBy, t.\"IsActive\" AS TouristSiteIsActive ");
                    query.Append("FROM \"Category\" c ");
                    query.Append("LEFT JOIN \"TouristSiteCategory\" t ON c.\"Id\" = t.\"CategoryId\"");
                    query.Append("WHERE c.\"Id\" = @categoryId ");
                    query.Append("AND c.\"IsActive\" = true");

                    cmd.Parameters.AddWithValue("@categoryId", id);
                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (categoryModel == null)
                            {
                                categoryModel = MapCategory(reader);
                            }
                            if (reader["TouristSiteCategoryId"] != DBNull.Value)
                            {
                                ITouristSiteCategoryModel touristSiteCategory = MapTouristSiteCategory(reader);
                                categoryModel.SiteCategories.Add(touristSiteCategory);
                            }
                        }
                    }
                }
            }
            return categoryModel;
        }

        public async Task AddCategoryAsync(ICategoryModel categoryModel)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO \"Category\" (\"Id\", \"Type\", \"Icon\", \"DateCreated\", \"DateUpdated\", \"CreatedBy\", \"UpdatedBy\", \"IsActive\", \"Fsq_CategoryId\") ");
                    query.Append("VALUES (@id, @type, @icon, @dateCreated, @dateUpdated, @createdBy, @updatedBy, @isActive, @fsq_CategoryId)");

                    cmd.CommandText = query.ToString();

                    cmd.Parameters.AddWithValue("@id", categoryModel.Id);
                    cmd.Parameters.AddWithValue("@type", categoryModel.Type ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@icon", categoryModel.Icon ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@dateCreated", categoryModel.DateCreated);
                    cmd.Parameters.AddWithValue("@dateUpdated", categoryModel.DateUpdated);
                    cmd.Parameters.AddWithValue("@createdBy", categoryModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@updatedBy", categoryModel.UpdatedBy);
                    cmd.Parameters.AddWithValue("@isActive", categoryModel.IsActive);
                    cmd.Parameters.AddWithValue("@fsq_CategoryId", categoryModel.Fsq_CategoryId ?? (object)DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<ICategoryModel> GetCategoryByFsqIdAsync(string id)
        {
            ICategoryModel categoryModel = null;
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT c.\"Id\", c.\"Type\", c.\"Icon\", c.\"DateCreated\" AS CategoryDateCreated, c.\"DateUpdated\" AS CategoryDateUpdated, c.\"CreatedBy\" AS CategoryCreatedBy, c.\"UpdatedBy\" AS CategoryUpdatedBy, c.\"IsActive\" AS CategoryIsActive, c.\"Fsq_CategoryId\", ");
                    query.Append("t.\"Id\" AS TouristSiteCategoryId, t.\"TouristSiteId\", t.\"CategoryId\", t.\"DateCreated\" AS TouristSiteDateCreated, t.\"DateUpdated\" AS TouristSiteDateUpdated, t.\"CreatedBy\" AS TouristSiteCreatedBy, t.\"UpdatedBy\" AS TouristSiteUpdatedBy, t.\"IsActive\" AS TouristSiteIsActive ");
                    query.Append("FROM \"Category\" c ");
                    query.Append("LEFT JOIN \"TouristSiteCategory\" t ON c.\"Id\" = t.\"CategoryId\" ");
                    query.Append("WHERE c.\"Fsq_CategoryId\" = @fsqCategoryId ");
                    query.Append("AND c.\"IsActive\" = true");

                    cmd.Parameters.AddWithValue("@fsqCategoryId", id);
                    cmd.CommandText = query.ToString();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (categoryModel == null)
                            {
                                categoryModel = MapCategory(reader);
                            }
                            if (reader["TouristSiteCategoryId"] != DBNull.Value)
                            {
                                ITouristSiteCategoryModel touristSiteCategory = MapTouristSiteCategory(reader);
                                categoryModel.SiteCategories.Add(touristSiteCategory);
                            }
                        }
                    }
                }
            }
            return categoryModel;
        }
        public async Task UpdateCategoryAsync(Guid id, ICategoryModel categoryData)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.Connection = con;

                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE \"Category\" SET ");

                    if (!string.IsNullOrEmpty(categoryData.Type))
                    {
                        query.Append("\"Type\" = @type, ");
                        cmd.Parameters.AddWithValue("@type", categoryData.Type);
                    }
                    if (!string.IsNullOrEmpty(categoryData.Icon))
                    {
                        query.Append("\"Icon\" = @icon, ");
                        cmd.Parameters.AddWithValue("@icon", categoryData.Icon);
                    }
                    if (categoryData.UpdatedBy != Guid.Empty)
                    {
                        query.Append("\"UpdatedBy\" = @updatedBy, ");
                        cmd.Parameters.AddWithValue("@updatedBy", categoryData.UpdatedBy);
                    }

                    query.Append("\"DateUpdated\" = @dateUpdated, ");
                    cmd.Parameters.AddWithValue("@dateUpdated", DateTime.Now);

                    categoryData.DateUpdated = DateTime.Now;

                    query.Length -= 2;
                    query.Append(" WHERE \"Id\" = @id");

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = query.ToString();

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteCategoryAsync(Guid id)
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
                        queryCategory.Append("UPDATE \"Category\" SET \"IsActive\" = false WHERE \"Id\" = @id");
                        cmd.CommandText = queryCategory.ToString();
                        await cmd.ExecuteNonQueryAsync();

                        StringBuilder queryTouristSiteCategory = new StringBuilder();
                        queryTouristSiteCategory.Append("UPDATE \"TouristSiteCategory\" SET \"IsActive\" = false WHERE \"CategoryId\" = @id");
                        cmd.CommandText = queryTouristSiteCategory.ToString();
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

        private ICategoryModel MapCategory(NpgsqlDataReader reader)
        {
            return new CategoryModel
            {
                Id = (Guid)reader["Id"],
                Type = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Type"])) ? Convert.ToString(reader["Type"]) : null,
                Icon = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Icon"])) ? Convert.ToString(reader["Icon"]) : null,
                DateCreated = (DateTime)reader["CategoryDateCreated"],
                DateUpdated = (DateTime)reader["CategoryDateUpdated"],
                CreatedBy = (Guid)reader["CategoryCreatedBy"],
                UpdatedBy = (Guid)reader["CategoryUpdatedBy"],
                IsActive = (bool)reader["CategoryIsActive"],
                Fsq_CategoryId = !string.IsNullOrWhiteSpace(Convert.ToString(reader["Fsq_CategoryId"])) ? Convert.ToString(reader["Fsq_CategoryId"]) : null,

                SiteCategories = Convert.IsDBNull(reader["TouristSiteCategoryId"]) ? null : new List<ITouristSiteCategoryModel>(),
            };
        }
        private ITouristSiteCategoryModel MapTouristSiteCategory(NpgsqlDataReader reader)
        {
            return new TouristSiteCategoryModel
            {
                Id = (Guid)reader["TouristSiteCategoryId"],
                CategoryId = (Guid)reader["CategoryId"],
                TouristSiteId = (Guid)reader["TouristSiteId"],
                DateCreated = (DateTime)reader["TouristSiteDateCreated"],
                DateUpdated = (DateTime)reader["TouristSiteDateUpdated"],
                CreatedBy = (Guid)reader["TouristSiteCreatedBy"],
                UpdatedBy = (Guid)reader["TouristSiteUpdatedBy"],
                IsActive = (bool)reader["TouristSiteIsActive"] };
        }

    }
}
