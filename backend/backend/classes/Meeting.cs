using System;
using namespace backend.classes
{
	public class Meeting
	{
        //meetings class attributes
		public string Date { get; private set; }
        //using Pet class as a data type so we wouldnt need verification
        public Pet Pet { get; private set; }
        public string UserID { get; private set; }



		public Meeting(string date, Pet pet, string userID)
		{

            //verifications 
            if(pet == null)
            {
                throw new ArgumentNullException("pet argument can not be empty");
            }

            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new ArgumentNullException("user Id can not be empty");
            }

            if (string.IsNullOrWhiteSpace(date))
            {
                throw new ArgumentNullException("date can not be empty");
            }

            //setting up the attributes with values
            Date = date;
            Pet = pet;
            UserID = userID
		}



        //setters
        protected void setDate(string date)
        {
            if (string.IsNullOrWhiteSpace(userID))
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

        protected void setUserID(string id)
        {
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new ArgumentNullException("user Id can not be empty");
            }

            UserID = id;
        }



	}
}

