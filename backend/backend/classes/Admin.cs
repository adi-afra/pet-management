namespace backend.classes
{
    public class Admin : User
    {
        public string Role { get; private set; } = "Admin";
        //constructor for Admin which adds a new field for their role
        public Admin(string id, string username, string password) : base(id, username, password)
        {
            Role = "Admin";
        }

        //overriding the getRole method to return the role of the user
        public override string getRole() { return Role; }

        //methods to change different fields of an instance of a Pet

        public void chnageId(Pet pet, String newId) {pet.setId(newId);}

        public void changeName(Pet pet, String newName) { pet.setName(newName); }
        public void changeAge(Pet pet, int newAge) { pet.setAge(newAge); }
        public void changeKind(Pet pet, String newKind) { pet.setKind(newKind); }
        public void changeBreed(Pet pet, String newBreed){ pet.setBreed(newBreed); }
    }
}
