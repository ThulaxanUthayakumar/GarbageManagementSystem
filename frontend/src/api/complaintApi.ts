import axiosInstance from './axiosInstance';
import type {
  ApiResponse,
  ComplaintDto,
  ComplaintStatus,
  CreateComplaintPayload,
  PagedResult,
  UpdateComplaintStatusRequest,
} from '../types';

export const complaintApi = {
  getPaged: (pageNumber: number, pageSize: number, searchTerm?: string, status?: ComplaintStatus) =>
    axiosInstance
      .get<ApiResponse<PagedResult<ComplaintDto>>>('/complaints', {
        params: { pageNumber, pageSize, searchTerm: searchTerm || undefined, status: status || undefined },
      })
      .then((r) => r.data.data),

  getMy: () => axiosInstance.get<ApiResponse<ComplaintDto[]>>('/complaints/my').then((r) => r.data.data),

  getById: (id: number) => axiosInstance.get<ApiResponse<ComplaintDto>>(`/complaints/${id}`).then((r) => r.data.data),

  create: (payload: CreateComplaintPayload) => {
    const form = new FormData();
    form.append('subject', payload.subject);
    form.append('description', payload.description);
    if (payload.image) form.append('image', payload.image);

    return axiosInstance
      .post<ApiResponse<ComplaintDto>>('/complaints', form, { headers: { 'Content-Type': 'multipart/form-data' } })
      .then((r) => r.data.data);
  },

  updateStatus: (id: number, payload: UpdateComplaintStatusRequest) =>
    axiosInstance.put<ApiResponse<ComplaintDto>>(`/complaints/${id}/status`, payload).then((r) => r.data.data),

  remove: (id: number) => axiosInstance.delete<ApiResponse<object>>(`/complaints/${id}`),
};
