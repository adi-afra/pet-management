using System;
namespace backend.classes
{
    //Dog class which inherits from Pet abstract class
	public class Dog : Pet
	{
        //Constructor 
		public Dog(string id,string name, int age, string breed)
		{
            //calling the parent constructor 
            : base(id, name, age, "Dog", breed)
		}

	}
}

