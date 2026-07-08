import { Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './components/ProtectedRoute';
import MainLayout from './components/layout/MainLayout';
import LoginPage from './pages/auth/LoginPage';
import RegisterPage from './pages/auth/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import ResidentsPage from './pages/residents/ResidentsPage';
import CollectionRequestsPage from './pages/collectionRequests/CollectionRequestsPage';
import SchedulesPage from './pages/schedules/SchedulesPage';
import ComplaintsPage from './pages/complaints/ComplaintsPage';
import ReportsPage from './pages/reports/ReportsPage';
import ProfilePage from './pages/profile/ProfilePage';
import NotFoundPage from './pages/NotFoundPage';
import UnauthorizedPage from './pages/UnauthorizedPage';

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/unauthorized" element={<UnauthorizedPage />} />

      {/* Any authenticated user (Admin or Resident) */}
      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/requests" element={<CollectionRequestsPage />} />
          <Route path="/schedules" element={<SchedulesPage />} />
          <Route path="/complaints" element={<ComplaintsPage />} />
          <Route path="/profile" element={<ProfilePage />} />

          {/* Admin-only */}
          <Route element={<ProtectedRoute allowedRoles={['Admin']} />}>
            <Route path="/residents" element={<ResidentsPage />} />
            <Route path="/reports" element={<ReportsPage />} />
          </Route>
        </Route>
      </Route>

      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
