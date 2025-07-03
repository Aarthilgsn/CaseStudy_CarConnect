using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectApp.Entity
{

    public class Customer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Customer() { }

        public Customer(int customerID, string firstName, string lastName, string email, string phoneNumber,
                        string address, string username, string password, DateTime registrationDate)
        {
            CustomerID = customerID;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Username = username;
            Password = password;
            RegistrationDate = registrationDate;
        }

        public bool Authenticate(string password)
        {
            return Password == password;
        }
    }
}