import axiosInstance from './axiosInstance';
import type {
  ApiResponse,
  CategoryCount,
  DashboardStats,
  MonthlyTrendPoint,
  ResidentDashboardSummary,
} from '../types';

export const dashboardApi = {
  getStats: () => axiosInstance.get<ApiResponse<DashboardStats>>('/dashboard/stats').then((r) => r.data.data),

  getResidentSummary: () =>
    axiosInstance.get<ApiResponse<ResidentDashboardSummary>>('/dashboard/resident-summary').then((r) => r.data.data),

  getCollectionTrend: (months = 6) =>
    axiosInstance
      .get<ApiResponse<MonthlyTrendPoint[]>>('/dashboard/collection-trend', { params: { months } })
      .then((r) => r.data.data),

  getWasteCategoryBreakdown: () =>
    axiosInstance.get<ApiResponse<CategoryCount[]>>('/dashboard/waste-category-breakdown').then((r) => r.data.data),
};
