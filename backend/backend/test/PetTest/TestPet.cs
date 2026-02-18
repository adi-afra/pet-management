using backend.classes;

namespace PetTest
{
    public class TestPet : Pet
    {
        public TestPet(string id, string name, int age, string kind, string breed)
            : base(id, name, age, kind, breed)
        {
        }
    }
}
