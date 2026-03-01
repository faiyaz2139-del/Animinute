import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../components/ui/table';
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from '../components/ui/alert';
import { toast } from 'sonner';
import { formatDate, formatCurrency } from '../lib/utils';
import { Calculator, Lock, FileText, ArrowLeft, Loader2, AlertTriangle, CheckCircle2 } from 'lucide-react';

const API = `${process.env.REACT_APP_BACKEND_URL}/api`;

export default function PayrollRunDetail() {
  const { runId } = useParams();
  const navigate = useNavigate();
  const [run, setRun] = useState(null);
  const [lines, setLines] = useState([]);
  const [period, setPeriod] = useState(null);
  const [loading, setLoading] = useState(true);
  const [calculating, setCalculating] = useState(false);
  const [locking, setLocking] = useState(false);
  const [generatingPayslips, setGeneratingPayslips] = useState(false);

  useEffect(() => {
    fetchData();
  }, [runId]);

  const fetchData = async () => {
    try {
      const [runsRes, linesRes, periodsRes] = await Promise.all([
        axios.get(`${API}/payroll-runs`),
        axios.get(`${API}/payroll-runs/${runId}/lines`),
        axios.get(`${API}/pay-periods`)
      ]);
      
      const currentRun = runsRes.data.find(r => r.id === runId);
      setRun(currentRun);
      setLines(linesRes.data);
      
      if (currentRun) {
        const currentPeriod = periodsRes.data.find(p => p.id === currentRun.pay_period_id);
        setPeriod(currentPeriod);
      }
    } catch (error) {
      toast.error('Failed to fetch payroll data');
    } finally {
      setLoading(false);
    }
  };

  const handleCalculate = async () => {
    setCalculating(true);
    try {
      await axios.post(`${API}/payroll-runs/${runId}/calculate`);
      toast.success('Payroll calculated successfully');
      fetchData();
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to calculate payroll');
    } finally {
      setCalculating(false);
    }
  };

  const handleLock = async () => {
    if (!window.confirm('Are you sure you want to lock this payroll run? This action cannot be undone.')) {
      return;
    }
    
    setLocking(true);
    try {
      await axios.post(`${API}/payroll-runs/${runId}/lock`);
      toast.success('Payroll run locked successfully');
      fetchData();
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to lock payroll run');
    } finally {
      setLocking(false);
    }
  };

  const handleGeneratePayslips = async () => {
    setGeneratingPayslips(true);
    try {
      const response = await axios.post(`${API}/payroll-runs/${runId}/generate-payslips`);
      toast.success(`Generated ${response.data.count} payslips`);
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to generate payslips');
    } finally {
      setGeneratingPayslips(false);
    }
  };

  const getStatusBadge = (status) => {
    const styles = {
      draft: 'status-draft',
      calculated: 'status-calculated',
      locked: 'status-locked'
    };
    return <Badge className={styles[status] || 'status-draft'}>{status}</Badge>;
  };

  // Calculate totals
  const totals = lines.reduce((acc, line) => ({
    gross: acc.gross + (line.gross_pay || 0),
    vacation: acc.vacation + (line.vacation_pay || 0),
    cpp: acc.cpp + (line.cpp || 0),
    ei: acc.ei + (line.ei || 0),
    federal: acc.federal + (line.federal_tax || 0),
    provincial: acc.provincial + (line.provincial_tax || 0),
    net: acc.net + (line.net_pay || 0),
  }), { gross: 0, vacation: 0, cpp: 0, ei: 0, federal: 0, provincial: 0, net: 0 });

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  if (!run) {
    return (
      <div className="text-center py-16">
        <p className="text-muted-foreground">Payroll run not found</p>
        <Button onClick={() => navigate('/payroll-runs')} className="mt-4">
          <ArrowLeft className="mr-2 h-4 w-4" /> Back to Payroll Runs
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6" data-testid="payroll-run-detail">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div className="flex items-center gap-4">
          <Button variant="ghost" onClick={() => navigate('/payroll-runs')} data-testid="back-btn">
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Payroll Run #{run.run_number}</h1>
            <p className="text-muted-foreground">
              {period ? `${formatDate(period.start_date)} - ${formatDate(period.end_date)}` : 'Loading...'}
            </p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {getStatusBadge(run.status)}
        </div>
      </div>

      {/* MVP Disclaimer */}
      <Alert>
        <AlertTriangle className="h-4 w-4" />
        <AlertTitle>MVP Disclaimer</AlertTitle>
        <AlertDescription>
          This is an MVP payroll calculator. Calculations may not reflect the latest CRA rates. 
          Please verify all results with your accountant before finalizing.
        </AlertDescription>
      </Alert>

      {/* Action Buttons */}
      <Card>
        <CardHeader>
          <CardTitle>Actions</CardTitle>
          <CardDescription>Manage this payroll run</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-4">
          {run.status === 'draft' && (
            <Button onClick={handleCalculate} disabled={calculating} data-testid="calculate-btn">
              {calculating && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              <Calculator className="mr-2 h-4 w-4" />
              Calculate Payroll
            </Button>
          )}
          
          {run.status === 'calculated' && (
            <>
              <Button onClick={handleCalculate} variant="outline" disabled={calculating}>
                {calculating && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                <Calculator className="mr-2 h-4 w-4" />
                Recalculate
              </Button>
              <Button onClick={handleLock} disabled={locking} data-testid="lock-btn">
                {locking && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                <Lock className="mr-2 h-4 w-4" />
                Lock Payroll Run
              </Button>
            </>
          )}
          
          {run.status === 'locked' && (
            <Button onClick={handleGeneratePayslips} disabled={generatingPayslips} data-testid="generate-payslips-btn">
              {generatingPayslips && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              <FileText className="mr-2 h-4 w-4" />
              Generate Payslips
            </Button>
          )}
        </CardContent>
      </Card>

      {/* Summary */}
      {lines.length > 0 && (
        <div className="grid gap-4 md:grid-cols-4">
          <Card className="card-hover">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Total Gross Pay</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{formatCurrency(totals.gross + totals.vacation)}</div>
              <p className="text-xs text-muted-foreground">Including vacation pay</p>
            </CardContent>
          </Card>
          
          <Card className="card-hover">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Total Deductions</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">
                {formatCurrency(totals.cpp + totals.ei + totals.federal + totals.provincial)}
              </div>
              <p className="text-xs text-muted-foreground">CPP, EI, and taxes</p>
            </CardContent>
          </Card>
          
          <Card className="card-hover">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Total Net Pay</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-green-600">{formatCurrency(totals.net)}</div>
              <p className="text-xs text-muted-foreground">Amount to employees</p>
            </CardContent>
          </Card>
          
          <Card className="card-hover">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Employees</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{lines.length}</div>
              <p className="text-xs text-muted-foreground">Processed in this run</p>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Payroll Lines */}
      <Card>
        <CardHeader>
          <CardTitle>Payroll Lines</CardTitle>
          <CardDescription>Detailed breakdown for each employee</CardDescription>
        </CardHeader>
        <CardContent>
          {lines.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              {run.status === 'draft' 
                ? 'Click "Calculate Payroll" to generate payroll lines'
                : 'No payroll lines found'}
            </div>
          ) : (
            <div className="rounded-md border overflow-x-auto">
              <Table className="data-table">
                <TableHeader>
                  <TableRow>
                    <TableHead>Employee</TableHead>
                    <TableHead className="text-right">Hours</TableHead>
                    <TableHead className="text-right">Gross</TableHead>
                    <TableHead className="text-right">Vacation</TableHead>
                    <TableHead className="text-right">CPP</TableHead>
                    <TableHead className="text-right">EI</TableHead>
                    <TableHead className="text-right">Fed Tax</TableHead>
                    <TableHead className="text-right">Prov Tax</TableHead>
                    <TableHead className="text-right">Net Pay</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {lines.map((line) => (
                    <TableRow key={line.id}>
                      <TableCell className="font-medium">{line.employee_name}</TableCell>
                      <TableCell className="text-right">
                        {line.regular_hours?.toFixed(1)} + {line.overtime_hours?.toFixed(1)} OT
                      </TableCell>
                      <TableCell className="text-right">{formatCurrency(line.gross_pay)}</TableCell>
                      <TableCell className="text-right">{formatCurrency(line.vacation_pay)}</TableCell>
                      <TableCell className="text-right">{formatCurrency(line.cpp)}</TableCell>
                      <TableCell className="text-right">{formatCurrency(line.ei)}</TableCell>
                      <TableCell className="text-right">{formatCurrency(line.federal_tax)}</TableCell>
                      <TableCell className="text-right">{formatCurrency(line.provincial_tax)}</TableCell>
                      <TableCell className="text-right font-medium text-green-600">
                        {formatCurrency(line.net_pay)}
                      </TableCell>
                    </TableRow>
                  ))}
                  {/* Totals Row */}
                  <TableRow className="bg-muted/50 font-medium">
                    <TableCell>TOTALS</TableCell>
                    <TableCell className="text-right">-</TableCell>
                    <TableCell className="text-right">{formatCurrency(totals.gross)}</TableCell>
                    <TableCell className="text-right">{formatCurrency(totals.vacation)}</TableCell>
                    <TableCell className="text-right">{formatCurrency(totals.cpp)}</TableCell>
                    <TableCell className="text-right">{formatCurrency(totals.ei)}</TableCell>
                    <TableCell className="text-right">{formatCurrency(totals.federal)}</TableCell>
                    <TableCell className="text-right">{formatCurrency(totals.provincial)}</TableCell>
                    <TableCell className="text-right text-green-600">{formatCurrency(totals.net)}</TableCell>
                  </TableRow>
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
