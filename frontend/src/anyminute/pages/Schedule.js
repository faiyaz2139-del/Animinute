import React, { useState, useEffect, useMemo } from 'react';
import { useAMAuth, AM_API_URL } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import { BlueButton } from '../components/SharedComponents';
import axios from 'axios';
import { ChevronLeft, ChevronRight, Plus, Edit2, Trash2, X } from 'lucide-react';

export default function AMSchedule() {
  const { user, token, isManager } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [users, setUsers] = useState([]);
  const [schedules, setSchedules] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState('');
  const [weekStart, setWeekStart] = useState(() => {
    const today = new Date();
    const dayOfWeek = today.getDay();
    const daysUntilSaturday = (dayOfWeek + 1) % 7;
    const saturday = new Date(today);
    saturday.setDate(today.getDate() - daysUntilSaturday);
    return saturday.toISOString().split('T')[0];
  });
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingSchedule, setEditingSchedule] = useState(null);
  const [formData, setFormData] = useState({
    user_id: '',
    scheduled_date: '',
    start_time: '09:00',
    end_time: '17:00',
    notes: ''
  });
  const [saving, setSaving] = useState(false);

  const config = { headers: { Authorization: `Bearer ${token}` } };

  const weekDays = useMemo(() => {
    const start = new Date(weekStart + 'T00:00:00');
    return Array.from({ length: 7 }, (_, i) => {
      const day = new Date(start);
      day.setDate(start.getDate() + i);
      return {
        date: day.toISOString().split('T')[0],
        dayName: day.toLocaleDateString('en-US', { weekday: 'long' }),
        dayShort: day.toLocaleDateString('en-US', { weekday: 'short' }),
        dayNum: day.getDate(),
        isToday: day.toISOString().split('T')[0] === new Date().toISOString().split('T')[0]
      };
    });
  }, [weekStart]);

  useEffect(() => {
    loadInitialData();
  }, [token]);

  useEffect(() => {
    if (selectedBusiness && token) {
      loadSchedules();
    }
  }, [selectedBusiness, weekStart, token]);

  const loadInitialData = async () => {
    try {
      const [bizRes, usersRes] = await Promise.all([
        axios.get(`${AM_API_URL}/businesses`, config),
        isManager ? axios.get(`${AM_API_URL}/users`, config) : Promise.resolve({ data: [] })
      ]);
      setBusinesses(bizRes.data);
      setUsers(usersRes.data);
      if (bizRes.data.length > 0) {
        setSelectedBusiness(bizRes.data[0].id);
      }
    } catch (err) {
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const loadSchedules = async () => {
    try {
      const endDate = new Date(weekStart + 'T00:00:00');
      endDate.setDate(endDate.getDate() + 6);
      const res = await axios.get(`${AM_API_URL}/schedules`, {
        ...config,
        params: {
          business_id: selectedBusiness,
          start_date: weekStart,
          end_date: endDate.toISOString().split('T')[0]
        }
      });
      setSchedules(res.data);
    } catch (err) {
      console.error('Error loading schedules:', err);
    }
  };

  const navigateWeek = (direction) => {
    const start = new Date(weekStart + 'T00:00:00');
    start.setDate(start.getDate() + direction * 7);
    setWeekStart(start.toISOString().split('T')[0]);
  };

  const openModal = (schedule = null, date = null) => {
    if (schedule) {
      setEditingSchedule(schedule);
      setFormData({
        user_id: schedule.user_id,
        scheduled_date: schedule.scheduled_date,
        start_time: schedule.start_time,
        end_time: schedule.end_time,
        notes: schedule.notes || ''
      });
    } else {
      setEditingSchedule(null);
      setFormData({
        user_id: users[0]?.id || '',
        scheduled_date: date || weekDays[0].date,
        start_time: '09:00',
        end_time: '17:00',
        notes: ''
      });
    }
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingSchedule(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      if (editingSchedule) {
        await axios.put(`${AM_API_URL}/schedules/${editingSchedule.id}`, {
          start_time: formData.start_time,
          end_time: formData.end_time,
          notes: formData.notes
        }, config);
      } else {
        await axios.post(`${AM_API_URL}/schedules`, {
          ...formData,
          business_id: selectedBusiness
        }, config);
      }
      await loadSchedules();
      closeModal();
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to save schedule');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (schedule) => {
    if (!window.confirm('Delete this schedule entry?')) return;
    
    try {
      await axios.delete(`${AM_API_URL}/schedules/${schedule.id}`, config);
      await loadSchedules();
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to delete schedule');
    }
  };

  const getSchedulesForDay = (date) => schedules.filter(s => s.scheduled_date === date);
  const getUserName = (userId) => {
    const u = users.find(u => u.id === userId);
    return u ? `${u.first_name} ${u.last_name}` : 'Unknown';
  };

  if (loading) {
    return (
      <Layout title="Schedule">
        <div style={{ display: 'flex', justifyContent: 'center', padding: '40px' }}>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout title="Schedule">
      <div style={{ marginBottom: '20px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <select
          value={selectedBusiness}
          onChange={(e) => setSelectedBusiness(e.target.value)}
          className="am-select"
          data-testid="schedule-business-select"
        >
          {businesses.map(biz => (
            <option key={biz.id} value={biz.id}>{biz.name}</option>
          ))}
        </select>
        
        {isManager && (
          <BlueButton onClick={() => openModal()} data-testid="add-schedule-btn">
            <Plus size={16} style={{ marginRight: '8px' }} />
            Add Schedule
          </BlueButton>
        )}
      </div>

      <div className="am-card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <button onClick={() => navigateWeek(-1)} className="am-icon-btn" data-testid="schedule-prev-week">
            <ChevronLeft size={20} />
          </button>
          
          <h3>Week of {new Date(weekStart + 'T00:00:00').toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' })}</h3>
          
          <button onClick={() => navigateWeek(1)} className="am-icon-btn" data-testid="schedule-next-week">
            <ChevronRight size={20} />
          </button>
        </div>

        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(7, 1fr)', gap: '1px', backgroundColor: '#e0e0e0', borderRadius: '8px', overflow: 'hidden' }}>
          {weekDays.map((day) => {
            const daySchedules = getSchedulesForDay(day.date);
            return (
              <div key={day.date} style={{ backgroundColor: day.isToday ? '#e3f2fd' : '#fff', minHeight: '180px' }}>
                <div style={{ padding: '8px', textAlign: 'center', backgroundColor: day.isToday ? '#bbdefb' : '#f5f5f5', borderBottom: '1px solid #e0e0e0' }}>
                  <div style={{ fontSize: '12px', color: '#666' }}>{day.dayShort}</div>
                  <div style={{ fontSize: '18px', fontWeight: 'bold' }}>{day.dayNum}</div>
                </div>
                
                <div style={{ padding: '8px' }}>
                  {daySchedules.map((schedule) => (
                    <div
                      key={schedule.id}
                      style={{ padding: '8px', marginBottom: '8px', backgroundColor: '#e3f2fd', borderRadius: '4px', fontSize: '12px', position: 'relative' }}
                    >
                      <div style={{ fontWeight: 'bold', color: '#1565c0' }}>{getUserName(schedule.user_id)}</div>
                      <div style={{ color: '#1976d2' }}>{schedule.start_time} - {schedule.end_time}</div>
                      {schedule.notes && <div style={{ color: '#64b5f6', marginTop: '4px' }}>{schedule.notes}</div>}
                      
                      {isManager && (
                        <div style={{ position: 'absolute', top: '4px', right: '4px', display: 'flex', gap: '4px' }}>
                          <button onClick={() => openModal(schedule)} className="am-icon-btn-sm">
                            <Edit2 size={12} />
                          </button>
                          <button onClick={() => handleDelete(schedule)} className="am-icon-btn-sm" style={{ color: '#f44336' }}>
                            <Trash2 size={12} />
                          </button>
                        </div>
                      )}
                    </div>
                  ))}
                  
                  {isManager && (
                    <button
                      onClick={() => openModal(null, day.date)}
                      style={{ width: '100%', padding: '8px', border: '1px dashed #bdbdbd', borderRadius: '4px', background: 'none', cursor: 'pointer', color: '#9e9e9e', fontSize: '12px' }}
                    >
                      + Add
                    </button>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {showModal && (
        <div className="am-modal-overlay">
          <div className="am-modal">
            <div className="am-modal-header">
              <h3>{editingSchedule ? 'Edit Schedule' : 'Add Schedule'}</h3>
              <button onClick={closeModal} className="am-icon-btn"><X size={20} /></button>
            </div>

            <form onSubmit={handleSubmit} className="am-modal-body">
              <div className="am-form-group">
                <label>Employee *</label>
                <select
                  value={formData.user_id}
                  onChange={(e) => setFormData({ ...formData, user_id: e.target.value })}
                  required
                  disabled={editingSchedule}
                  className="am-select"
                  data-testid="schedule-user-select"
                >
                  <option value="">Select employee...</option>
                  {users.filter(u => u.active).map(u => (
                    <option key={u.id} value={u.id}>{u.first_name} {u.last_name}</option>
                  ))}
                </select>
              </div>

              <div className="am-form-group">
                <label>Date *</label>
                <input
                  type="date"
                  value={formData.scheduled_date}
                  onChange={(e) => setFormData({ ...formData, scheduled_date: e.target.value })}
                  required
                  disabled={editingSchedule}
                  className="am-input"
                  data-testid="schedule-date-input"
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div className="am-form-group">
                  <label>Start Time *</label>
                  <input
                    type="time"
                    value={formData.start_time}
                    onChange={(e) => setFormData({ ...formData, start_time: e.target.value })}
                    required
                    className="am-input"
                    data-testid="schedule-start-time"
                  />
                </div>
                <div className="am-form-group">
                  <label>End Time *</label>
                  <input
                    type="time"
                    value={formData.end_time}
                    onChange={(e) => setFormData({ ...formData, end_time: e.target.value })}
                    required
                    className="am-input"
                    data-testid="schedule-end-time"
                  />
                </div>
              </div>

              <div className="am-form-group">
                <label>Notes</label>
                <input
                  type="text"
                  value={formData.notes}
                  onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                  className="am-input"
                  data-testid="schedule-notes-input"
                />
              </div>

              <div className="am-modal-footer">
                <button type="button" onClick={closeModal} className="am-btn am-btn-secondary">Cancel</button>
                <BlueButton type="submit" disabled={saving} data-testid="save-schedule-btn">
                  {saving ? 'Saving...' : 'Save'}
                </BlueButton>
              </div>
            </form>
          </div>
        </div>
      )}
    </Layout>
  );
}
