using System;

namespace backend.classes
{

    //abstract base class representing any system user
    public abstract class User
    {
        //properties

        public int Id { get; private set;}
        public string Username { get; private set; }
        public string Password { get; private set; }


        //constructor with validation
        
        protected User() {} //empty constructor for EF
        
        protected User(string username, string password)
        {
            Id = GenerateId();  // assign ID here
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

        private static int _lastId = 0; // static counter for simplicity
        private static int GenerateId()
        {
            _lastId++;
            return _lastId;
        }
        
        //each derived class must define its role
        public abstract string getRole();

    }

}
