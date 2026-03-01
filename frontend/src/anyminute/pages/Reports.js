import React, { useState, useEffect } from 'react';
import { useAMAuth, AM_API_URL } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import { BlueButton } from '../components/SharedComponents';
import axios from 'axios';
import { FileText, Download, Building2, TrendingUp, TrendingDown, Minus, BarChart2 } from 'lucide-react';

export default function AMReports() {
  const { token } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [reportData, setReportData] = useState([]);
  const [comparisonData, setComparisonData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [showComparison, setShowComparison] = useState(false);

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
    setComparisonData(null);
    try {
      const params = {};
      if (selectedBusiness) params.business_id = selectedBusiness;
      if (startDate) params.start_date = startDate;
      if (endDate) params.end_date = endDate;

      const res = await axios.get(`${AM_API_URL}/reports/by-business`, { ...config, params });
      setReportData(res.data);
      
      // If comparison is enabled, also fetch comparison data
      if (showComparison && startDate && endDate) {
        await fetchComparison();
      }
    } catch (err) {
      console.error('Error generating report:', err);
      alert('Failed to generate report');
    } finally {
      setGenerating(false);
    }
  };

  const fetchComparison = async () => {
    try {
      const params = { start_date: startDate, end_date: endDate };
      if (selectedBusiness) params.business_id = selectedBusiness;
      
      const res = await axios.get(`${AM_API_URL}/reports/compare`, { ...config, params });
      setComparisonData(res.data);
    } catch (err) {
      console.error('Error fetching comparison:', err);
    }
  };

  const toggleComparison = () => {
    const newValue = !showComparison;
    setShowComparison(newValue);
    if (newValue && reportData.length > 0 && startDate && endDate) {
      fetchComparison();
    } else {
      setComparisonData(null);
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

  const getTrendIcon = (change) => {
    if (change > 0) return <TrendingUp size={16} style={{ color: '#16a34a' }} />;
    if (change < 0) return <TrendingDown size={16} style={{ color: '#dc2626' }} />;
    return <Minus size={16} style={{ color: '#6b7280' }} />;
  };

  const getChangeColor = (change) => {
    if (change > 0) return '#16a34a';
    if (change < 0) return '#dc2626';
    return '#6b7280';
  };

  const formatChange = (value, percent) => {
    const sign = value > 0 ? '+' : '';
    return `${sign}${value} (${sign}${percent}%)`;
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

        {/* Compare to Prior Period Toggle */}
        <div style={{ marginTop: '16px', paddingTop: '16px', borderTop: '1px solid #e5e7eb' }}>
          <label style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
            <input
              type="checkbox"
              checked={showComparison}
              onChange={toggleComparison}
              style={{ width: '18px', height: '18px', accentColor: '#1a73e8' }}
              data-testid="compare-toggle"
            />
            <BarChart2 size={18} style={{ color: '#1a73e8' }} />
            <span style={{ fontWeight: '500' }}>Compare to Prior Period</span>
            <span style={{ color: '#6b7280', fontSize: '13px' }}>
              (Shows same-length period before your selected dates)
            </span>
          </label>
        </div>
      </div>

      {/* Comparison Summary Card */}
      {showComparison && comparisonData && (
        <div className="am-card" style={{ marginBottom: '24px', backgroundColor: '#f8fafc' }} data-testid="comparison-summary">
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '16px' }}>
            <BarChart2 size={20} style={{ color: '#1a73e8' }} />
            <h3 style={{ margin: 0 }}>Period Comparison</h3>
          </div>
          
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '16px', marginBottom: '20px' }}>
            {/* Current Period Card */}
            <div style={{ backgroundColor: 'white', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
              <div style={{ fontSize: '12px', color: '#6b7280', marginBottom: '4px' }}>CURRENT PERIOD</div>
              <div style={{ fontSize: '11px', color: '#9ca3af' }}>{comparisonData.current.start_date} → {comparisonData.current.end_date}</div>
              <div style={{ fontSize: '28px', fontWeight: '700', color: '#1a73e8', marginTop: '8px' }}>{comparisonData.current.total_hours}</div>
              <div style={{ fontSize: '12px', color: '#6b7280' }}>hours logged</div>
            </div>
            
            {/* Prior Period Card */}
            <div style={{ backgroundColor: 'white', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
              <div style={{ fontSize: '12px', color: '#6b7280', marginBottom: '4px' }}>PRIOR PERIOD</div>
              <div style={{ fontSize: '11px', color: '#9ca3af' }}>{comparisonData.prior.start_date} → {comparisonData.prior.end_date}</div>
              <div style={{ fontSize: '28px', fontWeight: '700', color: '#64748b', marginTop: '8px' }}>{comparisonData.prior.total_hours}</div>
              <div style={{ fontSize: '12px', color: '#6b7280' }}>hours logged</div>
            </div>
            
            {/* Change Card */}
            <div style={{ backgroundColor: 'white', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
              <div style={{ fontSize: '12px', color: '#6b7280', marginBottom: '4px' }}>CHANGE</div>
              <div style={{ display: 'flex', alignItems: 'center', gap: '6px', marginTop: '8px' }}>
                {getTrendIcon(comparisonData.summary.hours_change)}
                <span style={{ 
                  fontSize: '24px', 
                  fontWeight: '700', 
                  color: getChangeColor(comparisonData.summary.hours_change) 
                }}>
                  {comparisonData.summary.hours_change > 0 ? '+' : ''}{comparisonData.summary.hours_change}h
                </span>
              </div>
              <div style={{ 
                fontSize: '14px', 
                fontWeight: '500',
                color: getChangeColor(comparisonData.summary.hours_change_percent)
              }}>
                {comparisonData.summary.hours_change_percent > 0 ? '+' : ''}{comparisonData.summary.hours_change_percent}%
              </div>
            </div>

            {/* Users Change Card */}
            <div style={{ backgroundColor: 'white', padding: '16px', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
              <div style={{ fontSize: '12px', color: '#6b7280', marginBottom: '4px' }}>ACTIVE EMPLOYEES</div>
              <div style={{ fontSize: '20px', fontWeight: '700', color: '#374151', marginTop: '8px' }}>
                {comparisonData.current.unique_users}
                <span style={{ 
                  fontSize: '14px', 
                  marginLeft: '8px',
                  color: getChangeColor(comparisonData.summary.users_change)
                }}>
                  ({comparisonData.summary.users_change > 0 ? '+' : ''}{comparisonData.summary.users_change})
                </span>
              </div>
              <div style={{ fontSize: '12px', color: '#6b7280' }}>vs {comparisonData.prior.unique_users} prior</div>
            </div>
          </div>

          {/* Business Breakdown Table */}
          {comparisonData.business_comparison && comparisonData.business_comparison.length > 0 && (
            <div>
              <h4 style={{ fontSize: '14px', fontWeight: '600', marginBottom: '12px', color: '#374151' }}>Hours by Business</h4>
              <div style={{ overflowX: 'auto' }}>
                <table style={{ width: '100%', borderCollapse: 'collapse', backgroundColor: 'white', borderRadius: '8px', overflow: 'hidden' }}>
                  <thead>
                    <tr style={{ backgroundColor: '#f1f5f9' }}>
                      <th style={{ padding: '10px 12px', textAlign: 'left', fontSize: '12px', fontWeight: '600', color: '#475569' }}>Business</th>
                      <th style={{ padding: '10px 12px', textAlign: 'right', fontSize: '12px', fontWeight: '600', color: '#475569' }}>Current</th>
                      <th style={{ padding: '10px 12px', textAlign: 'right', fontSize: '12px', fontWeight: '600', color: '#475569' }}>Prior</th>
                      <th style={{ padding: '10px 12px', textAlign: 'right', fontSize: '12px', fontWeight: '600', color: '#475569' }}>Change</th>
                    </tr>
                  </thead>
                  <tbody>
                    {comparisonData.business_comparison.map((biz, idx) => (
                      <tr key={biz.business_id} style={{ borderBottom: '1px solid #e2e8f0' }}>
                        <td style={{ padding: '10px 12px', fontWeight: '500' }}>{biz.business_name}</td>
                        <td style={{ padding: '10px 12px', textAlign: 'right', fontWeight: '600' }}>{biz.current_hours}h</td>
                        <td style={{ padding: '10px 12px', textAlign: 'right', color: '#64748b' }}>{biz.prior_hours}h</td>
                        <td style={{ padding: '10px 12px', textAlign: 'right' }}>
                          <span style={{ 
                            display: 'inline-flex', 
                            alignItems: 'center', 
                            gap: '4px',
                            color: getChangeColor(biz.change)
                          }}>
                            {getTrendIcon(biz.change)}
                            <span>{biz.change > 0 ? '+' : ''}{biz.change}h</span>
                            <span style={{ fontSize: '12px', opacity: 0.8 }}>
                              ({biz.change_percent > 0 ? '+' : ''}{biz.change_percent}%)
                            </span>
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </div>
      )}

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
