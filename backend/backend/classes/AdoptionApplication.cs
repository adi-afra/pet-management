using System;

namespace backend.classes
{
    /// Represents a user’s request to adopt a specific pet
    /// It will later be stored in the database
    public class AdoptionApplication
    {
        // Unique identifier for the application
        public string Id { get; private set; }

        // The user who submitted the application
        public string UserId { get; private set; }

        // The pet the user wants to adopt
        public string PetId { get; private set; }

        // Current status of the application
        // Pending, Approved, Rejected, Withdrawn
        public string Status { get; private set; }

        // When the application was created
        public DateTime DateCreated { get; private set; }

        // The users reason for adopting the pet
        public string Message { get; private set; }

        /// Constructor ensures all required data is valid when creating an application
        /// Validation prevents empty or invalid applications from being created
        public AdoptionApplication(string id, string userId, string petId, string message)
        {
            // Basic validation to ensure required fields are provided
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be empty.");

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be empty.");

            if (string.IsNullOrWhiteSpace(petId))
                throw new ArgumentException("PetId cannot be empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.");

            // Assign values after validation
            Id = id;
            UserId = userId;
            PetId = petId;
            Message = message.Trim();

            // Default values when a new application is created
            Status = "Pending";
            DateCreated = DateTime.UtcNow;
        }
        
        /// Approves the adoption application
        /// In future this could trigger alerts or meetings
        public void Approve()
        {
            Status = "Approved";
        }
        
        /// Rejects the application
        public void Reject()
        {
            Status = "Rejected";
        }
        
        /// Allows the user to withdraw their application.
        /// Useful if they change their mind
        public void Withdraw()
        {
            Status = "Withdrawn";
        }
    }
}



