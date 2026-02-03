// import api from "../axios";

// /* ========= ROLE MASTER ========= */

// export const getAdminRoles = () => {
//   return api.get("/api/admin/roles");
// };

// export const createRole = (payload) => {
//   return api.post("/api/admin/roles", payload);
// };

// export const updateRole = (roleId, payload) => {
//   return api.put(`/api/admin/roles/${roleId}`, payload);
// };

// /* ========= USER ↔ ROLE MAPPING ========= */

// /**
//  * Get roles assigned to a user
//  */
// export const getUserRoles = (userId) => {
//   return api.get(`/api/admin/user-roles/${userId}`);
// };

// /**
//  * Assign role to user
//  * Backend expects: { userId, roleCode }
//  */
// export const assignUserRole = ({ userId, roleCode }) => {
//   return api.post("/api/admin/user-roles", {
//     userId,
//     roleCode,
//   });
// };

// /**
//  * Remove role from user
//  * DELETE must send body via `data`
//  */
// export const removeUserRole = ({ userId, roleCode }) => {
//   return api.delete("/api/admin/user-roles", {
//     data: {
//       userId,
//       roleCode,
//     },
//   });
// };



import api from "../axios";

/* ========= ROLE MASTER ========= */

export const getAdminRoles = async () => {
  const res = await api.get("/api/admin/roles");
  return res.data; // ARRAY
};

export const createRole = async (payload) => {
  const res = await api.post("/api/admin/roles", payload);
  return res.data;
};

export const updateRole = async (roleId, payload) => {
  const res = await api.put(`/api/admin/roles/${roleId}`, payload);
  return res.data;
};

/* ========= USER ↔ ROLE MAPPING ========= */

export const getUserRoles = async (userId) => {
  const res = await api.get(`/api/admin/user-roles/${userId}`);
  return res.data;
};

export const assignUserRole = async ({ userId, roleCode }) => {
  const res = await api.post("/api/admin/user-roles", {
    userId,
    roleCode,
  });
  return res.data;
};

export const removeUserRole = async ({ userId, roleCode }) => {
  const res = await api.delete("/api/admin/user-roles", {
    data: { userId, roleCode },
  });
  return res.data;
};
