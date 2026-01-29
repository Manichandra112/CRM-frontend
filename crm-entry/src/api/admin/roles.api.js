import api from "../axios";

/* ========= ROLE MASTER ========= */

export const getAdminRoles = () => {
  return api.get("/api/admin/roles");
};

export const createRole = (payload) => {
  return api.post("/api/admin/roles", payload);
};

export const updateRole = (roleId, payload) => {
  return api.put(`/api/admin/roles/${roleId}`, payload);
};

/* ========= USER ↔ ROLE MAPPING ========= */

/**
 * Get roles assigned to a user
 */
export const getUserRoles = (userId) => {
  return api.get(`/api/admin/user-roles/${userId}`);
};

/**
 * Assign role to user
 * Backend expects: { userId, roleCode }
 */
export const assignUserRole = ({ userId, roleCode }) => {
  return api.post("/api/admin/user-roles", {
    userId,
    roleCode,
  });
};

/**
 * Remove role from user
 * DELETE must send body via `data`
 */
export const removeUserRole = ({ userId, roleCode }) => {
  return api.delete("/api/admin/user-roles", {
    data: {
      userId,
      roleCode,
    },
  });
};
