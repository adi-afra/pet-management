namespace backend.classes
{
    public abstract class Pet
    {
        //all the fields are public so the children classes can see them (C# is like this) with public getters
        //and private setters
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Kind { get; private set; }
        public string Breed { get; private set; }
        public List<Meeting> Meetings { get; private set; }

        //constructor with all the fields as parameters, and validation for age and null or empty strings
        public Pet(string id, string name, int age, string kind, string breed)
        {
            if (age < 0)
            {
                throw new ArgumentException("Age cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(kind))
            {
                throw new ArgumentException("Kind cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(breed))
            {
                throw new ArgumentException("Breed cannot be null or empty.");
            }

            Id = id;
            Name = name;
            Age = age;
            Kind = kind;
            Breed = breed;
            Meetings = new List<Meeting>();
        }

        //protected setters for the fields, with validation for age (doing the validation in the property setter
        //can sometimes be a bit messy in my view, so I prefer to have separate methods for setting the fields with validation) 
        protected void setAge(int age)
        {
            if (age < 0)
            {
                throw new ArgumentException("Age cannot be negative.");
            }
            Age = age;
        }

        protected void setName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.");
            }
            Name = name;
        }

        protected void setKind(string kind)
        {
            if (string.IsNullOrWhiteSpace(kind))
            {
                throw new ArgumentException("Kind cannot be null or empty.");
            }
            Kind = kind;
        }

        protected void setBreed(string breed)
        {
            if (string.IsNullOrWhiteSpace(breed))
            {
                throw new ArgumentException("Breed cannot be null or empty.");
            }
            Breed = breed;
        }

        protected void setId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be null or empty.");
            }
            Id = id;


        }

        //methods for adding and removing items from the list (no validation for now)
        protected void addMeeting(Meeting meeting)
        {
            Meetings.Add(meeting);
        }

        protected void removeMeeting(Meeting meeting)
        {
            Meetings.Remove(meeting);
        }
    }
}
