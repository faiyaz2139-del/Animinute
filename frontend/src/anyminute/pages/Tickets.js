import React, { useState, useEffect } from 'react';
import { useAMAuth, AM_API_URL } from '../context/AMAuthContext';
import { Layout } from '../components/Layout';
import { BlueButton, Popup } from '../components/SharedComponents';
import axios from 'axios';
import { 
  Ticket, Plus, MessageCircle, Clock, AlertCircle, 
  CheckCircle, XCircle, ChevronRight, Send, ArrowLeft 
} from 'lucide-react';

const STATUS_COLORS = {
  open: { bg: '#fff3cd', color: '#856404', label: 'Open' },
  in_progress: { bg: '#cce5ff', color: '#004085', label: 'In Progress' },
  resolved: { bg: '#d4edda', color: '#155724', label: 'Resolved' },
  closed: { bg: '#e2e3e5', color: '#383d41', label: 'Closed' }
};

const PRIORITY_COLORS = {
  low: { bg: '#e2e3e5', color: '#383d41' },
  medium: { bg: '#cce5ff', color: '#004085' },
  high: { bg: '#fff3cd', color: '#856404' },
  urgent: { bg: '#f8d7da', color: '#721c24' }
};

export default function Tickets() {
  const { token, user: currentUser } = useAMAuth();
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [selectedTicket, setSelectedTicket] = useState(null);
  const [ticketDetails, setTicketDetails] = useState(null);
  const [saving, setSaving] = useState(false);
  const [popup, setPopup] = useState({ open: false });
  const [replyText, setReplyText] = useState('');
  const [form, setForm] = useState({
    subject: '',
    description: '',
    priority: 'medium'
  });

  const config = { headers: { Authorization: `Bearer ${token}` } };
  const isAdmin = currentUser?.role === 'admin';

  useEffect(() => {
    loadTickets();
  }, [token]);

  const loadTickets = async () => {
    try {
      const res = await axios.get(`${AM_API_URL}/tickets`, config);
      setTickets(res.data || []);
    } catch (err) {
      console.error('Error loading tickets:', err);
    } finally {
      setLoading(false);
    }
  };

  const loadTicketDetails = async (ticketId) => {
    try {
      const res = await axios.get(`${AM_API_URL}/tickets/${ticketId}`, config);
      setTicketDetails(res.data);
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: 'Failed to load ticket details' });
    }
  };

  const openTicket = async (ticket) => {
    setSelectedTicket(ticket);
    await loadTicketDetails(ticket.id);
  };

  const closeTicket = () => {
    setSelectedTicket(null);
    setTicketDetails(null);
    setReplyText('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.subject.trim() || !form.description.trim()) {
      setPopup({ open: true, type: 'error', title: 'Error', message: 'Please fill in all fields' });
      return;
    }

    setSaving(true);
    try {
      await axios.post(`${AM_API_URL}/tickets`, form, config);
      setPopup({ open: true, type: 'success', title: 'Success', message: 'Ticket created successfully!' });
      setShowModal(false);
      setForm({ subject: '', description: '', priority: 'medium' });
      loadTickets();
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: err.response?.data?.detail || 'Failed to create ticket' });
    } finally {
      setSaving(false);
    }
  };

  const handleReply = async () => {
    if (!replyText.trim()) return;

    setSaving(true);
    try {
      await axios.post(`${AM_API_URL}/tickets/${selectedTicket.id}/reply`, { message: replyText }, config);
      setReplyText('');
      await loadTicketDetails(selectedTicket.id);
      loadTickets();
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: 'Failed to send reply' });
    } finally {
      setSaving(false);
    }
  };

  const updateStatus = async (newStatus) => {
    try {
      await axios.put(`${AM_API_URL}/tickets/${selectedTicket.id}/status`, { status: newStatus }, config);
      await loadTicketDetails(selectedTicket.id);
      loadTickets();
      setPopup({ open: true, type: 'success', title: 'Success', message: 'Status updated!' });
    } catch (err) {
      setPopup({ open: true, type: 'error', title: 'Error', message: 'Failed to update status' });
    }
  };

  const formatDate = (dateStr) => {
    return new Date(dateStr).toLocaleString('en-US', {
      month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  };

  if (loading) {
    return (
      <Layout title="Support Tickets">
        <div style={{ display: 'flex', justifyContent: 'center', padding: '40px' }}>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </Layout>
    );
  }

  // Ticket Detail View
  if (selectedTicket && ticketDetails) {
    const { ticket, replies } = ticketDetails;
    const statusStyle = STATUS_COLORS[ticket.status] || STATUS_COLORS.open;

    return (
      <Layout title={`Ticket #${ticket.ticket_number}`}>
        <button
          onClick={closeTicket}
          style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '16px', background: 'none', border: 'none', cursor: 'pointer', color: '#1a73e8' }}
          data-testid="back-to-tickets-btn"
        >
          <ArrowLeft size={18} />
          Back to Tickets
        </button>

        <div className="am-card" style={{ marginBottom: '24px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '16px' }}>
            <div>
              <h2 style={{ marginBottom: '8px' }}>{ticket.subject}</h2>
              <div style={{ display: 'flex', gap: '12px', fontSize: '13px', color: '#666' }}>
                <span>By: {ticket.created_by_name}</span>
                <span>•</span>
                <span>{formatDate(ticket.created_at)}</span>
              </div>
            </div>
            <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
              <span style={{
                padding: '4px 12px',
                backgroundColor: PRIORITY_COLORS[ticket.priority]?.bg,
                color: PRIORITY_COLORS[ticket.priority]?.color,
                borderRadius: '12px',
                fontSize: '12px',
                fontWeight: '600',
                textTransform: 'capitalize'
              }}>
                {ticket.priority}
              </span>
              <span style={{
                padding: '4px 12px',
                backgroundColor: statusStyle.bg,
                color: statusStyle.color,
                borderRadius: '12px',
                fontSize: '12px',
                fontWeight: '600'
              }}>
                {statusStyle.label}
              </span>
            </div>
          </div>

          <div style={{ padding: '16px', backgroundColor: '#f8f9fa', borderRadius: '8px', marginBottom: '16px' }}>
            <p style={{ whiteSpace: 'pre-wrap', margin: 0 }}>{ticket.description}</p>
          </div>

          {/* Admin Status Controls */}
          {isAdmin && (
            <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
              <span style={{ fontSize: '13px', color: '#666', marginRight: '8px', alignSelf: 'center' }}>Change Status:</span>
              {['open', 'in_progress', 'resolved', 'closed'].map(status => (
                <button
                  key={status}
                  onClick={() => updateStatus(status)}
                  disabled={ticket.status === status}
                  style={{
                    padding: '6px 12px',
                    backgroundColor: ticket.status === status ? STATUS_COLORS[status].bg : '#f5f5f5',
                    color: ticket.status === status ? STATUS_COLORS[status].color : '#666',
                    border: 'none',
                    borderRadius: '6px',
                    cursor: ticket.status === status ? 'default' : 'pointer',
                    fontSize: '12px',
                    fontWeight: '500'
                  }}
                  data-testid={`status-btn-${status}`}
                >
                  {STATUS_COLORS[status].label}
                </button>
              ))}
            </div>
          )}
        </div>

        {/* Replies */}
        <div className="am-card" style={{ marginBottom: '24px' }}>
          <h3 style={{ marginBottom: '16px' }}>Conversation ({replies.length})</h3>
          
          {replies.length === 0 ? (
            <p style={{ color: '#666', textAlign: 'center', padding: '20px' }}>No replies yet</p>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
              {replies.map(reply => (
                <div
                  key={reply.id}
                  style={{
                    padding: '12px 16px',
                    backgroundColor: reply.is_admin_reply ? '#e8f0fe' : '#f8f9fa',
                    borderRadius: '8px',
                    borderLeft: reply.is_admin_reply ? '3px solid #1a73e8' : '3px solid #e0e0e0'
                  }}
                >
                  <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                    <span style={{ fontWeight: '600', fontSize: '13px' }}>
                      {reply.created_by_name}
                      {reply.is_admin_reply && <span style={{ color: '#1a73e8', marginLeft: '8px' }}>(Support)</span>}
                    </span>
                    <span style={{ fontSize: '12px', color: '#666' }}>{formatDate(reply.created_at)}</span>
                  </div>
                  <p style={{ margin: 0, whiteSpace: 'pre-wrap' }}>{reply.message}</p>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Reply Form */}
        {ticket.status !== 'closed' && (
          <div className="am-card">
            <h3 style={{ marginBottom: '12px' }}>Add Reply</h3>
            <textarea
              value={replyText}
              onChange={(e) => setReplyText(e.target.value)}
              placeholder="Type your message..."
              className="am-input"
              rows={3}
              style={{ width: '100%', resize: 'vertical' }}
              data-testid="reply-textarea"
            />
            <div style={{ marginTop: '12px', display: 'flex', justifyContent: 'flex-end' }}>
              <BlueButton onClick={handleReply} disabled={saving || !replyText.trim()} data-testid="send-reply-btn">
                <Send size={16} style={{ marginRight: '8px' }} />
                {saving ? 'Sending...' : 'Send Reply'}
              </BlueButton>
            </div>
          </div>
        )}

        <Popup {...popup} onClose={() => setPopup({ open: false })} />
      </Layout>
    );
  }

  // Tickets List View
  return (
    <Layout title="Support Tickets">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
        <p style={{ color: '#666', margin: 0 }}>
          {isAdmin ? 'Manage all support tickets' : 'Submit and track your support requests'}
        </p>
        <BlueButton onClick={() => setShowModal(true)} data-testid="new-ticket-btn">
          <Plus size={16} style={{ marginRight: '8px' }} />
          New Ticket
        </BlueButton>
      </div>

      {tickets.length === 0 ? (
        <div className="am-card" style={{ textAlign: 'center', padding: '60px 20px' }}>
          <Ticket size={48} style={{ color: '#bdbdbd', marginBottom: '16px' }} />
          <h3 style={{ color: '#757575', marginBottom: '8px' }}>No Tickets Yet</h3>
          <p style={{ color: '#9e9e9e', marginBottom: '24px' }}>Create a support ticket to get help from our team.</p>
          <BlueButton onClick={() => setShowModal(true)} data-testid="create-first-ticket-btn">
            Create Your First Ticket
          </BlueButton>
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
          {tickets.map(ticket => {
            const statusStyle = STATUS_COLORS[ticket.status] || STATUS_COLORS.open;
            return (
              <div
                key={ticket.id}
                className="am-card"
                style={{ cursor: 'pointer', transition: 'box-shadow 0.2s' }}
                onClick={() => openTicket(ticket)}
                data-testid={`ticket-row-${ticket.id}`}
              >
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <div style={{ flex: 1 }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '8px' }}>
                      <span style={{ color: '#666', fontSize: '13px' }}>#{ticket.ticket_number}</span>
                      <h4 style={{ margin: 0 }}>{ticket.subject}</h4>
                    </div>
                    <div style={{ display: 'flex', gap: '16px', fontSize: '13px', color: '#666' }}>
                      <span style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                        <Clock size={14} />
                        {formatDate(ticket.created_at)}
                      </span>
                      {isAdmin && (
                        <span>By: {ticket.created_by_name}</span>
                      )}
                    </div>
                  </div>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                    <span style={{
                      padding: '4px 12px',
                      backgroundColor: PRIORITY_COLORS[ticket.priority]?.bg,
                      color: PRIORITY_COLORS[ticket.priority]?.color,
                      borderRadius: '12px',
                      fontSize: '11px',
                      fontWeight: '600',
                      textTransform: 'capitalize'
                    }}>
                      {ticket.priority}
                    </span>
                    <span style={{
                      padding: '4px 12px',
                      backgroundColor: statusStyle.bg,
                      color: statusStyle.color,
                      borderRadius: '12px',
                      fontSize: '11px',
                      fontWeight: '600'
                    }}>
                      {statusStyle.label}
                    </span>
                    <ChevronRight size={20} style={{ color: '#999' }} />
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* New Ticket Modal */}
      {showModal && (
        <div className="am-modal-overlay" onClick={() => setShowModal(false)}>
          <div className="am-modal" onClick={e => e.stopPropagation()} data-testid="new-ticket-modal">
            <h3 style={{ marginBottom: '20px' }}>Create New Ticket</h3>
            <form onSubmit={handleSubmit}>
              <div className="am-form-group">
                <label>Subject *</label>
                <input
                  type="text"
                  value={form.subject}
                  onChange={(e) => setForm({ ...form, subject: e.target.value })}
                  className="am-input"
                  placeholder="Brief description of your issue"
                  required
                  data-testid="ticket-subject-input"
                />
              </div>

              <div className="am-form-group">
                <label>Priority</label>
                <select
                  value={form.priority}
                  onChange={(e) => setForm({ ...form, priority: e.target.value })}
                  className="am-select"
                  data-testid="ticket-priority-select"
                >
                  <option value="low">Low</option>
                  <option value="medium">Medium</option>
                  <option value="high">High</option>
                  <option value="urgent">Urgent</option>
                </select>
              </div>

              <div className="am-form-group">
                <label>Description *</label>
                <textarea
                  value={form.description}
                  onChange={(e) => setForm({ ...form, description: e.target.value })}
                  className="am-input"
                  rows={5}
                  placeholder="Provide details about your issue..."
                  required
                  style={{ resize: 'vertical' }}
                  data-testid="ticket-description-textarea"
                />
              </div>

              <div className="am-modal-footer">
                <button type="button" onClick={() => setShowModal(false)} className="am-btn am-btn-secondary">
                  Cancel
                </button>
                <BlueButton type="submit" disabled={saving} data-testid="submit-ticket-btn">
                  {saving ? 'Creating...' : 'Create Ticket'}
                </BlueButton>
              </div>
            </form>
          </div>
        </div>
      )}

      <Popup {...popup} onClose={() => setPopup({ open: false })} />
    </Layout>
  );
}
