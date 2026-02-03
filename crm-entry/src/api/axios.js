// // src/api/axios.js
// import axios from "axios";

// const api = axios.create({
//   baseURL:   
//   "https://albertine-nonempathic-heaven.ngrok-free.dev",
// //  "https://localhost:7246",
//   headers: {
//     "Content-Type": "application/json",
//   },
// });

// // Attach token on every request
// api.interceptors.request.use(
//   (config) => {
//     const token = localStorage.getItem("accessToken");
//     if (token) {
//       config.headers.Authorization = `Bearer ${token}`;
//     }
//     return config;
//   },
//   (error) => Promise.reject(error)
// );

// // Global 401 handling
// api.interceptors.response.use(
//   (response) => response,
//   (error) => {
//     if (error.response?.status === 401) {
//       localStorage.clear();
//       window.location.href = "/login";
//     }
//     return Promise.reject(error);
//   }
// );

// export default api;

// src/api/axios.js
import axios from "axios";

const api = axios.create({
  baseURL: "https://albertine-nonempathic-heaven.ngrok-free.dev",
  // baseURL: "https://localhost:7246",
  headers: {
    "Content-Type": "application/json",
    "ngrok-skip-browser-warning": "true", // 🔥 THIS IS THE FIX
  },
});

// Attach token on every request
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Global 401 handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.clear();
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default api;
