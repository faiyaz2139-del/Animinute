import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';
import { BlueButton, FormField, Popup } from '../components/SharedComponents';
import { AM_API_URL } from '../context/AMAuthContext';
import '../theme.css';

export default function AMForgotPassword() {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [popup, setPopup] = useState({ open: false });

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!email) {
      setPopup({ open: true, type: 'warning', title: 'Warning', message: 'Please enter your email address.' });
      return;
    }
    setLoading(true);
    try {
      await axios.post(`${AM_API_URL}/auth/forgot-password`, { email });
      setPopup({
        open: true,
        type: 'success',
        title: 'Email Sent',
        message: 'If the email exists in our system, a password reset link has been sent.'
      });
    } catch (err) {
      setPopup({
        open: true,
        type: 'error',
        title: 'Error',
        message: err.response?.data?.detail || 'An error occurred.'
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="am-app" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '100vh' }}>
      <div style={{ width: '100%', maxWidth: '400px', padding: '40px' }}>
        <div style={{ textAlign: 'center', marginBottom: '32px' }}>
          <img
            src="https://customer-assets.emergentagent.com/job_payroll-import-on/artifacts/6c0udnd7_image.png"
            alt="Any Minute"
            style={{ height: '50px', marginBottom: '12px' }}
          />
          <h2>Forgot Password</h2>
          <p style={{ color: '#666', marginTop: '8px' }}>Enter your email to receive a reset link</p>
        </div>

        <div className="am-card">
          <form onSubmit={handleSubmit}>
            <FormField
              label="Login Email"
              name="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
            <BlueButton type="submit" disabled={loading} style={{ width: '100%' }}>
              {loading ? 'Sending...' : 'Submit'}
            </BlueButton>
          </form>
        </div>

        <p style={{ textAlign: 'center', marginTop: '20px' }}>
          <Link to="/anyminute/login" className="am-link">← Back to Sign In</Link>
        </p>
      </div>

      <Popup {...popup} onClose={() => setPopup({ open: false })} />
    </div>
  );
}
