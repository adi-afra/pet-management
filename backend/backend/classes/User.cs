using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.classes
{

    //abstract base class representing any system user
    public abstract class User
    {
        //properties

        public int Id { get; private set;}
        public string Username { get; private set; } = string.Empty;

        [JsonIgnore]
        public string Password { get; private set; } = string.Empty;


        //constructor with validation

        protected User() {} //empty constructor for EF
        
        protected User(string username, string password)
        {
            Username = ValidateString(username, nameof(username));
            Password = ValidateString(password, nameof(password));
        }

        //update username with validation
        public void setUsername(string username)
        {
            Username = ValidateString(username, nameof(username));
        }

        //update password with validation
        public void setPassword(string password)
        {
            Password = ValidateString(password, nameof(password));
        }

        //helper method to avoid repeating validation logic
        protected string ValidateString(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty.");

            return value;
        }

        
        //each derived class must define its role
        public abstract string getRole();

    }

}
