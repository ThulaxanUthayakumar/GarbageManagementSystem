import axiosInstance from './axiosInstance';
import type {
  ApiResponse,
  ComplaintStatistics,
  DailyCollectionReport,
  MonthlyCollectionReport,
  WasteCategoryStatistics,
} from '../types';

export const reportApi = {
  getDailyCollections: (date?: string) =>
    axiosInstance
      .get<ApiResponse<DailyCollectionReport>>('/reports/daily-collections', { params: { date } })
      .then((r) => r.data.data),

  getMonthlyCollections: (year?: number, month?: number) =>
    axiosInstance
      .get<ApiResponse<MonthlyCollectionReport>>('/reports/monthly-collections', { params: { year, month } })
      .then((r) => r.data.data),

  getComplaintStatistics: () =>
    axiosInstance.get<ApiResponse<ComplaintStatistics>>('/reports/complaint-statistics').then((r) => r.data.data),

  getWasteCategoryStatistics: () =>
    axiosInstance.get<ApiResponse<WasteCategoryStatistics>>('/reports/waste-category-statistics').then((r) => r.data.data),
};
