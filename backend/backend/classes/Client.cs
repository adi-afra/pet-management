using System;
using System.Collections.Generic;

namespace backend.classes
{
    public class Client : User
    {
        // Initialize list 
        public List<Meeting> Meetings { get; private set; } = new List<Meeting>();

        // Empty constructor for EF
        public Client() : base() { }

        public Client(string username, string password) : base(username, password)
        {
        }

        public override string getRole() => "Client";

        public void CreateMeeting(DateTime date, Pet pet)
        {
            if (pet == null) throw new ArgumentNullException(nameof(pet));

            Meeting meeting = new Meeting(date, pet, this.Id);
            Meetings.Add(meeting);
            pet.AddMeeting(meeting);
        }
    }
}