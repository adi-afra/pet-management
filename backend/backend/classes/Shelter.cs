using System;
using System.Collections.Generic;

namespace backend.classes
{
    //represents a shelter/organization that manages pets
    public class Shelter
    {
        //properties with private setters to protect data integrity
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Address { get; private set; }

        //pets currently associated with this shelter (inventory)
        public List<Pet> Pets { get; private set; }

        //constructor with validation
        public Shelter(string id, string name, string email, string phone, string address)
        {
            Id = ValidateString(id, nameof(id));
            Name = ValidateString(name, nameof(name));
            Email = ValidateString(email, nameof(email));
            Phone = ValidateString(phone, nameof(phone));
            Address = ValidateString(address, nameof(address));

            //simple sanity checks (optional but helpful)
            if (!Email.Contains("@"))
                throw new ArgumentException("Email must be a valid email address.");

            if (Phone.Length < 7)
                throw new ArgumentException("Phone number seems too short.");

            Pets = new List<Pet>();
        }

        //validation helper method (same pattern as your Pet/User classes)
        protected string ValidateString(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty.");

            return value;
        }

        //setter methods with validation

        public void setId(string id) => Id = ValidateString(id, nameof(id));

        public void setName(string name) => Name = ValidateString(name, nameof(name));

        public void setEmail(string email)
        {
            Email = ValidateString(email, nameof(email));
            if (!Email.Contains("@"))
                throw new ArgumentException("Email must be a valid email address.");
        }

        public void setPhone(string phone)
        {
            Phone = ValidateString(phone, nameof(phone));
            if (Phone.Length < 7)
                throw new ArgumentException("Phone number seems too short.");
        }

        public void setAddress(string address) => Address = ValidateString(address, nameof(address));

        //inventory management methods

        public void addPet(Pet pet)
        {
            if (pet == null)
                throw new ArgumentNullException(nameof(pet));

            //optional: avoid duplicates by Id
            if (Pets.Exists(p => p.Id == pet.Id))
                throw new ArgumentException("A pet with the same Id already exists in this shelter.");

            Pets.Add(pet);
        }

        public void removePet(Pet pet)
        {
            if (pet == null)
                throw new ArgumentNullException(nameof(pet));

            Pets.Remove(pet);
        }

        public Pet? findPetById(string petId)
        {
            if (string.IsNullOrWhiteSpace(petId))
                throw new ArgumentException("petId cannot be null or empty.");

            return Pets.Find(p => p.Id == petId);
        }
    }
}
