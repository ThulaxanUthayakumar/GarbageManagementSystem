import axiosInstance from './axiosInstance';
import type {
  ApiResponse,
  CollectionRequestDto,
  CreateCollectionRequestPayload,
  PagedResult,
  RequestStatus,
} from '../types';

export const collectionRequestApi = {
  getPaged: (pageNumber: number, pageSize: number, searchTerm?: string, status?: RequestStatus) =>
    axiosInstance
      .get<ApiResponse<PagedResult<CollectionRequestDto>>>('/collectionrequests', {
        params: { pageNumber, pageSize, searchTerm: searchTerm || undefined, status: status || undefined },
      })
      .then((r) => r.data.data),

  getMy: () => axiosInstance.get<ApiResponse<CollectionRequestDto[]>>('/collectionrequests/my').then((r) => r.data.data),

  getById: (id: number) =>
    axiosInstance.get<ApiResponse<CollectionRequestDto>>(`/collectionrequests/${id}`).then((r) => r.data.data),

  create: (payload: CreateCollectionRequestPayload) => {
    const form = new FormData();
    form.append('wasteCategoryId', String(payload.wasteCategoryId));
    form.append('pickupDate', payload.pickupDate);
    if (payload.description) form.append('description', payload.description);
    if (payload.image) form.append('image', payload.image);

    return axiosInstance
      .post<ApiResponse<CollectionRequestDto>>('/collectionrequests', form, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
      .then((r) => r.data.data);
  },

  updateStatus: (id: number, status: RequestStatus) =>
    axiosInstance
      .put<ApiResponse<CollectionRequestDto>>(`/collectionrequests/${id}/status`, { status })
      .then((r) => r.data.data),

  remove: (id: number) => axiosInstance.delete<ApiResponse<object>>(`/collectionrequests/${id}`),
};
