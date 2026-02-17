using System;

namespace backend.classes
{
    public class Admin : User
    {
        //constructor for Admin which adds a new field for their role
        public Admin(string id, string username, string password) : 
            base(id, username, password)
        {
        }

        //overriding the getRole method to return the role of the user
        public override string getRole() { return "Admin"; }

        //methods to change different fields of an instance of a Pet

        public void chnageId(Pet pet, String newId) {pet.setId(newId);}

        public void changeName(Pet pet, String newName) { pet.setName(newName); }
        public void changeAge(Pet pet, int newAge) { pet.setAge(newAge); }
        public void changeKind(Pet pet, String newKind) { pet.setKind(newKind); }
        public void changeBreed(Pet pet, String newBreed){ pet.setBreed(newBreed); }

        
        // Approve an adoption application (Brandon added)
        public void ApproveApplication(AdoptionApplication application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            application.Approve();
        }

        // Reject an adoption application
        public void RejectApplication(AdoptionApplication application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            application.Reject();
        }

        // Mark a pet as adopted
        public void MarkPetAsAdopted(Pet pet)
        {
            if (pet == null)
                throw new ArgumentNullException(nameof(pet));

            pet.MarkAdopted();
        }

        // Cancel a meeting booked by a client
        public void CancelMeeting(Meeting meeting, Client client)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            client.Meetings.Remove(meeting);
            meeting.Pet.removeMeeting(meeting);
        }
    }
}
