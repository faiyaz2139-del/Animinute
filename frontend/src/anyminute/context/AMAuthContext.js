import { createContext, useContext, useState, useEffect } from 'react';
import axios from 'axios';

const AM_API = `${process.env.REACT_APP_BACKEND_URL}/api/am`;

const AMAuthContext = createContext(null);

export const useAMAuth = () => {
  const context = useContext(AMAuthContext);
  if (!context) throw new Error('useAMAuth must be used within AMAuthProvider');
  return context;
};

export const AMAuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [token, setToken] = useState(null);

  useEffect(() => {
    const savedToken = localStorage.getItem('am_token');
    if (savedToken) {
      setToken(savedToken);
      fetchUser(savedToken);
    } else {
      setLoading(false);
    }
  }, []);

  const fetchUser = async (authToken) => {
    try {
      const response = await axios.get(`${AM_API}/auth/me`, {
        headers: { Authorization: `Bearer ${authToken}` }
      });
      setUser(response.data);
    } catch (error) {
      localStorage.removeItem('am_token');
      setToken(null);
    } finally {
      setLoading(false);
    }
  };

  const login = async (email, password) => {
    const response = await axios.post(`${AM_API}/auth/login`, { email, password });
    const { token: newToken, user: userData } = response.data;
    localStorage.setItem('am_token', newToken);
    setToken(newToken);
    setUser(userData);
    return userData;
  };

  const register = async (data) => {
    const response = await axios.post(`${AM_API}/auth/register`, data);
    const { token: newToken, user: userData } = response.data;
    localStorage.setItem('am_token', newToken);
    setToken(newToken);
    setUser(userData);
    return response.data;
  };

  const logout = () => {
    localStorage.removeItem('am_token');
    setToken(null);
    setUser(null);
  };

  const isAdmin = user?.role === 'admin';
  const isManager = user?.role === 'manager' || user?.role === 'admin';

  return (
    <AMAuthContext.Provider value={{ user, loading, login, register, logout, fetchUser, token, isAdmin, isManager }}>
      {children}
    </AMAuthContext.Provider>
  );
};

export const AM_API_URL = AM_API;
