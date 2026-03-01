import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Layout } from '../components/Layout';
import { FormField, Loader } from '../components/SharedComponents';
import { AM_API_URL, useAMAuth } from '../context/AMAuthContext';

export default function AMDashboard() {
  const { token } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState('');
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  const config = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    if (token) {
      fetchBusinesses();
    }
  }, [token]);

  useEffect(() => {
    if (selectedBusiness && token) {
      fetchStats();
    }
  }, [selectedBusiness, token]);

  const fetchBusinesses = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/businesses`, config);
      setBusinesses(res.data);
      if (res.data.length > 0) {
        setSelectedBusiness(res.data[0].id);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchStats = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/dashboard/stats`, {
        ...config,
        params: { business_id: selectedBusiness }
      });
      setStats(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  if (loading) return <Layout title="Dashboard"><Loader /></Layout>;

  return (
    <Layout title="Dashboard">
      <div data-testid="am-dashboard">
        {/* Business Selector */}
        <div style={{ marginBottom: '24px', maxWidth: '400px', margin: '0 auto 24px' }}>
          <FormField
            name="business"
            type="select"
            value={selectedBusiness}
            onChange={(e) => setSelectedBusiness(e.target.value)}
            options={businesses.map(b => ({ value: b.id, label: b.name }))}
            placeholder="Select Business"
          />
        </div>

        {businesses.length === 0 ? (
          <div className="am-card" style={{ textAlign: 'center', padding: '60px' }}>
            <h3>Welcome to Any Minute!</h3>
            <p style={{ color: '#666', marginTop: '12px' }}>
              Get started by adding your first business.
            </p>
          </div>
        ) : (
          <>
            {/* Stats Cards */}
            <div className="am-grid am-grid-2" style={{ maxWidth: '600px', margin: '0 auto' }}>
              <div className="am-stat-card">
                <div className="am-stat-value">{stats?.total_working_hours || 0}</div>
                <div className="am-stat-label">Total Working Hours (This Week)</div>
              </div>
              <div className="am-stat-card">
                <div className="am-stat-value">{stats?.employee_count || 0}</div>
                <div className="am-stat-label">No. of Employees</div>
              </div>
            </div>

            {/* Week Info */}
            {stats && (
              <p style={{ textAlign: 'center', color: '#666', marginTop: '20px', fontSize: '13px' }}>
                Week: {stats.week_start} to {stats.week_end}
              </p>
            )}

            {/* Placeholder Chart Area */}
            <div className="am-card" style={{ marginTop: '24px', textAlign: 'center', padding: '40px' }}>
              <p style={{ color: '#999' }}>📊 Analytics chart placeholder</p>
            </div>
          </>
        )}
      </div>
    </Layout>
  );
}
