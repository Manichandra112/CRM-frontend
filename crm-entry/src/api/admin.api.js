// import api from "./axios";

// /* =======================
//    ADMIN USERS
//    ======================= */

// export const getAdminUsers = (page = 1, pageSize = 25) =>
//   api.get("/api/admin/users", {
//     params: { page, pageSize },
//   });

// export const getAdminUserDetails = (userId) =>
//   api.get(`/api/admin/users/${userId}`);

// export const getAdminUserSecurity = (userId) =>
//   api.get(`/api/admin/users/${userId}/security`);

// export const getAdminUserAuditLogs = (userId) =>
//   api.get(`/api/admin/users/${userId}/audit-logs`);

// /* =======================
//    ADMIN ROLES & PERMISSIONS
//    ======================= */

// export const getRoles = () =>
//   api.get("/api/admin/roles");

// export const getPermissions = () =>
//   api.get("/api/admin/permissions");

// /* =======================
//    USER COMMANDS
//    ======================= */

// export const lockUser = (userId) =>
//   api.put(`/api/users/${userId}/lock`, {});

// export const unlockUser = (userId) =>
//   api.put(`/api/users/${userId}/unlock`);
