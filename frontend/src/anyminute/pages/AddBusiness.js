import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';
import { Layout } from '../components/Layout';
import { FormField, BlueButton, Popup } from '../components/SharedComponents';
import { AM_API_URL } from '../context/AMAuthContext';

export default function AMAddBusiness() {
  const navigate = useNavigate();
  const location = useLocation();
  const editBusiness = location.state?.business;

  const [form, setForm] = useState({
    company_name: '', operating_name: '', primary_contact_name: '', business_number: '',
    legal_entity_type: 'Incorporation', website: '', street_number: '', address_line1: '',
    address_line2: '', suite_number: '', suite_type: '', city: '', province: '', postal_code: '', logo_url: ''
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [popup, setPopup] = useState({ open: false });
  const [showAddress, setShowAddress] = useState(false);

  useEffect(() => {
    if (editBusiness) {
      setForm({ ...form, ...editBusiness });
      setShowAddress(!!editBusiness.address_line1);
    }
  }, [editBusiness]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
    setErrors({ ...errors, [name]: '' });
  };

  const validate = () => {
    const errs = {};
    if (!form.company_name) errs.company_name = 'Company name is required';
    if (!form.primary_contact_name) errs.primary_contact_name = 'Primary contact is required';
    if (showAddress) {
      if (!form.street_number) errs.street_number = 'Street number is required';
      if (!form.address_line1) errs.address_line1 = 'Address line 1 is required';
      if (!form.city) errs.city = 'City is required';
      if (!form.province) errs.province = 'Province is required';
    }
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;
    setLoading(true);
    try {
      if (editBusiness) {
        await axios.put(`${AM_API_URL}/businesses/${editBusiness.id}`, form);
        setPopup({ open: true, type: 'success', title: 'Success', message: 'Business updated successfully!' });
      } else {
        await axios.post(`${AM_API_URL}/businesses`, form);
        setPopup({ open: true, type: 'success', title: 'Success', message: 'Business created successfully!' });
      }
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: err.response?.data?.detail || 'Failed to save business' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout title={editBusiness ? 'Edit Business' : 'Add Business'}>
      <div className="am-card" style={{ maxWidth: '700px' }} data-testid="add-business-form">
        <form onSubmit={handleSubmit}>
          {/* Logo Upload Placeholder */}
          <div className="am-field" style={{ marginBottom: '24px' }}>
            <label>Business Logo</label>
            <div style={{ border: '2px dashed #dadce0', padding: '20px', textAlign: 'center', borderRadius: '8px' }}>
              <span className="am-link">Upload Image</span>
              <p style={{ fontSize: '12px', color: '#666', marginTop: '8px' }}>PNG, JPG up to 2MB</p>
            </div>
          </div>

          <FormField label="Company Name" name="company_name" value={form.company_name} onChange={handleChange} error={errors.company_name} required />
          <FormField label="Operating Name" name="operating_name" value={form.operating_name} onChange={handleChange} />
          <FormField label="Primary Contact Name" name="primary_contact_name" value={form.primary_contact_name} onChange={handleChange} error={errors.primary_contact_name} required />
          <FormField label="Business Number" name="business_number" value={form.business_number} onChange={handleChange} />
          
          <FormField
            label="Legal Entity Type"
            name="legal_entity_type"
            type="select"
            value={form.legal_entity_type}
            onChange={handleChange}
            options={[
              { value: 'Incorporation', label: 'Incorporation' },
              { value: 'Partnership', label: 'Partnership' },
              { value: 'Sole-Proprietorship', label: 'Sole-Proprietorship' },
              { value: 'Non-Profit', label: 'Non-Profit' }
            ]}
          />
          
          <FormField label="Website" name="website" value={form.website} onChange={handleChange} />

          {/* Postal Code + Search */}
          <div style={{ display: 'flex', gap: '12px', alignItems: 'flex-end' }}>
            <div style={{ flex: 1 }}>
              <FormField label="Postal Code" name="postal_code" value={form.postal_code} onChange={handleChange} />
            </div>
            <BlueButton type="button" outline style={{ marginBottom: '16px' }}>Search</BlueButton>
          </div>

          {!showAddress && (
            <p>
              <span className="am-link" onClick={() => setShowAddress(true)}>+ Add New Address</span>
            </p>
          )}

          {showAddress && (
            <div style={{ border: '1px solid #dadce0', padding: '16px', borderRadius: '8px', marginBottom: '16px' }}>
              <h4 style={{ marginBottom: '16px' }}>Address Details</h4>
              <div className="am-grid am-grid-2">
                <FormField label="Street Number" name="street_number" value={form.street_number} onChange={handleChange} error={errors.street_number} required />
                <FormField label="Address Line 1" name="address_line1" value={form.address_line1} onChange={handleChange} error={errors.address_line1} required />
              </div>
              <FormField label="Address Line 2" name="address_line2" value={form.address_line2} onChange={handleChange} />
              <div className="am-grid am-grid-3">
                <FormField label="Suite Number" name="suite_number" value={form.suite_number} onChange={handleChange} />
                <FormField
                  label="Suite Type"
                  name="suite_type"
                  type="select"
                  value={form.suite_type}
                  onChange={handleChange}
                  options={[
                    { value: 'Apartment', label: 'Apartment' },
                    { value: 'Suite', label: 'Suite' },
                    { value: 'Unit', label: 'Unit' }
                  ]}
                />
                <FormField label="City" name="city" value={form.city} onChange={handleChange} error={errors.city} required />
              </div>
              <div className="am-grid am-grid-2">
                <FormField label="Province/State" name="province" value={form.province} onChange={handleChange} error={errors.province} required />
                <FormField label="Postal Code" name="postal_code" value={form.postal_code} onChange={handleChange} disabled />
              </div>
            </div>
          )}

          <BlueButton type="submit" disabled={loading} style={{ marginTop: '16px' }}>
            {loading ? 'Saving...' : 'Save'}
          </BlueButton>
        </form>
      </div>

      <Popup {...popup} onClose={() => { setPopup({ open: false }); if (popup.type === 'success') navigate('/anyminute/home'); }} />
    </Layout>
  );
}
