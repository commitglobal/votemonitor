/* eslint-disable unicorn/prefer-top-level-await */

import { z } from 'zod';
import { ngoAdminSchema } from './NgoAdmin';

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
  Pending = 'Pending',
  Deactivated = 'Deactivated',
}

export const newNgoSchema = ngoAdminSchema.extend({ name: z.string() });
export type NgoCreationFormData = z.infer<typeof newNgoSchema>;
