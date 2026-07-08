import axiosInstance from './axiosInstance';
import type {
  ApiResponse,
  CreateScheduleRequest,
  PagedResult,
  ScheduleDto,
  UpdateScheduleRequest,
} from '../types';

export const scheduleApi = {
  getPaged: (pageNumber: number, pageSize: number, zone?: string) =>
    axiosInstance
      .get<ApiResponse<PagedResult<ScheduleDto>>>('/schedules', { params: { pageNumber, pageSize, zone: zone || undefined } })
      .then((r) => r.data.data),

  getMy: () => axiosInstance.get<ApiResponse<ScheduleDto[]>>('/schedules/my').then((r) => r.data.data),

  create: (payload: CreateScheduleRequest) =>
    axiosInstance.post<ApiResponse<ScheduleDto>>('/schedules', payload).then((r) => r.data.data),

  update: (id: number, payload: UpdateScheduleRequest) =>
    axiosInstance.put<ApiResponse<ScheduleDto>>(`/schedules/${id}`, payload).then((r) => r.data.data),

  remove: (id: number) => axiosInstance.delete<ApiResponse<object>>(`/schedules/${id}`),
};
