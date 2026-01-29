
import api from "../axios";

export const getAdminUsers = (params) => {
  return api.get("/api/admin/users", { params });
};

export const getAdminUserById = (userId) => {
  return api.get(`/api/admin/users/${userId}`);
};

export const getUserSecurity = (userId) => {
  return api.get(`/api/admin/users/${userId}/security`);
};

export const getUserAuditLogs = (userId, params) => {
  return api.get(`/api/admin/users/${userId}/audit-logs`, {
    params,
  });
};
