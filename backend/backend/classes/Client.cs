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
		
		public Client( string username, string password,string email)
			: base(username, password,email)
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

			Meeting meeting = new Meeting(date, pet, this.Id, MeetingType.Adoption);

			Meetings.Add(meeting);
			pet.AddMeeting(meeting);
		}

		public void CreateSurrenderMeeting(DateTime date, string name, int age, string breed, PetType petType)
		{
			Pet pet;

			if (petType == PetType.Dog)
			{
				pet = new Dog(name, age,breed);
			}
			else if (petType == PetType.Cat)
			{
				pet = new Cat(name, age,breed);
			}
			else
			{
				throw new ArgumentException("Invalid pet type");
			}

			Meeting meeting = new Meeting(date, pet, this.Id, MeetingType.Surrender);

			pet.SetStatus(PetStatus.Potential);
			
			Meetings.Add(meeting);
			pet.AddMeeting(meeting);
		}
	}
}
