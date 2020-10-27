using System;

namespace ExpressionTreeToString.Tests {
    public class Person {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age() {
            if (DOB is null) { return null; }
            var dob = DOB.Value;
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
