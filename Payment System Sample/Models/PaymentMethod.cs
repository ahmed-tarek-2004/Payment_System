using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Payment_System_Project;
using System;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;

public class PaymentMethod
{
    public int PaymentMethodId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; }
    public string Details { get; set; }
    public bool IsDefault { get; set; }
    public User User { get; set; }
    public List<Transaction> Transactions { get; set; }



    public static void ApplyUser(ref User user, AppDbContext context)
    {

        user.methos = new();

        int id = user.UserId;
        var res = context.paymentMethods.Where(b => b.UserId == id).Select(b => b.Type).ToList();
        if (res != null)
        {
            //  foreach (var r in res) Console.WriteLine(r);


            if (res.Count == 1)
                user.methos.Add(res[0].ToString() ?? null);
            else
                user.methos.AddRange(res ?? null);
        }

    }
    public static void AddPayMethod(ref User user, AppDbContext context)
    {

        Console.Write("Enter Your New Method : ");
        string newMethod;
        do
        {
            newMethod = Console.ReadLine().Trim().ToLower();
        } while (newMethod.IsNullOrEmpty());

        int id = user.UserId;
        var res = context.paymentMethods.Where(b => b.UserId == id && b.Type.Trim().ToLower() == newMethod).Count();
        if (res > 0)
        {
            Console.WriteLine($"\n\t\t :: Method ({newMethod}) Alredy Exsits :: \n");
        }
        else
        {
            StringBuilder temp = new StringBuilder(newMethod);
            temp[0] = char.ToUpper(temp[0]);
            newMethod = temp.ToString();

            Console.WriteLine("Enter Any Details (Optionally) : ");
            string @Details = Console.ReadLine();
            if (@Details.IsNullOrEmpty())
                @Details = "";

            Console.WriteLine("Make It Default (Y/N) ? : ");
            string @default = Console.ReadLine();

            bool isDefault = (@default == "Y" || @default == "y" ? true : false);
            var newPaymet = new PaymentMethod { UserId = id, Type = newMethod, Details = @Details, IsDefault = isDefault };
            context.paymentMethods.Add(newPaymet);
            context.SaveChanges();
        }
    }


    public static void EditMethod(ref User user, AppDbContext context)
    {
        string oldName;
        do
        {
            Console.Write("Enter Method Name To Edit : ");
            oldName = Console.ReadLine().ToLower();

        } while (oldName.IsNullOrEmpty());

        StringBuilder temp = new StringBuilder(oldName);
        temp[0] = char.ToUpper(temp[0]);
        oldName = temp.ToString();

        //Console.WriteLine($"{oldName} --");

        //foreach(var r in user.methos)
        //{
        //    Console.WriteLine(r);
        //}


        if (user.methos.Contains(oldName.Trim()))
        {
            int choice;
            do
            {
                Console.Write("1- Change Method Name .\n" +
               "2- Change Method Details .\n" +
               "Enter Your Answer :");
                int.TryParse(Console.ReadLine(), out choice);

            } while (choice < 1 || choice > 2);

            int userId = user.UserId;
            var newPaymentMethod = context.paymentMethods.Where(b => b.UserId == userId && b.Type.ToLower().Trim() == oldName.ToLower().Trim()).SingleOrDefault();

            #region Change Payment Method Name

            if (choice == 1)
            {
                string newName;
                do
                {
                    Console.Write("Enter New Name For The Method : ");
                    newName = Console.ReadLine().Trim().ToLower();
                } while (newName.IsNullOrEmpty());

                temp = new StringBuilder(newName);
                temp[0] = char.ToUpper(temp[0]);
                newName = temp.ToString();

                if (!user.methos.Contains(newName))
                {
                    user.methos.Sort();
                    var res = user.methos.BinarySearch(oldName);
                    user.methos[res] = newName;
                    newPaymentMethod.Type = newName;
                    context.SaveChanges();
                    Console.WriteLine($"\n\t\t :: Method Name Changed Successfully To ({newName}) :: ");
                }

                else
                {
                    Console.WriteLine($"\n\t\t :: Method ({newName}) Is Exsits :: \n");
                }

            }
            #endregion


            #region Change Details Info

            else if (choice == 2)
            {
                string newDetails;
                do
                {
                    Console.Write($"Enter '{oldName}' New Details : ");
                    newDetails = Console.ReadLine();

                } while (newDetails.IsNullOrEmpty());


                newPaymentMethod.Details = newDetails;
                context.SaveChanges();
            }
            #endregion

        }
        else
        {
            Console.WriteLine($"\n\t\t :: Method ({oldName}) Is Not Exsit :: \n");
        }


    }


    public static void DeleteMethos(ref User user, AppDbContext context)
    {
        int userId = user.UserId;

        
        string method;
        do
        {
           
           for(int i=0;i<user.methos.Count;i++)
            {
                Console.WriteLine($"\n\t\t {i + 1}- {user.methos[i]}.");
            }

            Console.Write("Enter Method Name To Delete : ");
            method = Console.ReadLine();
        } while (method.IsNullOrEmpty());

        StringBuilder temp = new(method);
        temp[0] = char.ToUpper(temp[0]);
        method = temp.ToString();
        if (user.methos.Contains(method))
        {
            var res = context.paymentMethods.Where(b => b.UserId == userId && b.Type.Trim().ToLower() == method.Trim().ToLower()).SingleOrDefault();
            context.paymentMethods.Remove(res);
            try
            {
                context.SaveChanges();
                Console.WriteLine($"\n\t\t :: Method {method} Has Deleted Successfully :: ");
            }
            catch
            {
                Console.WriteLine("\n\t\t :: Can't Delete ,Method Have Transaction With It ::\n");
            }
        }
        else
        {
            Console.WriteLine($"\n\t\t :: Method ({method}) Is Not Exsit :: \n");
        }
    }


    public static void SetDefault(ref User user, AppDbContext context)
    {
        if (user.methos.Count == 0)
        {
            Console.WriteLine("\n\t\t :: No Methods Exsits :: \n");
            return;
        }

        int choice = -1;
        do
        {
            for (int i = 0; i < user.methos.Count; i++)
            {
                Console.WriteLine($"{i + 1}- Set By {user.methos[i]} .");
            }
            Console.Write("Enter Your Choice : ");
            int.TryParse(Console.ReadLine(), out choice);
        }
        while (choice < 1 || choice > user.methos.Count);

        string pay = user.methos[choice - 1];

        int id = user.UserId;
        var res = context.paymentMethods.Where(b => b.UserId == id && b.Type == pay).SingleOrDefault();
        var undo = context.paymentMethods.Where(b => b.UserId == id && b.IsDefault == true).SingleOrDefault();
        if (res != null)
        {
            res.IsDefault = true;
            if (undo != null)
            {
                undo.IsDefault = false;
            }
            Console.WriteLine($"\n\t\t :: {pay} Is Set Default Successfully :: \n");
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine($"\n\t\t :: An Error Ocurred :: \n");
        }
    }
}
