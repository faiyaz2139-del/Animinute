import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { Layout } from '../components/Layout';
import { FormField, BlueButton, Table, Loader } from '../components/SharedComponents';
import { AM_API_URL, useAMAuth } from '../context/AMAuthContext';

export default function AMEditUser() {
  const navigate = useNavigate();
  const { token } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [users, setUsers] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState('');
  const [loading, setLoading] = useState(true);

  // Auth headers for API calls
  const config = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    if (token) {
      fetchBusinesses();
    }
  }, [token]);

  useEffect(() => {
    if (selectedBusiness && token) {
      fetchUsers();
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
      console.error('Error fetching businesses:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchUsers = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/users`, { ...config, params: { business_id: selectedBusiness } });
      setUsers(res.data);
    } catch (err) {
      console.error('Error fetching users:', err);
    }
  };

  const columns = [
    { key: 'index', label: 'SI No', render: (_, r, i) => i + 1 },
    { key: 'name', label: 'Name', render: (_, r) => `${r.first_name} ${r.last_name}` },
    { key: 'email', label: 'Email', render: (v) => v || '-' },
    { key: 'role', label: 'Role', render: (v) => v?.charAt(0).toUpperCase() + v?.slice(1) },
    {
      key: 'actions',
      label: 'Actions',
      render: (_, row) => (
        <span className="am-link" onClick={() => navigate('/anyminute/add-user', { state: { user: row } })}>
          Edit
        </span>
      )
    }
  ];

  if (loading) return <Layout title="Edit User"><Loader /></Layout>;

  return (
    <Layout title="Edit User">
      <div className="am-card" data-testid="edit-user-page">
        <div style={{ maxWidth: '400px', marginBottom: '24px' }}>
          <FormField
            label="Select Business"
            name="business"
            type="select"
            value={selectedBusiness}
            onChange={(e) => setSelectedBusiness(e.target.value)}
            options={businesses.map(b => ({ value: b.id, label: b.name }))}
          />
        </div>

        <Table columns={columns} data={users} />
      </div>
    </Layout>
  );
}
