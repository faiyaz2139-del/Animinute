import React, { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import axios from 'axios';
import { Layout } from '../components/Layout';
import { FormField, BlueButton, Table, Popup, Loader } from '../components/SharedComponents';
import { AM_API_URL, useAMAuth } from '../context/AMAuthContext';

export default function AMAddProject() {
  const location = useLocation();
  const { token } = useAMAuth();
  const [businesses, setBusinesses] = useState([]);
  const [projects, setProjects] = useState([]);
  const [selectedBusiness, setSelectedBusiness] = useState(location.state?.businessId || '');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [popup, setPopup] = useState({ open: false });
  const [form, setForm] = useState({ project_name: '', start_date: '', end_date: '', business_id: '' });
  const [editingProject, setEditingProject] = useState(null);
  const [error, setError] = useState(null);

  const authHeaders = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    fetchBusinesses();
  }, []);

  useEffect(() => {
    if (selectedBusiness) {
      fetchProjects();
      setForm(f => ({ ...f, business_id: selectedBusiness }));
    }
  }, [selectedBusiness]);

  const fetchBusinesses = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/businesses`, authHeaders);
      setBusinesses(res.data || []);
      if (res.data?.length > 0 && !selectedBusiness) {
        setSelectedBusiness(res.data[0].id);
      }
      setError(null);
    } catch (err) {
      console.error(err);
      setError(err.response?.status === 401 ? 'Session expired. Please login again.' : 'Failed to load businesses.');
    } finally {
      setLoading(false);
    }
  };

  const fetchProjects = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/projects`, { ...authHeaders, params: { business_id: selectedBusiness } });
      setProjects(res.data || []);
    } catch (err) {
      console.error(err);
      setProjects([]);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.project_name || !form.start_date) {
      setPopup({ open: true, type: 'warning', title: 'Warning', message: 'Project name and start date are required.' });
      return;
    }
    setSaving(true);
    try {
      if (editingProject) {
        await axios.put(`${AM_API_URL}/projects/${editingProject.id}`, form, authHeaders);
        setPopup({ open: true, type: 'success', title: 'Success', message: 'Project updated!' });
      } else {
        await axios.post(`${AM_API_URL}/projects`, form, authHeaders);
        setPopup({ open: true, type: 'success', title: 'Success', message: 'Project created!' });
      }
      setForm({ project_name: '', start_date: '', end_date: '', business_id: selectedBusiness });
      setEditingProject(null);
      fetchProjects();
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: err.response?.data?.detail || 'Failed to save project' });
    } finally {
      setSaving(false);
    }
  };

  const handleEdit = (project) => {
    setEditingProject(project);
    setForm({
      project_name: project.project_name,
      start_date: project.start_date,
      end_date: project.end_date || '',
      business_id: project.business_id
    });
  };

  const handleDelete = async (project) => {
    if (!window.confirm('Delete this project?')) return;
    try {
      await axios.delete(`${AM_API_URL}/projects/${project.id}`, authHeaders);
      fetchProjects();
      setPopup({ open: true, type: 'success', title: 'Deleted', message: 'Project deleted.' });
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: 'Failed to delete project' });
    }
  };

  const columns = [
    { key: 'index', label: 'SI No', render: (_, r, i) => i + 1 },
    { key: 'project_name', label: 'Project Name' },
    { key: 'start_date', label: 'Start Date' },
    { key: 'end_date', label: 'End Date', render: (v) => v || '-' },
    {
      key: 'actions',
      label: 'Actions',
      render: (_, row) => (
        <div style={{ display: 'flex', gap: '12px' }}>
          <span className="am-link" onClick={() => handleEdit(row)}>Edit</span>
          <span className="am-link" style={{ color: '#d93025' }} onClick={() => handleDelete(row)}>Delete</span>
        </div>
      )
    }
  ];

  if (loading) return <Layout title="Add Project"><Loader /></Layout>;

  if (error) {
    return (
      <Layout title="Add Project">
        <div className="am-card" style={{ textAlign: 'center', padding: '40px' }}>
          <p style={{ color: '#d93025', marginBottom: '16px' }}>{error}</p>
          <BlueButton onClick={() => { setError(null); setLoading(true); fetchBusinesses(); }}>Retry</BlueButton>
        </div>
      </Layout>
    );
  }

  return (
    <Layout title="Add Project">
      <div className="am-grid am-grid-2" style={{ gap: '24px' }} data-testid="add-project-page">
        {/* Create Project Form */}
        <div className="am-card">
          <h3 className="am-card-title">{editingProject ? 'Edit Project' : 'Create Project'}</h3>
          <form onSubmit={handleSubmit}>
            <FormField label="Project Name" name="project_name" value={form.project_name} onChange={handleChange} required />
            <FormField label="Start Date" name="start_date" type="date" value={form.start_date} onChange={handleChange} required />
            <FormField label="End Date" name="end_date" type="date" value={form.end_date} onChange={handleChange} />
            <FormField
              label="Business"
              name="business_id"
              type="select"
              value={form.business_id}
              onChange={handleChange}
              options={businesses.map(b => ({ value: b.id, label: b.company_name }))}
              required
            />
            <div style={{ display: 'flex', gap: '12px' }}>
              <BlueButton type="submit" disabled={saving}>{saving ? 'Saving...' : 'Save'}</BlueButton>
              {editingProject && (
                <BlueButton type="button" outline onClick={() => { setEditingProject(null); setForm({ project_name: '', start_date: '', end_date: '', business_id: selectedBusiness }); }}>
                  Cancel
                </BlueButton>
              )}
            </div>
          </form>
        </div>

        {/* List of Projects */}
        <div className="am-card">
          <h3 className="am-card-title">List of Projects</h3>
          <div style={{ marginBottom: '16px' }}>
            <FormField
              name="filter_business"
              type="select"
              value={selectedBusiness}
              onChange={(e) => setSelectedBusiness(e.target.value)}
              options={businesses.map(b => ({ value: b.id, label: b.company_name }))}
            />
          </div>
          <Table columns={columns} data={projects} />
        </div>
      </div>

      <Popup {...popup} onClose={() => setPopup({ open: false })} />
    </Layout>
  );
}
