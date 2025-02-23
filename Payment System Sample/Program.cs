using Payment_System_Project;
using System;
using System.Runtime.InteropServices;
using BCrypt.Net;

namespace Payment_System
{
    public class Program
    {
        public static User user = new();
        public static void Main(string[] args)
        {
            var Context = new AppDbContext();
            //User user=new();
            while (true)
            {
                bool Registed = false;
                while (!Registed)
                {
                    //Console.Clear();
                    int choice = -1;
                    do
                    {
                        Console.Write("1- Login\n" +
                        "2- Create An Account\n" +
                        "Enter Your Choice : ");
                        int.TryParse(Console.ReadLine(), out choice);
                        // Console.WriteLine(choice);
                    }
                    while (choice < 1 || choice > 2);
                    Console.Clear();
                    if (choice == 1)
                    {
                        Console.Clear();
                        bool state = user.Login(Context, ref user);
                        if (state == true)
                        {
                            Registed = true;
                            AuditLog.addAuditLog("Logged In ", user.UserId, Context);
                        }
                    }
                    else
                    {
                        Console.WriteLine(user.Add_User(Context));
                    }
                }
                while (Registed)
                {
                    //InterFacecs interfaces =new InterFacecs();
                   
                    PaymentMethod.ApplyUser(ref user, Context);
                    int choice = -1;
                    do
                    {
                        Console.Write("\n\n1- Update Profile Endpoints.\n" +
                         "2- CRUD Operations For Payment Methods .\n" +
                         "3- CRUD Operation For TransAction.\n" +
                         "4- View Profile .\n" +
                         "5- See All Audit Log\\s\n" +
                         "6- Log out . " +
                         "Enter Your Choice : ");
                         int.TryParse(Console.ReadLine(), out choice);
                    }
                    while (choice < 1 || choice > 6);
                    if (choice == 1)
                    {
                        user.Update_User(ref user,Context);
                        AuditLog.addAuditLog("Update User ", user.UserId, Context);
                    }
                    else if (choice == 2)
                    {
                        choice = -1;
                        do
                        {
                            Console.Write("\n\n1- Add Payment Method.\n" +
                             "2- Edit Payment Method .\n" +
                             "3- Delete Payment Method .\n" +
                             "4- Set Method As Default .\n" +
                             "Enter Your Choice : ");
                            int.TryParse(Console.ReadLine(), out choice);
                        } while (choice < 1 || choice > 4);
                        
                        if (choice == 1)
                        {
                            PaymentMethod.AddPayMethod(ref user, Context);
                            AuditLog.addAuditLog("Add Payment Method ", user.UserId, Context);
                        }
                        else if (choice == 2)
                        {
                            PaymentMethod.EditMethod(ref user, Context);
                            AuditLog.addAuditLog("Edit Payment Method ", user.UserId, Context);
                        }
                        else if (choice == 3)
                        {
                            PaymentMethod.DeleteMethos(ref user, Context);
                            AuditLog.addAuditLog("Delete Payment Method ", user.UserId, Context);
                        }
                        else
                        {
                            PaymentMethod.SetDefault(ref user, Context);
                            AuditLog.addAuditLog("Set Default Payment Method ", user.UserId, Context);
                        }
                    }
                    else if (choice == 3)
                    {
                        Transaction transaction = new();
                        transaction.AddTransaction(ref user, Context);
                        AuditLog.addAuditLog("Make A Transaction ", user.UserId, Context);
                    }
                    else if(choice==4)
                    {
                        user.View(ref user, Context);
                        AuditLog.addAuditLog("View A Profile ", user.UserId, Context);
                    }
                    else if(choice==5)
                    {
                        AuditLog.addAuditLog("See All Audit Logs ", user.UserId, Context);
                        AuditLog.PrintAllActions(user.UserId, Context);
                    }
                    else
                    {
                        AuditLog.addAuditLog("Logged Out ", user.UserId, Context);
                        Registed = false;
                        user = new();
                        Console.WriteLine("-------------------------------------------");
                        break;
                    }
                }
            }
        }
    }
}