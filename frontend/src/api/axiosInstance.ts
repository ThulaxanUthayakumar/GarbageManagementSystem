import axios from 'axios';

export const API_BASE_URL: string = import.meta.env.VITE_API_URL || 'http://localhost:5001/api';

/**
 * Origin of the API (without the trailing /api), used to build absolute URLs
 * for uploaded images, which are stored in the DB as relative paths like
 * "/uploads/requests/xxx.jpg".
 */
export const API_ORIGIN = API_BASE_URL.replace(/\/api\/?$/, '');

export function resolveImageUrl(relativePath?: string | null): string | undefined {
  if (!relativePath) return undefined;
  if (relativePath.startsWith('http')) return relativePath;
  return `${API_ORIGIN}${relativePath}`;
}

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
});

axiosInstance.interceptors.request.use((config) => {
  const token = window.localStorage.getItem('gms-token');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      window.localStorage.removeItem('gms-token');
      window.localStorage.removeItem('gms-user');
      if (!window.location.pathname.startsWith('/login')) {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  },
);

/** Pulls a human-readable message out of a failed axios call, falling back gracefully. */
export function extractErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { message?: string; errors?: string[] } | undefined;
    if (data?.errors && data.errors.length > 0) return data.errors.join(' ');
    if (data?.message) return data.message;
    if (error.message) return error.message;
  }
  return 'Something went wrong. Please try again.';
}

export default axiosInstance;
