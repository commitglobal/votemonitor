import { getRouteApi } from '@tanstack/react-router';

export const useBreadcrumbsWithAliases = (routeId: string) => {
  const routeApi = getRouteApi(routeId as any);

  const routeParams = routeApi.useParams();

  type ParamAliases = keyof typeof routeParams;

  const defineParamsAliases = (aliases: ParamAliases) => {};

  return { defineParamsAliases };
};
