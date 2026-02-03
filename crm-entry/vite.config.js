import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],

  server: {
    host: "localhost",   
    port: 5173,          // fixed port
    strictPort: true,    // fail if port is already in use
    open: true,          // auto-open browser
  },
});
