using System;
using System.Collections.Generic;

namespace backend.classes
{
	//represents a client who can book meetings with pets
	public class Client : User
	{
		// List field to store client's meetings
		public List<Meeting> Meetings { get; private set; }

		// Constructor
		
		protected Client() {} //empty constructor for EF
		
		public Client( string username, string password)
			: base(username, password)
		{
			Meetings = new List<Meeting>();
		}

		// Override getRole to return client
		public override string getRole()
		{
			return "Client";
		}

		// Method to create a meeting object with the current id and adds it to the client's and
		//pet's meeting lists
		public void CreateMeeting(DateTime date, Pet pet)
		{
			if (pet == null)
			{
				throw new ArgumentNullException(nameof(pet), "Pet cannot be empty");
			}

			Meeting meeting = new Meeting(date, pet, this.Id);

			Meetings.Add(meeting);
			pet.AddMeeting(meeting);
		}
	}
}
