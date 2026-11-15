import i18n from "../common/config/i18n";
import { PollingStationVisitVM } from "../common/models/polling-station.model";
import {
  IncidentReportAPIResponse,
  IncidentReportLocationType,
} from "../services/api/incident-report/incident-report.api";
import { FormAPIModel } from "../services/definitions.api";

// the app language if the form has it, otherwise the form default language
export const getFormLanguage = (form: FormAPIModel | undefined): string => {
  const appLanguage = i18n.language.toUpperCase();
  if (!form || form.languages.includes(appLanguage)) return appLanguage;
  return form.defaultLanguage || form.languages[0];
};

export const getFormDisplayName = (
  form: FormAPIModel | undefined,
  language: string = getFormLanguage(form),
): string => {
  if (!form) return "";
  const name =
    form.name[language] || form.name[form.defaultLanguage] || Object.values(form.name)[0];
  return `${form.code} - ${name}`;
};

export const getIncidentReportLocationLabel = (
  report: Pick<
    IncidentReportAPIResponse,
    "locationType" | "pollingStationId" | "locationDescription"
  >,
  visits: PollingStationVisitVM[] | undefined,
): string => {
  switch (report.locationType) {
    case IncidentReportLocationType.PollingStation: {
      const visit = visits?.find((v) => v.pollingStationId === report.pollingStationId);
      return visit ? `${visit.number} - ${visit.address}` : "";
    }
    case IncidentReportLocationType.OtherPollingStation:
    case IncidentReportLocationType.NotRelatedToAPollingStation: {
      const label =
        report.locationType === IncidentReportLocationType.OtherPollingStation
          ? i18n.t("form.polling_station_id.options.other", { ns: "report_new_issue" })
          : i18n.t("form.polling_station_id.options.not_related", { ns: "report_new_issue" });
      return report.locationDescription ? `${label}: ${report.locationDescription}` : label;
    }
    default:
      return "";
  }
};
