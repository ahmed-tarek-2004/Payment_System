using Payment_System_Project;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Payment_System_Project;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

public class User
{
    public int UserId { get; set; }

    public Decimal Budget { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> methos { set; get; } = new();
    //public List<string> methos { set; get; } = new();
    public List<PaymentMethod> method { get; set; }
    public List<Transaction> transaction { get; set; }
    public List<AuditLog> auditLog { get; set; }

    public String Add_User(AppDbContext context)
    {

        User U = new();

        do
        {
            Console.Write("Enter Email : ");
            U.Email = Console.ReadLine();

        } while (U.Email.IsNullOrEmpty());

        if (CheckMail(U.Email, context, U) == U)
        {
            string pass;
            do
            {
                Console.Write("Enter Password : ");
                pass = Console.ReadLine();

            } while (pass.IsNullOrEmpty());

            U.Password = PasswordHelper.HashPassword(pass);


            do
            {
                Console.Write("Enter User Name : ");
                U.UserName = Console.ReadLine();

            } while (U.UserName.IsNullOrEmpty());


            int choice = -1;
            do
            {
                Console.Write("Enter Current Budget in Numeric Input  : ");

                int.TryParse(Console.ReadLine(), out choice);
                //Console.WriteLine(choice);
            } while (choice == -1 || choice == null);

            U.Budget = choice;

            context.Users.Add(U);
            context.SaveChanges();

            return $"\n\t\t :: User {U.UserName} Added Successfully ::\n";
        }
        return "\n\t\t :: User Is Already Exist ::\n";
    }
    public void Update_User(ref User user, AppDbContext context)
    {
        string pass;
        bool valid = false;
        do
        {
            do
            {
                Console.Write("Enter Password To Check : ");
                pass = Console.ReadLine();
            } while (Email.IsNullOrEmpty());

            int userID = user.UserId;

            var res = context.Users.Where(b => b.UserId == userID).Select(b => b.Password).SingleOrDefault();

            valid = PasswordHelper.VerifyPassword(pass.Trim(), res);
            if (!valid)
            {
                Console.WriteLine("\n\t\t :: Invalid Password ::\n");

            }
        } while (!valid);



        int choice = -1;
        do
        {
            Console.Write("Update : \n 1- User Name \n 2- Password\n 3- Both \n 0- Cancel\n Enter Your Answer : ");
            int.TryParse(Console.ReadLine(), out choice);
            // Console.WriteLine(choice);
        }
        while (choice < 0 || choice > 3);
        if (choice == 1)
        {
            do
            {
                Console.Write("Enter New User Name : ");
                UserName = Console.ReadLine();
            } while (UserName.IsNullOrEmpty());
            user.UserName = UserName;
        }
        if (choice == 2)
        {
            do
            {
                Console.Write("Enter New Password : ");
                Password = Console.ReadLine();
            } while (Password.IsNullOrEmpty());
            user.Password = PasswordHelper.HashPassword(Password);
        }
        if (choice == 3)
        {
            do
            {
                Console.Write("Enter User Name : ");
                UserName = Console.ReadLine();
            } while (UserName.IsNullOrEmpty());
            do
            {
                Console.Write("Enter Password : ");
                Password = Console.ReadLine();
            } while (Password.IsNullOrEmpty());
            user.UserName = UserName;
            user.Password = PasswordHelper.HashPassword(Password);
        }
        context.SaveChanges();
    }
    public bool Login(AppDbContext context, ref User user)
    {

        string temp;
        do
        {
            Console.Write("Enter Email : ");
            temp = Console.ReadLine();

        } while (temp.IsNullOrEmpty());

        User tempUser = new();
        tempUser = user;
        user = CheckMail(temp, context, tempUser);
        if (user != tempUser)
        {
            // Console.WriteLine(user.UserName);
            do
            {
                Console.Write("Enter Password : ");
                temp = Console.ReadLine();
            } while (temp.IsNullOrEmpty());


            var passTemp = PasswordHelper.VerifyPassword(temp.Trim(), user.Password);
            if (passTemp)
            {
                Console.WriteLine($"\n\t\t :: Welcome Back {user.UserName}::\n");
                return true;
            }
            else
            {
                user = new User();
                Console.WriteLine("\n\t\t :: Wrong Password ::\n");
                return false;
            }
        }
        else
        {
            user = new User();
            Console.WriteLine("\n\t\t :: User Not Found ::\n ");
            return false;
        }

    }

    public void View(ref User user, AppDbContext context)
    {
        Console.WriteLine($"\n\t\t User Id : {user.UserId}\n" +
            $"\n\t\t Email : {user.Email} \n" +
            $"\n\t\t Created At : {user.CreatedAt} \n" +
            $"\n\t\t Totoal Budget :{user.Budget} \n" +
            $"\n\t\t Payment Method Applyed :- ");

        if (user.methos.Count > 0)
        {
            for (int i = 0; i < user?.methos.Count; i++)
            {
                Console.WriteLine($"\n\t\t\t {i + 1}- {methos[i]} .");
            }
        }
        else
        {
            Console.WriteLine("\n\t\t :: No Payment Method Applied :: \n");
            return;
        }

            int userID = user.UserId;

        var res = context.Transactions
            .Join(
            context.Users,
            t => t.UserID,
            u => u.UserId,
            (tr, us) => new
            {
                tr.Status,
                tr.Amount,
                tr.TransactionId,
                tr.PaymentMethodId,
                tr.CreatedAt,
                tr.UserID,
                us.UserId
            })
            .Where(j => j.UserId == userID)
            .Join(
            context.paymentMethods,
            tran => tran.PaymentMethodId,
            p => p.PaymentMethodId,
           (tr, pay) => new
           {
               tr.Status,
               tr.Amount,
               tr.TransactionId,
               tr.CreatedAt,
               tr.UserID,
               pay.Type
           }
           ).ToList();
        if(res.Count==0)
        {
            Console.WriteLine("\n\t\t :: NO Transaction Applied :: ");
            return;
        }
        foreach (var trans in res)
        {
            Console.WriteLine($"\n\t\t Transaction ID : {trans.TransactionId}\n" +
                $"\n\t\t Amount : {trans.Amount} \n " +
                $"\n\t\t Status : {trans.Status} \n" +
                $"\n\t\t Created At : {trans.CreatedAt}");
            Console.WriteLine($"\n\t\t Done a Payment By : {trans.Type}");
            Console.WriteLine("---------------------------------------------------");
        }
        /*var paymentID = res.Select(b => b.PaymentMethodId).ToList();

        List<string> paymentMethods = new();


        for (int i = 0; i < paymentID?.Count; i++)
        {
            var method = context.paymentMethods.Where(b => b.UserId == userID && paymentID[i] == b.PaymentMethodId).Select(b => b.Type).ToList();
            if (method.Count > 0)
            {
                if (method.Count == 1)
                {
                    paymentMethods.Add(method[0]);
                }
                else
                {
                    paymentMethods.AddRange(method);
                }
            }
        }


        int k = 0;
        foreach (var trans in res)
        {
            Console.WriteLine($"\n\t\t Transaction ID : {trans.TransactionId}\n" +
                $"\n\t\t Amount : {trans.Amount} \n " +
                $"\n\t\t Status : {trans.Status} \n" +
                $"\n\t\t Created At : {trans.CreatedAt}");
            Console.WriteLine($"\n\t\t Done a Payment By : {paymentMethods[k]}");
            k++;
            Console.WriteLine("---------------------------------------------------");
        }*/

    }
    private User CheckMail(string User, AppDbContext context, User user)
    {
        var res = context.Users.Where(b => b.Email.ToLower().Trim() == User.ToLower().Trim()).SingleOrDefault();

        if (res != null)
            return res;
        else return user;
    }
}
