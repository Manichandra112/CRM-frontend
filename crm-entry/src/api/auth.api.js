// import api from "./axios";

// export const login = (payload) => {
//   return api.post("/api/auth/login", payload);
// };


// export const changePassword = (payload) => {
//   return api.post("/api/auth/change-password", payload);
// };


// export const forgotPassword = (payload) => {
//   return api.post("/api/auth/forgot-password", payload);
// };


// export const resetForgotPassword = (payload) => {
//   return api.post("/api/auth/reset-forgot-password", payload);
// };


import api from "./axios";

export const login = (payload) => {
  return api.post("/api/auth/login", payload, {
    skipAuth: true,
  });
};

export const forceChangePassword = (payload) => {
  return api.post("/api/auth/change-password", payload);
};

export const forgotPassword = (email) => {
  return api.post(
    "/api/auth/forgot-password",
    { email },
    { skipAuth: true }
  );
};

export const resetForgotPassword = (payload) => {
  return api.post(
    "/api/auth/reset-forgot-password",
    payload,
    { skipAuth: true }
  );
};
