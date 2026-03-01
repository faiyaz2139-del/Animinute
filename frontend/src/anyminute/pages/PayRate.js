import React, { useState, useEffect } from 'react';
import { useAMAuth, AM_API_URL } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import { BlueButton, Popup } from '../components/SharedComponents';
import axios from 'axios';
import { DollarSign, Plus, Edit2, Trash2, Building2, Users, Calendar } from 'lucide-react';

export default function PayRate() {
  const { token, user: currentUser } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [users, setUsers] = useState([]);
  const [payRates, setPayRates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingRate, setEditingRate] = useState(null);
  const [saving, setSaving] = useState(false);
  const [popup, setPopup] = useState({ open: false });
  const [filters, setFilters] = useState({ business_id: '', user_id: '' });
  const [form, setForm] = useState({
    user_id: '',
    business_id: '',
    rate_type: 'hourly',
    rate_amount: '',
    effective_from: new Date().toISOString().split('T')[0],
    effective_to: ''
  });

  const config = { headers: { Authorization: `Bearer ${token}` } };
  const isAdmin = currentUser?.role === 'admin';

  useEffect(() => {
    loadData();
  }, [token]);

  useEffect(() => {
    if (token) loadPayRates();
  }, [filters, token]);

  const loadData = async () => {
    try {
      const [bizRes, usersRes] = await Promise.all([
        axios.get(`${AM_API_URL}/businesses`, config),
        axios.get(`${AM_API_URL}/users`, config)
      ]);
      setBusinesses(bizRes.data || []);
      setUsers(usersRes.data || []);
    } catch (err) {
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const loadPayRates = async () => {
    try {
      const params = new URLSearchParams();
      if (filters.business_id) params.append('business_id', filters.business_id);
      if (filters.user_id) params.append('user_id', filters.user_id);
      
      const res = await axios.get(`${AM_API_URL}/pay-rates?${params.toString()}`, config);
      setPayRates((res.data || []).filter(r => r.active !== false));
    } catch (err) {
      console.error('Error loading pay rates:', err);
    }
  };

  const openModal = (rate = null) => {
    if (rate) {
      setEditingRate(rate);
      setForm({
        user_id: rate.user_id,
        business_id: rate.business_id,
        rate_type: rate.rate_type,
        rate_amount: rate.rate_amount,
        effective_from: rate.effective_from,
        effective_to: rate.effective_to || ''
      });
    } else {
      setEditingRate(null);
      setForm({
        user_id: users[0]?.id || '',
        business_id: businesses[0]?.id || '',
        rate_type: 'hourly',
        rate_amount: '',
        effective_from: new Date().toISOString().split('T')[0],
        effective_to: ''
      });
    }
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingRate(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.user_id || !form.business_id || !form.rate_amount) {
      setPopup({ open: true, type: 'error', title: 'Error', message: 'Please fill all required fields' });
      return;
    }
    
    setSaving(true);
    try {
      const payload = {
        ...form,
        rate_amount: parseFloat(form.rate_amount),
        effective_to: form.effective_to || null
      };
      
      if (editingRate) {
        await axios.put(`${AM_API_URL}/pay-rates/${editingRate.id}`, payload, config);
        setPopup({ open: true, type: 'success', title: 'Success', message: 'Pay rate updated!' });
      } else {
        await axios.post(`${AM_API_URL}/pay-rates`, payload, config);
        setPopup({ open: true, type: 'success', title: 'Success', message: 'Pay rate created!' });
      }
      closeModal();
      loadPayRates();
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: err.response?.data?.detail || 'Failed to save' });
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (rate) => {
    if (!window.confirm('Delete this pay rate?')) return;
    try {
      await axios.delete(`${AM_API_URL}/pay-rates/${rate.id}`, config);
      loadPayRates();
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: 'Failed to delete' });
    }
  };

  const getUserName = (userId) => {
    const u = users.find(u => u.id === userId);
    return u ? `${u.first_name} ${u.last_name}` : 'Unknown';
  };

  const getBusinessName = (bizId) => {
    const b = businesses.find(b => b.id === bizId);
    return b?.name || 'Unknown';
  };

  const getRateTypeColor = (type) => {
    switch (type) {
      case 'hourly': return '#1a73e8';
      case 'salary': return '#34a853';
      case 'overtime': return '#ea4335';
      default: return '#666';
    }
  };

  if (loading) {
    return (
      <Layout title="Pay Rates">
        <div style={{ display: 'flex', justifyContent: 'center', padding: '40px' }}>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </Layout>
    );
  }

  if (businesses.length === 0) {
    return (
      <Layout title="Pay Rates">
        <div className="am-card" style={{ textAlign: 'center', padding: '60px 20px' }}>
          <Building2 size={48} style={{ color: '#bdbdbd', marginBottom: '16px' }} />
          <h3 style={{ color: '#757575', marginBottom: '8px' }}>No Business Found</h3>
          <p style={{ color: '#9e9e9e' }}>Please create a business first to manage pay rates.</p>
        </div>
      </Layout>
    );
  }

  return (
    <Layout title="Pay Rates">
      {/* Filters */}
      <div className="am-card" style={{ marginBottom: '24px' }}>
        <div style={{ display: 'flex', gap: '16px', alignItems: 'center', flexWrap: 'wrap' }}>
          <div className="am-form-group" style={{ margin: 0, minWidth: '200px' }}>
            <label style={{ fontSize: '12px', marginBottom: '4px' }}>Business</label>
            <select
              value={filters.business_id}
              onChange={(e) => setFilters({ ...filters, business_id: e.target.value })}
              className="am-select"
              data-testid="filter-business"
            >
              <option value="">All Businesses</option>
              {businesses.map(b => (
                <option key={b.id} value={b.id}>{b.name}</option>
              ))}
            </select>
          </div>
          
          <div className="am-form-group" style={{ margin: 0, minWidth: '200px' }}>
            <label style={{ fontSize: '12px', marginBottom: '4px' }}>Employee</label>
            <select
              value={filters.user_id}
              onChange={(e) => setFilters({ ...filters, user_id: e.target.value })}
              className="am-select"
              data-testid="filter-user"
            >
              <option value="">All Employees</option>
              {users.map(u => (
                <option key={u.id} value={u.id}>{u.first_name} {u.last_name}</option>
              ))}
            </select>
          </div>
          
          {isAdmin && (
            <div style={{ marginLeft: 'auto' }}>
              <BlueButton onClick={() => openModal()} data-testid="add-rate-btn">
                <Plus size={16} style={{ marginRight: '8px' }} />
                Add Pay Rate
              </BlueButton>
            </div>
          )}
        </div>
      </div>

      {/* Pay Rates Table */}
      <div className="am-card">
        <h3 style={{ marginBottom: '16px' }}>Pay Rates ({payRates.length})</h3>
        
        {payRates.length === 0 ? (
          <div style={{ textAlign: 'center', padding: '40px 20px' }}>
            <DollarSign size={40} style={{ color: '#bdbdbd', marginBottom: '12px' }} />
            <p style={{ color: '#9e9e9e' }}>No pay rates found. Add a pay rate to get started.</p>
          </div>
        ) : (
          <div style={{ overflowX: 'auto' }}>
            <table className="am-table" data-testid="pay-rates-table">
              <thead>
                <tr>
                  <th>Employee</th>
                  <th>Business</th>
                  <th>Rate Type</th>
                  <th>Amount</th>
                  <th>Effective From</th>
                  <th>Effective To</th>
                  {isAdmin && <th>Actions</th>}
                </tr>
              </thead>
              <tbody>
                {payRates.map(rate => (
                  <tr key={rate.id}>
                    <td>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <Users size={16} style={{ color: '#666' }} />
                        {getUserName(rate.user_id)}
                      </div>
                    </td>
                    <td>{getBusinessName(rate.business_id)}</td>
                    <td>
                      <span style={{
                        padding: '4px 8px',
                        backgroundColor: getRateTypeColor(rate.rate_type) + '20',
                        color: getRateTypeColor(rate.rate_type),
                        borderRadius: '4px',
                        fontSize: '12px',
                        fontWeight: '600',
                        textTransform: 'capitalize'
                      }}>
                        {rate.rate_type}
                      </span>
                    </td>
                    <td style={{ fontWeight: '600' }}>${rate.rate_amount.toFixed(2)}</td>
                    <td>{rate.effective_from}</td>
                    <td>{rate.effective_to || '—'}</td>
                    {isAdmin && (
                      <td>
                        <div style={{ display: 'flex', gap: '8px' }}>
                          <button
                            onClick={() => openModal(rate)}
                            className="am-icon-btn"
                            title="Edit"
                            data-testid={`edit-rate-${rate.id}`}
                          >
                            <Edit2 size={16} />
                          </button>
                          <button
                            onClick={() => handleDelete(rate)}
                            className="am-icon-btn"
                            style={{ color: '#d93025' }}
                            title="Delete"
                            data-testid={`delete-rate-${rate.id}`}
                          >
                            <Trash2 size={16} />
                          </button>
                        </div>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Modal */}
      {showModal && (
        <div className="am-modal-overlay" onClick={closeModal}>
          <div className="am-modal" onClick={e => e.stopPropagation()} data-testid="pay-rate-modal">
            <h3 style={{ marginBottom: '20px' }}>{editingRate ? 'Edit Pay Rate' : 'Add Pay Rate'}</h3>
            <form onSubmit={handleSubmit}>
              <div className="am-form-group">
                <label>Employee *</label>
                <select
                  value={form.user_id}
                  onChange={(e) => setForm({ ...form, user_id: e.target.value })}
                  className="am-select"
                  required
                  disabled={!!editingRate}
                  data-testid="modal-user-select"
                >
                  <option value="">Select Employee</option>
                  {users.map(u => (
                    <option key={u.id} value={u.id}>{u.first_name} {u.last_name}</option>
                  ))}
                </select>
              </div>

              <div className="am-form-group">
                <label>Business *</label>
                <select
                  value={form.business_id}
                  onChange={(e) => setForm({ ...form, business_id: e.target.value })}
                  className="am-select"
                  required
                  disabled={!!editingRate}
                  data-testid="modal-business-select"
                >
                  <option value="">Select Business</option>
                  {businesses.map(b => (
                    <option key={b.id} value={b.id}>{b.name}</option>
                  ))}
                </select>
              </div>

              <div className="am-form-group">
                <label>Rate Type *</label>
                <select
                  value={form.rate_type}
                  onChange={(e) => setForm({ ...form, rate_type: e.target.value })}
                  className="am-select"
                  required
                  data-testid="modal-rate-type"
                >
                  <option value="hourly">Hourly</option>
                  <option value="salary">Salary</option>
                  <option value="overtime">Overtime</option>
                </select>
              </div>

              <div className="am-form-group">
                <label>Rate Amount ($/hr) *</label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  value={form.rate_amount}
                  onChange={(e) => setForm({ ...form, rate_amount: e.target.value })}
                  className="am-input"
                  required
                  placeholder="e.g. 25.00"
                  data-testid="modal-rate-amount"
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div className="am-form-group">
                  <label>Effective From *</label>
                  <input
                    type="date"
                    value={form.effective_from}
                    onChange={(e) => setForm({ ...form, effective_from: e.target.value })}
                    className="am-input"
                    required
                    data-testid="modal-effective-from"
                  />
                </div>
                <div className="am-form-group">
                  <label>Effective To</label>
                  <input
                    type="date"
                    value={form.effective_to}
                    onChange={(e) => setForm({ ...form, effective_to: e.target.value })}
                    className="am-input"
                    min={form.effective_from}
                    data-testid="modal-effective-to"
                  />
                  <p style={{ fontSize: '11px', color: '#666', marginTop: '4px' }}>Leave empty for ongoing</p>
                </div>
              </div>

              <div className="am-modal-footer">
                <button type="button" onClick={closeModal} className="am-btn am-btn-secondary">Cancel</button>
                <BlueButton type="submit" disabled={saving} data-testid="save-rate-btn">
                  {saving ? 'Saving...' : 'Save'}
                </BlueButton>
              </div>
            </form>
          </div>
        </div>
      )}

      <Popup {...popup} onClose={() => setPopup({ open: false })} />
    </Layout>
  );
}
