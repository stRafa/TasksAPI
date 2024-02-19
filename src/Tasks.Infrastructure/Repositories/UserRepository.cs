using System.Data.SqlClient;
using Tasks.Domain.Mission;
using Tasks.Domain.User;

namespace Tasks.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Create(User user)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "INSERT INTO Users (Id, Name, Email) VALUES (@Id, @Name, @Email)";
            var cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public async Task<User> GetById(Guid id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "SELECT Id, Name, Email FROM Users WHERE Id = @Id";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            connection.Open();
            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = (Guid)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Missions = GetUserMissions(id)
                };

                return user;
            }

            return null;
        }
    }

    public async Task<List<User>> GetAll()
    {
        var users = new List<User>();

        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "SELECT Id, Name, Email FROM Users";
            var cmd = new SqlCommand(query, connection);

            connection.Open();
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = (Guid)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Missions = GetUserMissions((Guid)reader["Id"])
                };

                users.Add(user);
            }
        }

        return users.ToList();
    }

    public void Update(User user)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "UPDATE Users SET Name = @Name, Email = @Email WHERE Id = @Id";
            var cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void Delete(Guid id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "DELETE FROM Users WHERE Id = @Id";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }

    private List<Mission> GetUserMissions(Guid userId)
    {
        var missions = new List<Mission>();

        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "SELECT Id, Title, Description, Position, Status FROM Missions WHERE UserId = @UserId ORDER BY Position ASC";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            connection.Open();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                missions.Add(new Mission
                {
                    Id = (Guid)reader["Id"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "",
                    Position = (short)reader["Position"],
                    Status = (EMissionStatus)(int)reader["Status"],
                    UserId = userId
                });
            }
        }

        return missions;
    }
}