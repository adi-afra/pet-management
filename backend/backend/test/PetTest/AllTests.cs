using System;
using Xunit;
using backend.classes;

namespace PetTest
{
    // =============================================
    // PET TESTS (uses TestPet since Pet is abstract)
    // =============================================
    public class PetTests
    {
        [Fact]
        public void Pet_ShouldCreateSuccessfully_WithValidInputs()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            Assert.Equal("p1", pet.Id);
            Assert.Equal("Buddy", pet.Name);
            Assert.Equal(2, pet.Age);
            Assert.Equal("Dog", pet.Kind);
            Assert.Equal("Labrador", pet.Breed);
        }

        [Fact]
        public void Pet_ShouldInitialiseEmptyMeetingsList()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            Assert.NotNull(pet.Meetings);
            Assert.Empty(pet.Meetings);
        }

        [Fact]
        public void Pet_ShouldThrow_WhenAgeIsNegative()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new TestPet("p1", "Buddy", -1, "Dog", "Labrador"));
            Assert.Contains("Age cannot be negative", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Pet_ShouldThrow_WhenIdIsNullOrEmpty(string invalidId)
        {
            Assert.Throws<ArgumentException>(() =>
                new TestPet(invalidId, "Buddy", 2, "Dog", "Labrador"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Pet_ShouldThrow_WhenNameIsNullOrEmpty(string invalidName)
        {
            Assert.Throws<ArgumentException>(() =>
                new TestPet("p1", invalidName, 2, "Dog", "Labrador"));
        }

        [Fact]
        public void Pet_SetAge_ShouldUpdateAge()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            pet.setAge(5);
            Assert.Equal(5, pet.Age);
        }

        [Fact]
        public void Pet_SetAge_ShouldThrow_WhenNegative()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            var ex = Assert.Throws<ArgumentException>(() => pet.setAge(-1));
            Assert.Contains("Age cannot be negative", ex.Message);
        }

        [Fact]
        public void Pet_SetName_ShouldUpdateName()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            pet.setName("Max");
            Assert.Equal("Max", pet.Name);
        }

        [Fact]
        public void Pet_SetName_ShouldThrow_WhenEmpty()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            Assert.Throws<ArgumentException>(() => pet.setName(""));
        }

        [Fact]
        public void Pet_SetKind_ShouldUpdateKind()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            pet.setKind("Cat");
            Assert.Equal("Cat", pet.Kind);
        }

        [Fact]
        public void Pet_SetBreed_ShouldUpdateBreed()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            pet.setBreed("Poodle");
            Assert.Equal("Poodle", pet.Breed);
        }

        [Fact]
        public void Pet_SetId_ShouldUpdateId()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            pet.setId("p99");
            Assert.Equal("p99", pet.Id);
        }

        [Fact]
        public void Pet_AddMeeting_ShouldAddToList()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            var meeting = new Meeting("2025-06-01", pet, "u1");
            pet.addMeeting(meeting);
            Assert.Single(pet.Meetings);
        }

        [Fact]
        public void Pet_AddMeeting_ShouldThrow_WhenNull()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            Assert.Throws<ArgumentNullException>(() => pet.addMeeting(null));
        }

        [Fact]
        public void Pet_RemoveMeeting_ShouldRemoveFromList()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            var meeting = new Meeting("2025-06-01", pet, "u1");
            pet.addMeeting(meeting);
            pet.removeMeeting(meeting);
            Assert.Empty(pet.Meetings);
        }

        [Fact]
        public void Pet_ShouldAllowAgeOfZero()
        {
            var pet = new TestPet("p1", "Buddy", 0, "Dog", "Labrador");
            Assert.Equal(0, pet.Age);
        }
    }

    // =============================================
    // DOG TESTS
    // =============================================
    public class DogTests
    {
        [Fact]
        public void Dog_ShouldCreateSuccessfully_WithValidInputs()
        {
            var dog = new Dog("d1", "Rex", 3, "Labrador");
            Assert.Equal("d1", dog.Id);
            Assert.Equal("Rex", dog.Name);
            Assert.Equal(3, dog.Age);
            Assert.Equal("Labrador", dog.Breed);
        }

        [Fact]
        public void Dog_KindShouldAlwaysBeDog()
        {
            var dog = new Dog("d1", "Rex", 3, "Labrador");
            Assert.Equal("Dog", dog.Kind);
        }

        [Fact]
        public void Dog_ShouldInheritFromPet()
        {
            var dog = new Dog("d1", "Rex", 3, "Labrador");
            Assert.IsAssignableFrom<Pet>(dog);
        }

        [Fact]
        public void Dog_ShouldThrow_WhenAgeIsNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                new Dog("d1", "Rex", -1, "Labrador"));
        }

        [Theory]
        [InlineData("Rex", "Labrador", 3)]
        [InlineData("Max", "Poodle", 5)]
        [InlineData("Bella", "Bulldog", 1)]
        public void Dog_ShouldHandleVariousBreeds(string name, string breed, int age)
        {
            var dog = new Dog("d1", name, age, breed);
            Assert.Equal(name, dog.Name);
            Assert.Equal(breed, dog.Breed);
            Assert.Equal(age, dog.Age);
            Assert.Equal("Dog", dog.Kind);
        }

        [Fact]
        public void Dog_ShouldInitialiseEmptyMeetingsList()
        {
            var dog = new Dog("d1", "Rex", 3, "Labrador");
            Assert.Empty(dog.Meetings);
        }
    }

    // =============================================
    // CAT TESTS
    // =============================================
    public class CatTests
    {
        [Fact]
        public void Cat_ShouldCreateSuccessfully_WithValidInputs()
        {
            var cat = new Cat("c1", "Whiskers", 2, "Persian");
            Assert.Equal("c1", cat.Id);
            Assert.Equal("Whiskers", cat.Name);
            Assert.Equal(2, cat.Age);
            Assert.Equal("Persian", cat.Breed);
        }

        [Fact]
        public void Cat_KindShouldAlwaysBeCat()
        {
            var cat = new Cat("c1", "Whiskers", 2, "Persian");
            Assert.Equal("Cat", cat.Kind);
        }

        [Fact]
        public void Cat_ShouldInheritFromPet()
        {
            var cat = new Cat("c1", "Whiskers", 2, "Persian");
            Assert.IsAssignableFrom<Pet>(cat);
        }

        [Fact]
        public void Cat_ShouldThrow_WhenAgeIsNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                new Cat("c1", "Whiskers", -1, "Persian"));
        }

        [Theory]
        [InlineData("Luna", "Siamese", 2)]
        [InlineData("Shadow", "Tabby", 5)]
        [InlineData("Mochi", "Maine Coon", 1)]
        public void Cat_ShouldHandleVariousBreeds(string name, string breed, int age)
        {
            var cat = new Cat("c1", name, age, breed);
            Assert.Equal(name, cat.Name);
            Assert.Equal(breed, cat.Breed);
            Assert.Equal(age, cat.Age);
            Assert.Equal("Cat", cat.Kind);
        }

        [Fact]
        public void Cat_ShouldInitialiseEmptyMeetingsList()
        {
            var cat = new Cat("c1", "Whiskers", 2, "Persian");
            Assert.Empty(cat.Meetings);
        }
    }

    // =============================================
    // USER TESTS (tested via Admin since User is abstract)
    // =============================================
    public class UserTests
    {
        [Fact]
        public void User_ShouldSetPropertiesOnCreation()
        {
            var admin = new Admin("u1", "adminUser", "pass123");
            Assert.Equal("u1", admin.Id);
            Assert.Equal("adminUser", admin.Username);
            Assert.Equal("pass123", admin.Password);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void User_ShouldThrow_WhenIdIsInvalid(string invalidId)
        {
            Assert.Throws<ArgumentException>(() =>
                new Admin(invalidId, "adminUser", "pass123"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void User_ShouldThrow_WhenUsernameIsInvalid(string invalidUsername)
        {
            Assert.Throws<ArgumentException>(() =>
                new Admin("u1", invalidUsername, "pass123"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void User_ShouldThrow_WhenPasswordIsInvalid(string invalidPassword)
        {
            Assert.Throws<ArgumentException>(() =>
                new Admin("u1", "adminUser", invalidPassword));
        }

        [Fact]
        public void User_SetUsername_ShouldUpdateUsername()
        {
            var admin = new Admin("u1", "adminUser", "pass123");
            admin.setUsername("newAdmin");
            Assert.Equal("newAdmin", admin.Username);
        }

        [Fact]
        public void User_SetUsername_ShouldThrow_WhenEmpty()
        {
            var admin = new Admin("u1", "adminUser", "pass123");
            Assert.Throws<ArgumentException>(() => admin.setUsername(""));
        }

        [Fact]
        public void User_SetPassword_ShouldUpdatePassword()
        {
            var admin = new Admin("u1", "adminUser", "pass123");
            admin.setPassword("newPass456");
            Assert.Equal("newPass456", admin.Password);
        }

        [Fact]
        public void User_SetPassword_ShouldThrow_WhenEmpty()
        {
            var admin = new Admin("u1", "adminUser", "pass123");
            Assert.Throws<ArgumentException>(() => admin.setPassword("   "));
        }
    }

    // =============================================
    // ADMIN TESTS
    // =============================================
    public class AdminTests
    {
        [Fact]
        public void Admin_GetRole_ShouldReturnAdmin()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            Assert.Equal("Admin", admin.getRole());
        }

        [Fact]
        public void Admin_ChangeName_ShouldUpdatePetName()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            admin.changeName(pet, "Rocky");
            Assert.Equal("Rocky", pet.Name);
        }

        [Fact]
        public void Admin_ChangeAge_ShouldUpdatePetAge()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            admin.changeAge(pet, 6);
            Assert.Equal(6, pet.Age);
        }

        [Fact]
        public void Admin_ChangeAge_ShouldThrow_WhenNegative()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            Assert.Throws<ArgumentException>(() => admin.changeAge(pet, -3));
        }

        [Fact]
        public void Admin_ChangeKind_ShouldUpdatePetKind()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            admin.changeKind(pet, "Cat");
            Assert.Equal("Cat", pet.Kind);
        }

        [Fact]
        public void Admin_ChangeBreed_ShouldUpdatePetBreed()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            admin.changeBreed(pet, "Golden Retriever");
            Assert.Equal("Golden Retriever", pet.Breed);
        }

        [Fact]
        public void Admin_ChangeId_ShouldUpdatePetId()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            admin.chnageId(pet, "p999");
            Assert.Equal("p999", pet.Id);
        }

        [Fact]
        public void Admin_ShouldInheritFromUser()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            Assert.IsAssignableFrom<User>(admin);
        }

        [Fact]
        public void Admin_ChangeName_ShouldThrow_WhenNameIsEmpty()
        {
            var admin = new Admin("a1", "adminUser", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            Assert.Throws<ArgumentException>(() => admin.changeName(pet, ""));
        }
    }

    // =============================================
    // CLIENT TESTS
    // =============================================
    public class ClientTests
    {
        [Fact]
        public void Client_ShouldCreateSuccessfully_WithValidInputs()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            Assert.Equal("c1", client.Id);
            Assert.Equal("johnDoe", client.Username);
        }

        [Fact]
        public void Client_GetRole_ShouldReturnClient()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            Assert.Equal("Client", client.getRole());
        }

        [Fact]
        public void Client_ShouldInitialiseEmptyMeetingsList()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            Assert.NotNull(client.Meetings);
            Assert.Empty(client.Meetings);
        }

        [Fact]
        public void Client_CreateMeeting_ShouldAddToClientMeetings()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            client.CreateMeeting("2025-06-15", pet);
            Assert.Single(client.Meetings);
        }

        [Fact]
        public void Client_CreateMeeting_ShouldAlsoAddToPetMeetings()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            client.CreateMeeting("2025-06-15", pet);
            Assert.Single(pet.Meetings);
        }

        [Fact]
        public void Client_CreateMeeting_ShouldThrow_WhenPetIsNull()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            Assert.Throws<ArgumentNullException>(() =>
                client.CreateMeeting("2025-06-15", null));
        }

        [Fact]
        public void Client_CreateMeeting_MeetingShouldHaveCorrectUserId()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            client.CreateMeeting("2025-06-15", pet);
            Assert.Equal("c1", client.Meetings[0].UserID);
        }

        [Fact]
        public void Client_CreateMeeting_MeetingShouldHaveCorrectPet()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            client.CreateMeeting("2025-06-15", pet);
            Assert.Equal("Buddy", client.Meetings[0].Pet.Name);
        }

        [Fact]
        public void Client_ShouldInheritFromUser()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            Assert.IsAssignableFrom<User>(client);
        }

        [Fact]
        public void Client_CreateMultipleMeetings_ShouldAllBeStored()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            var pet1 = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            var pet2 = new TestPet("p2", "Luna", 3, "Cat", "Persian");
            client.CreateMeeting("2025-06-15", pet1);
            client.CreateMeeting("2025-06-20", pet2);
            Assert.Equal(2, client.Meetings.Count);
        }
    }

    // =============================================
    // MEETING TESTS
    // =============================================
    public class MeetingTests
    {
        [Fact]
        public void Meeting_ShouldCreateSuccessfully_WithValidInputs()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            var meeting = new Meeting("2025-06-15", pet, "u1");
            Assert.Equal("2025-06-15", meeting.Date);
            Assert.Equal(pet, meeting.Pet);
            Assert.Equal("u1", meeting.UserID);
        }

        [Fact]
        public void Meeting_ShouldThrow_WhenDateIsNull()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            Assert.Throws<ArgumentNullException>(() =>
                new Meeting(null, pet, "u1"));
        }

        [Fact]
        public void Meeting_ShouldThrow_WhenPetIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Meeting("2025-06-15", null, "u1"));
        }

        [Fact]
        public void Meeting_ShouldThrow_WhenUserIdIsNull()
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            Assert.Throws<ArgumentNullException>(() =>
                new Meeting("2025-06-15", pet, null));
        }

        [Theory]
        [InlineData("2025-01-01")]
        [InlineData("2025-12-31")]
        [InlineData("2026-06-15")]
        public void Meeting_ShouldAcceptVariousDates(string date)
        {
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            var meeting = new Meeting(date, pet, "u1");
            Assert.Equal(date, meeting.Date);
        }
    }

    // =============================================
    // ADOPTION APPLICATION TESTS
    // =============================================
    public class AdoptionApplicationTests
    {
        [Fact]
        public void AdoptionApplication_ShouldCreateSuccessfully_WithValidInputs()
        {
            var app = new AdoptionApplication("app1", "u1", "p1", "I love dogs");
            Assert.Equal("app1", app.Id);
            Assert.Equal("u1", app.UserId);
            Assert.Equal("p1", app.PetId);
            Assert.Equal("I love dogs", app.Message);
        }

        [Fact]
        public void AdoptionApplication_StatusShouldDefaultToPending()
        {
            var app = new AdoptionApplication("app1", "u1", "p1", "I love dogs");
            Assert.Equal("Pending", app.Status);
        }

        [Fact]
        public void AdoptionApplication_DateCreatedShouldBeSetOnCreation()
        {
            var before = DateTime.UtcNow;
            var app = new AdoptionApplication("app1", "u1", "p1", "I love dogs");
            var after = DateTime.UtcNow;
            Assert.InRange(app.DateCreated, before, after);
        }

        [Fact]
        public void AdoptionApplication_Approve_ShouldSetStatusToApproved()
        {
            var app = new AdoptionApplication("app1", "u1", "p1", "I love dogs");
            app.Approve();
            Assert.Equal("Approved", app.Status);
        }

        [Fact]
        public void AdoptionApplication_Reject_ShouldSetStatusToRejected()
        {
            var app = new AdoptionApplication("app1", "u1", "p1", "I love dogs");
            app.Reject();
            Assert.Equal("Rejected", app.Status);
        }

        [Fact]
        public void AdoptionApplication_Withdraw_ShouldSetStatusToWithdrawn()
        {
            var app = new AdoptionApplication("app1", "u1", "p1", "I love dogs");
            app.Withdraw();
            Assert.Equal("Withdrawn", app.Status);
        }

        [Fact]
        public void AdoptionApplication_MessageShouldBeTrimmed()
        {
            var app = new AdoptionApplication("app1", "u1", "p1", "  I love dogs  ");
            Assert.Equal("I love dogs", app.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void AdoptionApplication_ShouldThrow_WhenIdIsInvalid(string invalidId)
        {
            Assert.Throws<ArgumentException>(() =>
                new AdoptionApplication(invalidId, "u1", "p1", "I love dogs"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void AdoptionApplication_ShouldThrow_WhenUserIdIsInvalid(string invalidUserId)
        {
            Assert.Throws<ArgumentException>(() =>
                new AdoptionApplication("app1", invalidUserId, "p1", "I love dogs"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void AdoptionApplication_ShouldThrow_WhenPetIdIsInvalid(string invalidPetId)
        {
            Assert.Throws<ArgumentException>(() =>
                new AdoptionApplication("app1", "u1", invalidPetId, "I love dogs"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void AdoptionApplication_ShouldThrow_WhenMessageIsInvalid(string invalidMessage)
        {
            Assert.Throws<ArgumentException>(() =>
                new AdoptionApplication("app1", "u1", "p1", invalidMessage));
        }

        [Fact]
        public void AdoptionApplication_StatusCanTransitionFromPendingToApproved()
        {
            var app = new AdoptionApplication("app1", "u1", "p1", "I love dogs");
            Assert.Equal("Pending", app.Status);
            app.Approve();
            Assert.Equal("Approved", app.Status);
        }

        [Fact]
        public void AdoptionApplication_StatusCanTransitionFromPendingToWithdrawn()
        {
            var app = new AdoptionApplication("app1", "u1", "p1", "I love dogs");
            app.Withdraw();
            Assert.Equal("Withdrawn", app.Status);
        }
    }
}