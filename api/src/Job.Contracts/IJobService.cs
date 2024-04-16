
namespace Job.Contracts;

public interface IJobService
{
   void SendEmail(string to, string subject, string body);
}
