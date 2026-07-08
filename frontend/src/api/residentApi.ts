import axiosInstance from './axiosInstance';
import type {
  ApiResponse,
  CreateResidentRequest,
  PagedResult,
  ResidentDto,
  UpdateResidentRequest,
} from '../types';

export const residentApi = {
  getPaged: (pageNumber: number, pageSize: number, searchTerm?: string) =>
    axiosInstance
      .get<ApiResponse<PagedResult<ResidentDto>>>('/residents', {
        params: { pageNumber, pageSize, searchTerm: searchTerm || undefined },
      })
      .then((r) => r.data.data),

  getById: (id: number) => axiosInstance.get<ApiResponse<ResidentDto>>(`/residents/${id}`).then((r) => r.data.data),

  create: (payload: CreateResidentRequest) =>
    axiosInstance.post<ApiResponse<ResidentDto>>('/residents', payload).then((r) => r.data.data),

  update: (id: number, payload: UpdateResidentRequest) =>
    axiosInstance.put<ApiResponse<ResidentDto>>(`/residents/${id}`, payload).then((r) => r.data.data),

  remove: (id: number) => axiosInstance.delete<ApiResponse<object>>(`/residents/${id}`),
};
