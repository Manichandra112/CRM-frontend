import { Navigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

const PostLoginRouter = () => {
  const {
    user,
    isAuthenticated,
    isAdmin,
    pwdResetRequired,
    loading,
    authChecking,
  } = useAuth();

  if (loading || authChecking) return null;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (pwdResetRequired) {
    return <Navigate to="/reset-password" replace />;
  }

  if (isAdmin) {
    return <Navigate to="/admin/users" replace />;
  }

  const role =
    user?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

  if (!role) {
    return <Navigate to="/access-denied" replace />;
  }

  if (role.startsWith("SOCIAL")) {
    return <Navigate to="/crm/socialmedia" replace />;
  }

  if (role.startsWith("SALES")) {
    return <Navigate to="/crm/sales" replace />;
  }

  if (role.startsWith("HR")) {
    return <Navigate to="/crm/hr" replace />;
  }

  return <Navigate to="/access-denied" replace />;
};

export default PostLoginRouter;
