import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function RequireAuth() {
  const {
    loading,
    authChecking,
    isAuthenticated,
    pwdResetRequired,
  } = useAuth();

  if (loading || authChecking) return null;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (pwdResetRequired) {
    return <Navigate to="/reset-password" replace />;
  }

  return <Outlet />;
}
