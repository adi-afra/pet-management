using System;

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
        public string PetId { get; private set; }  // EF Foreign Key
        public string UserId { get; private set; }


        protected Meeting() {} //Empty constructor for EF

		public Meeting(DateTime date, Pet pet, string userId)
		{
            //checking if any of the attributes is null, then throw an error
            //setting up the attributes with values
            Date = date;
            Pet = pet ?? throw new ArgumentNullException(nameof(pet),"pet argument can not be empty");
            UserId = userId ?? throw new ArgumentNullException(nameof(userId),"user Id can not be empty");
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

        protected void SetUserId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("user Id can not be empty");
            }

            UserId = id;
        }



	}
}
