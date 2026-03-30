using System;
namespace backend.classes
{
    //Dog class which inherits from Pet abstract class
	public class Dog : Pet
	{
        //Constructor 
        
        protected Dog() {} //Empty constructor for EF
        
		public Dog( string name, int age, string breed, string imageUrl)
		
            //calling the parent constructor 
            : base(name, age, breed, imageUrl) 
		{ 
			
		}

	}
}
