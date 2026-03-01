import React, { useState, useEffect } from 'react';
import { useAMAuth, AM_API_URL } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import axios from 'axios';
import { CreditCard, Users, Check, AlertCircle } from 'lucide-react';

const PLANS = {
  free: { name: 'Free', seats: 5, color: '#9e9e9e', features: ['Up to 5 users', 'Basic timesheets', 'Manual exports'] },
  basic: { name: 'Basic', seats: 25, color: '#1a73e8', features: ['Up to 25 users', 'All timesheet features', 'Payroll integration', 'Reports'] },
  pro: { name: 'Pro', seats: 999, color: '#7c3aed', features: ['Unlimited users', 'All features', 'Priority support', 'API access'] }
};

export default function Billing() {
  const { token, user: currentUser } = useAMAuth();
  const [billing, setBilling] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [editData, setEditData] = useState({ plan: '', seat_limit: 0 });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const config = { headers: { Authorization: `Bearer ${token}` } };
  const isAdmin = currentUser?.role === 'admin';

  useEffect(() => {
    loadBilling();
  }, [token]);

  const loadBilling = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/billing`, config);
      setBilling(res.data);
      setEditData({ plan: res.data.plan, seat_limit: res.data.seat_limit });
    } catch (err) {
      setError('Failed to load billing information');
    } finally {
      setLoading(false);
    }
  };

  const saveBilling = async () => {
    setSaving(true);
    setError('');
    setSuccess('');
    try {
      const res = await axios.put(`${AM_API_URL}/billing`, editData, config);
      setBilling(res.data);
      setEditMode(false);
      setSuccess('Billing updated successfully');
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.response?.data?.detail || 'Failed to update billing');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <Layout title="Billing">
        <div style={{ display: 'flex', justifyContent: 'center', padding: '40px' }}>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </Layout>
    );
  }

  const currentPlan = PLANS[billing?.plan] || PLANS.free;
  const usagePercent = billing ? Math.min((billing.seat_usage / billing.seat_limit) * 100, 100) : 0;
  const isNearLimit = usagePercent >= 80;

  return (
    <Layout title="Billing">
      {error && (
        <div style={{ padding: '12px 16px', backgroundColor: '#ffebee', color: '#c62828', borderRadius: '8px', marginBottom: '16px', display: 'flex', alignItems: 'center', gap: '8px' }} data-testid="billing-error">
          <AlertCircle size={18} />
          {error}
        </div>
      )}

      {success && (
        <div style={{ padding: '12px 16px', backgroundColor: '#e8f5e9', color: '#2e7d32', borderRadius: '8px', marginBottom: '16px', display: 'flex', alignItems: 'center', gap: '8px' }} data-testid="billing-success">
          <Check size={18} />
          {success}
        </div>
      )}

      {/* Current Plan Card */}
      <div className="am-card" style={{ marginBottom: '24px' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '24px' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
            <div style={{ width: '48px', height: '48px', borderRadius: '12px', backgroundColor: currentPlan.color + '20', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <CreditCard size={24} style={{ color: currentPlan.color }} />
            </div>
            <div>
              <h2 style={{ fontSize: '20px', fontWeight: '600', marginBottom: '4px' }}>Current Plan</h2>
              <p style={{ color: '#666', fontSize: '14px' }}>{billing?.tenant_name}</p>
            </div>
          </div>
          {billing?.status === 'blocked' && (
            <span style={{ padding: '6px 12px', backgroundColor: '#ffebee', color: '#c62828', borderRadius: '16px', fontSize: '12px', fontWeight: '600' }} data-testid="status-blocked">
              BLOCKED
            </span>
          )}
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: '16px', marginBottom: '24px' }}>
          <span style={{ padding: '8px 20px', backgroundColor: currentPlan.color, color: 'white', borderRadius: '20px', fontSize: '16px', fontWeight: '600' }} data-testid="current-plan-badge">
            {currentPlan.name}
          </span>
          {!editMode && isAdmin && (
            <button
              onClick={() => setEditMode(true)}
              style={{ padding: '8px 16px', backgroundColor: '#f5f5f5', border: '1px solid #e0e0e0', borderRadius: '8px', fontSize: '14px', cursor: 'pointer' }}
              data-testid="edit-plan-btn"
            >
              Change Plan
            </button>
          )}
        </div>

        {/* Seat Usage */}
        <div style={{ marginBottom: '24px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
              <Users size={18} style={{ color: '#666' }} />
              <span style={{ fontWeight: '500' }}>Seat Usage</span>
            </div>
            <span style={{ fontSize: '14px', color: isNearLimit ? '#c62828' : '#666' }} data-testid="seat-usage-text">
              {billing?.seat_usage} / {billing?.seat_limit} users
            </span>
          </div>
          <div style={{ height: '8px', backgroundColor: '#e0e0e0', borderRadius: '4px', overflow: 'hidden' }}>
            <div
              style={{
                height: '100%',
                width: `${usagePercent}%`,
                backgroundColor: isNearLimit ? '#c62828' : currentPlan.color,
                borderRadius: '4px',
                transition: 'width 0.3s ease'
              }}
              data-testid="seat-usage-bar"
            ></div>
          </div>
          {isNearLimit && (
            <p style={{ color: '#c62828', fontSize: '13px', marginTop: '8px' }} data-testid="seat-warning">
              You're approaching your seat limit. Consider upgrading your plan.
            </p>
          )}
        </div>

        {/* Features List */}
        <div>
          <p style={{ fontSize: '14px', fontWeight: '500', marginBottom: '12px', color: '#666' }}>Plan Features:</p>
          <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
            {currentPlan.features.map((feature, idx) => (
              <li key={idx} style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px', color: '#333' }}>
                <Check size={16} style={{ color: currentPlan.color }} />
                {feature}
              </li>
            ))}
          </ul>
        </div>
      </div>

      {/* Edit Mode */}
      {editMode && isAdmin && (
        <div className="am-card" style={{ marginBottom: '24px' }}>
          <h3 style={{ marginBottom: '20px' }}>Update Plan</h3>

          <div style={{ marginBottom: '20px' }}>
            <label style={{ display: 'block', marginBottom: '8px', fontWeight: '500' }}>Plan</label>
            <div style={{ display: 'flex', gap: '12px' }}>
              {Object.entries(PLANS).map(([key, plan]) => (
                <button
                  key={key}
                  onClick={() => setEditData({ ...editData, plan: key, seat_limit: plan.seats })}
                  style={{
                    flex: 1,
                    padding: '16px',
                    border: editData.plan === key ? `2px solid ${plan.color}` : '2px solid #e0e0e0',
                    backgroundColor: editData.plan === key ? plan.color + '10' : 'white',
                    borderRadius: '8px',
                    cursor: 'pointer',
                    textAlign: 'center'
                  }}
                  data-testid={`select-plan-${key}`}
                >
                  <div style={{ fontWeight: '600', color: plan.color, marginBottom: '4px' }}>{plan.name}</div>
                  <div style={{ fontSize: '12px', color: '#666' }}>{plan.seats === 999 ? 'Unlimited' : `${plan.seats} seats`}</div>
                </button>
              ))}
            </div>
          </div>

          <div style={{ marginBottom: '20px' }}>
            <label style={{ display: 'block', marginBottom: '8px', fontWeight: '500' }}>Custom Seat Limit</label>
            <input
              type="number"
              min="1"
              value={editData.seat_limit}
              onChange={(e) => setEditData({ ...editData, seat_limit: parseInt(e.target.value) || 1 })}
              style={{ width: '120px', padding: '10px 12px', border: '1px solid #e0e0e0', borderRadius: '8px', fontSize: '16px' }}
              data-testid="seat-limit-input"
            />
            <p style={{ fontSize: '13px', color: '#666', marginTop: '8px' }}>
              Override the default seat limit for this plan
            </p>
          </div>

          <div style={{ display: 'flex', gap: '12px' }}>
            <button
              onClick={saveBilling}
              disabled={saving}
              style={{ padding: '12px 24px', backgroundColor: '#1a73e8', color: 'white', border: 'none', borderRadius: '8px', fontWeight: '500', cursor: saving ? 'not-allowed' : 'pointer', opacity: saving ? 0.7 : 1 }}
              data-testid="save-billing-btn"
            >
              {saving ? 'Saving...' : 'Save Changes'}
            </button>
            <button
              onClick={() => { setEditMode(false); setEditData({ plan: billing.plan, seat_limit: billing.seat_limit }); }}
              style={{ padding: '12px 24px', backgroundColor: '#f5f5f5', border: '1px solid #e0e0e0', borderRadius: '8px', fontWeight: '500', cursor: 'pointer' }}
              data-testid="cancel-edit-btn"
            >
              Cancel
            </button>
          </div>
        </div>
      )}

      {/* Read-only Notice */}
      {!isAdmin && (
        <div className="am-card" style={{ backgroundColor: '#f5f5f5' }}>
          <p style={{ color: '#666', textAlign: 'center', margin: 0 }}>
            Contact your administrator to change plan or seat limits.
          </p>
        </div>
      )}
    </Layout>
  );
}
