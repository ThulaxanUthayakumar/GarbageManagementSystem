import axiosInstance from './axiosInstance';
import type {
  ApiResponse,
  AuthResponse,
  ChangePasswordRequest,
  LoginRequest,
  RegisterRequest,
  UserDto,
} from '../types';

export const authApi = {
  register: (payload: RegisterRequest) =>
    axiosInstance.post<ApiResponse<AuthResponse>>('/auth/register', payload).then((r) => r.data.data),

  login: (payload: LoginRequest) =>
    axiosInstance.post<ApiResponse<AuthResponse>>('/auth/login', payload).then((r) => r.data.data),

  logout: () => axiosInstance.post<ApiResponse<object>>('/auth/logout'),

  changePassword: (payload: ChangePasswordRequest) =>
    axiosInstance.post<ApiResponse<object>>('/auth/change-password', payload).then((r) => r.data),

  me: () => axiosInstance.get<ApiResponse<UserDto>>('/auth/me').then((r) => r.data.data),
};
