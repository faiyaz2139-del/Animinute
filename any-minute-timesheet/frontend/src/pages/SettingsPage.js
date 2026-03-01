import React, { useState, useEffect } from 'react';
import { api } from '../context/AuthContext';
import { Settings, Key, Copy, RefreshCw, Check } from 'lucide-react';

export default function SettingsPage() {
  const [settings, setSettings] = useState(null);
  const [loading, setLoading] = useState(true);
  const [regenerating, setRegenerating] = useState(false);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    loadSettings();
  }, []);

  const loadSettings = async () => {
    try {
      const res = await api.get('/tenant/settings');
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
      const res = await api.post('/tenant/settings/regenerate-key');
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
      // Fallback for older browsers
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
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6" data-testid="settings-page">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Settings</h1>
        <p className="text-slate-600">Manage your organization settings</p>
      </div>

      {/* Tenant Info */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h2 className="text-lg font-semibold text-slate-900 mb-4">Organization</h2>
        
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-500 mb-1">Tenant Name</label>
            <p className="text-slate-900 font-medium">{settings?.tenant_name || 'Demo Tenant'}</p>
          </div>
          
          <div>
            <label className="block text-sm font-medium text-slate-500 mb-1">Contact Email</label>
            <p className="text-slate-900">{settings?.contact_email || '-'}</p>
          </div>
        </div>
      </div>

      {/* Payroll Integration API Key */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <div className="flex items-center gap-3 mb-4">
          <div className="h-10 w-10 rounded-lg bg-amber-100 flex items-center justify-center">
            <Key className="h-5 w-5 text-amber-600" />
          </div>
          <div>
            <h2 className="text-lg font-semibold text-slate-900">Payroll Integration API Key</h2>
            <p className="text-sm text-slate-500">Use this key to connect with Payroll Canada</p>
          </div>
        </div>

        <div className="bg-slate-50 rounded-lg p-4 mb-4">
          <label className="block text-sm font-medium text-slate-500 mb-2">API Key (X-PAYROLL-KEY)</label>
          <div className="flex items-center gap-2">
            <code className="flex-1 p-3 bg-white border border-slate-200 rounded-lg font-mono text-sm text-slate-900 overflow-x-auto">
              {settings?.payroll_api_key || 'Not generated'}
            </code>
            <button
              onClick={copyToClipboard}
              className="p-3 bg-white border border-slate-200 rounded-lg hover:bg-slate-50 transition-colors"
              title="Copy to clipboard"
              data-testid="copy-api-key-btn"
            >
              {copied ? (
                <Check className="h-5 w-5 text-green-600" />
              ) : (
                <Copy className="h-5 w-5 text-slate-600" />
              )}
            </button>
          </div>
        </div>

        <div className="flex items-center justify-between">
          <p className="text-sm text-slate-500">
            Include this key in the <code className="px-1 py-0.5 bg-slate-100 rounded text-xs">X-PAYROLL-KEY</code> header when calling the payroll integration endpoints.
          </p>
          <button
            onClick={regenerateKey}
            disabled={regenerating}
            className="inline-flex items-center gap-2 px-4 py-2 text-amber-700 bg-amber-50 hover:bg-amber-100 rounded-lg transition-colors disabled:opacity-50"
            data-testid="regenerate-key-btn"
          >
            <RefreshCw className={`h-4 w-4 ${regenerating ? 'animate-spin' : ''}`} />
            {regenerating ? 'Regenerating...' : 'Regenerate Key'}
          </button>
        </div>
      </div>

      {/* Payroll Integration Endpoints */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h2 className="text-lg font-semibold text-slate-900 mb-4">Payroll Integration Endpoints</h2>
        
        <div className="space-y-4">
          <div className="p-4 bg-slate-50 rounded-lg">
            <div className="flex items-center gap-2 mb-2">
              <span className="px-2 py-0.5 bg-green-100 text-green-700 text-xs font-medium rounded">GET</span>
              <code className="text-sm text-slate-900">/am-api/payroll/employees</code>
            </div>
            <p className="text-sm text-slate-600">Returns list of employees with their mapping keys</p>
          </div>

          <div className="p-4 bg-slate-50 rounded-lg">
            <div className="flex items-center gap-2 mb-2">
              <span className="px-2 py-0.5 bg-green-100 text-green-700 text-xs font-medium rounded">GET</span>
              <code className="text-sm text-slate-900">/am-api/payroll/approved-entries?start=YYYY-MM-DD&end=YYYY-MM-DD</code>
            </div>
            <p className="text-sm text-slate-600">Returns approved timesheet entries within date range</p>
          </div>

          <div className="p-4 bg-slate-50 rounded-lg">
            <div className="flex items-center gap-2 mb-2">
              <span className="px-2 py-0.5 bg-blue-100 text-blue-700 text-xs font-medium rounded">POST</span>
              <code className="text-sm text-slate-900">/am-api/payroll/lock?start=YYYY-MM-DD&end=YYYY-MM-DD</code>
            </div>
            <p className="text-sm text-slate-600">Locks approved entries after payroll processing</p>
          </div>
        </div>
      </div>
    </div>
  );
}
