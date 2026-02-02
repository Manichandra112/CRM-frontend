import api from "../axios";

export const updateUser = (userId, payload) => {
  return api.put(`/api/users/${userId}`, payload);
};

export const lockUser = (userId, reason = "Locked by admin") => {
  return api.put(`/api/users/${userId}/lock`, {
    reason,
  });
};

export const unlockUser = (userId) => {
  return api.put(`/api/users/${userId}/unlock`);
};


export const assignManager = (userId, managerId) => {
  return api.put(`/api/users/${userId}/manager`, {
    managerId,
  });
};
export const getManagers = () => {
  return api.get("/api/users/admin/managers");
};


export const createUser = (payload) => {
  return api.post("/api/users", payload);
};

