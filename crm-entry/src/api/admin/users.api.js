
// import api from "../axios";

// export const getAdminUsers = (params) => {
//   return api.get("/api/admin/users", { params });
// };

// export const getAdminUserById = (userId) => {
//   return api.get(`/api/admin/users/${userId}`);
// };

// export const getUserSecurity = (userId) => {
//   return api.get(`/api/admin/users/${userId}/security`);
// };

// export const getUserAuditLogs = (userId, params) => {
//   return api.get(`/api/admin/users/${userId}/audit-logs`, {
//     params,
//   });
// };



import api from "../axios";

/* ========= USERS ========= */

export const getAdminUsers = async (params) => {
  const res = await api.get("/api/admin/users", { params });
  return res.data;
};

export const getAdminUserById = async (userId) => {
  const res = await api.get(`/api/admin/users/${userId}`);
  return res.data;
};

export const getUserSecurity = async (userId) => {
  const res = await api.get(`/api/admin/users/${userId}/security`);
  return res.data;
};

export const getUserAuditLogs = async (userId, params) => {
  const res = await api.get(
    `/api/admin/users/${userId}/audit-logs`,
    { params }
  );
  return res.data;
};
