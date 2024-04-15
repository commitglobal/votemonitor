namespace SubmissionsFaker.Clients.NgoAdmin.Models;

public class RatingQuestionRequest : BaseQuestionRequest
{
    /// <summary>
    /// OneTo3 |OneTo4|OneTo5| OneTo6| OneTo7 | OneTo8 | OneTo9| OneTo10
    /// </summary>
    public string Scale { get; set; }
}
