using backend.classes;
using System;


namespace backend.classes
{
    //saved pets class 
    public class SavedPets
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int PetId { get; set; }

        public User User { get; set; }
        public Pet Pet { get; set; }
    }
}