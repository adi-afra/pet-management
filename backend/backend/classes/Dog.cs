using System;
namespace backend.classes
{
    //Dog class which inherits from Pet abstract class
	public class Dog : Pet
	{
        //Constructor 
        
        protected Dog() {} //Empty constructor for EF
        
		public Dog( string name, int age, string breed)
		
            //calling the parent constructor 
            : base(name, age, breed) 
		{ 
			
		}

	}
}
