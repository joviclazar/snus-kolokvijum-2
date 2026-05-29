public class Person
{
    public string Username { get; set; }
    public string City { get; set; }
    public int Age { get; set; }
    public string PhoneNumber { get; set; }

    public Person(string username, string city, int age, string phoneNumber)
    {
        Username = username;
        City = city;
        Age = age;
        PhoneNumber = phoneNumber;
    }
}