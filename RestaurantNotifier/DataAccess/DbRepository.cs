using RestaurantNotifier.Model;
using System.Data;
using System.Data.SqlClient;

namespace RestaurantNotifier.DataAccess;

public class DbRepository
{
    private readonly string _connectionString;

    public DbRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    // Insert a new restaurant record
    public void InsertRestaurant(List<Place> places)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = "INSERT INTO GooglePlaces (place_id, name, rating, user_ratings, date_added) " +
                           "VALUES (@place_id, @name, @rating, @user_ratings, @date_added)";

            places.ForEach(place =>
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@place_id", place.place_id);
                    command.Parameters.AddWithValue("@name", place.name);
                    command.Parameters.AddWithValue("@rating", place.rating);
                    command.Parameters.AddWithValue("@user_ratings", place.user_ratings_total);
                    command.Parameters.AddWithValue("@date_added", DateTime.Now);
    
                    command.ExecuteNonQuery();
                }
            });
        }
    }
    
    
}