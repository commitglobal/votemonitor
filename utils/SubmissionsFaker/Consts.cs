namespace SubmissionsFaker;

public class Consts
{
    public const int CHUNK_SIZE = 2;
    public const int NUMBER_OF_POLLING_STATIONS_TO_VISIT = 2000; // should be greater than NUMBER_OF_SUBMISSIONS
    public const int NUMBER_OF_OBSERVERS = 10;

    public const int NUMBER_OF_SUBMISSIONS = NUMBER_OF_OBSERVERS * 15;

    public const int NUMBER_OF_QUICK_REPORTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 50;
    public const int NUMBER_OF_QUICK_REPORTS_ATTACHMENTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 20;

    public const int NUMBER_OF_NOTES = (int)(NUMBER_OF_OBSERVERS * 0.2) + 50;
    public const int NUMBER_OF_ATTACHMENTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 20;

    public const int NUMBER_OF_LOCATIONS_TO_VISIT = 200; // should be greater than NUMBER_OF_CITIZEN_REPORTS

    public const int NUMBER_OF_CITIZEN_REPORTS = 150;
    public const int NUMBER_OF_CITIZEN_REPORTS_NOTES = (int)(NUMBER_OF_CITIZEN_REPORTS * 0.2) + 50;
    public const int NUMBER_OF_CITIZEN_REPORTS_ATTACHMENTS = (int)(NUMBER_OF_CITIZEN_REPORTS * 0.2) + 20;
    
    public const int NUMBER_OF_INCIDENT_REPORTS = 150;
    public const int NUMBER_OF_INCIDENT_REPORTS_NOTES = (int)(NUMBER_OF_INCIDENT_REPORTS * 0.2) + 50;
}

