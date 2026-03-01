import { useState, useEffect } from 'react';
import axios from 'axios';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { formatCurrency } from '../lib/utils';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  LineChart,
  Line,
  Area,
  AreaChart
} from 'recharts';
import { 
  TrendingUp, 
  DollarSign, 
  Users, 
  Calculator,
  PieChartIcon,
  BarChart3,
  Loader2,
  ArrowUp,
  ArrowDown
} from 'lucide-react';
import { toast } from 'sonner';

const API = `${process.env.REACT_APP_BACKEND_URL}/api`;

export default function Analytics() {
  const [loading, setLoading] = useState(true);
  const [summary, setSummary] = useState(null);
  const [trends, setTrends] = useState([]);
  const [deductionsBreakdown, setDeductionsBreakdown] = useState([]);
  const [employeeCosts, setEmployeeCosts] = useState([]);
  const [forecast, setForecast] = useState({ monthly: [], annual_projection: {} });

  useEffect(() => {
    fetchAnalytics();
  }, []);

  const fetchAnalytics = async () => {
    try {
      const [summaryRes, trendsRes, deductionsRes, costsRes, forecastRes] = await Promise.all([
        axios.get(`${API}/analytics/summary`),
        axios.get(`${API}/analytics/payroll-trends`),
        axios.get(`${API}/analytics/deductions-breakdown`),
        axios.get(`${API}/analytics/employee-costs`),
        axios.get(`${API}/analytics/cost-forecast`)
      ]);
      
      setSummary(summaryRes.data);
      setTrends(trendsRes.data);
      setDeductionsBreakdown(deductionsRes.data);
      setEmployeeCosts(costsRes.data);
      setForecast(forecastRes.data);
    } catch (error) {
      toast.error('Failed to fetch analytics data');
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const COLORS = ['#1a237e', '#6c8fef', '#10b981', '#f59e0b'];

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  return (
    <div className="space-y-6" data-testid="analytics-page">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Payroll Analytics</h1>
        <p className="text-muted-foreground">Insights and forecasts for better financial decisions</p>
      </div>

      {/* Summary Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card className="card-hover animate-fadeIn stagger-1" data-testid="total-gross-card">
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">
              Total Gross Paid
            </CardTitle>
            <DollarSign className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{formatCurrency(summary?.total_gross_paid || 0)}</div>
            <p className="text-xs text-muted-foreground mt-1">
              Across {summary?.total_payroll_runs || 0} payroll runs
            </p>
          </CardContent>
        </Card>

        <Card className="card-hover animate-fadeIn stagger-2" data-testid="total-deductions-card">
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">
              Total Deductions
            </CardTitle>
            <Calculator className="h-4 w-4 text-amber-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{formatCurrency(summary?.total_deductions || 0)}</div>
            <p className="text-xs text-muted-foreground mt-1">
              CPP, EI, Federal & Provincial taxes
            </p>
          </CardContent>
        </Card>

        <Card className="card-hover animate-fadeIn stagger-3" data-testid="total-net-card">
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">
              Total Net Paid
            </CardTitle>
            <TrendingUp className="h-4 w-4 text-primary" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">{formatCurrency(summary?.total_net_paid || 0)}</div>
            <p className="text-xs text-muted-foreground mt-1">
              To {summary?.active_employees || 0} employees
            </p>
          </CardContent>
        </Card>

        <Card className="card-hover animate-fadeIn stagger-4" data-testid="avg-payroll-card">
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">
              Avg. Payroll Per Run
            </CardTitle>
            <BarChart3 className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{formatCurrency(summary?.avg_payroll_per_run || 0)}</div>
            <p className="text-xs text-muted-foreground mt-1">
              {forecast?.pay_frequency === 'weekly' ? 'Weekly' : 'Bi-weekly'} frequency
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Charts Row 1 */}
      <div className="grid gap-6 lg:grid-cols-2">
        {/* Payroll Trends */}
        <Card className="animate-fadeIn">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <TrendingUp className="h-5 w-5 text-primary" />
              Payroll Trends
            </CardTitle>
            <CardDescription>Gross, deductions, and net pay over time</CardDescription>
          </CardHeader>
          <CardContent>
            {trends.length === 0 ? (
              <div className="h-[300px] flex items-center justify-center text-muted-foreground">
                No payroll data yet. Complete a payroll run to see trends.
              </div>
            ) : (
              <ResponsiveContainer width="100%" height={300}>
                <AreaChart data={trends}>
                  <defs>
                    <linearGradient id="colorGross" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#1a237e" stopOpacity={0.2}/>
                      <stop offset="95%" stopColor="#1a237e" stopOpacity={0}/>
                    </linearGradient>
                    <linearGradient id="colorNet" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#10b981" stopOpacity={0.2}/>
                      <stop offset="95%" stopColor="#10b981" stopOpacity={0}/>
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                  <XAxis dataKey="period" className="text-xs" tick={{ fill: 'hsl(var(--muted-foreground))' }} />
                  <YAxis className="text-xs" tick={{ fill: 'hsl(var(--muted-foreground))' }} tickFormatter={(v) => `$${(v/1000).toFixed(0)}k`} />
                  <Tooltip 
                    contentStyle={{ 
                      backgroundColor: 'hsl(var(--card))', 
                      border: '1px solid hsl(var(--border))',
                      borderRadius: '8px'
                    }}
                    formatter={(value) => formatCurrency(value)}
                  />
                  <Legend />
                  <Area type="monotone" dataKey="gross" name="Gross Pay" stroke="#1a237e" fillOpacity={1} fill="url(#colorGross)" />
                  <Area type="monotone" dataKey="net" name="Net Pay" stroke="#10b981" fillOpacity={1} fill="url(#colorNet)" />
                  <Line type="monotone" dataKey="deductions" name="Deductions" stroke="#f59e0b" strokeWidth={2} dot={false} />
                </AreaChart>
              </ResponsiveContainer>
            )}
          </CardContent>
        </Card>

        {/* Deductions Breakdown */}
        <Card className="animate-fadeIn">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <PieChartIcon className="h-5 w-5 text-secondary" />
              Deductions Breakdown
            </CardTitle>
            <CardDescription>Distribution of payroll deductions by type</CardDescription>
          </CardHeader>
          <CardContent>
            {deductionsBreakdown.every(d => d.value === 0) ? (
              <div className="h-[300px] flex items-center justify-center text-muted-foreground">
                No deduction data yet.
              </div>
            ) : (
              <div className="flex items-center">
                <ResponsiveContainer width="60%" height={300}>
                  <PieChart>
                    <Pie
                      data={deductionsBreakdown}
                      cx="50%"
                      cy="50%"
                      innerRadius={60}
                      outerRadius={100}
                      paddingAngle={5}
                      dataKey="value"
                    >
                      {deductionsBreakdown.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color || COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip formatter={(value) => formatCurrency(value)} />
                  </PieChart>
                </ResponsiveContainer>
                <div className="w-[40%] space-y-3">
                  {deductionsBreakdown.map((item, index) => (
                    <div key={item.name} className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <div 
                          className="w-3 h-3 rounded-full" 
                          style={{ backgroundColor: item.color || COLORS[index % COLORS.length] }}
                        />
                        <span className="text-sm">{item.name}</span>
                      </div>
                      <span className="text-sm font-medium">{formatCurrency(item.value)}</span>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Charts Row 2 */}
      <div className="grid gap-6 lg:grid-cols-2">
        {/* Employee Costs */}
        <Card className="animate-fadeIn">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Users className="h-5 w-5 text-blue-600" />
              Employee Cost Distribution
            </CardTitle>
            <CardDescription>Gross pay breakdown per employee (latest run)</CardDescription>
          </CardHeader>
          <CardContent>
            {employeeCosts.length === 0 ? (
              <div className="h-[300px] flex items-center justify-center text-muted-foreground">
                No employee cost data yet.
              </div>
            ) : (
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={employeeCosts} layout="vertical">
                  <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                  <XAxis type="number" tick={{ fill: 'hsl(var(--muted-foreground))' }} tickFormatter={(v) => `$${v}`} />
                  <YAxis dataKey="name" type="category" width={100} tick={{ fill: 'hsl(var(--muted-foreground))', fontSize: 12 }} />
                  <Tooltip 
                    contentStyle={{ 
                      backgroundColor: 'hsl(var(--card))', 
                      border: '1px solid hsl(var(--border))',
                      borderRadius: '8px'
                    }}
                    formatter={(value) => formatCurrency(value)}
                  />
                  <Legend />
                  <Bar dataKey="gross" name="Gross Pay" fill="#1a237e" radius={[0, 4, 4, 0]} />
                  <Bar dataKey="net" name="Net Pay" fill="#10b981" radius={[0, 4, 4, 0]} />
                </BarChart>
              </ResponsiveContainer>
            )}
          </CardContent>
        </Card>

        {/* Annual Forecast */}
        <Card className="animate-fadeIn">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <TrendingUp className="h-5 w-5 text-green-600" />
              Annual Cost Forecast
            </CardTitle>
            <CardDescription>Projected payroll costs for the year</CardDescription>
          </CardHeader>
          <CardContent>
            {!forecast?.annual_projection?.gross ? (
              <div className="h-[300px] flex items-center justify-center text-muted-foreground">
                Complete a payroll run to see forecasts.
              </div>
            ) : (
              <div className="space-y-6">
                <div className="grid grid-cols-2 gap-4">
                  <div className="p-4 rounded-lg bg-primary/5 border border-primary/10">
                    <p className="text-sm text-muted-foreground">Projected Annual Gross</p>
                    <p className="text-2xl font-bold text-primary">{formatCurrency(forecast.annual_projection.gross)}</p>
                  </div>
                  <div className="p-4 rounded-lg bg-green-500/5 border border-green-500/10">
                    <p className="text-sm text-muted-foreground">Projected Annual Net</p>
                    <p className="text-2xl font-bold text-green-600">{formatCurrency(forecast.annual_projection.net)}</p>
                  </div>
                </div>
                
                <div className="space-y-3">
                  <h4 className="font-medium text-sm">Annual Deduction Breakdown</h4>
                  <div className="grid grid-cols-2 gap-3">
                    <div className="flex justify-between items-center p-2 rounded bg-muted/50">
                      <span className="text-sm">CPP</span>
                      <span className="font-medium">{formatCurrency(forecast.annual_projection.cpp)}</span>
                    </div>
                    <div className="flex justify-between items-center p-2 rounded bg-muted/50">
                      <span className="text-sm">EI</span>
                      <span className="font-medium">{formatCurrency(forecast.annual_projection.ei)}</span>
                    </div>
                    <div className="flex justify-between items-center p-2 rounded bg-muted/50">
                      <span className="text-sm">Total Tax</span>
                      <span className="font-medium">{formatCurrency(forecast.annual_projection.tax)}</span>
                    </div>
                    <div className="flex justify-between items-center p-2 rounded bg-amber-500/10">
                      <span className="text-sm font-medium">Total Employer Cost</span>
                      <span className="font-bold">{formatCurrency(forecast.annual_projection.employer_cost)}</span>
                    </div>
                  </div>
                </div>

                <div className="text-xs text-muted-foreground text-center pt-2 border-t">
                  Based on {forecast.periods_per_year} pay periods per year ({forecast.pay_frequency})
                </div>
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Monthly Projection Chart */}
      {forecast?.monthly?.length > 0 && (
        <Card className="animate-fadeIn">
          <CardHeader>
            <CardTitle>Monthly Payroll Projection</CardTitle>
            <CardDescription>Projected vs actual payroll costs by month</CardDescription>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={250}>
              <BarChart data={forecast.monthly}>
                <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                <XAxis dataKey="month" tick={{ fill: 'hsl(var(--muted-foreground))' }} />
                <YAxis tick={{ fill: 'hsl(var(--muted-foreground))' }} tickFormatter={(v) => `$${(v/1000).toFixed(0)}k`} />
                <Tooltip 
                  contentStyle={{ 
                    backgroundColor: 'hsl(var(--card))', 
                    border: '1px solid hsl(var(--border))',
                    borderRadius: '8px'
                  }}
                  formatter={(value) => formatCurrency(value)}
                />
                <Legend />
                <Bar dataKey="projected" name="Projected" fill="#6c8fef" radius={[4, 4, 0, 0]} />
                <Bar dataKey="actual" name="Actual" fill="#1a237e" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
