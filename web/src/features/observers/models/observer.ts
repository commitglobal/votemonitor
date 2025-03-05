import { z } from 'zod';

export enum ObserverStatus {
  Active = 'Active',
  Pending = 'Pending',
  Deactivated = 'Deactivated',
}

export interface Observer {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  status: ObserverStatus;
  phoneNumber: string;
}

export const addObserverFormSchema = z.object({
  lastName: z.string().min(1, {
    message: 'Last name must be at least 1 characters long',
  }),
  firstName: z.string().min(1, {
    message: 'First name must be at least 1 characters long',
  }),
  email: z.string().min(1, { message: 'Email is required' }).email('Please enter a valid email address'),
  phoneNumber: z.string(),
  password: z.string().min(8, 'Password must be at least 8 characters long'),
});

export type AddObserverFormData = z.infer<typeof addObserverFormSchema>;

export const editObserverFormSchema = addObserverFormSchema.omit({ email: true, password: true });

export type EditObserverFormData = z.infer<typeof editObserverFormSchema>;
