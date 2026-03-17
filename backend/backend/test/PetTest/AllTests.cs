using System;
using Xunit;
using backend.classes;

namespace PetTest
{
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
        public void Pet_ShouldThrow_WhenIdIsNullOrEmpty(string invalidId)
        {
            Assert.Throws<ArgumentException>(() =>
                new TestPet(invalidId, "Buddy", 2, "Dog", "Labrador"));
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
            Assert.Throws<ArgumentException>(() => pet.setAge(-1));
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

    public class DogTests
    {
        [Fact]
        public void Dog_ShouldCreateSuccessfully()
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

        [Fact]
        public void Dog_ShouldInitialiseEmptyMeetingsList()
        {
            var dog = new Dog("d1", "Rex", 3, "Labrador");
            Assert.Empty(dog.Meetings);
        }
    }

    public class CatTests
    {
        [Fact]
        public void Cat_ShouldCreateSuccessfully()
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

        [Fact]
        public void Cat_ShouldInitialiseEmptyMeetingsList()
        {
            var cat = new Cat("c1", "Whiskers", 2, "Persian");
            Assert.Empty(cat.Meetings);
        }
    }

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

        [Fact]
        public void User_ShouldThrow_WhenIdIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new Admin("", "adminUser", "pass123"));
        }

        [Fact]
        public void User_ShouldThrow_WhenUsernameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new Admin("u1", "", "pass123"));
        }

        [Fact]
        public void User_ShouldThrow_WhenPasswordIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new Admin("u1", "adminUser", ""));
        }

        [Fact]
        public void User_SetUsername_ShouldUpdate()
        {
            var admin = new Admin("u1", "adminUser", "pass123");
            admin.setUsername("newAdmin");
            Assert.Equal("newAdmin", admin.Username);
        }

        [Fact]
        public void User_SetPassword_ShouldUpdate()
        {
            var admin = new Admin("u1", "adminUser", "pass123");
            admin.setPassword("newPass456");
            Assert.Equal("newPass456", admin.Password);
        }
    }

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
    }

    public class ClientTests
    {
        [Fact]
        public void Client_ShouldCreateSuccessfully()
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
        public void Client_CreateMeeting_ShouldHaveCorrectUserId()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            var pet = new TestPet("p1", "Buddy", 2, "Dog", "Labrador");
            client.CreateMeeting("2025-06-15", pet);
            Assert.Equal("c1", client.Meetings[0].UserID);
        }

        [Fact]
        public void Client_ShouldInheritFromUser()
        {
            var client = new Client("c1", "johnDoe", "pass123");
            Assert.IsAssignableFrom<User>(client);
        }
    }

    public class MeetingTests
    {
        [Fact]
        public void Meeting_ShouldCreateSuccessfully()
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
    }

    public class AdoptionApplicationTests
    {
        [Fact]
        public void AdoptionApplication_ShouldCreateSuccessfully()
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

        [Fact]
        public void AdoptionApplication_ShouldThrow_WhenIdIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new AdoptionApplication("", "u1", "p1", "I love dogs"));
        }

        [Fact]
        public void AdoptionApplication_ShouldThrow_WhenUserIdIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new AdoptionApplication("app1", "", "p1", "I love dogs"));
        }

        [Fact]
        public void AdoptionApplication_ShouldThrow_WhenPetIdIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new AdoptionApplication("app1", "u1", "", "I love dogs"));
        }

        [Fact]
        public void AdoptionApplication_ShouldThrow_WhenMessageIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new AdoptionApplication("app1", "u1", "p1", ""));
        }
    }
}
