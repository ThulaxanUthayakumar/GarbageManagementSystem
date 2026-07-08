import axiosInstance from './axiosInstance';
import type { ApiResponse, WasteCategoryDto } from '../types';

export const wasteCategoryApi = {
  getAll: () => axiosInstance.get<ApiResponse<WasteCategoryDto[]>>('/wastecategories').then((r) => r.data.data),
};
