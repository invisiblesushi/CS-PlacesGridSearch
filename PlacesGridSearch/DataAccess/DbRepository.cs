using System.Data;
using System.Data.SqlClient;
using PlacesGridSearch.Model;

namespace PlacesGridSearch.DataAccess;

public class DbRepository
{
    private readonly string _connectionString;

    public DbRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    // Insert a new restaurant record.
    public void InsertRestaurant(List<Place> places)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
            
        var sqlStrExist = "SELECT COUNT(1) FROM GooglePlaces WHERE place_id = @place_id";

        var sqlStr = @"INSERT INTO GooglePlaces (place_id, name, rating, user_ratings, date_added) 
                                    VALUES (@place_id, @name, @rating, @user_ratings, @date_added)";

        places.ForEach(place =>
        {
            using SqlCommand checkCommand = new SqlCommand(sqlStrExist, connection);
            checkCommand.Parameters.AddWithValue("@place_id", place.place_id);
            var existingRecords = (int)checkCommand.ExecuteScalar();

            if (existingRecords != 0)
                return;

            using SqlCommand command = new SqlCommand(sqlStr, connection);
            command.Parameters.AddWithValue("@place_id", place.place_id);
            command.Parameters.AddWithValue("@name", place.name);
            command.Parameters.AddWithValue("@rating", place.rating);
            command.Parameters.AddWithValue("@user_ratings", place.user_ratings_total);
            command.Parameters.AddWithValue("@date_added", DateTime.Now);
        
            command.ExecuteNonQuery();
        });
    }
}