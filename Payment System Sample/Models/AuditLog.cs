using Payment_System_Project;
using System;
public class AuditLog
{
    public AuditLog()
    {

    }
    public int LogId { get; set; }
    public string Action { get; set; }

    public int UserID { get; set; }
    public DateTime Timestamp { get; set; }
    public virtual User User { get; set; }


    public static void addAuditLog(string action , int userId,AppDbContext context)
    {
        var auditLog = new AuditLog()
        {
            Action = action,
            UserID = userId
        };
        context.AuditLogs.Add(auditLog);
        context.SaveChanges();
    }

    public static void PrintAllActions( int userID , AppDbContext context)
    {
        
        var res = context.AuditLogs.Where(b => b.UserID == userID).ToList();
        foreach(var activity in res)
        {
            Console.WriteLine($"\n\t\t :: Action ID : {activity.LogId} :: \n" +
                $"\n\t\t :: Action : {activity.Action} ::\n" +
                $"\n\t\t :: Done At : {activity.Timestamp} ::\n");
        }
    }

}
