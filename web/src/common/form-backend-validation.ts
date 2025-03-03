import { AxiosError } from 'axios';
import { FieldValues, Path, UseFormReturn } from 'react-hook-form';
import { ProblemDetails } from './types';

export const addFormValidationErrorsFromBackend = <T extends FieldValues>(
  form: UseFormReturn<T>,
  error: AxiosError<ProblemDetails>
) => {
  error?.response?.data.errors?.forEach((error) => {
    form.setError(error.name as Path<T>, { type: 'custom', message: error.reason });
  });
};
