import { authApi, type ILoginResponse, type LoginDTO } from '@/common/auth-api';

export async function login(payload: LoginDTO): Promise<ILoginResponse> {
  const response = await authApi.post<ILoginResponse>('auth/login', payload);
  return response.data;
}

