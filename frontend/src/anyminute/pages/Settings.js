import React, { useState, useEffect } from 'react';
import { useAMAuth, AM_API_URL } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import { BlueButton } from '../components/SharedComponents';
import axios from 'axios';
import { Key, Copy, RefreshCw, Check } from 'lucide-react';

export default function AMSettings() {
  const { token } = useAMAuth();
  const [settings, setSettings] = useState(null);
  const [loading, setLoading] = useState(true);
  const [regenerating, setRegenerating] = useState(false);
  const [copied, setCopied] = useState(false);

  const config = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    loadSettings();
  }, [token]);

  const loadSettings = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/tenant/settings`, config);
      setSettings(res.data);
    } catch (err) {
      console.error('Error loading settings:', err);
    } finally {
      setLoading(false);
    }
  };

  const regenerateKey = async () => {
    if (!window.confirm('Are you sure you want to regenerate the API key? The old key will stop working immediately.')) {
      return;
    }

    setRegenerating(true);
    try {
      const res = await axios.post(`${AM_API_URL}/tenant/settings/regenerate-key`, {}, config);
      setSettings(prev => ({ ...prev, payroll_api_key: res.data.payroll_api_key }));
    } catch (err) {
      alert('Failed to regenerate key');
    } finally {
      setRegenerating(false);
    }
  };

  const copyToClipboard = async () => {
    if (!settings?.payroll_api_key) return;
    
    try {
      await navigator.clipboard.writeText(settings.payroll_api_key);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      const textarea = document.createElement('textarea');
      textarea.value = settings.payroll_api_key;
      document.body.appendChild(textarea);
      textarea.select();
      document.execCommand('copy');
      document.body.removeChild(textarea);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  if (loading) {
    return (
      <Layout title="Settings">
        <div style={{ display: 'flex', justifyContent: 'center', padding: '40px' }}>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout title="Settings">
      <div className="am-card" style={{ marginBottom: '24px' }}>
        <h3 style={{ marginBottom: '16px' }}>Organization</h3>
        
        <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
          <div>
            <label style={{ fontSize: '12px', color: '#666', display: 'block', marginBottom: '4px' }}>Tenant Name</label>
            <p style={{ fontWeight: 'bold' }}>{settings?.tenant_name || 'Demo Tenant'}</p>
          </div>
          
          <div>
            <label style={{ fontSize: '12px', color: '#666', display: 'block', marginBottom: '4px' }}>Contact Email</label>
            <p>{settings?.contact_email || '-'}</p>
          </div>
        </div>
      </div>

      <div className="am-card">
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '16px' }}>
          <div style={{ width: '40px', height: '40px', borderRadius: '8px', backgroundColor: '#fff3e0', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <Key size={20} style={{ color: '#ff9800' }} />
          </div>
          <div>
            <h3 style={{ marginBottom: '4px' }}>Payroll Integration API Key</h3>
            <p style={{ fontSize: '14px', color: '#666' }}>Use this key to connect with Payroll Canada</p>
          </div>
        </div>

        <div style={{ backgroundColor: '#f5f5f5', borderRadius: '8px', padding: '16px', marginBottom: '16px' }}>
          <label style={{ fontSize: '12px', color: '#666', display: 'block', marginBottom: '8px' }}>API Key (X-PAYROLL-KEY)</label>
          <div style={{ display: 'flex', gap: '8px' }}>
            <code style={{ flex: 1, padding: '12px', backgroundColor: '#fff', border: '1px solid #e0e0e0', borderRadius: '4px', fontFamily: 'monospace', fontSize: '14px', overflow: 'auto' }}>
              {settings?.payroll_api_key || 'Not generated'}
            </code>
            <button
              onClick={copyToClipboard}
              style={{ padding: '12px', backgroundColor: '#fff', border: '1px solid #e0e0e0', borderRadius: '4px', cursor: 'pointer' }}
              title="Copy to clipboard"
              data-testid="copy-api-key-btn"
            >
              {copied ? <Check size={20} style={{ color: '#4caf50' }} /> : <Copy size={20} style={{ color: '#757575' }} />}
            </button>
          </div>
        </div>

        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <p style={{ fontSize: '14px', color: '#666' }}>
            Include this key in the <code style={{ backgroundColor: '#f5f5f5', padding: '2px 6px', borderRadius: '4px', fontSize: '12px' }}>X-PAYROLL-KEY</code> header when calling the payroll integration endpoints.
          </p>
          <button
            onClick={regenerateKey}
            disabled={regenerating}
            className="am-btn"
            style={{ backgroundColor: '#fff3e0', color: '#e65100' }}
            data-testid="regenerate-key-btn"
          >
            <RefreshCw size={16} style={{ marginRight: '8px' }} className={regenerating ? 'animate-spin' : ''} />
            {regenerating ? 'Regenerating...' : 'Regenerate Key'}
          </button>
        </div>
      </div>

      <div className="am-card" style={{ marginTop: '24px' }}>
        <h3 style={{ marginBottom: '16px' }}>Payroll Integration Endpoints</h3>
        
        <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
          <div style={{ padding: '16px', backgroundColor: '#f5f5f5', borderRadius: '8px' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px' }}>
              <span style={{ padding: '2px 8px', backgroundColor: '#e8f5e9', color: '#2e7d32', fontSize: '12px', fontWeight: 'bold', borderRadius: '4px' }}>GET</span>
              <code style={{ fontSize: '14px' }}>/api/am/payroll/employees</code>
            </div>
            <p style={{ fontSize: '14px', color: '#666' }}>Returns list of employees with their mapping keys</p>
          </div>

          <div style={{ padding: '16px', backgroundColor: '#f5f5f5', borderRadius: '8px' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px' }}>
              <span style={{ padding: '2px 8px', backgroundColor: '#e8f5e9', color: '#2e7d32', fontSize: '12px', fontWeight: 'bold', borderRadius: '4px' }}>GET</span>
              <code style={{ fontSize: '14px' }}>/api/am/payroll/approved-entries?start=YYYY-MM-DD&end=YYYY-MM-DD</code>
            </div>
            <p style={{ fontSize: '14px', color: '#666' }}>Returns approved timesheet entries within date range</p>
          </div>

          <div style={{ padding: '16px', backgroundColor: '#f5f5f5', borderRadius: '8px' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px' }}>
              <span style={{ padding: '2px 8px', backgroundColor: '#e3f2fd', color: '#1565c0', fontSize: '12px', fontWeight: 'bold', borderRadius: '4px' }}>POST</span>
              <code style={{ fontSize: '14px' }}>/api/am/payroll/lock?start=YYYY-MM-DD&end=YYYY-MM-DD</code>
            </div>
            <p style={{ fontSize: '14px', color: '#666' }}>Locks approved entries after payroll processing</p>
          </div>
        </div>
      </div>
    </Layout>
  );
}
