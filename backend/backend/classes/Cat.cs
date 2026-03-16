using System;

namespace backend.classes
{
	//Cat class inherits from the Pet class
	public class Cat : Pet
	{
		// Constructor
		
		protected Cat() {} //Empty constructor for EF
		
		public Cat( string name, int age, string breed)
			: base(name, age, breed)
		{
			
		}
	}
}
