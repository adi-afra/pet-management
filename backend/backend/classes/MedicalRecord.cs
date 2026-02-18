using System;

namespace backend.classes
{
    //represents a medical record associated with a specific pet
    public class MedicalRecord
    {
        //properties with private setters to protect data integrity
        public string Id { get; private set; }
        public string PetId { get; private set; }
        public string Diagnosis { get; private set; }
        public string Treatment { get; private set; }
        public DateTime RecordDate { get; private set; }

        //constructor with validation
        public MedicalRecord(string id, string petId, string diagnosis, string treatment, DateTime recordDate)
        {
            Id = ValidateString(id, nameof(id));
            PetId = ValidateString(petId, nameof(petId));
            Diagnosis = ValidateString(diagnosis, nameof(diagnosis));
            Treatment = ValidateString(treatment, nameof(treatment));

            if (recordDate > DateTime.Now)
                throw new ArgumentException("Record date cannot be in the future.");

            RecordDate = recordDate;
        }

        //validation helper method (same pattern as Pet class)
        protected string ValidateString(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty.");

            return value;
        }

        //setter methods with validation

        public void setDiagnosis(string diagnosis)
        {
            Diagnosis = ValidateString(diagnosis, nameof(diagnosis));
        }

        public void setTreatment(string treatment)
        {
            Treatment = ValidateString(treatment, nameof(treatment));
        }

        public void setRecordDate(DateTime recordDate)
        {
            if (recordDate > DateTime.Now)
                throw new ArgumentException("Record date cannot be in the future.");

            RecordDate = recordDate;
        }

        public void setPetId(string petId)
        {
            PetId = ValidateString(petId, nameof(petId));
        }

        public void setId(string id)
        {
            Id = ValidateString(id, nameof(id));
        }
    }
}
