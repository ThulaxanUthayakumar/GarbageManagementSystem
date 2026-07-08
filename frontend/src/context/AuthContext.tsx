import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import { authApi } from '../api/authApi';
import { extractErrorMessage } from '../api/axiosInstance';
import type { ChangePasswordRequest, LoginRequest, RegisterRequest, UserDto } from '../types';

interface AuthContextType {
  user: UserDto | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isLoading: boolean;
  login: (payload: LoginRequest) => Promise<void>;
  register: (payload: RegisterRequest) => Promise<void>;
  logout: () => void;
  changePassword: (payload: ChangePasswordRequest) => Promise<void>;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const TOKEN_KEY = 'gms-token';
const USER_KEY = 'gms-user';

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const storedUser = window.localStorage.getItem(USER_KEY);
    const storedToken = window.localStorage.getItem(TOKEN_KEY);

    if (storedUser && storedToken) {
      try {
        setUser(JSON.parse(storedUser));
      } catch {
        window.localStorage.removeItem(USER_KEY);
        window.localStorage.removeItem(TOKEN_KEY);
      }
    }
    setIsLoading(false);
  }, []);

  const persistSession = (token: string, userDto: UserDto) => {
    window.localStorage.setItem(TOKEN_KEY, token);
    window.localStorage.setItem(USER_KEY, JSON.stringify(userDto));
    setUser(userDto);
  };

  const login = async (payload: LoginRequest) => {
    const result = await authApi.login(payload);
    persistSession(result.token, result.user);
  };

  const register = async (payload: RegisterRequest) => {
    const result = await authApi.register(payload);
    persistSession(result.token, result.user);
  };

  const logout = () => {
    authApi.logout().catch(() => {
      // best-effort only; JWTs are stateless so client-side cleanup is what matters
    });
    window.localStorage.removeItem(TOKEN_KEY);
    window.localStorage.removeItem(USER_KEY);
    setUser(null);
  };

  const changePassword = async (payload: ChangePasswordRequest) => {
    await authApi.changePassword(payload);
  };

  const refreshUser = async () => {
    try {
      const freshUser = await authApi.me();
      window.localStorage.setItem(USER_KEY, JSON.stringify(freshUser));
      setUser(freshUser);
    } catch (error) {
      throw new Error(extractErrorMessage(error));
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        isAdmin: user?.role === 'Admin',
        isLoading,
        login,
        register,
        logout,
        changePassword,
        refreshUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuthContext() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuthContext must be used within an AuthProvider');
  }
  return context;
}
