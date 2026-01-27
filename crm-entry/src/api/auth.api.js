// src/api/auth.api.js
import api from "./axios";

export const login = async (payload) => {
  const response = await api.post("/api/auth/login", payload);
  return response.data;
};

export const refreshToken = async (payload) => {
  const response = await api.post("/api/auth/refresh", payload);
  return response.data;
};
