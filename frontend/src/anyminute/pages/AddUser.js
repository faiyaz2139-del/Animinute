import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';
import { Layout } from '../components/Layout';
import { FormField, BlueButton, Popup, Loader } from '../components/SharedComponents';
import { AM_API_URL } from '../context/AMAuthContext';

export default function AMAddUser() {
  const navigate = useNavigate();
  const location = useLocation();
  const editUser = location.state?.user;

  const [businesses, setBusinesses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [popup, setPopup] = useState({ open: false });
  const [form, setForm] = useState({
    business_id: '', role: 'employee', first_name: '', last_name: '', email: '', password: '',
    phone: '', mobile: '', dob: '', gender: '', hire_date: '', postal_code: ''
  });
  const [errors, setErrors] = useState({});

  useEffect(() => {
    fetchBusinesses();
    if (editUser) {
      setForm({ ...form, ...editUser, password: '' });
    }
  }, []);

  const fetchBusinesses = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/businesses`);
      setBusinesses(res.data);
      if (res.data.length > 0 && !editUser) {
        setForm(f => ({ ...f, business_id: res.data[0].id }));
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
    setErrors({ ...errors, [name]: '' });
  };

  const validate = () => {
    const errs = {};
    if (!form.business_id) errs.business_id = 'Please select a business';
    if (!form.first_name) errs.first_name = 'First name is required';
    if (!form.last_name) errs.last_name = 'Last name is required';
    if (!editUser && form.email && !form.password) errs.password = 'Password required for login access';
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;
    setSaving(true);
    try {
      const payload = { ...form };
      if (!payload.password) delete payload.password;
      
      if (editUser) {
        await axios.put(`${AM_API_URL}/users/${editUser.id}`, payload);
        setPopup({ open: true, type: 'success', title: 'Success', message: 'User updated successfully!' });
      } else {
        await axios.post(`${AM_API_URL}/users`, payload);
        setPopup({ open: true, type: 'success', title: 'Success', message: 'User created successfully!' });
      }
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: err.response?.data?.detail || 'Failed to save user' });
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <Layout title="Add User"><Loader /></Layout>;

  return (
    <Layout title={editUser ? 'Edit User' : 'Add User'}>
      <div className="am-card" style={{ maxWidth: '700px' }} data-testid="add-user-form">
        <h3 className="am-card-title">Personal Details</h3>
        
        <form onSubmit={handleSubmit}>
          <div className="am-grid am-grid-2">
            <FormField
              label="Assign Business"
              name="business_id"
              type="select"
              value={form.business_id}
              onChange={handleChange}
              options={businesses.map(b => ({ value: b.id, label: b.company_name }))}
              error={errors.business_id}
              required
            />
            <FormField
              label="Assign Role"
              name="role"
              type="select"
              value={form.role}
              onChange={handleChange}
              options={[
                { value: 'employee', label: 'Employee' },
                { value: 'manager', label: 'Manager' },
                { value: 'subscriber', label: 'Subscriber (Owner)' }
              ]}
            />
          </div>

          <div className="am-grid am-grid-2">
            <FormField label="First Name" name="first_name" value={form.first_name} onChange={handleChange} error={errors.first_name} required />
            <FormField label="Last Name" name="last_name" value={form.last_name} onChange={handleChange} error={errors.last_name} required />
          </div>

          <div className="am-grid am-grid-3">
            <FormField label="Date of Birth" name="dob" type="date" value={form.dob} onChange={handleChange} />
            <FormField
              label="Gender"
              name="gender"
              type="select"
              value={form.gender}
              onChange={handleChange}
              options={[
                { value: 'male', label: 'Male' },
                { value: 'female', label: 'Female' },
                { value: 'other', label: 'Other' }
              ]}
            />
            <FormField label="Hire Date" name="hire_date" type="date" value={form.hire_date} onChange={handleChange} disabled={!!editUser} />
          </div>

          <FormField label="Email (For Online Access)" name="email" type="email" value={form.email} onChange={handleChange} />
          {!editUser && form.email && (
            <FormField label="Password" name="password" type="password" value={form.password} onChange={handleChange} error={errors.password} />
          )}

          <div className="am-grid am-grid-2">
            <FormField label="Contact Phone" name="phone" value={form.phone} onChange={handleChange} />
            <FormField label="Mobile" name="mobile" value={form.mobile} onChange={handleChange} />
          </div>

          <div style={{ display: 'flex', gap: '12px', alignItems: 'flex-end' }}>
            <div style={{ flex: 1 }}>
              <FormField label="Postal Code" name="postal_code" value={form.postal_code} onChange={handleChange} />
            </div>
            <BlueButton type="button" outline style={{ marginBottom: '16px' }}>Search</BlueButton>
          </div>

          <BlueButton type="submit" disabled={saving} style={{ marginTop: '16px' }}>
            {saving ? 'Saving...' : editUser ? 'Update User' : 'Create User'}
          </BlueButton>
        </form>
      </div>

      <Popup {...popup} onClose={() => { setPopup({ open: false }); if (popup.type === 'success') navigate('/anyminute/home'); }} />
    </Layout>
  );
}
