using System;

namespace backend.classes
{
    //represents an administrator with permission to modify pets
    public class Admin : User
    {
        
        protected Admin() {} //Empty constructor for EF
        
        //constructor for Admin which adds a new field for their role
        public Admin(string id, string username, string password) : 
            base(id, username, password)
        {
        }

        //overriding the getRole method to return the role of the user
        public override string getRole()
        {
            return "Admin";
        }

        //methods to change different fields of an instance of a Pet

        public void ChnageId(Pet pet, string newId) { pet.SetId(newId); }

        public void ChangeName(Pet pet, string newName) { pet.SetName(newName); }
        public void ChangeAge(Pet pet, int newAge) { pet.SetAge(newAge); }
        public void ChangeKind(Pet pet, string newKind) { pet.SetKind(newKind); }
        public void ChangeBreed(Pet pet, string newBreed) { pet.SetBreed(newBreed); }
    }
}
