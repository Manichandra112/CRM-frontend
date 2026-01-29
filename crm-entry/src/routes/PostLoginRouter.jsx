import { Navigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

const PostLoginRouter = () => {
  const { user, isAuthenticated, isAdmin, loading } = useAuth();

  if (loading) return null;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // ADMIN → Admin control plane
  if (isAdmin) {
    return <Navigate to="/admin" replace />;
  }

  const role =
    user?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

  if (!role) {
    return <Navigate to="/access-denied" replace />;
  }

  // ✅ SOCIAL USERS → Social Media CRM
  if (role.startsWith("SOCIAL")) {
  return <Navigate to="/crm/socialmedia" replace />;
}
  // SALES / HR (future)
  if (role.startsWith("SALES")) {
    return <Navigate to="/crm/sales" replace />;
  }

  if (role.startsWith("HR")) {
    return <Navigate to="/crm/hr" replace />;
  }

  return <Navigate to="/access-denied" replace />;
};

export default PostLoginRouter;
