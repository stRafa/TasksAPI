using System.Data.SqlClient;
using System.Security.Claims;
using Tasks.Domain.Passport;

namespace Tasks.Infrastructure.Repositories;

public class PassportRepository : IPassportRepository
{
    private readonly string _connectionString;

    public PassportRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Passport> GetPassportByUsername(string username)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            const string query = @"SELECT U.Id, U.Name, U.Email AS Username, U.PasswordHash, U.PasswordSalt, U.Status, U.CreatedAt, C.Id AS C.Type, C.Value, R.Id AS RoleId, R.Name AS RoleName
                      FROM Users U 
                      INNER JOIN Claims C ON U.Id = C.UserId 
                      INNER JOIN UserRoles UR ON U.Id = UR.UserId
                      INNER JOIN Roles R ON UR.RoleId = R.Id 
                      WHERE U.Email = @Email";

            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", username);

            connection.Open();
            var reader = await cmd.ExecuteReaderAsync();

            Passport passport = null;

            while (await reader.ReadAsync())
            {
                passport ??= new Passport
                {
                    Id = (Guid)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Username = reader["Username"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString(),
                    PasswordSalt = reader["PasswordSalt"].ToString(),
                    Status = (bool)reader["Status"],
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    Claims = [],
                    Roles = []
                };

                passport.Claims.Add(new Claim(reader["Type"].ToString(), reader["Value"].ToString()));

                passport.Roles.Add(new Role
                {
                    Id = (Guid)reader["RoleId"],
                    Name = reader["RoleName"].ToString()
                });
            }

            return passport;
        }
    }


    public void CreatePassport(Passport passport)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var transaction = connection.BeginTransaction();

            try
            {
                string userQuery = @"INSERT INTO Users (Id, Name, Email, PasswordHash, PasswordSalt, Status, CreatedAt) 
                                 VALUES (@Id, @Name, @Email, @PasswordHash, @PasswordSalt, @Status, @CreatedAt)";

                var userCmd = new SqlCommand(userQuery, connection, transaction);
                userCmd.Parameters.AddWithValue("@Id", passport.Id);
                userCmd.Parameters.AddWithValue("@Name", passport.Name);
                userCmd.Parameters.AddWithValue("@Email", passport.Username);
                userCmd.Parameters.AddWithValue("@PasswordHash", passport.PasswordHash);
                userCmd.Parameters.AddWithValue("@PasswordSalt", passport.PasswordSalt);
                userCmd.Parameters.AddWithValue("@Status", passport.Status);
                userCmd.Parameters.AddWithValue("@CreatedAt", passport.CreatedAt);

                userCmd.ExecuteNonQuery();

                foreach (var claim in passport.Claims)
                {
                    string claimQuery = @"INSERT INTO Claims (UserId, Type, Value) 
                                      VALUES (@UserId, @Type, @Value)";

                    var claimCmd = new SqlCommand(claimQuery, connection, transaction);
                    claimCmd.Parameters.AddWithValue("@UserId", passport.Id);
                    claimCmd.Parameters.AddWithValue("@Type", claim.Type);
                    claimCmd.Parameters.AddWithValue("@Value", claim.Value);

                    claimCmd.ExecuteNonQuery();
                }

                foreach (var role in passport.Roles)
                {
                    string userRoleQuery = @"INSERT INTO UserRoles (UserId, RoleId) 
                                         VALUES (@UserId, @RoleId)";

                    var userRoleCmd = new SqlCommand(userRoleQuery, connection, transaction);
                    userRoleCmd.Parameters.AddWithValue("@UserId", passport.Id);
                    userRoleCmd.Parameters.AddWithValue("@RoleId", role.Id);

                    userRoleCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error creating passport", ex);
            }
        }
    }
    
    public void AddRoleToUser(Guid userId, string roleName)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var transaction = connection.BeginTransaction();

            try
            {
                string roleIdQuery = @"SELECT Id FROM Roles WHERE Name = @Name";
                var roleIdCmd = new SqlCommand(roleIdQuery, connection, transaction);
                roleIdCmd.Parameters.AddWithValue("@Name", roleName);

                Guid roleId = (Guid)roleIdCmd.ExecuteScalar();

                string userRoleQuery = @"INSERT INTO UserRoles (UserId, RoleId) 
                                     VALUES (@UserId, @RoleId)";
            
                var userRoleCmd = new SqlCommand(userRoleQuery, connection, transaction);
                userRoleCmd.Parameters.AddWithValue("@UserId", userId);
                userRoleCmd.Parameters.AddWithValue("@RoleId", roleId);

                userRoleCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error adding role to user", ex);
            }
        }
    }

    
    public void RemoveUserClaim(Guid userId, string claimType, string claimValue)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
                const string claimDeleteQuery = @"DELETE FROM Claims 
                                        WHERE UserId = @UserId AND Type = @Type AND Value = @Value";
            
                var claimDeleteCmd = new SqlCommand(claimDeleteQuery, connection);
                claimDeleteCmd.Parameters.AddWithValue("@UserId", userId);
                claimDeleteCmd.Parameters.AddWithValue("@Type", claimType);
                claimDeleteCmd.Parameters.AddWithValue("@Value", claimValue);

                connection.Open();
                claimDeleteCmd.ExecuteNonQuery();
        }
    }
    
    public void RemoveUserRole(Guid userId, string roleName)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
                const string roleIdQuery = @"SELECT Id FROM Roles WHERE Name = @Name";
                var roleIdCmd = new SqlCommand(roleIdQuery, connection);
                roleIdCmd.Parameters.AddWithValue("@Name", roleName);

                connection.Open();
                Guid roleId = (Guid)roleIdCmd.ExecuteScalar();
                
                string userRoleDeleteQuery = @"DELETE FROM UserRoles 
                                           WHERE UserId = @UserId AND RoleId = @RoleId";
            
                var userRoleDeleteCmd = new SqlCommand(userRoleDeleteQuery, connection);
                userRoleDeleteCmd.Parameters.AddWithValue("@UserId", userId);
                userRoleDeleteCmd.Parameters.AddWithValue("@RoleId", roleId);

                userRoleDeleteCmd.ExecuteNonQuery();
        }
    }

}