import { Routes, Route, Navigate } from "react-router-dom";

import Dashboard from "@/pages/Dashboard";
import LayoutDashboard from "@/components/LayoutDashboard";
import FormEditor from "@/components/FormEditor";

const Router = () => {

  return (
    <Routes>
      <Route element={<LayoutDashboard />}>
        <Route index element={<Dashboard />} />
        <Route path="/forms/:formId/edit" element={<FormEditor />} />
      </Route>
      <Route path="*" element={<Navigate to="/" />} />
    </Routes>
  );
};

export default Router;
