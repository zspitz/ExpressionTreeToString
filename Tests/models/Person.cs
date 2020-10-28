using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionTreeToString.Tests {
    public class Person {
        public Person() { }
        public Person(string lastname, string firstname) {
            LastName = lastname;
            FirstName = firstname;
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age {
            get {
                if (DOB is null) { return null; }
                var dob = DOB.Value;
                var today = DateTime.Today;
                var age = today.Year - dob.Year;
                if (dob.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public Person[] Relatives { get; } = new Person[] { };
        public List<Person> Relatives2 { get; } = new List<Person>();
        public Person? Father { get; }
        public bool Notify() => true;
    }
}
