import React, { useState, useEffect } from 'react';
import { api, useAuth } from '../context/AuthContext';
import { Clock, Building2, Users, AlertCircle, Plus } from 'lucide-react';

export default function DashboardPage() {
  const { user, isManager } = useAuth();
  const [stats, setStats] = useState(null);
  const [pendingTimesheets, setPendingTimesheets] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      const [statsRes, pendingRes] = await Promise.all([
        api.get('/dashboard/stats'),
        isManager ? api.get('/timesheet-weeks', { params: { status: 'submitted' } }) : Promise.resolve({ data: [] })
      ]);
      setStats(statsRes.data);
      setPendingTimesheets(pendingRes.data || []);
    } catch (err) {
      console.error('Error loading dashboard:', err);
    } finally {
      setLoading(false);
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
    <div className="space-y-6" data-testid="dashboard-page">
      {/* Welcome */}
      <div>
        <h1 className="text-2xl font-bold text-slate-900">
          Welcome back, {user?.first_name}!
        </h1>
        <p className="text-slate-600">Here's your timesheet overview for this week</p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          icon={Clock}
          label="My Hours This Week"
          value={stats?.my_hours_this_week?.toFixed(1) || '0'}
          color="blue"
        />
        <StatCard
          icon={Building2}
          label="Active Businesses"
          value={stats?.business_count || '0'}
          color="green"
        />
        <StatCard
          icon={Users}
          label="Team Members"
          value={stats?.user_count || '0'}
          color="purple"
        />
        <StatCard
          icon={AlertCircle}
          label="Pending Approvals"
          value={stats?.pending_approvals || '0'}
          color="amber"
        />
      </div>

      {/* Current Week Info */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h2 className="text-lg font-semibold text-slate-900 mb-4">Current Week</h2>
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div>
            <p className="text-sm text-slate-500">Week Starting</p>
            <p className="text-lg font-medium text-slate-900">
              {stats?.current_week_start ? new Date(stats.current_week_start + 'T00:00:00').toLocaleDateString('en-US', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric'
              }) : 'N/A'}
            </p>
          </div>
          <a
            href="/timesheet"
            className="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors"
            data-testid="enter-time-btn"
          >
            <Plus className="h-4 w-4" />
            Enter Time
          </a>
        </div>
      </div>

      {/* Pending Approvals (Managers only) */}
      {isManager && pendingTimesheets.length > 0 && (
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
          <h2 className="text-lg font-semibold text-slate-900 mb-4">
            Pending Approvals ({pendingTimesheets.length})
          </h2>
          <div className="space-y-3">
            {pendingTimesheets.slice(0, 5).map((ts) => (
              <div
                key={ts.id}
                className="flex items-center justify-between p-3 bg-slate-50 rounded-lg"
              >
                <div>
                  <p className="font-medium text-slate-900">
                    Week of {new Date(ts.week_start_date + 'T00:00:00').toLocaleDateString()}
                  </p>
                  <p className="text-sm text-slate-500">
                    {ts.total_hours?.toFixed(1) || 0} hours
                  </p>
                </div>
                <a
                  href="/timesheet"
                  className="px-3 py-1.5 text-sm bg-amber-100 text-amber-700 rounded-lg hover:bg-amber-200 transition-colors"
                >
                  Review
                </a>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Quick Actions */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h2 className="text-lg font-semibold text-slate-900 mb-4">Quick Actions</h2>
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
          <QuickActionCard href="/timesheet" icon={Clock} label="Enter Time" />
          <QuickActionCard href="/schedule" icon="calendar" label="View Schedule" />
          {isManager && <QuickActionCard href="/reports" icon="file" label="View Reports" />}
          {isManager && <QuickActionCard href="/users" icon="users" label="Manage Users" />}
        </div>
      </div>
    </div>
  );
}

function StatCard({ icon: Icon, label, value, color }) {
  const colors = {
    blue: 'bg-blue-50 text-blue-600',
    green: 'bg-green-50 text-green-600',
    purple: 'bg-purple-50 text-purple-600',
    amber: 'bg-amber-50 text-amber-600',
  };

  return (
    <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-5">
      <div className={`inline-flex items-center justify-center w-10 h-10 rounded-lg ${colors[color]} mb-3`}>
        <Icon className="h-5 w-5" />
      </div>
      <p className="text-2xl font-bold text-slate-900">{value}</p>
      <p className="text-sm text-slate-500">{label}</p>
    </div>
  );
}

function QuickActionCard({ href, icon, label }) {
  const iconMap = {
    calendar: (
      <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
      </svg>
    ),
    file: (
      <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
      </svg>
    ),
    users: (
      <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
      </svg>
    ),
  };

  const IconComponent = typeof icon === 'string' ? null : icon;

  return (
    <a
      href={href}
      className="flex flex-col items-center justify-center p-4 bg-slate-50 hover:bg-slate-100 rounded-lg transition-colors"
    >
      <div className="text-slate-600 mb-2">
        {IconComponent ? <IconComponent className="h-6 w-6" /> : iconMap[icon]}
      </div>
      <span className="text-sm font-medium text-slate-700">{label}</span>
    </a>
  );
}
