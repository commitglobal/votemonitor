import { Guide, guideType } from "../services/api/get-guides.api";

export const filterResources = (resources: Guide[], searchTerm: string) => {
  return resources?.filter(
    (resource) =>
      resource.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      resource.fileName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (resource.guideType === guideType.TEXT &&
        resource.text?.toLowerCase().includes(searchTerm.toLowerCase())),
  );
};
