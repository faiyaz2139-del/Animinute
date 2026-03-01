import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider, useAuth } from "./context/AuthContext";
import { ThemeProvider } from "./context/ThemeContext";
import { Toaster } from "./components/ui/sonner";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Employees from "./pages/Employees";
import PayPeriods from "./pages/PayPeriods";
import ImportTimesheets from "./pages/ImportTimesheets";
import PayrollRun from "./pages/PayrollRun";
import PayrollRunDetail from "./pages/PayrollRunDetail";
import Payslips from "./pages/Payslips";
import Reports from "./pages/Reports";
import Analytics from "./pages/Analytics";
import AuditLog from "./pages/AuditLog";
import Settings from "./pages/Settings";
import DashboardLayout from "./components/DashboardLayout";

// Any Minute imports
import { AMAuthProvider, useAMAuth } from "./anyminute/context/AMAuthContext";
import AMLogin from "./anyminute/pages/Login";
import AMSignup from "./anyminute/pages/Signup";
import AMDashboard from "./anyminute/pages/Dashboard";
import AMHome from "./anyminute/pages/Home";
import AMAddBusiness from "./anyminute/pages/AddBusiness";
import AMAddUser from "./anyminute/pages/AddUser";
import AMEditUser from "./anyminute/pages/EditUser";
import AMAddProject from "./anyminute/pages/AddProject";
import AMTimesheet from "./anyminute/pages/Timesheet";
import AMSchedule from "./anyminute/pages/Schedule";
import AMReports from "./anyminute/pages/Reports";
import AMSettings from "./anyminute/pages/Settings";
import AMBilling from "./anyminute/pages/Billing";
import AMPayRate from "./anyminute/pages/PayRate";
import AMTickets from "./anyminute/pages/Tickets";
import AMAuditLogs from "./anyminute/pages/AuditLogs";
import { TestChecklist as AMTestChecklist } from "./anyminute/pages/ComingSoon";

import "./App.css";

const ProtectedRoute = ({ children }) => {
  const { user, loading } = useAuth();
  
  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      </div>
    );
  }
  
  if (!user) {
    return <Navigate to="/login" replace />;
  }
  
  return children;
};

const AdminRoute = ({ children }) => {
  const { user } = useAuth();
  
  if (user?.role !== 'admin' && user?.role !== 'manager') {
    return <Navigate to="/dashboard" replace />;
  }
  
  return children;
};

// Any Minute Protected Route
const AMProtectedRoute = ({ children }) => {
  const { user, loading } = useAMAuth();
  
  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      </div>
    );
  }
  
  if (!user) {
    return <Navigate to="/anyminute/login" replace />;
  }
  
  return children;
};

const AMAdminRoute = ({ children }) => {
  const { user, isAdmin } = useAMAuth();
  
  if (!isAdmin) {
    return <Navigate to="/anyminute/dashboard" replace />;
  }
  
  return children;
};

function AppRoutes() {
  const { user } = useAuth();
  
  return (
    <Routes>
      <Route path="/login" element={user ? <Navigate to="/dashboard" replace /> : <Login />} />
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      
      <Route element={<ProtectedRoute><DashboardLayout /></ProtectedRoute>}>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/employees" element={<AdminRoute><Employees /></AdminRoute>} />
        <Route path="/pay-periods" element={<AdminRoute><PayPeriods /></AdminRoute>} />
        <Route path="/import-timesheets" element={<AdminRoute><ImportTimesheets /></AdminRoute>} />
        <Route path="/payroll-runs" element={<AdminRoute><PayrollRun /></AdminRoute>} />
        <Route path="/payroll-runs/:runId" element={<AdminRoute><PayrollRunDetail /></AdminRoute>} />
        <Route path="/payslips" element={<Payslips />} />
        <Route path="/reports" element={<AdminRoute><Reports /></AdminRoute>} />
        <Route path="/analytics" element={<AdminRoute><Analytics /></AdminRoute>} />
        <Route path="/audit-log" element={<AdminRoute><AuditLog /></AdminRoute>} />
        <Route path="/settings" element={<AdminRoute><Settings /></AdminRoute>} />
      </Route>
    </Routes>
  );
}

