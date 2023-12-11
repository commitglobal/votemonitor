import React from "react";
import ReactDOM from "react-dom/client";
import { Provider } from "react-redux";
import { BrowserRouter } from "react-router-dom";
import Router from "./router";
import "./index.css";
import { store } from "./redux/store";
import { ToastContainer } from "react-toastify";
import ScrollToTop from "@/components/ScrollToTop";
import 'react-toastify/dist/ReactToastify.css';

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <Provider store={store}>
      <BrowserRouter>
        <ToastContainer position="top-center" />
        {/* <AuthMiddleware> */}
          <Router />
        {/* </AuthMiddleware> */}
        <ScrollToTop />
      </BrowserRouter>
    </Provider>
  </React.StrictMode>
);
