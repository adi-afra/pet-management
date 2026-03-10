using System;
using System.Collections.Generic;


namespace backend.classes
{
    //abstract base class representing a pet in the shelter
    public abstract class Pet
    {
        //all the fields are public so the children classes can see them (C# is like this) with public getters

        //properties with private setters to protect data integrity
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Kind { get; private set; }
        public string Breed { get; private set; }

        //list of meetings associated with this pet
        public List<Meeting> Meetings { get; private set; } = new List<Meeting>();

        //constructor with all the fields as parameters, and validation for age and null or empty strings
        
        protected Pet() {} //Empty constructor for EF
        
        public Pet(string id, string name, int age, string kind, string breed)
        {
            Id = ValidateString(id, nameof(id));
            Name = ValidateString(name, nameof(name));
            Kind = ValidateString(kind, nameof(kind));
            Breed = ValidateString(breed, nameof(breed));

            if (age < 0)
                throw new ArgumentException("Age cannot be negative.");

            Age = age;
            Meetings = new List<Meeting>();
        }

        //validation helper
        protected string ValidateString(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty.");

            return value;
        }

        //protected setters for the fields, with validation for age (doing the validation in the property setter
        //can sometimes be a bit messy in my view, so I prefer to have separate methods for setting the fields with validation) 
        public void SetAge(int age)
        {
            if (age < 0)
            {
                throw new ArgumentException("Age cannot be negative.");
            }
            Age = age;
        }

        public void SetName(string name) => Name = ValidateString(name, nameof(name));
        public void SetKind(string kind) => Kind = ValidateString(kind, nameof(kind));
        public void SetBreed(string breed) => Breed = ValidateString(breed, nameof(breed));
        public void SetId(string id) => Id = ValidateString(id, nameof(id));

        //methods for adding and removing items from the list (no validation for now)
        public void AddMeeting(Meeting meeting)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));

            Meetings.Add(meeting);
        }

        //removes a meeting from the pet
        public void RemoveMeeting(Meeting meeting)
        {
            Meetings.Remove(meeting);
        }
    }
}
