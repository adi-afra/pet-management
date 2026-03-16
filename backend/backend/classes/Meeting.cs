using System;
using System.ComponentModel.DataAnnotations;
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
        public int UserId { get; private set; }


        protected Meeting() {} //Empty constructor for EF

		public Meeting(DateTime date, Pet pet, int userId)
		{
            //checking if any of the attributes is null, then throw an error
            //setting up the attributes with values

            Id = GenerateId();
            Date = date;
            Pet = pet ?? throw new ArgumentNullException(nameof(pet));
            UserId = userId;
            PetId = pet.Id;
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

        private static int _lastId = 0; // static counter for simplicity
        private static int GenerateId()
        {
            _lastId++;
            return _lastId;
        }

	}
}
