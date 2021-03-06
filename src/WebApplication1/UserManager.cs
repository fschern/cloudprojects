﻿using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using WebApplication1.Models;
using System.Collections.Generic;

namespace WebApplication1
{
  public class UserManager : IUserManager
  {
    public void AddUser(User user)
    {
      using (DbConnection connection = GetConnection())
      {
        DbCommand cmd = new MySqlCommand("INSERT INTO User VALUES(@id, @name);", connection as MySqlConnection);

        cmd.Parameters.Add(new MySqlParameter("id", user.ID));
        cmd.Parameters.Add(new MySqlParameter("name", user.Name));
        cmd.ExecuteNonQuery();
      }
    }

    public IEnumerable<User> GetAllUsers()
    {
      IList<User> users = new List<User>();

      using (DbConnection connection = GetConnection())
      {
        DbCommand cmd = new MySqlCommand("SELECT * FROM User;", connection as MySqlConnection);
        DbDataReader reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
          while (reader.Read())
          {
            users.Add(new User()
            {
              ID = reader.GetInt32(0),
              Name = reader.GetString(1)
            });
          }
        }
      }

      return users;
    }

    public User GetUser(int id)
    {
      using (DbConnection connection = GetConnection())
      {
        DbCommand cmd = new MySqlCommand("SELECT * FROM User WHERE id=@id;", connection as MySqlConnection);

        cmd.Parameters.Add(new MySqlParameter("id", id));
        DbDataReader reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
          while (reader.Read())
          {
            return new User() { ID = reader.GetInt32(1), Name = reader[1].ToString() };
          }
        }
      }

      return null;
    }

    private void EnsureTableExists(DbConnection connection)
    {
      DbCommand cmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS User (id int, name varchar(255));");

      cmd.Connection = connection;
      cmd.ExecuteNonQuery();
    }

    private DbConnection GetConnection()
    {
      MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();      
      builder.UserID = "userVCB";
      builder.Password = "mMlxj4suB5Wgx1Jk";
      builder.Database = "sampledb";
      builder.SslMode = MySqlSslMode.None;
      builder.PersistSecurityInfo = true;
      builder.Port = uint.Parse(Environment.GetEnvironmentVariable("MARIADB_SERVICE_PORT"));
      builder.Server = Environment.GetEnvironmentVariable("MARIADB_SERVICE_HOST");

      MySqlConnection connection = new MySqlConnection(builder.GetConnectionString(true));
      connection.Open();
      return connection;
    }
  }
}
