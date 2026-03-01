import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';
import { Layout } from '../components/Layout';
import { FormField, BlueButton, Popup } from '../components/SharedComponents';
import { AM_API_URL, useAMAuth } from '../context/AMAuthContext';

export default function AMAddBusiness() {
  const navigate = useNavigate();
  const location = useLocation();
  const { token } = useAMAuth();
  const editBusiness = location.state?.business;

  const [form, setForm] = useState({
    company_name: '', operating_name: '', primary_contact_name: '', business_number: '',
    legal_entity_type: 'Incorporation', website: '', street_number: '', address_line1: '',
    address_line2: '', suite_number: '', suite_type: '', city: '', province: '', postal_code: '', 
    logo_url: '', contact_email: '', contact_phone: ''
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [popup, setPopup] = useState({ isOpen: false });
  const [showAddress, setShowAddress] = useState(false);

  // Auth headers for API calls
  const config = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    if (editBusiness) {
      // Map backend fields back to form fields
      setForm({ 
        ...form, 
        company_name: editBusiness.name || '',
        address_line1: editBusiness.address || '',
        contact_email: editBusiness.contact_email || '',
        contact_phone: editBusiness.contact_phone || '',
        ...editBusiness 
      });
      setShowAddress(!!editBusiness.address);
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

  // Build the address string from form fields
  const buildAddress = () => {
    if (!showAddress) return null;
    const parts = [];
    if (form.street_number) parts.push(form.street_number);
    if (form.address_line1) parts.push(form.address_line1);
    if (form.suite_number && form.suite_type) parts.push(`${form.suite_type} ${form.suite_number}`);
    if (form.address_line2) parts.push(form.address_line2);
    if (form.city) parts.push(form.city);
    if (form.province) parts.push(form.province);
    if (form.postal_code) parts.push(form.postal_code);
    return parts.join(', ');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;
    setLoading(true);
    
    // Map form fields to backend schema
    const payload = {
      name: form.company_name,
      address: buildAddress(),
      contact_email: form.contact_email || null,
      contact_phone: form.contact_phone || null
    };
    
    try {
      if (editBusiness) {
        await axios.put(`${AM_API_URL}/businesses/${editBusiness.id}`, payload, config);
        setPopup({ isOpen: true, type: 'success', title: 'Success', message: 'Business updated successfully!' });
      } else {
        await axios.post(`${AM_API_URL}/businesses`, payload, config);
        setPopup({ isOpen: true, type: 'success', title: 'Success', message: 'Business created successfully!' });
      }
    } catch (err) {
      console.error('Business save error:', err.response?.data || err);
      setPopup({ isOpen: true, type: 'error', title: 'Error', message: err.response?.data?.detail || 'Failed to save business' });
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

          <FormField label="Company Name" name="company_name" value={form.company_name} onChange={handleChange} error={errors.company_name} required data-testid="business-name-input" />
          <FormField label="Operating Name" name="operating_name" value={form.operating_name} onChange={handleChange} />
          <FormField label="Primary Contact Name" name="primary_contact_name" value={form.primary_contact_name} onChange={handleChange} error={errors.primary_contact_name} required />
          <FormField label="Contact Email" name="contact_email" value={form.contact_email} onChange={handleChange} type="email" />
          <FormField label="Contact Phone" name="contact_phone" value={form.contact_phone} onChange={handleChange} />
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
                    { value: '', label: 'Select...' },
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

          <BlueButton type="submit" disabled={loading} style={{ marginTop: '16px' }} data-testid="save-business-btn">
            {loading ? 'Saving...' : 'Save'}
          </BlueButton>
        </form>
      </div>

      <Popup {...popup} onClose={() => { setPopup({ open: false }); if (popup.type === 'success') navigate('/anyminute/home'); }} />
    </Layout>
  );
}
