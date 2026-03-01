import React from 'react';
import { useNavigate } from 'react-router-dom';
import { LeftNav, TopBar } from '../components/Layout';
import { ArrowLeft } from 'lucide-react';

const ComingSoon = ({ title = 'Coming Soon', description = 'This feature is under development.' }) => {
  const navigate = useNavigate();
  
  return (
    <div className="am-layout">
      <LeftNav />
      <div className="am-main">
        <TopBar title={title} />
        <div className="am-content" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '60vh' }}>
          <div style={{ textAlign: 'center', maxWidth: '400px' }}>
            <h1 style={{ fontSize: '28px', fontWeight: '600', color: '#1a73e8', marginBottom: '16px' }}>
              {title}
            </h1>
            <p style={{ color: '#5f6368', fontSize: '16px', marginBottom: '24px' }}>
              {description}
            </p>
            <p style={{ color: '#999', fontSize: '14px', marginBottom: '32px' }}>
              Phase 2 Feature - Coming Soon
            </p>
            <button
              onClick={() => navigate('/anyminute/dashboard')}
              style={{
                display: 'inline-flex',
                alignItems: 'center',
                gap: '8px',
                padding: '12px 24px',
                backgroundColor: '#1a73e8',
                color: 'white',
                border: 'none',
                borderRadius: '8px',
                fontSize: '14px',
                fontWeight: '500',
                cursor: 'pointer'
              }}
              data-testid="back-to-dashboard-btn"
            >
              <ArrowLeft size={18} />
              Back to Dashboard
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export const Tickets = () => (
  <ComingSoon 
    title="Tickets" 
    description="Submit and track support tickets for your team." 
  />
);

export const PlanUpgrade = () => (
  <ComingSoon 
    title="Plan Upgrade" 
    description="Upgrade your subscription plan for more features and users." 
  />
);

export const TestChecklist = () => (
  <ComingSoon 
    title="Test Checklist" 
    description="QA testing checklist for your timesheet workflows." 
  />
);

export default ComingSoon;
