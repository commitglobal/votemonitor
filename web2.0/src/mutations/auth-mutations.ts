import { useMutation } from '@tanstack/react-query'
import { resetPassword } from '@/services/api/auth/reset-password.api'

export const useResetPasswordMutation = () =>
  useMutation({
    mutationFn: async (email: string) => await resetPassword(email),
  })
