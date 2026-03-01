import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAMAuth } from '../context/AMAuthContext';
import { BlueButton, FormField, Popup } from '../components/SharedComponents';
import '../theme.css';

export default function AMSignup() {
  const { register } = useAMAuth();
  const navigate = useNavigate();
  const [accountType, setAccountType] = useState(null);
  const [form, setForm] = useState({
    first_name: '', last_name: '', email: '', phone: '', ext: '', mobile: '',
    postal_code: '', plan: 'basic', password: '', confirm_password: ''
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [popup, setPopup] = useState({ open: false });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
    setErrors({ ...errors, [name]: '' });
  };

  const validate = () => {
    const errs = {};
    if (!form.first_name) errs.first_name = 'First name is required';
    if (!form.last_name) errs.last_name = 'Last name is required';
    if (!form.email) errs.email = 'Email is required';
    if (!form.password) errs.password = 'Password is required';
    if (form.password !== form.confirm_password) errs.confirm_password = 'Passwords do not match';
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;
    setLoading(true);
    try {
      await register(form);
      setPopup({
        open: true,
        type: 'success',
        title: 'Registration Successful',
        message: 'Please check your email for next steps to complete your account setup.'
      });
    } catch (err) {
      setPopup({
        open: true,
        type: 'error',
        title: 'Registration Failed',
        message: err.response?.data?.detail || 'An error occurred during registration.'
      });
    } finally {
      setLoading(false);
    }
  };

  const handlePopupClose = () => {
    setPopup({ open: false });
    if (popup.type === 'success') {
      navigate('/anyminute/dashboard');
    }
  };

  // Account Type Selection
  if (!accountType) {
    return (
      <div className="am-app" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '100vh' }}>
        <div style={{ textAlign: 'center', maxWidth: '500px', padding: '40px' }}>
          <img
            src="https://customer-assets.emergentagent.com/job_payroll-import-on/artifacts/6c0udnd7_image.png"
            alt="Any Minute"
            style={{ height: '60px', marginBottom: '24px' }}
          />
          <h2 style={{ marginBottom: '32px' }}>Choose Account Type</h2>
          
          <div style={{ display: 'flex', gap: '20px', justifyContent: 'center' }}>
            <div 
              className="am-card" 
              style={{ cursor: 'pointer', padding: '30px', flex: 1 }}
              onClick={() => setAccountType('subscriber')}
              data-testid="select-subscriber"
            >
              <h3 style={{ color: '#1a73e8', marginBottom: '8px' }}>New Subscriber</h3>
              <p style={{ fontSize: '13px', color: '#666' }}>Track time for your business</p>
            </div>
            <div 
              className="am-card" 
              style={{ cursor: 'pointer', padding: '30px', flex: 1, opacity: 0.6 }}
              onClick={() => setAccountType('reseller')}
              data-testid="select-reseller"
            >
              <h3 style={{ color: '#1a73e8', marginBottom: '8px' }}>Reseller Account</h3>
              <p style={{ fontSize: '13px', color: '#666' }}>Manage multiple clients</p>
            </div>
          </div>

          <p style={{ marginTop: '24px' }}>
            Already have an account? <Link to="/anyminute/login" className="am-link">Sign In</Link>
          </p>
        </div>
      </div>
    );
  }

  // Reseller - Phase 2 Placeholder
  if (accountType === 'reseller') {
    return (
      <div className="am-app" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '100vh' }}>
        <div className="am-coming-soon">
          <h2>Reseller Registration</h2>
          <p>Reseller registration is coming soon in Phase 2.</p>
          <BlueButton onClick={() => setAccountType(null)} style={{ marginTop: '20px' }}>
            Go Back
          </BlueButton>
        </div>
      </div>
    );
  }

  // New Subscriber Registration
  return (
    <div className="am-app" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '100vh', padding: '40px' }}>
      <div style={{ width: '100%', maxWidth: '600px' }}>
        <div style={{ textAlign: 'center', marginBottom: '32px' }}>
          <img
            src="https://customer-assets.emergentagent.com/job_payroll-import-on/artifacts/6c0udnd7_image.png"
            alt="Any Minute"
            style={{ height: '50px', marginBottom: '12px' }}
          />
          <h2>New Subscriber Registration</h2>
        </div>

        <div className="am-card">
          <form onSubmit={handleSubmit}>
            <div className="am-grid am-grid-2">
              <FormField label="First Name" name="first_name" value={form.first_name} onChange={handleChange} error={errors.first_name} required />
              <FormField label="Last Name" name="last_name" value={form.last_name} onChange={handleChange} error={errors.last_name} required />
            </div>
            
            <FormField label="Email" name="email" type="email" value={form.email} onChange={handleChange} error={errors.email} required />
            
            <div className="am-grid am-grid-3">
              <FormField label="Phone" name="phone" value={form.phone} onChange={handleChange} />
              <FormField label="Ext" name="ext" value={form.ext} onChange={handleChange} />
              <FormField label="Mobile" name="mobile" value={form.mobile} onChange={handleChange} />
            </div>
            
            <div style={{ display: 'flex', gap: '12px', alignItems: 'flex-end' }}>
              <div style={{ flex: 1 }}>
                <FormField label="Postal Code" name="postal_code" value={form.postal_code} onChange={handleChange} />
              </div>
              <BlueButton type="button" outline style={{ marginBottom: '16px' }}>Search</BlueButton>
            </div>

            <FormField
              label="Subscription Plan"
              name="plan"
              type="select"
              value={form.plan}
              onChange={handleChange}
              options={[
                { value: 'basic', label: 'Basic' },
                { value: 'standard', label: 'Standard' },
                { value: 'enterprise', label: 'Enterprise' }
              ]}
            />

            <div className="am-grid am-grid-2">
              <FormField label="Password" name="password" type="password" value={form.password} onChange={handleChange} error={errors.password} required />
              <FormField label="Confirm Password" name="confirm_password" type="password" value={form.confirm_password} onChange={handleChange} error={errors.confirm_password} required />
            </div>

            <BlueButton type="submit" disabled={loading} style={{ width: '100%', marginTop: '16px' }}>
              {loading ? 'Creating Account...' : 'Sign Up'}
            </BlueButton>
          </form>
        </div>

        <p style={{ textAlign: 'center', marginTop: '20px' }}>
          <Link to="/anyminute/login" className="am-link">← Back to Sign In</Link>
        </p>
      </div>

      <Popup {...popup} onClose={handlePopupClose} />
    </div>
  );
}
