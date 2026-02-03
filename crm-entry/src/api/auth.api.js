import api from "./axios";

export const login = async (payload) => {
  const res = await api.post("/api/auth/login", payload, {
    skipAuth: true,
  });
  return res.data;
};

export const forceChangePassword = async (payload) => {
  const res = await api.post("/api/auth/change-password", payload);
  return res.data;
};

export const forgotPassword = async (email) => {
  const res = await api.post(
    "/api/auth/forgot-password",
    { email },
    { skipAuth: true }
  );
  return res.data;
};

export const resetForgotPassword = async (payload) => {
  const res = await api.post(
    "/api/auth/reset-forgot-password",
    payload,
    { skipAuth: true }
  );
  return res.data;
};
