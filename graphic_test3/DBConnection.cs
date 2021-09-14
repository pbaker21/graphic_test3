using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;


/*
 
*/



class DBConnection
{
    private MySqlConnection connection;

    LogClass mylogs = new LogClass();




    //Constructor
    public DBConnection(string server, string port, string database, string user, string password)
    {
        string connectionString = @"SERVER=" + server + ";" + "PORT=" + port + ";" + "DATABASE=" + database + ";" + "UID=" + user + ";" + "PASSWORD=" + password + ";SslMode=none;";

        connection = new MySqlConnection(connectionString);
    }

    



    public string test_local_connection()
    {
        string result = "OK";

        try
        {
            connection.Open();
            
            connection.Close();
        }
        catch (MySqlException ex)
        {
            // Console.WriteLine("SQL No. " + ex.Number + " = " + ex.Message);

            mylogs.Logs("SQL Local Connection Error: ", ex.Number + " = " + ex.Message);

            return ex.Message;
        }

        return result;
    }







    private bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (MySqlException ex)
        {
            //When handling errors, your application's response based on the error number.
            //
            //The two most common error numbers when connecting are as follows:
            //   0: Cannot connect to server.
            //1045: Invalid user name and/or password.

            Console.WriteLine("SQL No. " + ex.Number);

            switch (ex.Number)
            {
                case 0:
                    Console.WriteLine("SQL Connection : Cannot connect to server.");
                    break;

                case 1045:
                    Console.WriteLine("SQL Connection : Invalid username/password, please try again");
                    break;
            }
            return false;
        }
    }






    public int GetTotalPhotos() // get the total number of records in the `incoming_photos` table
    {
        int totals = 0;

        //Open connection
        if (this.OpenConnection() == true)
        {
            //string query = "SELECT count(*) as total FROM `incoming_photos` WHERE DATE(stamp) = DATE(NOW())";
            string query = "SELECT count(*) as total FROM `incoming_photos` WHERE DATE(stamp) = DATE_ADD(CURDATE(), INTERVAL -5 DAY);";

            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);

            //Create a data reader and Execute the command
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    totals = Int32.Parse(reader.GetString(0));
                }
            }

            //close Data Reader
            reader.Close();
        }

        //close Connection
        this.CloseConnection();

        return totals;
    }


       

    #region GetPaginatedValues
    /// <summary>
    /// </summary>
    /// <param name="current_page"></param>
    /// <param name="items_per_page"></param>
    /// <returns></returns>
    /// 
    public int[] GetPaginatedValues(int current_page = 1, int items_per_page = 12)
    {
        int[] pagin = new int[3];

        int total_items = GetTotalPhotos();
        
        int num_pages = (total_items / items_per_page);
        // mylogs.Logs("SQL GetPaginatedValues", num_pages.ToString() + ", " + total_items);

        current_page = (current_page > 0) ? current_page : 1;

        int page = (current_page <= num_pages) ? ((current_page - 1) * items_per_page) : ((num_pages - 1) * items_per_page);


        pagin[0] = page;
        pagin[1] = items_per_page;
        pagin[2] = num_pages;

        // LIMIT page, items_per_page

        return pagin;
    }

    #endregion 




    public List<DataInserts> LoadPhotosToList(int page, int per_page, string fromdate, string todate)    // create or update todays new list of photos
    {
        List<DataInserts> data = new List<DataInserts>();


        if (this.OpenConnection() == true)
        {
            var myquery = "SELECT incoming_photos.pk_id, incoming_photos.photocode, incoming_photos.tag_id, incoming_photos.tag_location, incoming_photos.stamp FROM `incoming_photos`  WHERE DATE(incoming_photos.stamp) BETWEEN '"+fromdate+"' AND '"+todate+"'   GROUP by incoming_photos.photocode ORDER BY incoming_photos.max_date DESC, incoming_photos.photocode DESC LIMIT " + page + "," + per_page;

             mylogs.Logs("SQL LoadPhotoToList", myquery);

                using (MySqlCommand cmd = new MySqlCommand(myquery, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.Add(new DataInserts
                                {
                                    pk_id        = reader["pk_id"].ToString(),
                                    photocode    = reader["photocode"].ToString(),
                                    tag_id       = reader["tag_id"].ToString(),
                                    tag_location = reader["tag_location"].ToString(),
                                    datestamp    = reader["stamp"].ToString()
                                });
                        
                            }//while
                        }// if
                    }// using
                }// using                
            }//if
            
            this.CloseConnection();

            return data;
        }

  







    //Close connection
    private bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }




}
