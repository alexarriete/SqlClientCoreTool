using SqlClientCoreTool;
using SqlClientCoreTool.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLCLIENTUTEST.Data
{
    internal class User: IAR
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public User() { }
        public User(int key)
        {
            Name = "Pepe";
            Surname = "Pérez";
            UserName = "josepo";
            Email = "josepo@acernuda.com";
            Password = "UnaPassCualquiera";            
        }

        public static List<User> GetUserList(int diferent)
        {
            List<User> users = new List<User>();

            for (int i = 0; i < 1000; i++)
            {
                if (i == diferent)
                {
                    User user2 = new User();
                    user2.Name = "María";
                    user2.Surname = "Rodríguez";
                    user2.UserName = "mariaro";
                    user2.Email = "mariaro@acernuda.com";
                    user2.Password = "UnaPassCualquiera";

                    users.Add(user2);
                }
                else
                {
                    users.Add(new User(1));
                }
            }
            
            return users;
        }
    }
}
