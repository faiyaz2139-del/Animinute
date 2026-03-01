import React, { useState, useEffect } from 'react';
import { useAMAuth } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import { ClipboardList, Filter, Search, User, Building2, FileText, DollarSign, Ticket, Loader2 } from 'lucide-react';
import axios from 'axios';
import { toast } from 'sonner';

const API = `${process.env.REACT_APP_BACKEND_URL}/api/am`;

const AuditLogs = () => {
  const { token, isAdmin } = useAMAuth();
  const [logs, setLogs] = useState([]);
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const [filterEntity, setFilterEntity] = useState('');
  const [filterAction, setFilterAction] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  const headers = { Authorization: `Bearer ${token}` };

  useEffect(() => {
    if (isAdmin) {
      fetchLogs();
      fetchSummary();
    }
  }, [isAdmin, filterEntity, filterAction]);

  const fetchLogs = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (filterEntity) params.append('entity_type', filterEntity);
      if (filterAction) params.append('action', filterAction);
      params.append('limit', '200');
      
      const response = await axios.get(`${API}/audit-logs?${params.toString()}`, { headers });
      setLogs(response.data);
    } catch (error) {
      toast.error('Failed to fetch audit logs');
    } finally {
      setLoading(false);
    }
  };

  const fetchSummary = async () => {
    try {
      const response = await axios.get(`${API}/audit-logs/summary`, { headers });
      setSummary(response.data);
    } catch (error) {
      console.error('Failed to fetch summary:', error);
    }
  };

  const getActionBadge = (action) => {
    const styles = {
      CREATE: { bg: '#dcfce7', color: '#166534', label: 'Created' },
      UPDATE: { bg: '#dbeafe', color: '#1e40af', label: 'Updated' },
      DELETE: { bg: '#fee2e2', color: '#991b1b', label: 'Deleted' },
    };
    const style = styles[action] || { bg: '#f3f4f6', color: '#374151', label: action };
    return (
      <span 
        style={{ 
          backgroundColor: style.bg, 
          color: style.color,
          padding: '4px 10px',
          borderRadius: '12px',
          fontSize: '12px',
          fontWeight: '500'
        }}
      >
        {style.label}
      </span>
    );
  };

  const getEntityIcon = (type) => {
    const icons = {
      user: <User size={16} />,
      business: <Building2 size={16} />,
      timesheet: <FileText size={16} />,
      pay_rate: <DollarSign size={16} />,
      ticket: <Ticket size={16} />,
    };
    return icons[type] || <ClipboardList size={16} />;
  };

  const formatTimestamp = (ts) => {
    if (!ts) return '-';
    const date = new Date(ts);
    return date.toLocaleString('en-CA', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const formatChanges = (log) => {
    if (!log.changes || Object.keys(log.changes).length === 0) {
      if (log.metadata) {
        return Object.entries(log.metadata)
          .map(([k, v]) => `${k}: ${v}`)
          .join(', ');
      }
      return '-';
    }
    
    return Object.entries(log.changes)
      .map(([field, change]) => {
        const oldVal = change.old !== null && change.old !== undefined ? String(change.old) : '(empty)';
        const newVal = change.new !== null && change.new !== undefined ? String(change.new) : '(empty)';
        return `${field}: ${oldVal} → ${newVal}`;
      })
      .join('; ');
  };

  const filteredLogs = logs.filter(log => {
    if (!searchTerm) return true;
    const search = searchTerm.toLowerCase();
    return (
      (log.entity_name || '').toLowerCase().includes(search) ||
      (log.actor_name || '').toLowerCase().includes(search) ||
      (log.entity_type || '').toLowerCase().includes(search)
    );
  });

  if (!isAdmin) {
    return (
      <Layout title="Audit Logs">
        <div className="am-card" style={{ textAlign: 'center', padding: '60px 20px' }}>
          <ClipboardList size={48} style={{ color: '#9ca3af', marginBottom: '16px' }} />
          <h2 style={{ marginBottom: '8px' }}>Admin Access Required</h2>
          <p style={{ color: '#6b7280' }}>Only administrators can view audit logs.</p>
        </div>
      </Layout>
    );
  }

  return (
    <Layout title="Audit Logs">
      <div style={{ padding: '24px' }} data-testid="audit-logs-page">
        {/* Summary Cards */}
        {summary && (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '16px', marginBottom: '24px' }}>
            <div className="am-card" style={{ padding: '16px', textAlign: 'center' }}>
              <div style={{ fontSize: '28px', fontWeight: '700', color: '#1a73e8' }}>{summary.total}</div>
              <div style={{ color: '#6b7280', fontSize: '14px' }}>Total Events</div>
            </div>
            <div className="am-card" style={{ padding: '16px', textAlign: 'center' }}>
              <div style={{ fontSize: '28px', fontWeight: '700', color: '#16a34a' }}>{summary.by_action?.create || 0}</div>
              <div style={{ color: '#6b7280', fontSize: '14px' }}>Created</div>
            </div>
            <div className="am-card" style={{ padding: '16px', textAlign: 'center' }}>
              <div style={{ fontSize: '28px', fontWeight: '700', color: '#2563eb' }}>{summary.by_action?.update || 0}</div>
              <div style={{ color: '#6b7280', fontSize: '14px' }}>Updated</div>
            </div>
            <div className="am-card" style={{ padding: '16px', textAlign: 'center' }}>
              <div style={{ fontSize: '28px', fontWeight: '700', color: '#dc2626' }}>{summary.by_action?.delete || 0}</div>
              <div style={{ color: '#6b7280', fontSize: '14px' }}>Deleted</div>
            </div>
          </div>
        )}

        {/* Filters */}
        <div className="am-card" style={{ marginBottom: '24px' }}>
          <div style={{ padding: '16px', borderBottom: '1px solid #e5e7eb' }}>
            <h2 style={{ margin: 0, display: 'flex', alignItems: 'center', gap: '8px' }}>
              <ClipboardList size={20} />
              Activity History
            </h2>
            <p style={{ margin: '4px 0 0', color: '#6b7280', fontSize: '14px' }}>
              Track all changes and actions in your organization
            </p>
          </div>
          
          <div style={{ padding: '16px', display: 'flex', flexWrap: 'wrap', gap: '12px', alignItems: 'center' }}>
            <div style={{ position: 'relative', flex: '1', minWidth: '200px' }}>
              <Search size={16} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: '#9ca3af' }} />
              <input
                type="text"
                placeholder="Search by name, entity..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="am-input"
                style={{ paddingLeft: '36px', width: '100%' }}
                data-testid="audit-search-input"
              />
            </div>
            
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
              <Filter size={16} style={{ color: '#6b7280' }} />
              <select
                value={filterEntity}
                onChange={(e) => setFilterEntity(e.target.value)}
                className="am-input"
                style={{ minWidth: '140px' }}
                data-testid="entity-filter"
              >
                <option value="">All Entities</option>
                <option value="user">Users</option>
                <option value="business">Businesses</option>
                <option value="timesheet">Timesheets</option>
                <option value="pay_rate">Pay Rates</option>
                <option value="ticket">Tickets</option>
              </select>
              
              <select
                value={filterAction}
                onChange={(e) => setFilterAction(e.target.value)}
                className="am-input"
                style={{ minWidth: '120px' }}
                data-testid="action-filter"
              >
                <option value="">All Actions</option>
                <option value="CREATE">Created</option>
                <option value="UPDATE">Updated</option>
                <option value="DELETE">Deleted</option>
              </select>
            </div>
          </div>
        </div>

        {/* Logs Table */}
        <div className="am-card">
          {loading ? (
            <div style={{ padding: '60px', textAlign: 'center' }}>
              <Loader2 size={32} className="am-spin" style={{ color: '#1a73e8' }} />
              <p style={{ color: '#6b7280', marginTop: '12px' }}>Loading audit logs...</p>
            </div>
          ) : filteredLogs.length === 0 ? (
            <div style={{ padding: '60px', textAlign: 'center' }}>
              <ClipboardList size={48} style={{ color: '#9ca3af', marginBottom: '12px' }} />
              <p style={{ color: '#6b7280' }}>No audit logs found</p>
            </div>
          ) : (
            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ backgroundColor: '#f9fafb' }}>
                    <th style={{ padding: '12px 16px', textAlign: 'left', fontWeight: '600', fontSize: '13px', color: '#374151', borderBottom: '1px solid #e5e7eb' }}>Timestamp</th>
                    <th style={{ padding: '12px 16px', textAlign: 'left', fontWeight: '600', fontSize: '13px', color: '#374151', borderBottom: '1px solid #e5e7eb' }}>Action</th>
                    <th style={{ padding: '12px 16px', textAlign: 'left', fontWeight: '600', fontSize: '13px', color: '#374151', borderBottom: '1px solid #e5e7eb' }}>Entity</th>
                    <th style={{ padding: '12px 16px', textAlign: 'left', fontWeight: '600', fontSize: '13px', color: '#374151', borderBottom: '1px solid #e5e7eb' }}>Name</th>
                    <th style={{ padding: '12px 16px', textAlign: 'left', fontWeight: '600', fontSize: '13px', color: '#374151', borderBottom: '1px solid #e5e7eb' }}>Changed By</th>
                    <th style={{ padding: '12px 16px', textAlign: 'left', fontWeight: '600', fontSize: '13px', color: '#374151', borderBottom: '1px solid #e5e7eb' }}>Changes</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredLogs.map((log) => (
                    <tr 
                      key={log.id} 
                      style={{ borderBottom: '1px solid #f3f4f6' }}
                      data-testid={`audit-row-${log.id}`}
                    >
                      <td style={{ padding: '12px 16px', fontSize: '13px', color: '#6b7280', whiteSpace: 'nowrap' }}>
                        {formatTimestamp(log.timestamp)}
                      </td>
                      <td style={{ padding: '12px 16px' }}>
                        {getActionBadge(log.action)}
                      </td>
                      <td style={{ padding: '12px 16px' }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '6px', color: '#374151' }}>
                          {getEntityIcon(log.entity_type)}
                          <span style={{ textTransform: 'capitalize' }}>{log.entity_type?.replace('_', ' ')}</span>
                        </div>
                      </td>
                      <td style={{ padding: '12px 16px', fontWeight: '500', color: '#111827', maxWidth: '200px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {log.entity_name || '-'}
                      </td>
                      <td style={{ padding: '12px 16px', color: '#374151' }}>
                        {log.actor_name || '-'}
                      </td>
                      <td style={{ padding: '12px 16px', fontSize: '13px', color: '#6b7280', maxWidth: '300px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {formatChanges(log)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      <style>{`
        .am-spin {
          animation: spin 1s linear infinite;
        }
        @keyframes spin {
          from { transform: rotate(0deg); }
          to { transform: rotate(360deg); }
        }
      `}</style>
    </Layout>
  );
};

export default AuditLogs;
