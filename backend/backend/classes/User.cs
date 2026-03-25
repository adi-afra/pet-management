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
        
        public string Email { get; private set; }

        //constructor with validation
        
        protected User() {} //empty constructor for EF
        
        protected User(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }

        //setter for username
        public void setUsername(string username)
        {
            Username = username;
        }

        //setter for password
        public void setPassword(string password)
        {
            Password = password;
        }
        
        //setter for email
        public void SetEmail(string email)
        {
            Email = email;
        }

        


        
        //each derived class must define its role
        public abstract string getRole();

    }

}
