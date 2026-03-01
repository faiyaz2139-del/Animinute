import React, { useState, useEffect } from 'react';
import { useAMAuth, AM_API_URL } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import { BlueButton } from '../components/SharedComponents';
import axios from 'axios';
import { FileText, Download, Building2 } from 'lucide-react';

export default function AMReports() {
  const { token } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [reportData, setReportData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);

  const config = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    loadBusinesses();
    const end = new Date();
    const start = new Date();
    start.setDate(start.getDate() - 30);
    setStartDate(start.toISOString().split('T')[0]);
    setEndDate(end.toISOString().split('T')[0]);
  }, [token]);

  const loadBusinesses = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/businesses`, config);
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

      const res = await axios.get(`${AM_API_URL}/reports/by-business`, { ...config, params });
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
      <Layout title="Reports">
        <div style={{ display: 'flex', justifyContent: 'center', padding: '40px' }}>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout title="Reports">
      <div className="am-card" style={{ marginBottom: '24px' }}>
        <h3 style={{ marginBottom: '16px' }}>Report Filters</h3>
        
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px' }}>
          <div className="am-form-group">
            <label>Business</label>
            <select
              value={selectedBusiness}
              onChange={(e) => setSelectedBusiness(e.target.value)}
              className="am-select"
              data-testid="report-business-select"
            >
              <option value="">All Businesses</option>
              {businesses.map(biz => (
                <option key={biz.id} value={biz.id}>{biz.name}</option>
              ))}
            </select>
          </div>

          <div className="am-form-group">
            <label>Start Date</label>
            <input
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className="am-input"
              data-testid="report-start-date"
            />
          </div>

          <div className="am-form-group">
            <label>End Date</label>
            <input
              type="date"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              className="am-input"
              data-testid="report-end-date"
            />
          </div>

          <div className="am-form-group" style={{ display: 'flex', alignItems: 'flex-end' }}>
            <BlueButton onClick={generateReport} disabled={generating} style={{ width: '100%' }} data-testid="generate-report-btn">
              {generating ? 'Generating...' : 'Generate Report'}
            </BlueButton>
          </div>
        </div>
      </div>

      {reportData.length > 0 && (
        <div className="am-card">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
            <h3>Report Results</h3>
            <button onClick={downloadCSV} className="am-btn am-btn-secondary" data-testid="download-csv-btn">
              <Download size={16} style={{ marginRight: '8px' }} />
              Download CSV
            </button>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
            {reportData.map((biz) => (
              <div key={biz.business_id} style={{ border: '1px solid #e0e0e0', borderRadius: '8px', overflow: 'hidden' }}>
                <div style={{ padding: '12px 16px', backgroundColor: '#f5f5f5', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <Building2 size={20} />
                    <span style={{ fontWeight: 'bold' }}>{biz.business_name}</span>
                  </div>
                  <span style={{ fontSize: '14px', color: '#666' }}>
                    Total: <strong>{biz.total_hours} hours</strong>
                  </span>
                </div>
                
                <table className="am-table" style={{ marginBottom: 0 }}>
                  <thead>
                    <tr>
                      <th style={{ textAlign: 'left' }}>Employee</th>
                      <th style={{ textAlign: 'right' }}>Hours</th>
                    </tr>
                  </thead>
                  <tbody>
                    {biz.user_breakdown.map((user) => (
                      <tr key={user.user_id}>
                        <td>{user.user_name}</td>
                        <td style={{ textAlign: 'right', fontWeight: 'bold' }}>{user.hours}</td>
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
        <div className="am-card" style={{ textAlign: 'center', padding: '60px 20px' }}>
          <FileText size={48} style={{ color: '#bdbdbd', marginBottom: '16px' }} />
          <h3 style={{ color: '#757575' }}>No Report Data</h3>
          <p style={{ color: '#9e9e9e' }}>Click "Generate Report" to view timesheet data</p>
        </div>
      )}
    </Layout>
  );
}
