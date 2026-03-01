import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAMAuth } from '../context/AMAuthContext';
import {
  LayoutDashboard, Home, Building2, Users, UserPlus, UserCog,
  FolderPlus, FileText, Calendar, Ticket, CreditCard, Settings, LogOut, TestTube, DollarSign
} from 'lucide-react';

const navItems = [
  { to: '/anyminute/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/anyminute/home', icon: Home, label: 'Home' },
  { to: '/anyminute/add-business', icon: Building2, label: 'Add Business' },
  { to: '/anyminute/add-user', icon: UserPlus, label: 'Add User' },
  { to: '/anyminute/edit-user', icon: UserCog, label: 'Edit User' },
  { to: '/anyminute/add-project', icon: FolderPlus, label: 'Add Project' },
  { to: '/anyminute/timesheet', icon: FileText, label: 'Timesheet' },
  { to: '/anyminute/schedule', icon: Calendar, label: 'Schedule' },
  { to: '/anyminute/reports', icon: FileText, label: 'Reports' },
  { to: '/anyminute/pay-rates', icon: DollarSign, label: 'Pay Rates' },
  { to: '/anyminute/tickets', icon: Ticket, label: 'Tickets' },
  { to: '/anyminute/plan-upgrade', icon: CreditCard, label: 'Billing' },
  { to: '/anyminute/settings', icon: Settings, label: 'Settings' },
  { to: '/anyminute/test-checklist', icon: TestTube, label: 'Test Checklist' },
];

export const LeftNav = () => {
  const { user, logout } = useAMAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/anyminute/login');
  };

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
        {navItems.map(item => (
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
