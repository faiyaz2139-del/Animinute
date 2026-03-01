import React, { useState, useEffect } from 'react';
import { api } from '../context/AuthContext';
import { FileText, Download, Building2 } from 'lucide-react';

export default function ReportsPage() {
  const [businesses, setBusinesses] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [reportData, setReportData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);

  useEffect(() => {
    loadBusinesses();
    // Set default dates (last 30 days)
    const end = new Date();
    const start = new Date();
    start.setDate(start.getDate() - 30);
    setStartDate(start.toISOString().split('T')[0]);
    setEndDate(end.toISOString().split('T')[0]);
  }, []);

  const loadBusinesses = async () => {
    try {
      const res = await api.get('/businesses');
      setBusinesses(res.data);
    } catch (err) {
      console.error('Error loading businesses:', err);
    } finally {
      setLoading(false);
    }
  };

  const generateReport = async () => {
    setGenerating(true);
    try {
      const params = {};
      if (selectedBusiness) params.business_id = selectedBusiness;
      if (startDate) params.start_date = startDate;
      if (endDate) params.end_date = endDate;

      const res = await api.get('/reports/by-business', { params });
      setReportData(res.data);
    } catch (err) {
      console.error('Error generating report:', err);
      alert('Failed to generate report');
    } finally {
      setGenerating(false);
    }
  };

  const downloadCSV = () => {
    if (reportData.length === 0) return;

    let csv = 'Business,Employee,Hours\n';
    reportData.forEach(biz => {
      biz.user_breakdown.forEach(user => {
        csv += `"${biz.business_name}","${user.user_name}",${user.hours}\n`;
      });
      csv += `"${biz.business_name}","TOTAL",${biz.total_hours}\n`;
    });

    const blob = new Blob([csv], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `timesheet_report_${startDate}_${endDate}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6" data-testid="reports-page">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Reports</h1>
        <p className="text-slate-600">Generate timesheet reports by business</p>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h2 className="text-lg font-semibold text-slate-900 mb-4">Report Filters</h2>
        
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Business</label>
            <select
              value={selectedBusiness}
              onChange={(e) => setSelectedBusiness(e.target.value)}
              className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
              data-testid="report-business-select"
            >
              <option value="">All Businesses</option>
              {businesses.map(biz => (
                <option key={biz.id} value={biz.id}>{biz.name}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Start Date</label>
            <input
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
              data-testid="report-start-date"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">End Date</label>
            <input
              type="date"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
              data-testid="report-end-date"
            />
          </div>

          <div className="flex items-end">
            <button
              onClick={generateReport}
              disabled={generating}
              className="w-full px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors disabled:opacity-50"
              data-testid="generate-report-btn"
            >
              {generating ? 'Generating...' : 'Generate Report'}
            </button>
          </div>
        </div>
      </div>

      {/* Report Results */}
      {reportData.length > 0 && (
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-slate-900">Report Results</h2>
            <button
              onClick={downloadCSV}
              className="inline-flex items-center gap-2 px-4 py-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
              data-testid="download-csv-btn"
            >
              <Download className="h-4 w-4" />
              Download CSV
            </button>
          </div>

          <div className="space-y-6">
            {reportData.map((biz) => (
              <div key={biz.business_id} className="border border-slate-200 rounded-lg overflow-hidden">
                <div className="bg-slate-50 px-4 py-3 flex items-center gap-3">
                  <Building2 className="h-5 w-5 text-slate-600" />
                  <span className="font-medium text-slate-900">{biz.business_name}</span>
                  <span className="ml-auto text-sm text-slate-600">
                    Total: <span className="font-bold text-slate-900">{biz.total_hours} hours</span>
                  </span>
                </div>
                
                <table className="w-full">
                  <thead className="bg-slate-50 border-t border-slate-200">
                    <tr>
                      <th className="text-left px-4 py-2 text-xs font-medium text-slate-500 uppercase">Employee</th>
                      <th className="text-right px-4 py-2 text-xs font-medium text-slate-500 uppercase">Hours</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-100">
                    {biz.user_breakdown.map((user) => (
                      <tr key={user.user_id} className="hover:bg-slate-50">
                        <td className="px-4 py-3 text-slate-900">{user.user_name}</td>
                        <td className="px-4 py-3 text-right font-medium text-slate-900">{user.hours}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ))}
          </div>
        </div>
      )}

      {reportData.length === 0 && !generating && (
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-8 text-center">
          <FileText className="h-12 w-12 text-slate-300 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-slate-900 mb-2">No Report Data</h3>
          <p className="text-slate-500">Click "Generate Report" to view timesheet data</p>
        </div>
      )}
    </div>
  );
}
