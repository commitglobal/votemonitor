// vite.config.ts
import { TanStackRouterVite } from "file:///C:/Users/iod/Documents/GitHub/votemonitor/web/node_modules/.pnpm/@tanstack+router-plugin@1.9_bc9a2dcea164b2db82d8acfbaed72462/node_modules/@tanstack/router-plugin/dist/esm/vite.js";
import react from "file:///C:/Users/iod/Documents/GitHub/votemonitor/web/node_modules/.pnpm/@vitejs+plugin-react-swc@3._161aef0f9ecaf5dbcf8e37a0aba3156f/node_modules/@vitejs/plugin-react-swc/index.mjs";
import path from "node:path";
import { normalizePath } from "file:///C:/Users/iod/Documents/GitHub/votemonitor/web/node_modules/.pnpm/vite@5.4.2_@types+node@20.17.13_less@4.2.0_terser@5.29.2/node_modules/vite/dist/node/index.js";
import { viteStaticCopy } from "file:///C:/Users/iod/Documents/GitHub/votemonitor/web/node_modules/.pnpm/vite-plugin-static-copy@1.0_1c2fb56a8496335a7d0feb6007cdc4b1/node_modules/vite-plugin-static-copy/dist/index.js";
import { defineConfig } from "file:///C:/Users/iod/Documents/GitHub/votemonitor/web/node_modules/.pnpm/vitest@0.33.0_jsdom@22.1.0__0b1f028f6f583742f1a1ef7b57277f1c/node_modules/vitest/dist/config.js";
var __vite_injected_original_dirname = "C:\\Users\\iod\\Documents\\GitHub\\votemonitor\\web";
var vite_config_default = defineConfig({
  plugins: [
    react(),
    TanStackRouterVite(),
    viteStaticCopy({
      targets: [
        {
          src: normalizePath(path.resolve("./src/assets/locales")),
          dest: normalizePath(path.resolve("./dist"))
        }
      ]
    })
  ],
  server: {
    host: true,
    strictPort: true
  },
  resolve: {
    alias: {
      "@": path.resolve(__vite_injected_original_dirname, "./src")
    }
  },
  test: {
    environment: "jsdom",
    setupFiles: ["./vitest.setup.ts"],
    css: true
  }
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJDOlxcXFxVc2Vyc1xcXFxpb2RcXFxcRG9jdW1lbnRzXFxcXEdpdEh1YlxcXFx2b3RlbW9uaXRvclxcXFx3ZWJcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfZmlsZW5hbWUgPSBcIkM6XFxcXFVzZXJzXFxcXGlvZFxcXFxEb2N1bWVudHNcXFxcR2l0SHViXFxcXHZvdGVtb25pdG9yXFxcXHdlYlxcXFx2aXRlLmNvbmZpZy50c1wiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9pbXBvcnRfbWV0YV91cmwgPSBcImZpbGU6Ly8vQzovVXNlcnMvaW9kL0RvY3VtZW50cy9HaXRIdWIvdm90ZW1vbml0b3Ivd2ViL3ZpdGUuY29uZmlnLnRzXCI7aW1wb3J0IHsgVGFuU3RhY2tSb3V0ZXJWaXRlIH0gZnJvbSBcIkB0YW5zdGFjay9yb3V0ZXItcGx1Z2luL3ZpdGVcIlxyXG5pbXBvcnQgcmVhY3QgZnJvbSBcIkB2aXRlanMvcGx1Z2luLXJlYWN0LXN3Y1wiO1xyXG5pbXBvcnQgcGF0aCBmcm9tIFwibm9kZTpwYXRoXCI7XHJcbmltcG9ydCB7IG5vcm1hbGl6ZVBhdGggfSBmcm9tIFwidml0ZVwiO1xyXG5pbXBvcnQgeyB2aXRlU3RhdGljQ29weSB9IGZyb20gXCJ2aXRlLXBsdWdpbi1zdGF0aWMtY29weVwiO1xyXG5pbXBvcnQgeyBkZWZpbmVDb25maWcgfSBmcm9tIFwidml0ZXN0L2NvbmZpZ1wiO1xyXG5cclxuLy8gaHR0cHM6Ly92aXRlanMuZGV2L2NvbmZpZy9cclxuZXhwb3J0IGRlZmF1bHQgZGVmaW5lQ29uZmlnKHtcclxuXHRwbHVnaW5zOiBbcmVhY3QoKSwgVGFuU3RhY2tSb3V0ZXJWaXRlKCksIFxyXG5cdFx0dml0ZVN0YXRpY0NvcHkoe1xyXG5cdFx0dGFyZ2V0czogW1xyXG5cdFx0ICB7XHJcblx0XHRcdHNyYzogbm9ybWFsaXplUGF0aChwYXRoLnJlc29sdmUoJy4vc3JjL2Fzc2V0cy9sb2NhbGVzJykpLFxyXG5cdFx0XHRkZXN0OiBub3JtYWxpemVQYXRoKHBhdGgucmVzb2x2ZSgnLi9kaXN0JykpXHJcblx0XHQgIH1cclxuXHRcdF1cclxuXHQgIH0pXSxcclxuXHRzZXJ2ZXI6IHtcclxuXHRcdGhvc3Q6IHRydWUsXHJcblx0XHRzdHJpY3RQb3J0OiB0cnVlLFxyXG5cdH0sXHJcbiAgcmVzb2x2ZToge1xyXG4gICAgYWxpYXM6IHtcclxuICAgICAgJ0AnOiBwYXRoLnJlc29sdmUoX19kaXJuYW1lLCAnLi9zcmMnKSxcclxuICAgIH0sXHJcbiAgfSxcclxuXHR0ZXN0OiB7XHJcblx0XHRlbnZpcm9ubWVudDogXCJqc2RvbVwiLFxyXG5cdFx0c2V0dXBGaWxlczogW1wiLi92aXRlc3Quc2V0dXAudHNcIl0sXHJcblx0XHRjc3M6IHRydWUsXHJcblx0fSxcclxufSk7Il0sCiAgIm1hcHBpbmdzIjogIjtBQUF1VSxTQUFTLDBCQUEwQjtBQUMxVyxPQUFPLFdBQVc7QUFDbEIsT0FBTyxVQUFVO0FBQ2pCLFNBQVMscUJBQXFCO0FBQzlCLFNBQVMsc0JBQXNCO0FBQy9CLFNBQVMsb0JBQW9CO0FBTDdCLElBQU0sbUNBQW1DO0FBUXpDLElBQU8sc0JBQVEsYUFBYTtBQUFBLEVBQzNCLFNBQVM7QUFBQSxJQUFDLE1BQU07QUFBQSxJQUFHLG1CQUFtQjtBQUFBLElBQ3JDLGVBQWU7QUFBQSxNQUNmLFNBQVM7QUFBQSxRQUNQO0FBQUEsVUFDRCxLQUFLLGNBQWMsS0FBSyxRQUFRLHNCQUFzQixDQUFDO0FBQUEsVUFDdkQsTUFBTSxjQUFjLEtBQUssUUFBUSxRQUFRLENBQUM7QUFBQSxRQUN6QztBQUFBLE1BQ0Y7QUFBQSxJQUNDLENBQUM7QUFBQSxFQUFDO0FBQUEsRUFDSixRQUFRO0FBQUEsSUFDUCxNQUFNO0FBQUEsSUFDTixZQUFZO0FBQUEsRUFDYjtBQUFBLEVBQ0MsU0FBUztBQUFBLElBQ1AsT0FBTztBQUFBLE1BQ0wsS0FBSyxLQUFLLFFBQVEsa0NBQVcsT0FBTztBQUFBLElBQ3RDO0FBQUEsRUFDRjtBQUFBLEVBQ0QsTUFBTTtBQUFBLElBQ0wsYUFBYTtBQUFBLElBQ2IsWUFBWSxDQUFDLG1CQUFtQjtBQUFBLElBQ2hDLEtBQUs7QUFBQSxFQUNOO0FBQ0QsQ0FBQzsiLAogICJuYW1lcyI6IFtdCn0K
