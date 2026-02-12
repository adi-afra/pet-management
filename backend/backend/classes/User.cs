using System;

namespace backend.classes;

public abstract class User
{
    //attributes
    public string Id { get; }
    public string Username { get; private set; }
    public string Password { get; private set; }


    //constructor with validation
    public User(string id, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentNullException(nameof(id),"The id cannot be null");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username),"username cannot be empty");
        }
        
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password),"password cannot be empty");
        }

        Id = id;
        Username = username;
        Password = password;
    }

    //setters for username and password
    public void setUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username),"username cannot be empty");
        }

        Username = username;
    }

    public void setPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password),"password cannot be empty");
        }

        Password = password;
    }

    
    public abstract string getRole();

}