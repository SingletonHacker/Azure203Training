using Microsoft.Azure.Cosmos.Table;

namespace Functions
{
    public class Person : TableEntity
    {
        public Person(string lastName, string firstName)
        {
            this.PartitionKey = lastName; this.RowKey = firstName;
            FirstName = firstName;
            LastName = lastName;
        }

        public Person()
        {
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public static Person FromString(string person)
        {
            return new Person(person, person);
        }

        public override string ToString()
        {
            return $"{FirstName}_{LastName}";
        }
    }
}
