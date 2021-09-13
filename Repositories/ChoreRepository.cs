using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    /// <summary>
    /// This class is responsible for interacting with Chore data.
    /// It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    class ChoreRepository : BaseRepository
    {
        /// <summary>
        /// When new RoomRepository is instantiated, pass the connection string along to the Base Repository
        /// </summary>
        public ChoreRepository(string connectionString) : base(connectionString) { }

        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the rooms we retrieve from the database
                    List<Chore> chores = new List<Chore>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue,
                        };

                        // then we add the chore object to our list
                        chores.Add(chore);
                    }

                    // we then close() the reader.
                    reader.Close();

                    // Return the list of chore to whomever called this method.
                    return chores;
                }
            }
        }
        /// <summary>
        /// Returns a single room with the given id.
        /// </summary>
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id ";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Chore chore = null;

                    if (reader.Read())
                    {
                        chore = new Chore
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                    }
                    reader.Close();
                    return chore;
                }
            }
        }
        /// <summary>
        /// Add a new chore to the database
        /// NOTE: This method sends data to the database,
        /// it does not get anything from the database, so there is nothing to return
        /// </summary>
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                                OUTPUT INSERTED.Id
                                                VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }
            // When this method is finished we can look in the database and see the newly added room.
        }
    }
}
