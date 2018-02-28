using System;
namespace TestApp.models
{
    public class User
    {
        public int id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public DateTime? LastLogin { get; set; }

    }
}
