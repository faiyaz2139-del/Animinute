import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAMAuth } from '../context/AMAuthContext';
import { BlueButton, FormField, Popup } from '../components/SharedComponents';
import '../theme.css';

export default function AMLogin() {
  const { login } = useAMAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(form.email, form.password);
      navigate('/anyminute/dashboard');
    } catch (err) {
      setError(err.response?.data?.detail || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="am-app" style={{ display: 'flex', minHeight: '100vh' }}>
      <div style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '40px' }}>
        <div style={{ width: '100%', maxWidth: '400px' }}>
          {/* Logo & Tagline */}
          <div style={{ textAlign: 'center', marginBottom: '40px' }}>
            <img
              src="https://customer-assets.emergentagent.com/job_payroll-import-on/artifacts/6c0udnd7_image.png"
              alt="Any Minute"
              style={{ height: '60px', marginBottom: '12px' }}
            />
            <p style={{ color: '#666', fontSize: '14px' }}>
              Time Tracker For Teams – Any Where, Any Time
            </p>
          </div>

          {/* Login Form */}
          <div className="am-card">
            <h2 style={{ marginBottom: '24px', textAlign: 'center' }}>Sign In</h2>
            
            {error && <div className="am-error" style={{ marginBottom: '16px', textAlign: 'center' }}>{error}</div>}
            
            <form onSubmit={handleSubmit}>
              <FormField
                label="Email"
                name="email"
                type="email"
                value={form.email}
                onChange={(e) => setForm({ ...form, email: e.target.value })}
                required
              />
              <FormField
                label="Password"
                name="password"
                type="password"
                value={form.password}
                onChange={(e) => setForm({ ...form, password: e.target.value })}
                required
              />
              
              <BlueButton type="submit" disabled={loading} className="w-full" style={{ width: '100%', marginTop: '8px' }}>
                {loading ? 'Signing in...' : 'SIGN IN'}
              </BlueButton>
            </form>

            <div style={{ marginTop: '20px', textAlign: 'center' }}>
              <Link to="/anyminute/forgot-password" className="am-link">Forgot Password?</Link>
            </div>
          </div>

          <p style={{ textAlign: 'center', marginTop: '24px', color: '#666' }}>
            Don't Have Any Minute Account Yet?{' '}
            <Link to="/anyminute/signup" className="am-link">Sign up now</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
