using System;
using backend.classes;

namespace backend.classes
{
    public class SavedPet
    {
        public int Id { get; set; }

        // The ID of the user who saved the pet
        public int ClientId { get; set; }

        // The ID of the pet being saved
        public int PetId { get; set; }
    }
}