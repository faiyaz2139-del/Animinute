import React from 'react';

// Blue Button Component - BRD Spec
export const BlueButton = ({ children, onClick, disabled, type = 'button', className = '', outline = false }) => (
  <button
    type={type}
    onClick={onClick}
    disabled={disabled}
    className={`am-btn ${outline ? 'am-btn-outline' : ''} ${className}`}
    data-testid="blue-button"
  >
    {children}
  </button>
);

// Form Field Component
export const FormField = ({ label, name, type = 'text', value, onChange, error, required, options, placeholder, disabled }) => (
  <div className={`am-field ${error ? 'has-error' : ''}`}>
    {label && <label htmlFor={name}>{label}{required && ' *'}</label>}
    {type === 'select' ? (
      <select
        id={name}
        name={name}
        value={value}
        onChange={onChange}
        className="am-select"
        disabled={disabled}
        data-testid={`field-${name}`}
      >
        <option value="">{placeholder || 'Select...'}</option>
        {options?.map(opt => (
          <option key={opt.value} value={opt.value}>{opt.label}</option>
        ))}
      </select>
    ) : type === 'textarea' ? (
      <textarea
        id={name}
        name={name}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        disabled={disabled}
        rows={4}
        data-testid={`field-${name}`}
      />
    ) : (
      <input
        id={name}
        name={name}
        type={type}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        disabled={disabled}
        data-testid={`field-${name}`}
      />
    )}
    {error && <div className="am-error">{error}</div>}
  </div>
);

// Popup Component - BRD Spec (Warning/Info/Error/Success)
export const Popup = ({ isOpen, onClose, type = 'info', title, message, onConfirm, confirmText = 'OK', showCancel = false }) => {
  if (!isOpen) return null;

  const typeStyles = {
    warning: { bg: '#fef7e0', border: '#f9ab00', icon: '⚠️' },
    info: { bg: '#e8f0fe', border: '#1a73e8', icon: 'ℹ️' },
    error: { bg: '#fee2e2', border: '#d93025', icon: '❌' },
    success: { bg: '#dcfce7', border: '#0f9d58', icon: '✓' }
  };

  const style = typeStyles[type] || typeStyles.info;

  return (
    <div className="am-popup-overlay" style={{
      position: 'fixed', inset: 0, backgroundColor: 'rgba(0,0,0,0.5)',
      display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 9999
    }}>
      <div className="am-popup" style={{
        backgroundColor: 'white', borderRadius: '8px', padding: '24px',
        maxWidth: '400px', width: '90%', borderTop: `4px solid ${style.border}`
      }} data-testid={`popup-${type}`}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '16px' }}>
          <span style={{ fontSize: '24px' }}>{style.icon}</span>
          <h3 style={{ margin: 0, fontSize: '18px', fontWeight: 600 }}>{title}</h3>
        </div>
        <p style={{ margin: '0 0 20px', color: '#333' }}>{message}</p>
        <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
          {showCancel && (
            <button onClick={onClose} className="am-btn am-btn-outline">Cancel</button>
          )}
          <button onClick={onConfirm || onClose} className="am-btn" data-testid="popup-confirm">
            {confirmText}
          </button>
        </div>
      </div>
    </div>
  );
};

// Table Component
export const Table = ({ columns, data, onRowClick }) => (
  <table className="am-table">
    <thead>
      <tr>
        {columns.map(col => (
          <th key={col.key} style={{ width: col.width }}>{col.label}</th>
        ))}
      </tr>
    </thead>
    <tbody>
      {data.length === 0 ? (
        <tr>
          <td colSpan={columns.length} style={{ textAlign: 'center', padding: '40px', color: '#666' }}>
            No data available
          </td>
        </tr>
      ) : (
        data.map((row, idx) => (
          <tr key={row.id || idx} onClick={() => onRowClick?.(row)} style={{ cursor: onRowClick ? 'pointer' : 'default' }}>
            {columns.map(col => (
              <td key={col.key}>{col.render ? col.render(row[col.key], row) : row[col.key]}</td>
            ))}
          </tr>
        ))
      )}
    </tbody>
  </table>
);

// Status Badge
export const StatusBadge = ({ status }) => {
  const statusClass = {
    pending: 'am-badge-pending',
    approved: 'am-badge-approved',
    rejected: 'am-badge-rejected'
  }[status] || 'am-badge-pending';

  return <span className={`am-badge ${statusClass}`}>{status}</span>;
};

// Coming Soon Placeholder
export const ComingSoon = ({ title }) => (
  <div className="am-coming-soon" data-testid="coming-soon">
    <h2>{title}</h2>
    <p>This feature is coming soon in Phase 2.</p>
  </div>
);

// Loading Spinner
export const Loader = () => (
  <div style={{ display: 'flex', justifyContent: 'center', padding: '40px' }}>
    <div style={{
      width: '40px', height: '40px', border: '3px solid #e5e7eb',
      borderTopColor: '#1a73e8', borderRadius: '50%',
      animation: 'spin 1s linear infinite'
    }} />
    <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
  </div>
);
