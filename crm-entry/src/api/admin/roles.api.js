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

export const getUserRoles = (userId) => {
  return api.get(`/api/admin/user-roles/${userId}`);
};

export const assignUserRole = ({ userId, roleId }) => {
  return api.post("/api/admin/user-roles", {
    userId,
    roleId,
  });
};

export const removeUserRole = ({ userId, roleId }) => {
  return api.delete("/api/admin/user-roles", {
    data: {
      userId,
      roleId,
    },
  });
};
