/* eslint-disable unicorn/prefer-top-level-await */

import { z } from 'zod';

export interface NGO {
  id: string;
  name: string;
  status: NGOStatus;
  numberOfNgoAdmins: number;
  numberOfElectionsMonitoring: number;
  dateOfLastElection: string;
}

export enum NGOStatus {
  Activated = 'Activated',
  Deactivated = 'Deactivated',
}

export const newNgoSchema = z.object({
  name: z
    .string()
    .min(2, { message: 'Name must be at least 2 characters.' })
    .max(256, { message: 'Name must not exceed 256 characters.' }),
});

export type NgoCreationFormData = z.infer<typeof newNgoSchema>;
export const editNgoSchema = z.object({
  name: z
    .string()
    .min(2, { message: 'Name must be at least 2 characters.' })
    .max(256, { message: 'Name must not exceed 256 characters.' }),
});

export type EditNgoFormData = z.infer<typeof editNgoSchema>;
