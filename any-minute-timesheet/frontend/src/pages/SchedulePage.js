import React, { useState, useEffect, useMemo } from 'react';
import { api, useAuth } from '../context/AuthContext';
import { Calendar, ChevronLeft, ChevronRight, Plus, X, Edit2, Trash2 } from 'lucide-react';
import { format, addDays, startOfWeek, parseISO, addWeeks, subWeeks } from 'date-fns';

export default function SchedulePage() {
  const { user, isManager } = useAuth();
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
    return format(saturday, 'yyyy-MM-dd');
  });
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingSchedule, setEditingSchedule] = useState(null);
  const [formData, setFormData] = useState({
    user_id: '',
    scheduled_date: '',
    start_time: '',
    end_time: '',
    notes: ''
  });
  const [saving, setSaving] = useState(false);

  const weekDays = useMemo(() => {
    const start = parseISO(weekStart);
    return Array.from({ length: 7 }, (_, i) => {
      const day = addDays(start, i);
      return {
        date: format(day, 'yyyy-MM-dd'),
        dayName: format(day, 'EEEE'),
        dayShort: format(day, 'EEE'),
        dayNum: format(day, 'd'),
        monthDay: format(day, 'MMM d'),
        isToday: format(day, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
      };
    });
  }, [weekStart]);

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    if (selectedBusiness) {
      loadSchedules();
    }
  }, [selectedBusiness, weekStart]);

  const loadInitialData = async () => {
    try {
      const [bizRes, usersRes] = await Promise.all([
        api.get('/businesses'),
        isManager ? api.get('/users') : Promise.resolve({ data: [] })
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
      const endDate = format(addDays(parseISO(weekStart), 6), 'yyyy-MM-dd');
      const res = await api.get('/schedules', {
        params: {
          business_id: selectedBusiness,
          start_date: weekStart,
          end_date: endDate
        }
      });
      setSchedules(res.data);
    } catch (err) {
      console.error('Error loading schedules:', err);
    }
  };

  const navigateWeek = (direction) => {
    const start = parseISO(weekStart);
    const newStart = direction > 0 ? addWeeks(start, 1) : subWeeks(start, 1);
    setWeekStart(format(newStart, 'yyyy-MM-dd'));
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
        await api.put(`/schedules/${editingSchedule.id}`, {
          start_time: formData.start_time,
          end_time: formData.end_time,
          notes: formData.notes
        });
      } else {
        await api.post('/schedules', {
          ...formData,
          business_id: selectedBusiness
        });
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
      await api.delete(`/schedules/${schedule.id}`);
      await loadSchedules();
    } catch (err) {
      alert(err.response?.data?.detail || 'Failed to delete schedule');
    }
  };

  const getSchedulesForDay = (date) => {
    return schedules.filter(s => s.scheduled_date === date);
  };

  const getUserName = (userId) => {
    const u = users.find(u => u.id === userId);
    return u ? `${u.first_name} ${u.last_name}` : 'Unknown';
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6" data-testid="schedule-page">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Schedule</h1>
          <p className="text-slate-600">View and manage work schedules</p>
        </div>
        
        <div className="flex items-center gap-4">
          <select
            value={selectedBusiness}
            onChange={(e) => setSelectedBusiness(e.target.value)}
            className="px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
            data-testid="schedule-business-select"
          >
            {businesses.map(biz => (
              <option key={biz.id} value={biz.id}>{biz.name}</option>
            ))}
          </select>
          
          {isManager && (
            <button
              onClick={() => openModal()}
              className="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors"
              data-testid="add-schedule-btn"
            >
              <Plus className="h-4 w-4" />
              Add Schedule
            </button>
          )}
        </div>
      </div>

      {/* Week Navigation */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200">
        <div className="flex items-center justify-between p-4 border-b border-slate-200">
          <button
            onClick={() => navigateWeek(-1)}
            className="p-2 hover:bg-slate-100 rounded-lg"
            data-testid="schedule-prev-week"
          >
            <ChevronLeft className="h-5 w-5" />
          </button>
          
          <h2 className="text-lg font-semibold text-slate-900">
            Week of {format(parseISO(weekStart), 'MMM d, yyyy')}
          </h2>
          
          <button
            onClick={() => navigateWeek(1)}
            className="p-2 hover:bg-slate-100 rounded-lg"
            data-testid="schedule-next-week"
          >
            <ChevronRight className="h-5 w-5" />
          </button>
        </div>

        {/* Calendar Grid */}
        <div className="grid grid-cols-7 divide-x divide-slate-200">
          {weekDays.map((day) => {
            const daySchedules = getSchedulesForDay(day.date);
            return (
              <div
                key={day.date}
                className={`min-h-[200px] ${day.isToday ? 'bg-blue-50' : ''}`}
              >
                <div className={`p-2 text-center border-b border-slate-200 ${day.isToday ? 'bg-blue-100' : 'bg-slate-50'}`}>
                  <div className={`text-xs font-medium ${day.isToday ? 'text-blue-600' : 'text-slate-500'}`}>
                    {day.dayShort}
                  </div>
                  <div className={`text-lg font-bold ${day.isToday ? 'text-blue-600' : 'text-slate-900'}`}>
                    {day.dayNum}
                  </div>
                </div>
                
                <div className="p-2 space-y-2">
                  {daySchedules.map((schedule) => (
                    <div
                      key={schedule.id}
                      className="p-2 bg-blue-100 rounded-lg text-xs group relative"
                    >
                      <div className="font-medium text-blue-900">
                        {getUserName(schedule.user_id)}
                      </div>
                      <div className="text-blue-700">
                        {schedule.start_time} - {schedule.end_time}
                      </div>
                      {schedule.notes && (
                        <div className="text-blue-600 truncate">{schedule.notes}</div>
                      )}
                      
                      {isManager && (
                        <div className="absolute top-1 right-1 hidden group-hover:flex gap-1">
                          <button
                            onClick={() => openModal(schedule)}
                            className="p-1 bg-white rounded hover:bg-slate-100"
                          >
                            <Edit2 className="h-3 w-3 text-slate-600" />
                          </button>
                          <button
                            onClick={() => handleDelete(schedule)}
                            className="p-1 bg-white rounded hover:bg-red-50"
                          >
                            <Trash2 className="h-3 w-3 text-red-600" />
                          </button>
                        </div>
                      )}
                    </div>
                  ))}
                  
                  {isManager && (
                    <button
                      onClick={() => openModal(null, day.date)}
                      className="w-full p-2 border border-dashed border-slate-300 rounded-lg text-slate-400 hover:border-blue-400 hover:text-blue-600 text-xs"
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

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-md">
            <div className="flex items-center justify-between p-4 border-b border-slate-200">
              <h3 className="text-lg font-semibold text-slate-900">
                {editingSchedule ? 'Edit Schedule' : 'Add Schedule'}
              </h3>
              <button onClick={closeModal} className="p-2 text-slate-400 hover:text-slate-600">
                <X className="h-5 w-5" />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Employee *</label>
                <select
                  value={formData.user_id}
                  onChange={(e) => setFormData({ ...formData, user_id: e.target.value })}
                  required
                  disabled={editingSchedule}
                  className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none disabled:bg-slate-100"
                  data-testid="schedule-user-select"
                >
                  <option value="">Select employee...</option>
                  {users.filter(u => u.active).map(u => (
                    <option key={u.id} value={u.id}>{u.first_name} {u.last_name}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Date *</label>
                <input
                  type="date"
                  value={formData.scheduled_date}
                  onChange={(e) => setFormData({ ...formData, scheduled_date: e.target.value })}
                  required
                  disabled={editingSchedule}
                  className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none disabled:bg-slate-100"
                  data-testid="schedule-date-input"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Start Time *</label>
                  <input
                    type="time"
                    value={formData.start_time}
                    onChange={(e) => setFormData({ ...formData, start_time: e.target.value })}
                    required
                    className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                    data-testid="schedule-start-time"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">End Time *</label>
                  <input
                    type="time"
                    value={formData.end_time}
                    onChange={(e) => setFormData({ ...formData, end_time: e.target.value })}
                    required
                    className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                    data-testid="schedule-end-time"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Notes</label>
                <input
                  type="text"
                  value={formData.notes}
                  onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                  className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                  data-testid="schedule-notes-input"
                />
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={closeModal}
                  className="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={saving}
                  className="flex-1 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg disabled:opacity-50"
                  data-testid="save-schedule-btn"
                >
                  {saving ? 'Saving...' : 'Save'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
