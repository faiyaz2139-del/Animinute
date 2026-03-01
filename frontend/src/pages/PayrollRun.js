import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
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
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../components/ui/select';
import { toast } from 'sonner';
import { formatDate, formatCurrency } from '../lib/utils';
import { Plus, Calculator, Eye, Loader2 } from 'lucide-react';

const API = `${process.env.REACT_APP_BACKEND_URL}/api`;

export default function PayrollRun() {
  const navigate = useNavigate();
  const [periods, setPeriods] = useState([]);
  const [runs, setRuns] = useState([]);
  const [selectedPeriod, setSelectedPeriod] = useState('');
  const [loading, setLoading] = useState(true);
  const [creating, setCreating] = useState(false);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [periodsRes, runsRes] = await Promise.all([
        axios.get(`${API}/pay-periods`),
        axios.get(`${API}/payroll-runs`)
      ]);
      setPeriods(periodsRes.data.filter(p => p.status !== 'locked'));
      setRuns(runsRes.data);
    } catch (error) {
      toast.error('Failed to fetch data');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateRun = async () => {
    if (!selectedPeriod) {
      toast.error('Please select a pay period');
      return;
    }
    
    setCreating(true);
    try {
      const response = await axios.post(`${API}/payroll-runs`, {
        pay_period_id: selectedPeriod
      });
      toast.success('Payroll run created');
      navigate(`/payroll-runs/${response.data.id}`);
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to create payroll run');
    } finally {
      setCreating(false);
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

  return (
    <div className="space-y-6" data-testid="payroll-runs-page">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Payroll Runs</h1>
          <p className="text-muted-foreground">Create and manage payroll calculations</p>
        </div>
      </div>

      {/* Create New Run */}
      <Card>
        <CardHeader>
          <CardTitle>Create New Payroll Run</CardTitle>
          <CardDescription>Select a pay period to start a new payroll calculation</CardDescription>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-4">
              <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
            </div>
          ) : periods.length === 0 ? (
            <p className="text-muted-foreground">No pay periods available. Create a pay period first.</p>
          ) : (
            <div className="flex flex-col sm:flex-row gap-4">
              <div className="flex-1">
                <Select value={selectedPeriod} onValueChange={setSelectedPeriod}>
                  <SelectTrigger data-testid="period-select">
                    <SelectValue placeholder="Select a pay period" />
                  </SelectTrigger>
                  <SelectContent>
                    {periods.map((period) => (
                      <SelectItem key={period.id} value={period.id}>
                        {formatDate(period.start_date)} - {formatDate(period.end_date)} ({period.status})
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <Button 
                onClick={handleCreateRun} 
                disabled={!selectedPeriod || creating}
                data-testid="create-run-btn"
              >
                {creating && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                <Plus className="mr-2 h-4 w-4" />
                Create Run
              </Button>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Existing Runs */}
      <Card>
        <CardHeader>
          <CardTitle>All Payroll Runs</CardTitle>
          <CardDescription>View and manage your payroll runs</CardDescription>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          ) : runs.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No payroll runs yet. Create your first payroll run above.
            </div>
          ) : (
            <div className="rounded-md border">
              <Table className="data-table">
                <TableHeader>
                  <TableRow>
                    <TableHead>Run #</TableHead>
                    <TableHead>Pay Period</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Calculated At</TableHead>
                    <TableHead>Locked At</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {runs.map((run) => {
                    const period = periods.find(p => p.id === run.pay_period_id) || 
                                   { start_date: 'N/A', end_date: 'N/A' };
                    return (
                      <TableRow key={run.id} data-testid={`run-row-${run.id}`}>
                        <TableCell className="font-medium">#{run.run_number}</TableCell>
                        <TableCell>
                          {formatDate(period.start_date)} - {formatDate(period.end_date)}
                        </TableCell>
                        <TableCell>{getStatusBadge(run.status)}</TableCell>
                        <TableCell>
                          {run.calculated_at ? formatDate(run.calculated_at) : '-'}
                        </TableCell>
                        <TableCell>
                          {run.locked_at ? formatDate(run.locked_at) : '-'}
                        </TableCell>
                        <TableCell className="text-right">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => navigate(`/payroll-runs/${run.id}`)}
                            data-testid={`view-run-${run.id}`}
                          >
                            <Eye className="mr-2 h-4 w-4" />
                            View
                          </Button>
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
