using System;
using backend.classes;

namespace PetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Manual Pet Testing ===");

            try
            {
                // Create pet
                var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
                Console.WriteLine("Pet created successfully!");
                PrintPet(pet);

                // Test setAge
                pet.setAge(5);
                Console.WriteLine("Age updated:");
                PrintPet(pet);

                // Test invalid age
                Console.WriteLine("Trying invalid age...");
                pet.setAge(-10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void PrintPet(Pet pet)
        {
            Console.WriteLine($"Id: {pet.Id}");
            Console.WriteLine($"Name: {pet.Name}");
            Console.WriteLine($"Age: {pet.Age}");
            Console.WriteLine($"Kind: {pet.Kind}");
            Console.WriteLine($"Breed: {pet.Breed}");
            Console.WriteLine("----------------------");
        }
    }
}
