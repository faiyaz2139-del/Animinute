import React, { useState, useEffect, useMemo } from 'react';
import { useAMAuth, AM_API_URL } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import { BlueButton } from '../components/SharedComponents';
import axios from 'axios';
import { ChevronLeft, ChevronRight, Check, Send, AlertCircle, Trash2, Building2, CheckCircle, XCircle } from 'lucide-react';

// Status color mapping
const STATUS_COLORS = {
  pending: { bg: '#fff3cd', color: '#856404', label: 'Pending' },
  approved: { bg: '#d4edda', color: '#155724', label: 'Approved' },
  rejected: { bg: '#f8d7da', color: '#721c24', label: 'Rejected' },
  absent: { bg: '#e2e3e5', color: '#383d41', label: 'Absent' }
};

export default function AMTimesheet() {
  const { user, token, isManager } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState('');
  const [weekStart, setWeekStart] = useState(() => {
    const today = new Date();
    const dayOfWeek = today.getDay();
    const daysUntilSaturday = (dayOfWeek + 1) % 7;
    const saturday = new Date(today);
    saturday.setDate(today.getDate() - daysUntilSaturday);
    return saturday.toISOString().split('T')[0];
  });
  const [weekData, setWeekData] = useState(null);
  const [entries, setEntries] = useState({});
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [pendingWeeks, setPendingWeeks] = useState([]);

  const config = { headers: { Authorization: `Bearer ${token}` } };

  const weekDays = useMemo(() => {
    const start = new Date(weekStart + 'T00:00:00');
    return Array.from({ length: 7 }, (_, i) => {
      const day = new Date(start);
      day.setDate(start.getDate() + i);
      return {
        date: day.toISOString().split('T')[0],
        dayName: day.toLocaleDateString('en-US', { weekday: 'short' }),
        dayNum: day.getDate(),
        isToday: day.toISOString().split('T')[0] === new Date().toISOString().split('T')[0]
      };
    });
  }, [weekStart]);

  useEffect(() => {
    loadBusinesses();
  }, [token]);

  useEffect(() => {
    if (selectedBusiness && token) {
      loadWeekData();
    }
  }, [selectedBusiness, weekStart, token]);

  useEffect(() => {
    if (isManager && token) {
      loadPendingWeeks();
    }
  }, [isManager, token]);

  const loadBusinesses = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/businesses`, config);
      setBusinesses(res.data);
      if (res.data.length > 0) {
        setSelectedBusiness(res.data[0].id);
      }
    } catch (err) {
      console.error('Error loading businesses:', err);
    } finally {
      setLoading(false);
    }
  };

  const loadWeekData = async () => {
    try {
      const weekRes = await axios.post(`${AM_API_URL}/timesheet-weeks`, {
        user_id: user.id,
        business_id: selectedBusiness,
        week_start_date: weekStart
      }, config);
      setWeekData(weekRes.data);

      const entriesRes = await axios.get(`${AM_API_URL}/timesheet-entries`, {
        ...config,
        params: { week_id: weekRes.data.id }
      });

      const entriesMap = {};
      weekDays.forEach(day => {
        const entry = entriesRes.data.find(e => e.work_date === day.date);
        entriesMap[day.date] = entry || {
          work_date: day.date,
          start_time: '',
          end_time: '',
          break_minutes: 0,
          notes: '',
          net_hours: 0
        };
      });
      setEntries(entriesMap);
    } catch (err) {
      console.error('Error loading week data:', err);
    }
  };

  const loadPendingWeeks = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/timesheet-weeks`, {
        ...config,
        params: { status: 'submitted' }
      });
      setPendingWeeks(res.data);
    } catch (err) {
      console.error('Error loading pending weeks:', err);
    }
  };

  const navigateWeek = (direction) => {
    const start = new Date(weekStart + 'T00:00:00');
    start.setDate(start.getDate() + direction * 7);
    setWeekStart(start.toISOString().split('T')[0]);
  };

  const handleEntryChange = (date, field, value) => {
    setEntries(prev => ({
      ...prev,
      [date]: { ...prev[date], [field]: value }
    }));
  };

  const calculateNetHours = (entry) => {
    if (!entry.start_time || !entry.end_time) return 0;
    try {
      const start = new Date(`2000-01-01T${entry.start_time}`);
      let end = new Date(`2000-01-01T${entry.end_time}`);
      if (end < start) end.setDate(end.getDate() + 1);
      const totalMinutes = (end - start) / 60000;
      const netMinutes = totalMinutes - (entry.break_minutes || 0);
      return Math.max(0, netMinutes / 60).toFixed(2);
    } catch {
      return 0;
    }
  };

  const saveEntry = async (date) => {
    const entry = entries[date];
    if (!entry.start_time && !entry.end_time) return;

    setSaving(true);
    try {
      if (entry.id) {
        await axios.put(`${AM_API_URL}/timesheet-entries/${entry.id}`, {
          start_time: entry.start_time || null,
          end_time: entry.end_time || null,
          break_minutes: parseInt(entry.break_minutes) || 0,
          notes: entry.notes || ''
        }, config);
      } else {
        const res = await axios.post(`${AM_API_URL}/timesheet-entries`, {
          business_id: selectedBusiness,
          work_date: date,
          start_time: entry.start_time || null,
          end_time: entry.end_time || null,
          break_minutes: parseInt(entry.break_minutes) || 0,
          notes: entry.notes || ''
        }, config);
        setEntries(prev => ({ ...prev, [date]: res.data }));
      }
      await loadWeekData();
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to save entry');
    } finally {
      setSaving(false);
    }
  };

  const submitTimesheet = async () => {
    if (!weekData) return;
    
    for (const day of weekDays) {
      const entry = entries[day.date];
      if (entry.start_time || entry.end_time) {
        await saveEntry(day.date);
      }
    }

    try {
      await axios.post(`${AM_API_URL}/timesheet-weeks/${weekData.id}/submit`, {}, config);
      await loadWeekData();
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to submit timesheet');
    }
  };

  const handleApproveReject = async (weekId, status, reason = null) => {
    try {
      await axios.post(`${AM_API_URL}/timesheet-weeks/${weekId}/approve`, {
        status,
        rejection_reason: reason
      }, config);
      await loadPendingWeeks();
      if (weekData?.id === weekId) await loadWeekData();
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to update timesheet');
    }
  };

  const updateEntryStatus = async (entryId, newStatus) => {
    try {
      await axios.put(`${AM_API_URL}/timesheet-entries/${entryId}/status`, {
        status: newStatus
      }, config);
      await loadWeekData();
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to update status');
    }
  };

  const bulkApproveWeek = async () => {
    if (!weekData) return;
    try {
      await axios.post(`${AM_API_URL}/timesheet-weeks/${weekData.id}/bulk-approve`, {}, config);
      await loadWeekData();
      await loadPendingWeeks();
      alert('All entries approved!');
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to bulk approve');
    }
  };

  const bulkRejectWeek = async () => {
    const reason = prompt('Rejection reason (optional):');
    if (!weekData) return;
    try {
      await axios.post(`${AM_API_URL}/timesheet-weeks/${weekData.id}/bulk-reject?reason=${encodeURIComponent(reason || '')}`, {}, config);
      await loadWeekData();
      await loadPendingWeeks();
      alert('All entries rejected!');
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to bulk reject');
    }
  };

  const totalHours = useMemo(() => {
    return Object.values(entries).reduce((sum, entry) => {
      return sum + parseFloat(calculateNetHours(entry) || 0);
    }, 0).toFixed(2);
  }, [entries]);

  const canEdit = weekData && !weekData.locked && weekData.status !== 'approved';
  const canSubmit = weekData && (weekData.status === 'draft' || weekData.status === 'rejected');

  if (loading) {
    return (
      <Layout title="Timesheet">
        <div style={{ display: 'flex', justifyContent: 'center', padding: '40px' }}>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout title="Timesheet">
      <div style={{ marginBottom: '20px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <select
          value={selectedBusiness}
          onChange={(e) => setSelectedBusiness(e.target.value)}
          className="am-select"
          data-testid="business-select"
        >
          {businesses.map(biz => (
            <option key={biz.id} value={biz.id}>{biz.name}</option>
          ))}
        </select>
      </div>

      <div className="am-card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <button onClick={() => navigateWeek(-1)} className="am-icon-btn" data-testid="prev-week-btn">
            <ChevronLeft size={20} />
          </button>
          
          <div style={{ textAlign: 'center' }}>
            <h3>Week of {new Date(weekStart + 'T00:00:00').toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' })}</h3>
            {weekData && (
              <span className={`am-badge am-badge-${weekData.status}`}>
                {weekData.status.charAt(0).toUpperCase() + weekData.status.slice(1)}
                {weekData.locked && ' (Locked)'}
              </span>
            )}
          </div>
          
          <button onClick={() => navigateWeek(1)} className="am-icon-btn" data-testid="next-week-btn">
            <ChevronRight size={20} />
          </button>
        </div>

        {weekData?.rejection_reason && (
          <div className="am-error" style={{ marginBottom: '20px', display: 'flex', gap: '8px', alignItems: 'flex-start' }}>
            <AlertCircle size={20} />
            <div>
              <strong>Rejected:</strong> {weekData.rejection_reason}
            </div>
          </div>
        )}

        <div style={{ overflowX: 'auto' }}>
          <table className="am-table">
            <thead>
              <tr>
                <th>Day</th>
                <th>Start</th>
                <th>End</th>
                <th>Break (min)</th>
                <th>Net Hours</th>
                <th>Status</th>
                <th>Notes</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {weekDays.map((day) => {
                const entry = entries[day.date] || {};
                const netHours = calculateNetHours(entry);
                const entryStatus = entry.entry_status || 'pending';
                const statusStyle = STATUS_COLORS[entryStatus] || STATUS_COLORS.pending;
                
                return (
                  <tr key={day.date} style={{ backgroundColor: day.isToday ? '#e3f2fd' : 'transparent' }}>
                    <td>
                      <div style={{ fontWeight: day.isToday ? 'bold' : 'normal' }}>{day.dayName}</div>
                      <div style={{ fontSize: '12px', color: '#666' }}>{day.dayNum}</div>
                    </td>
                    <td>
                      <input
                        type="time"
                        value={entry.start_time || ''}
                        onChange={(e) => handleEntryChange(day.date, 'start_time', e.target.value)}
                        disabled={!canEdit}
                        className="am-input am-input-sm"
                        data-testid={`start-time-${day.date}`}
                      />
                    </td>
                    <td>
                      <input
                        type="time"
                        value={entry.end_time || ''}
                        onChange={(e) => handleEntryChange(day.date, 'end_time', e.target.value)}
                        disabled={!canEdit}
                        className="am-input am-input-sm"
                        data-testid={`end-time-${day.date}`}
                      />
                    </td>
                    <td>
                      <input
                        type="number"
                        min="0"
                        value={entry.break_minutes || ''}
                        onChange={(e) => handleEntryChange(day.date, 'break_minutes', e.target.value)}
                        disabled={!canEdit}
                        className="am-input am-input-sm"
                        style={{ width: '70px' }}
                        data-testid={`break-${day.date}`}
                      />
                    </td>
                    <td style={{ fontWeight: 'bold' }}>{netHours}</td>
                    {/* Entry Status */}
                    <td>
                      {entry.id && isManager ? (
                        <select
                          value={entryStatus}
                          onChange={(e) => updateEntryStatus(entry.id, e.target.value)}
                          className="am-select am-select-sm"
                          style={{ 
                            backgroundColor: statusStyle.bg, 
                            color: statusStyle.color,
                            fontWeight: '600',
                            fontSize: '11px',
                            padding: '4px 8px',
                            borderRadius: '4px'
                          }}
                          data-testid={`status-${day.date}`}
                        >
                          <option value="pending">Pending</option>
                          <option value="approved">Approved</option>
                          <option value="rejected">Rejected</option>
                          <option value="absent">Absent</option>
                        </select>
                      ) : entry.id ? (
                        <span style={{
                          padding: '4px 8px',
                          backgroundColor: statusStyle.bg,
                          color: statusStyle.color,
                          borderRadius: '4px',
                          fontSize: '11px',
                          fontWeight: '600'
                        }}>
                          {statusStyle.label}
                        </span>
                      ) : null}
                    </td>
                    <td>
                      <input
                        type="text"
                        value={entry.notes || ''}
                        onChange={(e) => handleEntryChange(day.date, 'notes', e.target.value)}
                        disabled={!canEdit}
                        placeholder="Notes..."
                        className="am-input am-input-sm"
                        style={{ width: '120px' }}
                        data-testid={`notes-${day.date}`}
                      />
                    </td>
                    <td style={{ display: 'flex', gap: '4px' }}>
                      {canEdit && (entry.start_time || entry.end_time) && (
                        <button
                          onClick={() => saveEntry(day.date)}
                          disabled={saving}
                          className="am-icon-btn"
                          title="Save"
                        >
                          <Check size={16} />
                        </button>
                      )}
                    </td>
                  </tr>
                );
              })}
            </tbody>
            <tfoot>
              <tr style={{ backgroundColor: '#f5f5f5' }}>
                <td colSpan="4" style={{ textAlign: 'right', fontWeight: 'bold' }}>Total Hours:</td>
                <td style={{ fontWeight: 'bold', fontSize: '18px' }}>{totalHours}</td>
                <td colSpan="2"></td>
              </tr>
            </tfoot>
          </table>
        </div>

        {canSubmit && (
          <div style={{ marginTop: '20px', display: 'flex', justifyContent: 'flex-end' }}>
            <BlueButton onClick={submitTimesheet} data-testid="submit-timesheet-btn">
              <Send size={16} style={{ marginRight: '8px' }} />
              Submit for Approval
            </BlueButton>
          </div>
        )}
      </div>

      {isManager && pendingWeeks.length > 0 && (
        <div className="am-card" style={{ marginTop: '24px' }}>
          <h3 style={{ marginBottom: '16px' }}>Pending Approvals ({pendingWeeks.length})</h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
            {pendingWeeks.map((week) => (
              <div
                key={week.id}
                style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '12px', backgroundColor: '#f5f5f5', borderRadius: '8px' }}
              >
                <div>
                  <strong>Week of {new Date(week.week_start_date + 'T00:00:00').toLocaleDateString()}</strong>
                  <div style={{ fontSize: '14px', color: '#666' }}>{week.total_hours?.toFixed(1) || 0} hours</div>
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button
                    onClick={() => handleApproveReject(week.id, 'approved')}
                    className="am-btn am-btn-success"
                    data-testid={`approve-${week.id}`}
                  >
                    Approve
                  </button>
                  <button
                    onClick={() => {
                      const reason = prompt('Rejection reason:');
                      if (reason) handleApproveReject(week.id, 'rejected', reason);
                    }}
                    className="am-btn am-btn-danger"
                    data-testid={`reject-${week.id}`}
                  >
                    Reject
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </Layout>
  );
}
