using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class DTOIP2Location {
   public long Id {  get; set; }
 public   long IpFromBigInt { get; set; }
   public long IpToBigInt { get; set; }

    public  string CountryCode {  get; set; } 
    public string CountryName { get; set; }

    public DTOIP2Location()
    {
        this.CountryCode = "";
        this.CountryName = "";
    }

    public DTOIP2Location(long id,long ip_from,long ip_to,string CountryCode,string CoutryName)
    {
     this.Id = id;
        this.IpFromBigInt = ip_from;
        this.IpToBigInt = ip_to;
        this.CountryCode = CountryCode;
        this.CountryName = CoutryName;

    }
}

namespace ConnectionLayer
{
    public static class clsIP2Location
    {
        //GetLocaionByIP2LocationID

        public static async Task<DTOIP2Location?> GetIp2LocationById(long id)
        {
            try
            {

                // Using statement ensures proper disposal of resources
                using (var connection = new NpgsqlConnection(ConnectionLayer.clsConnectionGenral.ConnectionString))
                {
                    await connection.OpenAsync();

                    const string query = @"
            SELECT * FROM ip2location 
            WHERE id = @id
            LIMIT @limit";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@limit", 1);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if (reader["country_code"] != null &&
                                    reader["country_name"] != null &&
                                    long.TryParse(reader["id"]?.ToString(), out long recordId) &&
                                    long.TryParse(reader["ip_from"]?.ToString(), out long ip_from) &&
                                    long.TryParse(reader["ip_to"]?.ToString(), out long ip_to))
                                {
                                    return new DTOIP2Location
                                    {
                                        Id = recordId,
                                        IpFromBigInt = ip_from,
                                        IpToBigInt = ip_to,
                                        CountryCode = reader.GetString(reader.GetOrdinal("country_code")),
                                        CountryName = reader.GetString(reader.GetOrdinal("country_name")),
                                        // Map other columns as needed
                                    };
                                }
                            }
                        }
                    }
                }

                return null;  // Return null if no record with the specified ID is found
            }
