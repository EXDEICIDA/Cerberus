using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;

namespace Cerberus.MVVM.Model
{
    public class DatabaseModel
    {
        private readonly string _connectionString;

        public DatabaseModel(string databaseFilePath = @"C:\Users\DEICIDA\source\repos\Cerberus\MVVM\Model\AppServices.db")
        {
            string databasePath = Path.GetFullPath(databaseFilePath);

            _connectionString = $"Data Source={databasePath};Version=3;";

            // Ensure database initialization
            InitializeDatabase(databasePath);
        }

        private void InitializeDatabase(string databasePath)
        {
            if (!File.Exists(databasePath))
            {
                try
                {
                    SQLiteConnection.CreateFile(databasePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating database file: {ex.Message}");
                    throw; // Propagate the exception
                }
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    //  Shows table
                    string createShowsTableQuery = @"CREATE TABLE IF NOT EXISTS Shows (
                                        Id INTEGER PRIMARY KEY,
                                        Title TEXT NOT NULL,
                                        Poster TEXT,
                                        Plot TEXT,
                                        Genre TEXT,
                                        Decade INTEGER,
                                        ImdbRating REAL)";
                    using (var command = new SQLiteCommand(createShowsTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    //  LikedShows table
                    string createLikedShowsTableQuery = @"CREATE TABLE IF NOT EXISTS LikedShows (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Title TEXT NOT NULL,
                                        Poster TEXT,
                                        Plot TEXT,
                                        Genre TEXT,
                                        Decade TEXT,
                                        ImdbRating REAL)";
                    using (var command = new SQLiteCommand(createLikedShowsTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Create Movies table
                    string createMoviesTableQuery = @"CREATE TABLE IF NOT EXISTS Movies (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Title TEXT NOT NULL,
                                Poster TEXT,
                                Plot TEXT,
                                Genre TEXT,
                                ReleaseYear INTEGER,
                                ImdbRating REAL)";
                    using (var command = new SQLiteCommand(createMoviesTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing database: {ex.Message}");
                    throw; // Propagate the exception
                }

            }
        }


        //This is an Insertion method created for writing the selected movie to the database
        public void InsertMovie(Movies movie)
        {
            using(var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string insertQuery = "Insert Into Movies (Title, Poster, Plot, Genre, Decade, ImdbRating) VALUES (@Title, @Poster, @Plot, @Genre, @Decade, @ImdbRating)";
                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title",movie.Title);
                        command.Parameters.AddWithValue("@Poster", movie.Poster);
                        command.Parameters.AddWithValue("@Plot", movie.Plot);
                        command.Parameters.AddWithValue("@Genre", movie.Genre);
                        command.Parameters.AddWithValue("@Decade", movie.ReleaseYear);
                        command.Parameters.AddWithValue("@ImdbRating", movie.ImdbRating);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting movie into database: {ex.Message}");
                    throw;
                }

            }
        }

        public void InsertShow(Show show)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO Shows (Title, Poster, Plot, Genre, Decade, ImdbRating) VALUES (@Title, @Poster, @Plot, @Genre, @Decade, @ImdbRating)";
                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", show.Title);
                        command.Parameters.AddWithValue("@Poster", show.Poster);
                        command.Parameters.AddWithValue("@Plot", show.Plot);
                        command.Parameters.AddWithValue("@Genre", show.Genre);
                        command.Parameters.AddWithValue("@Decade", show.Decade);
                        command.Parameters.AddWithValue("@ImdbRating", show.ImdbRating);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting show into database: {ex.Message}");
                    throw; // Propagate the exception
                }
            }
        }

        public void InsertToLiked(Show show)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO LikedShows (Title, Poster, Plot, Genre, Decade, ImdbRating) VALUES (@Title, @Poster, @Plot, @Genre, @Decade, @ImdbRating)";
                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", show.Title);
                        command.Parameters.AddWithValue("@Poster", show.Poster);
                        command.Parameters.AddWithValue("@Plot", show.Plot);
                        command.Parameters.AddWithValue("@Genre", show.Genre);
                        command.Parameters.AddWithValue("@Decade", show.Decade);
                        command.Parameters.AddWithValue("@ImdbRating", show.ImdbRating);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting liked show into database: {ex.Message}");
                    throw; // Propagate the exception
                }
            }
        }


        // A method for insertion of liked movies into the database
        public void InsertLiked(Movies movie)
        {
            using ( var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO LikedShows (Title, Poster, Plot, Genre, Decade, ImdbRating) VALUES (@Title, @Poster, @Plot, @Genre, @Decade, @ImdbRating)";
                    using ( var command = new SQLiteCommand(insertQuery,connection))
                    {
                        command.Parameters.AddWithValue("@Title", movie.Title);
                        command.Parameters.AddWithValue("@Poster",movie.Poster);
                        command.Parameters.AddWithValue("@Plot",movie.Plot);
                        command.Parameters.AddWithValue("@Genre", movie.Genre);
                        command.Parameters.AddWithValue("@Decade", movie.ReleaseYear);
                        command.Parameters.AddWithValue("@ImdbRating", movie.ImdbRating);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                   Console.WriteLine($"Error inserting liked into movies into database: {ex.Message}");
                    throw;
                }
            }

        }

        public ObservableCollection<Movies> GetAllMovies()
        {
            var movies = new ObservableCollection<Movies>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Movies";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var movie = new Movies
                            {
                                Id = reader.GetInt32(0).ToString(),
                                Title = reader.GetString(1),
                                Poster = reader.GetString(2),
                                Plot = reader.GetString(3),
                                Genre = reader.GetString(4),
                                ReleaseDate = reader.IsDBNull(5) ? (DateTime?)null : ConvertYearToDateTime(reader.GetString(5)),
                                ImdbRating = reader.IsDBNull(6) ? 0 : reader.GetDouble(6)
                            };
                            movies.Add(movie);
                        }
                    }
                }
            }

            return movies;
        }



        private DateTime? ConvertYearToDateTime(string yearString)
        {
            if (int.TryParse(yearString, out int year))
            {
                return new DateTime(year, 1, 1);
            }
            return null;
        }



        public ObservableCollection<Show> GetAllShows()
        {
            var shows = new ObservableCollection<Show>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Shows";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            shows.Add(new Show
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Poster = reader.GetString(2),
                                Plot = reader.GetString(3),
                                Genre = reader.GetString(4),
                                Decade = reader.GetInt32(5),
                                ImdbRating = reader.GetDouble(6)
                            });
                        }
                    }
                }
            }
            return shows;
        }
    }

   
}
