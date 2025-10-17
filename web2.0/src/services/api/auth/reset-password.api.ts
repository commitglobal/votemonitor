import publicAPI from '@/services/public.api'

export const resetPassword = async (email: string): Promise<void> => {
  return publicAPI
    .post(`/auth/forgot-password`, {
      email,
    })
    .then((res) => res.data)
}