catch(Exception ex)
            {
                return null;
            }
        }

        //GetIP2LocationUsingConditionAndMaxMinConstraint
        public static async Task<DTOIP2Location?> GetIp2Location(long ipAddress)
        {

            try
            {
                // Using statement ensures proper disposal of resources
                using (var connection = new NpgsqlConnection(ConnectionLayer.clsConnectionGenral.ConnectionString))
                {
                    await connection.OpenAsync();

                    const string query = @"
            SELECT * FROM ip2location 
            WHERE ip_from <= @ipAddress AND ip_to >= @ipAddress
            LIMIT @limit";  // Assuming we want just one matching record

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ipAddress", ipAddress);
                        command.Parameters.AddWithValue("@limit", 1);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if (reader["country_code"] != null &&
                                    reader["country_name"] != null &&
                                    long.TryParse(reader["id"]?.ToString(), out long Id) &&
                                    long.TryParse(reader["ip_from"]?.ToString(), out long ip_from) &&
                                    long.TryParse(reader["ip_to"]?.ToString(), out long ip_to))
                                {
                                    return new DTOIP2Location
                                    {
                                        Id = Id,
                                        IpFromBigInt = ip_from,
                                        IpToBigInt = ip_to,
                                        CountryCode = reader.GetString(reader.GetOrdinal("country_code")),
                                        CountryName = reader.GetString(reader.GetOrdinal("country_name")),
                                    };
                                }
                            }
                        }
                    }
                }

                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        //GetAllIPAddresses 
        public static async Task<List<DTOIP2Location>?> GetAllIp2Location()
        {
            try
            {
                var records = new List<DTOIP2Location>();

                // Using statement ensures proper disposal of resources
                using (var connection = new NpgsqlConnection(ConnectionLayer.clsConnectionGenral.ConnectionString))
                {

                    const string query = "SELECT * FROM ip2location";
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                if (reader["country_code"] != null && reader["country_name"] != null && long.TryParse(reader["id"]?.ToString(), out long Id) && long.TryParse(reader["ip_from"]?.ToString(), out long ip_from) && long.TryParse(reader["ip_to"]?.ToString(), out long ip_to))
                                {
                                    DTOIP2Location record = new DTOIP2Location
                                    {
                                        // Map the columns from the reader to your object properties
                                        // Adjust according to your actual table structure
                                        Id = Id,
                                        IpFromBigInt = ip_from,
                                        IpToBigInt = ip_to,
                                        CountryCode = reader.GetString(reader.GetOrdinal("country_code")),
                                        CountryName = reader.GetString(reader.GetOrdinal("country_name")),
                                        // Map other columns as needed
                                    };

                                    records.Add(record);
                                }


                            }
                        }
                    }
                }

                return records;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        //DeleteIPAddressLocation
        public static async Task<bool> DeleteIp2Location (long id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionLayer.clsConnectionGenral.ConnectionString))
                {
                    await connection.OpenAsync();

                    const string query = @"
            DELETE FROM ip2location 
            WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }

            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> DeleteIp2Location(string countryCode)
        {

            try
            {
                using (var connection = new NpgsqlConnection(ConnectionLayer.clsConnectionGenral.ConnectionString))
                {
                    await connection.OpenAsync();

                    const string query = @"
            DELETE FROM ip2location 
            WHERE country_code = @countryCode";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@countryCode", countryCode);

                        return (await command.ExecuteNonQueryAsync())!=0;
                    }
                }


            }
            catch (Exception ex) {

                return false;
                    }
        }
        //UpdateIPAddressLocation
        public static async Task<bool> UpdateIp2Location (DTOIP2Location updatedRecord)
        {

            try
            {
                using (var connection = new NpgsqlConnection(ConnectionLayer.clsConnectionGenral.ConnectionString))
                {
                    await connection.OpenAsync();

                    const string query = @"
            UPDATE ip2location 
            SET 
                ip_from = @ipFrom, 
                ip_to = @ipTo, 
                country_code = @countryCode, 
                country_name = @countryName
             WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", updatedRecord.Id);
                        command.Parameters.AddWithValue("@ipFrom", updatedRecord.IpFromBigInt);
                        command.Parameters.AddWithValue("@ipTo", updatedRecord.IpToBigInt);
                        command.Parameters.AddWithValue("@countryCode", updatedRecord.CountryCode);
                        command.Parameters.AddWithValue("@countryName", updatedRecord.CountryName);
                        // Add parameters for other properties

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected == 1;
                    }
                }

            }
            catch (Exception ex) {

                return false;
            }
        }


        //Add
        public static async Task<long> AddIp2LocationAsync(DTOIP2Location newRecord)
        {
            try
            {

                using (var connection = new NpgsqlConnection(ConnectionLayer.clsConnectionGenral.ConnectionString))
                {
                    await connection.OpenAsync();

                    // Using RETURNING clause to get the generated ID
                    const string query = @"
            INSERT INTO ip2location (
                ip_from, 
                ip_to, 
                country_code, 
                country_name
                -- Add other columns as needed
            )
            VALUES (
                @ipFrom, 
                @ipTo, 
                @countryCode, 
                @countryName
                -- Add other values as needed
            )
            RETURNING id;";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ipFrom", newRecord.IpFromBigInt);
                        command.Parameters.AddWithValue("@ipTo", newRecord.IpToBigInt);
                        command.Parameters.AddWithValue("@countryCode", newRecord.CountryCode);
                        command.Parameters.AddWithValue("@countryName", newRecord.CountryName);
                        // Add parameters for other properties

                        // Execute and return the generated ID
                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt64(result);
                    }
                }


            }
            catch(Exception ex) {
            
               return -1;

            }
        }


    }
}
