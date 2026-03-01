import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { formatCurrency, formatDate } from '../lib/utils';
import { useAuth } from '../context/AuthContext';
import { 
  Users, 
  Calendar, 
  FileDown, 
  Calculator, 
  ArrowRight,
  TrendingUp,
  Clock,
  CheckCircle2
} from 'lucide-react';

const API = `${process.env.REACT_APP_BACKEND_URL}/api`;

export default function Dashboard() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      const response = await axios.get(`${API}/dashboard/stats`);
      setStats(response.data);
    } catch (error) {
      console.error('Failed to fetch stats:', error);
    } finally {
      setLoading(false);
    }
  };

  const isAdmin = user?.role === 'admin' || user?.role === 'manager';

  return (
    <div className="space-y-8" data-testid="dashboard">
      {/* Header */}
      <div className="space-y-2">
        <h1 className="text-3xl font-bold tracking-tight">
          Welcome back, {user?.first_name}
        </h1>
        <p className="text-muted-foreground">
          Here's an overview of your payroll operations
        </p>
      </div>

      {isAdmin && (
        <>
          {/* Stats Grid */}
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <Card className="card-hover animate-fadeIn stagger-1" data-testid="stat-employees">
              <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Active Employees
                </CardTitle>
                <Users className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold">{stats?.employee_count || 0}</div>
                <p className="text-xs text-muted-foreground mt-1">
                  In your organization
                </p>
              </CardContent>
            </Card>

            <Card className="card-hover animate-fadeIn stagger-2" data-testid="stat-periods">
              <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Open Pay Periods
                </CardTitle>
                <Calendar className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold">{stats?.open_pay_periods || 0}</div>
                <p className="text-xs text-muted-foreground mt-1">
                  Awaiting processing
                </p>
              </CardContent>
            </Card>

            <Card className="card-hover animate-fadeIn stagger-3" data-testid="stat-imports">
              <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Pending Imports
                </CardTitle>
                <FileDown className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold">{stats?.pending_imports || 0}</div>
                <p className="text-xs text-muted-foreground mt-1">
                  Timesheets to import
                </p>
              </CardContent>
            </Card>

            <Card className="card-hover animate-fadeIn stagger-4" data-testid="stat-last-run">
              <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Last Payroll Run
                </CardTitle>
                <Calculator className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-xl font-bold">
                  {stats?.last_payroll_run 
                    ? formatDate(stats.last_payroll_run.locked_at) 
                    : 'No runs yet'}
                </div>
                <p className="text-xs text-muted-foreground mt-1">
                  {stats?.last_payroll_run ? 'Completed successfully' : 'Get started below'}
                </p>
              </CardContent>
            </Card>
          </div>

          {/* Quick Actions */}
          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            <Card className="card-hover animate-fadeIn stagger-2">
              <CardHeader>
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                    <FileDown className="h-5 w-5 text-primary" />
                  </div>
                  <div>
                    <CardTitle className="text-lg">Import Timesheets</CardTitle>
                    <CardDescription>Fetch approved hours from your timesheet system</CardDescription>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <Button 
                  onClick={() => navigate('/import-timesheets')} 
                  className="w-full btn-scale"
                  data-testid="action-import"
                >
                  Start Import <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </CardContent>
            </Card>

            <Card className="card-hover animate-fadeIn stagger-3">
              <CardHeader>
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 rounded-lg bg-green-500/10 flex items-center justify-center">
                    <Calculator className="h-5 w-5 text-green-600" />
                  </div>
                  <div>
                    <CardTitle className="text-lg">Run Payroll</CardTitle>
                    <CardDescription>Calculate pay for the current period</CardDescription>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <Button 
                  onClick={() => navigate('/payroll-runs')} 
                  variant="outline"
                  className="w-full btn-scale"
                  data-testid="action-payroll"
                >
                  View Payroll Runs <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </CardContent>
            </Card>

            <Card className="card-hover animate-fadeIn stagger-4">
              <CardHeader>
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 rounded-lg bg-blue-500/10 flex items-center justify-center">
                    <Users className="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <CardTitle className="text-lg">Manage Employees</CardTitle>
                    <CardDescription>Add or update employee information</CardDescription>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <Button 
                  onClick={() => navigate('/employees')} 
                  variant="outline"
                  className="w-full btn-scale"
                  data-testid="action-employees"
                >
                  View Employees <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </CardContent>
            </Card>
          </div>

          {/* Workflow Guide */}
          <Card className="animate-fadeIn stagger-5">
            <CardHeader>
              <CardTitle>Payroll Workflow</CardTitle>
              <CardDescription>Follow these steps to complete your payroll</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="grid gap-4 md:grid-cols-5">
                {[
                  { step: 1, title: 'Create Pay Period', icon: Calendar, desc: 'Define start and end dates' },
                  { step: 2, title: 'Import Timesheets', icon: FileDown, desc: 'Fetch approved hours' },
                  { step: 3, title: 'Create Payroll Run', icon: Calculator, desc: 'Start new calculation' },
                  { step: 4, title: 'Review & Lock', icon: CheckCircle2, desc: 'Verify and finalize' },
                  { step: 5, title: 'Generate Payslips', icon: TrendingUp, desc: 'Create PDFs for employees' },
                ].map((item, index) => (
                  <div key={item.step} className="flex flex-col items-center text-center p-4 rounded-lg bg-muted/50">
                    <div className="h-12 w-12 rounded-full bg-primary/10 flex items-center justify-center mb-3">
                      <item.icon className="h-6 w-6 text-primary" />
                    </div>
                    <span className="text-xs font-semibold text-primary mb-1">Step {item.step}</span>
                    <h4 className="font-medium text-sm">{item.title}</h4>
                    <p className="text-xs text-muted-foreground mt-1">{item.desc}</p>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </>
      )}

      {/* Employee View */}
      {user?.role === 'employee' && (
        <Card>
          <CardHeader>
            <CardTitle>Your Payslips</CardTitle>
            <CardDescription>View and download your payment history</CardDescription>
          </CardHeader>
          <CardContent>
            <Button onClick={() => navigate('/payslips')} data-testid="view-payslips-btn">
              View My Payslips <ArrowRight className="ml-2 h-4 w-4" />
            </Button>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
