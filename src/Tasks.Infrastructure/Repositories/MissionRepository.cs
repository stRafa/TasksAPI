using System.Data.SqlClient;
using Tasks.Domain.Mission;

namespace Tasks.Infrastructure.Repositories;

public class MissionRepository : IMissionRepository
{
    private readonly string _connectionString;

    public MissionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Mission>> GetAll()
    {
        var missions = new List<Mission>();

        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "SELECT Id, Title, Description, Position, Status, UserId FROM Missions";
            var cmd = new SqlCommand(query, connection);

            connection.Open();
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                missions.Add(new Mission
                {
                    Id = (Guid)reader["Id"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString(),
                    Position = (short)reader["Position"],
                    Status = (EMissionStatus)(int)reader["Status"],
                    UserId = (Guid)reader["UserId"]
                });
            }
        }

        return missions;
    }

    public async Task<Mission> GetById(Guid id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "SELECT Id, Title, Description, Position, Status, UserId FROM Missions WHERE Id = @Id";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            connection.Open();
            var reader = await cmd.ExecuteReaderAsync();

            await reader.ReadAsync();

            return new Mission
            {
                Id = (Guid)reader["Id"],
                Title = reader["Title"].ToString(),
                Description = reader["Description"].ToString(),
                Position = (short)reader["Position"],
                Status = (EMissionStatus)(int)reader["Status"],
                UserId = (Guid)reader["UserId"]
            };
        }
    }

    public async Task<List<Mission>> GetByUserId(Guid userId)
    {
        var missions = new List<Mission>();

        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "SELECT Id, Title, Description, Position, Status, UserId FROM Missions WHERE UserId = @UserId ORDER BY Position ASC";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            connection.Open();

            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                missions.Add(new Mission
                {
                    Id = (Guid)reader["Id"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString(),
                    Position = (short)reader["Position"],
                    Status = (EMissionStatus)(int)reader["Status"],
                    UserId = (Guid)reader["UserId"]
                });
            }
        }
        return missions;
    }

    
    public void Create(Mission mission)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "INSERT INTO Missions (Id, Title, Description, Position, Status, UserId) " +
                        "VALUES (@Id, @Title, @Description, @Position, @Status, @UserId)";
            var cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Id", mission.Id);
            cmd.Parameters.AddWithValue("@Title", mission.Title);
            cmd.Parameters.AddWithValue("@Description", mission.Description);
            cmd.Parameters.AddWithValue("@Position", mission.Position);
            cmd.Parameters.AddWithValue("@Status", (int)mission.Status);
            cmd.Parameters.AddWithValue("@UserId", mission.UserId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void Update(Mission mission)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "UPDATE Missions SET Title = @Title, Description = @Description, " +
                        "Position = @Position, Status = @Status, UserId = @UserId WHERE Id = @Id";
            var cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Id", mission.Id);
            cmd.Parameters.AddWithValue("@Title", mission.Title);
            cmd.Parameters.AddWithValue("@Description", mission.Description);
            cmd.Parameters.AddWithValue("@Position", mission.Position);
            cmd.Parameters.AddWithValue("@Status", (int)mission.Status);
            cmd.Parameters.AddWithValue("@UserId", mission.UserId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
    
    public void UpdateMany(List<Mission> missions)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var mission in missions)
                    {
                        var query = "UPDATE Missions SET Title = @Title, Description = @Description, " +
                                    "Position = @Position, Status = @Status, UserId = @UserId WHERE Id = @Id";
                        var cmd = new SqlCommand(query, connection, transaction);

                        cmd.Parameters.AddWithValue("@Id", mission.Id);
                        cmd.Parameters.AddWithValue("@Title", mission.Title);
                        cmd.Parameters.AddWithValue("@Description", mission.Description);
                        cmd.Parameters.AddWithValue("@Position", mission.Position);
                        cmd.Parameters.AddWithValue("@Status", (int)mission.Status);
                        cmd.Parameters.AddWithValue("@UserId", mission.UserId);

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; 
                }
            }
        }
    }

    public void Delete(Guid id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "DELETE FROM Missions WHERE Id = @Id";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }

    
}