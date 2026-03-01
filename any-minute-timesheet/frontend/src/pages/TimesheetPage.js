import React, { useState, useEffect, useMemo } from 'react';
import { api, useAuth } from '../context/AuthContext';
import { Clock, ChevronLeft, ChevronRight, Check, X, Send, AlertCircle } from 'lucide-react';
import { format, addDays, startOfWeek, parseISO } from 'date-fns';

export default function TimesheetPage() {
  const { user, isManager } = useAuth();
  const [businesses, setBusinesses] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState('');
  const [weekStart, setWeekStart] = useState(() => {
    // Get current Saturday (week starts Saturday)
    const today = new Date();
    const dayOfWeek = today.getDay();
    const daysUntilSaturday = (dayOfWeek + 1) % 7;
    const saturday = new Date(today);
    saturday.setDate(today.getDate() - daysUntilSaturday);
    return format(saturday, 'yyyy-MM-dd');
  });
  const [weekData, setWeekData] = useState(null);
  const [entries, setEntries] = useState({});
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [pendingWeeks, setPendingWeeks] = useState([]);

  // Generate week days starting from Saturday
  const weekDays = useMemo(() => {
    const start = parseISO(weekStart);
    return Array.from({ length: 7 }, (_, i) => {
      const day = addDays(start, i);
      return {
        date: format(day, 'yyyy-MM-dd'),
        dayName: format(day, 'EEE'),
        dayNum: format(day, 'd'),
        isToday: format(day, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
      };
    });
  }, [weekStart]);

  useEffect(() => {
    loadBusinesses();
  }, []);

  useEffect(() => {
    if (selectedBusiness) {
      loadWeekData();
    }
  }, [selectedBusiness, weekStart]);

  useEffect(() => {
    if (isManager) {
      loadPendingWeeks();
    }
  }, [isManager]);

  const loadBusinesses = async () => {
    try {
      const res = await api.get('/businesses');
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
      // Get or create week
      const weekRes = await api.post('/timesheet-weeks', {
        user_id: user.id,
        business_id: selectedBusiness,
        week_start_date: weekStart
      });
      setWeekData(weekRes.data);

      // Get entries for this week
      const entriesRes = await api.get('/timesheet-entries', {
        params: { week_id: weekRes.data.id }
      });

      // Index entries by date
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
      const res = await api.get('/timesheet-weeks', { params: { status: 'submitted' } });
      setPendingWeeks(res.data);
    } catch (err) {
      console.error('Error loading pending weeks:', err);
    }
  };

  const navigateWeek = (direction) => {
    const start = parseISO(weekStart);
    const newStart = addDays(start, direction * 7);
    setWeekStart(format(newStart, 'yyyy-MM-dd'));
  };

  const handleEntryChange = (date, field, value) => {
    setEntries(prev => ({
      ...prev,
      [date]: {
        ...prev[date],
        [field]: value
      }
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
        await api.put(`/timesheet-entries/${entry.id}`, {
          start_time: entry.start_time || null,
          end_time: entry.end_time || null,
          break_minutes: parseInt(entry.break_minutes) || 0,
          notes: entry.notes || ''
        });
      } else {
        const res = await api.post('/timesheet-entries', {
          business_id: selectedBusiness,
          work_date: date,
          start_time: entry.start_time || null,
          end_time: entry.end_time || null,
          break_minutes: parseInt(entry.break_minutes) || 0,
          notes: entry.notes || ''
        });
        setEntries(prev => ({
          ...prev,
          [date]: res.data
        }));
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
    
    // Save all entries first
    for (const day of weekDays) {
      const entry = entries[day.date];
      if (entry.start_time || entry.end_time) {
        await saveEntry(day.date);
      }
    }

    try {
      await api.post(`/timesheet-weeks/${weekData.id}/submit`);
      await loadWeekData();
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to submit timesheet');
    }
  };

  const handleApproveReject = async (weekId, status, reason = null) => {
    try {
      await api.post(`/timesheet-weeks/${weekId}/approve`, {
        status,
        rejection_reason: reason
      });
      await loadPendingWeeks();
      if (weekData?.id === weekId) {
        await loadWeekData();
      }
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to update timesheet');
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
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6" data-testid="timesheet-page">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Timesheet</h1>
          <p className="text-slate-600">Enter your work hours</p>
        </div>
        
        <div className="flex items-center gap-4">
          <select
            value={selectedBusiness}
            onChange={(e) => setSelectedBusiness(e.target.value)}
            className="px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
            data-testid="business-select"
          >
            {businesses.map(biz => (
              <option key={biz.id} value={biz.id}>{biz.name}</option>
            ))}
          </select>
        </div>
      </div>

      {/* Week Navigation */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-4">
        <div className="flex items-center justify-between mb-4">
          <button
            onClick={() => navigateWeek(-1)}
            className="p-2 hover:bg-slate-100 rounded-lg"
            data-testid="prev-week-btn"
          >
            <ChevronLeft className="h-5 w-5" />
          </button>
          
          <div className="text-center">
            <h2 className="text-lg font-semibold text-slate-900">
              Week of {format(parseISO(weekStart), 'MMM d, yyyy')}
            </h2>
            {weekData && (
              <span className={`inline-flex px-2 py-0.5 text-xs font-medium rounded-full mt-1 ${
                weekData.status === 'approved' ? 'bg-green-100 text-green-700' :
                weekData.status === 'submitted' ? 'bg-amber-100 text-amber-700' :
                weekData.status === 'rejected' ? 'bg-red-100 text-red-700' :
                'bg-slate-100 text-slate-700'
              }`}>
                {weekData.status.charAt(0).toUpperCase() + weekData.status.slice(1)}
                {weekData.locked && ' (Locked)'}
              </span>
            )}
          </div>
          
          <button
            onClick={() => navigateWeek(1)}
            className="p-2 hover:bg-slate-100 rounded-lg"
            data-testid="next-week-btn"
          >
            <ChevronRight className="h-5 w-5" />
          </button>
        </div>

        {weekData?.rejection_reason && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm flex items-start gap-2">
            <AlertCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
            <div>
              <p className="font-medium">Rejected</p>
              <p>{weekData.rejection_reason}</p>
            </div>
          </div>
        )}

        {/* Timesheet Grid */}
        <div className="overflow-x-auto">
          <table className="w-full min-w-[700px]">
            <thead>
              <tr className="border-b border-slate-200">
                <th className="text-left py-3 px-2 text-sm font-medium text-slate-500">Day</th>
                <th className="text-left py-3 px-2 text-sm font-medium text-slate-500">Start</th>
                <th className="text-left py-3 px-2 text-sm font-medium text-slate-500">End</th>
                <th className="text-left py-3 px-2 text-sm font-medium text-slate-500">Break (min)</th>
                <th className="text-left py-3 px-2 text-sm font-medium text-slate-500">Net Hours</th>
                <th className="text-left py-3 px-2 text-sm font-medium text-slate-500">Notes</th>
                <th className="py-3 px-2"></th>
              </tr>
            </thead>
            <tbody>
              {weekDays.map((day) => {
                const entry = entries[day.date] || {};
                const netHours = calculateNetHours(entry);
                return (
                  <tr key={day.date} className={`border-b border-slate-100 ${day.isToday ? 'bg-blue-50' : ''}`}>
                    <td className="py-3 px-2">
                      <div className={`font-medium ${day.isToday ? 'text-blue-600' : 'text-slate-900'}`}>
                        {day.dayName}
                      </div>
                      <div className="text-sm text-slate-500">{day.dayNum}</div>
                    </td>
                    <td className="py-3 px-2">
                      <input
                        type="time"
                        value={entry.start_time || ''}
                        onChange={(e) => handleEntryChange(day.date, 'start_time', e.target.value)}
                        disabled={!canEdit}
                        className="px-2 py-1.5 border border-slate-300 rounded-lg text-sm w-28 disabled:bg-slate-100"
                        data-testid={`start-time-${day.date}`}
                      />
                    </td>
                    <td className="py-3 px-2">
                      <input
                        type="time"
                        value={entry.end_time || ''}
                        onChange={(e) => handleEntryChange(day.date, 'end_time', e.target.value)}
                        disabled={!canEdit}
                        className="px-2 py-1.5 border border-slate-300 rounded-lg text-sm w-28 disabled:bg-slate-100"
                        data-testid={`end-time-${day.date}`}
                      />
                    </td>
                    <td className="py-3 px-2">
                      <input
                        type="number"
                        min="0"
                        value={entry.break_minutes || ''}
                        onChange={(e) => handleEntryChange(day.date, 'break_minutes', e.target.value)}
                        disabled={!canEdit}
                        className="px-2 py-1.5 border border-slate-300 rounded-lg text-sm w-20 disabled:bg-slate-100"
                        data-testid={`break-${day.date}`}
                      />
                    </td>
                    <td className="py-3 px-2">
                      <span className="font-medium text-slate-900">{netHours}</span>
                    </td>
                    <td className="py-3 px-2">
                      <input
                        type="text"
                        value={entry.notes || ''}
                        onChange={(e) => handleEntryChange(day.date, 'notes', e.target.value)}
                        disabled={!canEdit}
                        placeholder="Notes..."
                        className="px-2 py-1.5 border border-slate-300 rounded-lg text-sm w-full min-w-[100px] disabled:bg-slate-100"
                        data-testid={`notes-${day.date}`}
                      />
                    </td>
                    <td className="py-3 px-2">
                      {canEdit && (entry.start_time || entry.end_time) && (
                        <button
                          onClick={() => saveEntry(day.date)}
                          disabled={saving}
                          className="p-1.5 text-blue-600 hover:bg-blue-50 rounded-lg disabled:opacity-50"
                          title="Save"
                        >
                          <Check className="h-4 w-4" />
                        </button>
                      )}
                    </td>
                  </tr>
                );
              })}
            </tbody>
            <tfoot>
              <tr className="bg-slate-50">
                <td colSpan="4" className="py-3 px-2 text-right font-medium text-slate-700">
                  Total Hours:
                </td>
                <td className="py-3 px-2 font-bold text-slate-900">{totalHours}</td>
                <td colSpan="2"></td>
              </tr>
            </tfoot>
          </table>
        </div>

        {/* Submit Button */}
        {canSubmit && (
          <div className="mt-4 flex justify-end">
            <button
              onClick={submitTimesheet}
              className="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors"
              data-testid="submit-timesheet-btn"
            >
              <Send className="h-4 w-4" />
              Submit for Approval
            </button>
          </div>
        )}
      </div>

      {/* Pending Approvals (Managers) */}
      {isManager && pendingWeeks.length > 0 && (
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
          <h2 className="text-lg font-semibold text-slate-900 mb-4">
            Pending Approvals ({pendingWeeks.length})
          </h2>
          <div className="space-y-3">
            {pendingWeeks.map((week) => (
              <div
                key={week.id}
                className="flex items-center justify-between p-4 bg-slate-50 rounded-lg"
              >
                <div>
                  <p className="font-medium text-slate-900">
                    Week of {format(parseISO(week.week_start_date), 'MMM d, yyyy')}
                  </p>
                  <p className="text-sm text-slate-500">
                    {week.total_hours?.toFixed(1) || 0} hours
                  </p>
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={() => handleApproveReject(week.id, 'approved')}
                    className="inline-flex items-center gap-1 px-3 py-1.5 bg-green-600 hover:bg-green-700 text-white text-sm rounded-lg"
                    data-testid={`approve-${week.id}`}
                  >
                    <Check className="h-4 w-4" />
                    Approve
                  </button>
                  <button
                    onClick={() => {
                      const reason = prompt('Rejection reason:');
                      if (reason) handleApproveReject(week.id, 'rejected', reason);
                    }}
                    className="inline-flex items-center gap-1 px-3 py-1.5 bg-red-600 hover:bg-red-700 text-white text-sm rounded-lg"
                    data-testid={`reject-${week.id}`}
                  >
                    <X className="h-4 w-4" />
                    Reject
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
