using System;

namespace backend.classes
{
    //represents a scheduled meeting between a client and a pet
	public class Meeting
	{
        //meetings class attributes
		public string Date { get; private set; }
        //using Pet class as a data type so we wouldnt need verification
        public Pet Pet { get; private set; }
        public string UserID { get; private set; }



		public Meeting(string date, Pet pet, string userId)
		{
            //checking if any of the attributes is null, then throw an error
            //setting up the attributes with values
            Date = date ?? throw new ArgumentNullException(nameof(date),"date can not be empty");;
            Pet = pet ?? throw new ArgumentNullException(nameof(pet),"pet argument can not be empty");
            UserID = userId ?? throw new ArgumentNullException(nameof(userId),"user Id can not be empty");
        }



        //setters
        protected void setDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                throw new ArgumentNullException("user Id can not be empty");
            }

            Date = date;
        }

        protected void setPet(Pet pet)
        {
            if (pet == null)
            {
                throw new ArgumentNullException("pet argument can not be empty");
            }

            Pet = pet;
        }

        protected void setUserId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("user Id can not be empty");
            }

            UserID = id;
        }



	}
}

