// src/api/admin.api.js
import api from "./axios";

/* =======================
   ADMIN USERS (CONTROL PANEL)
   ======================= */

// Admin → Users list (Zoho-style)
export const getAdminUsers = (page = 1, pageSize = 25) =>
  api.get("/api/admin/users", {
    params: { page, pageSize },
  });

// Admin → User details
export const getAdminUserDetails = (userId) =>
  api.get(`/api/admin/users/${userId}`);

// Admin → User security info
export const getAdminUserSecurity = (userId) =>
  api.get(`/api/admin/users/${userId}/security`);

// Admin → User audit logs
export const getAdminUserAuditLogs = (userId) =>
  api.get(`/api/admin/users/${userId}/audit-logs`);

/* =======================
   ADMIN CONFIG
   ======================= */

export const getDomains = () =>
  api.get("/api/admin/domains");

export const getRoles = () =>
  api.get("/api/admin/roles");

export const getPermissions = () =>
  api.get("/api/admin/permissions");

/* =======================
   USER ACTIONS (COMMANDS)
   ======================= */

export const lockUser = (userId) =>
  api.put(`/api/users/${userId}/lock`, {});

export const unlockUser = (userId) =>
  api.put(`/api/users/${userId}/unlock`);

// future
// export const assignManager = (userId, managerId) =>
//   api.put(`/api/users/${userId}/manager`, { managerId });
