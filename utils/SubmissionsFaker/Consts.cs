﻿namespace SubmissionsFaker;

public class Consts
{
  public const int CHUNK_SIZE = 2;
  public const int NUMBER_OF_POLLING_STATIONS_TO_VISIT = 1000;
  public const int NUMBER_OF_OBSERVERS = 100;
  
  public const int NUMBER_OF_SUBMISSIONS = NUMBER_OF_OBSERVERS * 15;
  
  public const int NUMBER_OF_QUICK_REPORTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 50;
  public const int NUMBER_OF_QUICK_REPORTS_ATTACHMENTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 20;
  
  public const int NUMBER_OF_NOTES = (int)(NUMBER_OF_OBSERVERS * 0.2) + 50;
  public const int NUMBER_OF_ATTACHMENTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 20;
}