function AllRoutes() {
  const { user: pcUser } = useAuth();
  const { user: amUser } = useAMAuth();
  
  return (
    <Routes>
      {/* Payroll Canada Routes */}
      <Route path="/login" element={pcUser ? <Navigate to="/dashboard" replace /> : <Login />} />
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route element={<ProtectedRoute><DashboardLayout /></ProtectedRoute>}>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/employees" element={<AdminRoute><Employees /></AdminRoute>} />
        <Route path="/pay-periods" element={<AdminRoute><PayPeriods /></AdminRoute>} />
        <Route path="/import-timesheets" element={<AdminRoute><ImportTimesheets /></AdminRoute>} />
        <Route path="/payroll-runs" element={<AdminRoute><PayrollRun /></AdminRoute>} />
        <Route path="/payroll-runs/:runId" element={<AdminRoute><PayrollRunDetail /></AdminRoute>} />
        <Route path="/payslips" element={<Payslips />} />
        <Route path="/reports" element={<AdminRoute><Reports /></AdminRoute>} />
        <Route path="/analytics" element={<AdminRoute><Analytics /></AdminRoute>} />
        <Route path="/audit-log" element={<AdminRoute><AuditLog /></AdminRoute>} />
        <Route path="/settings" element={<AdminRoute><Settings /></AdminRoute>} />
      </Route>
      
      {/* Any Minute Routes */}
      <Route path="/anyminute/login" element={amUser ? <Navigate to="/anyminute/dashboard" replace /> : <AMLogin />} />
      <Route path="/anyminute/signup" element={amUser ? <Navigate to="/anyminute/dashboard" replace /> : <AMSignup />} />
      <Route path="/anyminute/dashboard" element={<AMProtectedRoute><AMDashboard /></AMProtectedRoute>} />
      <Route path="/anyminute/home" element={<AMProtectedRoute><AMHome /></AMProtectedRoute>} />
      <Route path="/anyminute/add-business" element={<AMProtectedRoute><AMAdminRoute><AMAddBusiness /></AMAdminRoute></AMProtectedRoute>} />
      <Route path="/anyminute/add-user" element={<AMProtectedRoute><AMAdminRoute><AMAddUser /></AMAdminRoute></AMProtectedRoute>} />
      <Route path="/anyminute/edit-user" element={<AMProtectedRoute><AMAdminRoute><AMEditUser /></AMAdminRoute></AMProtectedRoute>} />
      <Route path="/anyminute/add-project" element={<AMProtectedRoute><AMAddProject /></AMProtectedRoute>} />
      <Route path="/anyminute/timesheet" element={<AMProtectedRoute><AMTimesheet /></AMProtectedRoute>} />
      <Route path="/anyminute/schedule" element={<AMProtectedRoute><AMSchedule /></AMProtectedRoute>} />
      <Route path="/anyminute/reports" element={<AMProtectedRoute><AMReports /></AMProtectedRoute>} />
      <Route path="/anyminute/tickets" element={<AMProtectedRoute><AMTickets /></AMProtectedRoute>} />
      <Route path="/anyminute/audit-logs" element={<AMProtectedRoute><AMAdminRoute><AMAuditLogs /></AMAdminRoute></AMProtectedRoute>} />
      <Route path="/anyminute/plan-upgrade" element={<AMProtectedRoute><AMBilling /></AMProtectedRoute>} />
      <Route path="/anyminute/pay-rates" element={<AMProtectedRoute><AMAdminRoute><AMPayRate /></AMAdminRoute></AMProtectedRoute>} />
      <Route path="/anyminute/test-checklist" element={<AMProtectedRoute><AMTestChecklist /></AMProtectedRoute>} />
      <Route path="/anyminute/settings" element={<AMProtectedRoute><AMAdminRoute><AMSettings /></AMAdminRoute></AMProtectedRoute>} />
      <Route path="/anyminute" element={<Navigate to="/anyminute/login" replace />} />
    </Routes>
  );
}

function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <AMAuthProvider>
          <BrowserRouter>
            <AllRoutes />
            <Toaster position="top-right" richColors />
          </BrowserRouter>
        </AMAuthProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
