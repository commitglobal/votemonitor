using Dapper;
using Job.Contracts.Jobs;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.FileGenerators;
using Vote.Monitor.Core.Queries;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Queries;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions;

public class ExportFormSubmissionsSimplifiedJob(
	VoteMonitorContext context,
	INpgsqlConnectionFactory dbConnectionFactory,
	IFileStorageService fileStorageService,
	ILogger<ExportFormSubmissionsSimplifiedJob> logger,
	ITimeProvider timeProvider) : IExportFormSubmissionsSimplifiedJob
{
	public async Task Run(Guid electionRoundId, Guid ngoId, Guid exportedDataId, CancellationToken ct)
	{
		var exportedData = await context
			.ExportedData
			.Where(x => x.Id == exportedDataId)
			.FirstOrDefaultAsync(ct);

		if (exportedData == null)
		{
			logger.LogWarning("ExportData was not found for {electionRoundId}  {exportedDataId}",
				electionRoundId, exportedDataId);
			throw new ExportedDataWasNotFoundException(ExportedDataType.FormSubmissions, electionRoundId,
				exportedDataId);
		}

		try
		{
			if (exportedData.ExportStatus == ExportedDataStatus.Completed)
			{
				logger.LogWarning("ExportData was completed for {electionRoundId} {ngoId} {exportedDataId}",
					electionRoundId, ngoId, exportedDataId);
				return;
			}

			var utcNow = timeProvider.UtcNow;

			var psiForm = await context
				.PollingStationInformationForms
				.Where(x => x.ElectionRoundId == electionRoundId)
				.AsNoTracking()
				.FirstOrDefaultAsync(ct);
			var filters = exportedData.FormSubmissionsFilters ?? new ExportFormSubmissionsFilters();

			var publishedForms = await context
				.Forms
				.FromSqlInterpolated(
					@$"SELECT f.* FROM ""Forms"" f 
                       INNER JOIN ""GetAvailableForms""({electionRoundId}, {ngoId}, {filters.DataSource.ToString()}) af on af.""FormId"" = f.""Id""              ")
				.Where(x => x.Status != FormStatus.Drafted)
				.OrderBy(x => x.CreatedOn)
				.AsNoTracking()
				.ToListAsync(ct);

			var submissions = await GetSubmissions(electionRoundId, ngoId, filters, exportedData.FilterConditions, ct);

			foreach (var submission in submissions)
			{
				foreach (var attachment in submission.Attachments)
				{
					var result =
						await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);

					if (result is GetPresignedUrlResult.Ok okResult)
					{
						attachment.PresignedUrl = okResult.Url;
					}
				}
			}

			var excelFileGenerator = ExcelFileGenerator.New();
			if (psiForm != null)
			{
				var psiDataTable = FormSubmissionsSimplifiedDataTable
					.FromForm(psiForm)
					.WithData()
					.ForSubmissions(submissions)
					.Please();

				excelFileGenerator.WithSheet("PSI", psiDataTable.header, psiDataTable.dataTable);
			}

			for (var index = 0; index < publishedForms.Count; index++)
			{
				var form = publishedForms[index];
				var sheetData = FormSubmissionsSimplifiedDataTable
					.FromForm(form)
					.WithData()
					.ForSubmissions(submissions)
					.Please();

				excelFileGenerator.WithSheet((index + 1) + "-" + form.Code, sheetData.header, sheetData.dataTable);
			}

			var base64EncodedData = excelFileGenerator.Please();
			var fileName = $"form-submissions-{utcNow:yyyyMMdd_HHmmss}.xlsx";
			exportedData.Complete(fileName, base64EncodedData, utcNow);

			await context.SaveChangesAsync(ct);
		}
		catch (Exception e)
		{
			logger.LogError(e, "An error occured when exporting data");
			exportedData.Fail();
			await context.SaveChangesAsync(ct);

			throw;
		}
	}

	private async Task<List<SubmissionModel>> GetSubmissions(Guid electionRoundId, Guid ngoId,
		ExportFormSubmissionsFilters filters, System.Text.Json.JsonDocument? filterConditions, CancellationToken ct)
	{
		var builder = new SqlBuilder();
		builder.AddParameters(new
		{
			electionRoundId,
			ngoId,
			dataSource = filters.DataSource.ToString()
		});

		builder.ApplyFilters(new FormSubmissionEntriesFilterCriteria
		{
			CoalitionMemberId = filters.CoalitionMemberId,
			MonitoringObserverId = filters.MonitoringObserverId,
			SearchText = filters.SearchText,
			FormType = filters.FormTypeFilter?.ToString(),
			Level1 = filters.Level1Filter,
			Level2 = filters.Level2Filter,
			Level3 = filters.Level3Filter,
			Level4 = filters.Level4Filter,
			Level5 = filters.Level5Filter,
			PollingStationNumber = filters.PollingStationNumberFilter,
			PollingStationId = filters.PollingStationId,
			HasFlaggedAnswers = filters.HasFlaggedAnswers,
			FollowUpStatus = filters.FollowUpStatus?.ToString(),
			Tags = filters.TagsFilter,
			MonitoringObserverStatus = filters.MonitoringObserverStatus?.ToString(),
			FormId = filters.FormId,
			HasNotes = filters.HasNotes,
			HasAttachments = filters.HasAttachments,
			QuestionsAnswered = filters.QuestionsAnswered?.ToString(),
			FromDate = filters.FromDateFilter,
			ToDate = filters.ToDateFilter,
			IsCompleted = filters.IsCompletedFilter
		});

		var filter = new FilterSqlCompiler(V2ReportFilterFields.FormSubmissions)
			.Build(FilterRuleJson.Deserialize(filterConditions));
		builder.Where(filter.Sql, filter.Parameters);
		builder.OrderBy(@"s.""TimeSubmitted"" DESC");

		var template = builder.AddTemplate(
			"""
            SELECT
            	s."SubmissionId",
            	CASE
            		WHEN s."FormType" = 'PSI' THEN 1
            		ELSE ROW_NUMBER() OVER (
            			PARTITION BY s."FormType", s."PollingStationId", s."FormId", s."MonitoringObserverId"
            			ORDER BY s."CreatedAt"
            		)
            	END AS "SubmissionNumber",
            	s."FormId",
            	s."FormType",
            	s."TimeSubmitted",
            	s."Level1",
            	s."Level2",
            	s."Level3",
            	s."Level4",
            	s."Level5",
            	s."Number",
            	s."NgoName",
            	s."MonitoringObserverId",
            	s."DisplayName",
            	s."Email",
            	s."PhoneNumber",
            	s."Tags",
            	s."Attachments",
            	s."Notes",
            	s."Answers",
            	s."FollowUpStatus",
            	s."IsCompleted"
            FROM
            	"GetFormSubmissionEntries"(@electionRoundId, @ngoId, @dataSource) s
            /**where**/
            /**orderby**/
            """);

		using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
		var submissions = await dbConnection.QueryAsync<SubmissionModel>(template.RawSql, template.Parameters);
		return submissions.ToList();
	}
}
