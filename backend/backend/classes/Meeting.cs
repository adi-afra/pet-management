using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
namespace backend.classes
{
    //represents a scheduled meeting between a client and a pet
	public class Meeting
	{
        //meetings class attributes
        public int Id { get; private set;  }
		public DateTime Date { get; private set; }
        //using Pet class as a data type so we wouldnt need verification
        public Pet Pet { get; private set; } 
        public int PetId { get; private set; }  // EF Foreign Key
        public int UserId { get; private set; } //EF Foreign key 
    
        public MeetingType Type { get; private set; } //Meeting type

        
        
        protected Meeting() {} //Empty constructor for EF

		public Meeting(DateTime date, Pet pet, int userId, MeetingType type)
		{
            //checking if any of the attributes is null, then throw an error
            //setting up the attributes with values
            
            Date = date;
            Pet = pet ?? throw new ArgumentNullException(nameof(pet));
            UserId = userId;
            PetId = pet.Id;
            Type = type;
        }



        //setters
        protected void SetDate(DateTime date)
        {
            Date = date;
        }

        protected void SetPet(Pet pet)
        {
            if (pet == null)
            {
                throw new ArgumentNullException("pet argument can not be empty");
            }

            Pet = pet;
        }
        

	}
}
