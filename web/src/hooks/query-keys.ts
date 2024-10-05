export const staticDataKeys = {
  all: ['static-data'] as const,
  countries: () => [...staticDataKeys.all, 'countries'] as const,
  languages: () => [...staticDataKeys.all, 'languages'] as const,
};
