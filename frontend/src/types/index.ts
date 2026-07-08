// ============================================================
// These types mirror the backend DTOs exactly. The API serializes enums as
// strings (see Program.cs JsonStringEnumConverter), so status fields below
// are string literal unions rather than numbers.
// ============================================================

export type UserRole = 'Admin' | 'Resident';

export type RequestStatus = 'Pending' | 'InProgress' | 'Completed' | 'Cancelled';
export type ScheduleStatus = 'Scheduled' | 'Completed' | 'Missed' | 'Cancelled';
export type ComplaintStatus = 'Open' | 'InProgress' | 'Resolved' | 'Closed';

// ---------- Common ----------

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[] | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
}

// ---------- Auth ----------

export interface UserDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string | null;
  role: UserRole;
  profileImageUrl?: string | null;
  residentId?: number | null;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: UserDto;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
  phoneNumber: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  zone: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

// ---------- Residents ----------

export interface ResidentDto {
  id: number;
  applicationUserId: string;
  fullName: string;
  email: string;
  phoneNumber?: string | null;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  zone: string;
  isActive: boolean;
  dateJoined: string;
  totalRequests: number;
  totalComplaints: number;
}

export interface CreateResidentRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  zone: string;
}

export interface UpdateResidentRequest {
  fullName: string;
  phoneNumber: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  zone: string;
  isActive: boolean;
}

// ---------- Waste Categories ----------

export interface WasteCategoryDto {
  id: number;
  name: string;
  description?: string | null;
  iconName?: string | null;
}

// ---------- Collection Requests ----------

export interface CollectionRequestDto {
  id: number;
  residentId: number;
  residentName: string;
  zone: string;
  wasteCategoryId: number;
  wasteCategoryName: string;
  description?: string | null;
  imageUrl?: string | null;
  pickupDate: string;
  status: RequestStatus;
  createdAt: string;
  updatedAt?: string | null;
  completedDate?: string | null;
}

export interface CreateCollectionRequestPayload {
  wasteCategoryId: number;
  description?: string;
  pickupDate: string;
  image?: File | null;
}

// ---------- Schedules ----------

export interface ScheduleDto {
  id: number;
  zone: string;
  wasteCategoryId?: number | null;
  wasteCategoryName: string;
  scheduledDate: string;
  scheduledTime: string;
  collectorName?: string | null;
  status: ScheduleStatus;
  notes?: string | null;
  createdAt: string;
}

export interface CreateScheduleRequest {
  zone: string;
  wasteCategoryId?: number | null;
  scheduledDate: string;
  scheduledTime: string;
  collectorName?: string;
  notes?: string;
}

export interface UpdateScheduleRequest extends CreateScheduleRequest {
  status: ScheduleStatus;
}

// ---------- Complaints ----------

export interface ComplaintDto {
  id: number;
  residentId: number;
  residentName: string;
  zone: string;
  subject: string;
  description: string;
  imageUrl?: string | null;
  status: ComplaintStatus;
  adminRemarks?: string | null;
  createdAt: string;
  updatedAt?: string | null;
  resolvedDate?: string | null;
}

export interface CreateComplaintPayload {
  subject: string;
  description: string;
  image?: File | null;
}

export interface UpdateComplaintStatusRequest {
  status: ComplaintStatus;
  adminRemarks?: string;
}

// ---------- Dashboard ----------

export interface DashboardStats {
  totalResidents: number;
  activeResidents: number;
  pendingRequests: number;
  inProgressRequests: number;
  completedCollections: number;
  totalComplaints: number;
  openComplaints: number;
  upcomingSchedules: number;
}

export interface ResidentDashboardSummary {
  myTotalRequests: number;
  myPendingRequests: number;
  myCompletedRequests: number;
  myOpenComplaints: number;
  myTotalComplaints: number;
  zone: string;
}

export interface MonthlyTrendPoint {
  month: string;
  completed: number;
  pending: number;
  cancelled: number;
}

export interface CategoryCount {
  category: string;
  count: number;
}

// ---------- Reports ----------

export interface DailyCollectionReport {
  date: string;
  totalRequests: number;
  completed: number;
  pending: number;
  inProgress: number;
  cancelled: number;
  byCategory: CategoryCount[];
}

export interface DailyPoint {
  day: number;
  count: number;
}

export interface MonthlyCollectionReport {
  year: number;
  month: number;
  monthName: string;
  totalRequests: number;
  completed: number;
  pending: number;
  inProgress: number;
  cancelled: number;
  dailyBreakdown: DailyPoint[];
}

export interface ComplaintStatistics {
  total: number;
  open: number;
  inProgress: number;
  resolved: number;
  closed: number;
  averageResolutionDays: number;
}

export interface WasteCategoryStatistics {
  totalRequests: number;
  categories: CategoryCount[];
}
