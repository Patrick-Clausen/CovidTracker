using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseLibrary
{
    public class TestCenterManagement
    {
        public TestCenterManagement()
        {
            PhoneNumber = 0;
            Email = string.Empty;
            Name = string.Empty;
        }

        //Has no id, since it only ever exists as a part of a TestCenter

        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}