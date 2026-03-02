using System;
using backend.classes;

namespace PetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==================================");
                Console.WriteLine("        MANUAL TEST RUNNER");
                Console.WriteLine("==================================");
                Console.WriteLine("1) Test Pet");
                Console.WriteLine("2) Test MedicalRecord");
                Console.WriteLine("3) Test Shelter");
                Console.WriteLine("4) Test Admin (modifies Pet)");
                Console.WriteLine("0) Exit");
                Console.Write("\nChoose an option: ");

                var choice = Console.ReadLine();

                Console.WriteLine("\n----------------------------------\n");

                switch (choice)
                {
                    case "1":
                        TestPet();
                        break;
                    case "2":
                        TestMedicalRecord();
                        break;
                    case "3":
                        TestShelter();
                        break;
                    case "4":
                        TestAdmin();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to go back to the menu...");
                Console.ReadKey();
            }
        }

        // =========================
        // 1) PET TESTS
        // =========================
        static void TestPet()
        {
            Console.WriteLine("=== Manual Pet Testing ===");

            try
            {
                var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
                Console.WriteLine("Pet created successfully!");
                PrintPet(pet);

                pet.setAge(5);
                Console.WriteLine("Age updated:");
                PrintPet(pet);

                pet.setName("Max");
                Console.WriteLine("Name updated:");
                PrintPet(pet);

                Console.WriteLine("Trying invalid age...");
                pet.setAge(-10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pet Exception caught: {ex.Message}");
            }
        }

        static void PrintPet(Pet pet)
        {
            Console.WriteLine($"Id: {pet.Id}");
            Console.WriteLine($"Name: {pet.Name}");
            Console.WriteLine($"Age: {pet.Age}");
            Console.WriteLine($"Kind: {pet.Kind}");
            Console.WriteLine($"Breed: {pet.Breed}");
            Console.WriteLine($"Meetings count: {pet.Meetings.Count}");
            Console.WriteLine("----------------------");
        }

        // =========================
        // 2) MEDICAL RECORD TESTS
        // =========================
        static void TestMedicalRecord()
        {
            Console.WriteLine("=== Manual MedicalRecord Testing ===");

            try
            {
                var record = new MedicalRecord(
                    "m1",
                    "p1",
                    "Ear Infection",
                    "Antibiotics for 7 days",
                    DateTime.Now.AddDays(-1)
                );

                Console.WriteLine("MedicalRecord created successfully!");
                PrintMedicalRecord(record);

                record.setDiagnosis("Updated Diagnosis");
                Console.WriteLine("Diagnosis updated:");
                PrintMedicalRecord(record);

                record.setTreatment("New Treatment Plan");
                Console.WriteLine("Treatment updated:");
                PrintMedicalRecord(record);

                Console.WriteLine("Trying invalid future date...");
                record.setRecordDate(DateTime.Now.AddDays(5));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MedicalRecord Exception caught: {ex.Message}");
            }
        }

        static void PrintMedicalRecord(MedicalRecord record)
        {
            Console.WriteLine($"Id: {record.Id}");
            Console.WriteLine($"PetId: {record.PetId}");
            Console.WriteLine($"Diagnosis: {record.Diagnosis}");
            Console.WriteLine($"Treatment: {record.Treatment}");
            Console.WriteLine($"RecordDate: {record.RecordDate}");
            Console.WriteLine("----------------------");
        }

        // =========================
        // 3) SHELTER TESTS
        // =========================
        static void TestShelter()
        {
            Console.WriteLine("=== Manual Shelter Testing ===");

            try
            {
                var shelter = new Shelter("s1", "DM Shelter", "dm@shelter.com", "12345678", "London");
                Console.WriteLine("Shelter created successfully!");
                PrintShelter(shelter);

                var pet1 = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
                var pet2 = new TestPet("p2", "Luna", 1, "Dog", "Husky");

                shelter.addPet(pet1);
                shelter.addPet(pet2);
                Console.WriteLine("Added 2 pets to shelter.");
                PrintShelter(shelter);

                var found = shelter.findPetById("p2");
                Console.WriteLine("Finding pet with id p2:");
                if (found != null) PrintPet(found);
                else Console.WriteLine("Pet not found.");

                shelter.removePet(pet1);
                Console.WriteLine("Removed pet p1.");
                PrintShelter(shelter);

                Console.WriteLine("Trying to add duplicate pet id p2...");
                shelter.addPet(new TestPet("p2", "Milo", 3, "Dog", "Beagle"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Shelter Exception caught: {ex.Message}");
            }
        }

        static void PrintShelter(Shelter shelter)
        {
            Console.WriteLine($"Id: {shelter.Id}");
            Console.WriteLine($"Name: {shelter.Name}");
            Console.WriteLine($"Email: {shelter.Email}");
            Console.WriteLine($"Phone: {shelter.Phone}");
            Console.WriteLine($"Address: {shelter.Address}");
            Console.WriteLine($"Pets count: {shelter.Pets.Count}");
            Console.WriteLine("----------------------");
        }

        // =========================
        // 4) ADMIN TESTS
        // =========================
        static void TestAdmin()
        {
            Console.WriteLine("=== Manual Admin Testing (Admin modifies Pet) ===");

            try
            {
                var admin = new Admin("a1", "adminUser", "password123");
                var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");

                Console.WriteLine($"Admin role: {admin.getRole()}");
                Console.WriteLine("Original pet:");
                PrintPet(pet);

                admin.changeName(pet, "Rocky");
                admin.changeAge(pet, 4);
                admin.changeBreed(pet, "Golden Retriever");

                Console.WriteLine("Pet after admin changes:");
                PrintPet(pet);

                Console.WriteLine("Trying invalid change (negative age)...");
                admin.changeAge(pet, -2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Admin Exception caught: {ex.Message}");
            }
        }
    }
}