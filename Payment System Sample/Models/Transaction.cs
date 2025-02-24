using Microsoft.IdentityModel.Tokens;
using Payment_System_Project;
using System;

public class Transaction
{
    public int TransactionId { get; set; }
    public int UserID { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public User User { get; set; }

    public void AddTransaction(ref User user, AppDbContext context)
    {
        int choice = -1;

        if (user.methos.Count == 0)
        {
            Console.WriteLine("\n\t\t :: No Payment Method Exsists. ::\n");
            return;
        }

        int userID = user.UserId;



        bool isDefault = context.paymentMethods.Where(b => userID == b.UserId && b.IsDefault == true).Select(b => b.IsDefault).SingleOrDefault();

        //foreach(var r in ress)
        //{
        //    Console.WriteLine(r);
        //}


        int add = (isDefault ? 1 : 0);
        string defaultType = "";
        if (isDefault)
        {
            defaultType = context.paymentMethods.Where(b => b.IsDefault).Select(b => b.Type).SingleOrDefault();
        }
        var trans = context.Database.BeginTransaction();
        try
        {
            do
            {
                Console.WriteLine("Chooes Your Payment Method :-");
                for (int i = 0; i < user.methos.Count; i++)
                {
                    Console.WriteLine($"\t {i + 1}- {user.methos[i]} .");
                }

                if (isDefault)
                {
                    Console.WriteLine($"\t {user.methos.Count + 1}- By Default .");

                }
                Console.Write("Enter Your Choice : ");

                int.TryParse(Console.ReadLine(), out choice);
            }
            while (choice < 1 || choice > user.methos.Count + add);

            decimal amount = 0;
            do
            {
                Console.Write("Enter The Amount : ");
                decimal.TryParse(Console.ReadLine(), out amount);
            } while (amount < 0 || amount == null);


            decimal userAmount = user.Budget;
            bool enough = true;

            string tempType;
            if (!isDefault)
            {
                tempType = user.methos[choice - 1];
            }
            else
            {
                tempType = defaultType;
            }
            int userId = user.UserId;
            var res = context.paymentMethods.Where(b => b.UserId == userId && tempType == b.Type).SingleOrDefault();
            var tranaction = new Transaction()
            {
                UserID = user.UserId,
                PaymentMethodId = res.PaymentMethodId,
                Amount = amount,
            };

            if (amount > userAmount)
            {
                Console.WriteLine("\n\t\t :: Don't Have Enough Money :: \n");
                enough = false;
            }
            else
            {
                user.Budget -= amount;
            }

            tranaction.Status = (enough ? "succeeded" : "Failed");
            context.Transactions.Add(tranaction);
            context.SaveChanges();
            if (enough)
                Console.WriteLine("\n\t\t :: Transaction IS Completed Successflly. :: \n");

        }
        catch(Exception ex)
        {
            Console.WriteLine($"\n\t\t :: Transaction Failed. Error: {ex.Message} :: \n");
            trans.Rollback();
        }
    }
}
