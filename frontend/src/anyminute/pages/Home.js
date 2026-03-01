import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Layout } from '../components/Layout';
import { Table, BlueButton, Popup, Loader } from '../components/SharedComponents';
import { AM_API_URL, useAMAuth } from '../context/AMAuthContext';
import { useNavigate } from 'react-router-dom';

export default function AMHome() {
  const navigate = useNavigate();
  const { token } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [projectPopup, setProjectPopup] = useState({ isOpen: false, business: null, projects: [] });

  // Auth headers for API calls
  const config = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    if (token) {
      fetchBusinesses();
    }
  }, [token]);

  const fetchBusinesses = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/businesses`, config);
      setBusinesses(res.data);
    } catch (err) {
      console.error('Error fetching businesses:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchProjects = async (business) => {
    try {
      const res = await axios.get(`${AM_API_URL}/projects`, { ...config, params: { business_id: business.id } });
      setProjectPopup({ isOpen: true, business, projects: res.data });
    } catch (err) {
      console.error('Error fetching projects:', err);
    }
  };

  const columns = [
    { key: 'index', label: 'SI No', width: '80px', render: (_, row, idx) => idx + 1 },
    { key: 'name', label: 'Business Name' },
    { key: 'address', label: 'Address', render: (v) => v || '-' },
    {
      key: 'actions',
      label: 'Actions',
      render: (_, row) => (
        <div style={{ display: 'flex', gap: '12px' }}>
          <span className="am-link" onClick={() => navigate('/anyminute/add-business', { state: { business: row } })}>
            Update Profile
          </span>
          <span className="am-link" onClick={() => fetchProjects(row)}>View Projects</span>
          <span className="am-link" onClick={() => navigate('/anyminute/add-project', { state: { businessId: row.id } })}>
            Add Project
          </span>
        </div>
      )
    }
  ];

  if (loading) return <Layout title="Home"><Loader /></Layout>;

  return (
    <Layout title="Home">
      <div data-testid="am-home">
        <div className="am-card">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
            <h3 className="am-card-title" style={{ margin: 0 }}>Businesses</h3>
            <BlueButton onClick={() => navigate('/anyminute/add-business')} data-testid="add-business-btn">Add Business</BlueButton>
          </div>
          <Table columns={columns} data={businesses} />
        </div>
      </div>

      {/* Projects Popup */}
      {projectPopup.isOpen && (
        <div style={{
          position: 'fixed', inset: 0, backgroundColor: 'rgba(0,0,0,0.5)',
          display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 9999
        }}>
          <div className="am-card" style={{ width: '90%', maxWidth: '700px', maxHeight: '80vh', overflow: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
              <h3>Projects - {projectPopup.business?.name}</h3>
              <button onClick={() => setProjectPopup({ isOpen: false, business: null, projects: [] })} style={{ border: 'none', background: 'none', fontSize: '20px', cursor: 'pointer' }}>×</button>
            </div>
            <Table
              columns={[
                { key: 'index', label: 'SI No', render: (_, r, i) => i + 1 },
                { key: 'project_name', label: 'Project Name' },
                { key: 'start_date', label: 'Start Date' },
                { key: 'end_date', label: 'End Date', render: (v) => v || '-' }
              ]}
              data={projectPopup.projects}
            />
          </div>
        </div>
      )}
    </Layout>
  );
}
