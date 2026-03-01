import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAMAuth } from '../context/AMAuthContext';
import {
  LayoutDashboard, Home, Building2, Users, UserPlus, UserCog,
  FolderPlus, FileText, Calendar, Ticket, CreditCard, Settings, LogOut, TestTube, DollarSign, ClipboardList
} from 'lucide-react';

// Nav items with role-based access control
// allowedRoles: array of roles that can see this item (empty = all roles)
const navItems = [
  { to: '/anyminute/dashboard', icon: LayoutDashboard, label: 'Dashboard', allowedRoles: [] },
  { to: '/anyminute/home', icon: Home, label: 'Home', allowedRoles: [] },
  { to: '/anyminute/add-business', icon: Building2, label: 'Add Business', allowedRoles: ['admin'] },
  { to: '/anyminute/add-user', icon: UserPlus, label: 'Add User', allowedRoles: ['admin'] },
  { to: '/anyminute/edit-user', icon: UserCog, label: 'Edit User', allowedRoles: ['admin', 'manager'] },
  { to: '/anyminute/add-project', icon: FolderPlus, label: 'Add Project', allowedRoles: ['admin', 'manager'] },
  { to: '/anyminute/timesheet', icon: FileText, label: 'Timesheet', allowedRoles: [] },
  { to: '/anyminute/schedule', icon: Calendar, label: 'Schedule', allowedRoles: ['admin', 'manager'] },
  { to: '/anyminute/reports', icon: FileText, label: 'Reports', allowedRoles: ['admin', 'manager', 'accountant'] },
  { to: '/anyminute/pay-rates', icon: DollarSign, label: 'Pay Rates', allowedRoles: ['admin'] },
  { to: '/anyminute/tickets', icon: Ticket, label: 'Tickets', allowedRoles: [] },
  { to: '/anyminute/audit-logs', icon: ClipboardList, label: 'Audit Logs', allowedRoles: ['admin'] },
  { to: '/anyminute/plan-upgrade', icon: CreditCard, label: 'Billing', allowedRoles: ['admin'] },
  { to: '/anyminute/settings', icon: Settings, label: 'Settings', allowedRoles: ['admin'] },
  { to: '/anyminute/test-checklist', icon: TestTube, label: 'Test Checklist', allowedRoles: ['admin'] },
];

export const LeftNav = () => {
  const { user, logout } = useAMAuth();
  const navigate = useNavigate();
  const userRole = user?.role || 'employee';

  const handleLogout = () => {
    logout();
    navigate('/anyminute/login');
  };

  // Filter nav items based on user role
  const visibleNavItems = navItems.filter(item => {
    if (!item.allowedRoles || item.allowedRoles.length === 0) return true;
    return item.allowedRoles.includes(userRole);
  });

  return (
    <nav className="am-left-nav">
      <div className="am-left-nav-header">
        <img
          src="https://customer-assets.emergentagent.com/job_payroll-import-on/artifacts/6c0udnd7_image.png"
          alt="Any Minute"
          className="am-left-nav-logo"
        />
        <p style={{ fontSize: '11px', color: '#666', marginTop: '4px' }}>
          Time Tracker For Teams
        </p>
      </div>
      
      <div style={{ padding: '8px 0' }}>
        {visibleNavItems.map(item => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) => `am-left-nav-item ${isActive ? 'active' : ''}`}
            data-testid={`nav-${item.label.toLowerCase().replace(/\s+/g, '-')}`}
          >
            <item.icon size={20} />
            <span>{item.label}</span>
            {item.phase2 && <span style={{ fontSize: '10px', color: '#999', marginLeft: 'auto' }}>P2</span>}
          </NavLink>
        ))}
      </div>

      <div style={{ position: 'absolute', bottom: 0, left: 0, right: 0, padding: '12px 20px', borderTop: '1px solid #dadce0' }}>
        <button
          onClick={handleLogout}
          className="am-left-nav-item"
          style={{ width: '100%', border: 'none', background: 'none', cursor: 'pointer' }}
          data-testid="logout-btn"
        >
          <LogOut size={20} />
          <span>Logout</span>
        </button>
      </div>
    </nav>
  );
};

export const TopBar = ({ title }) => {
  const { user } = useAMAuth();
  
  return (
    <header className="am-top-bar">
      <h1 className="am-top-bar-title">{title}</h1>
      <div className="am-top-bar-user">
        <span style={{ fontSize: '14px' }}>{user?.first_name} {user?.last_name}</span>
        <div className="am-top-bar-avatar">
          {user?.first_name?.[0]}{user?.last_name?.[0]}
        </div>
      </div>
    </header>
  );
};

export const Layout = ({ title, children }) => (
  <div className="am-app">
    <LeftNav />
    <TopBar title={title} />
    <main className="am-main">
      {children}
    </main>
  </div>
);
